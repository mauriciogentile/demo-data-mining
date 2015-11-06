using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using AutoMapper;
using Demo.Data;
using FileHelpers;
using FileHelpers.Events;
using FileHelpers.Options;
using Microsoft.Win32;

namespace FileImporter
{
    public partial class MainWindow
    {
        private readonly SynchronizationContext synchronizationContext;
        private readonly ObservableCollection<PropertyVm> propertyList = new ObservableCollection<PropertyVm>();
        private const double acceptableVariation = 0.1;

        public MainWindow()
        {
            InitializeComponent();
            Mapper.CreateMap<Property, PropertyVm>();
            synchronizationContext = SynchronizationContext.Current;
            ProgressBar1.Maximum = 100;
            ProgressBar1.Visibility = Visibility.Hidden;
            DataContext = propertyList;
        }

        void PickFile(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            var dlg = new OpenFileDialog
            {
                DefaultExt = ".csv",
                Filter = "CSV Files (*.csv)|*.csv"
            };

            // Display OpenFileDialog by calling ShowDialog method 
            bool? result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                TextBox1.Text = filename;
            }
        }

        async void ProcessFile(object sender, RoutedEventArgs e)
        {
            var filePath = TextBox1.Text;

            propertyList.Clear();

            var properties = await Task<IEnumerable<Property>>.Factory.StartNew(() =>
            {
                var engine = new FileHelperEngine<Property>();
                engine.Progress += engine_Progress;
                var properties1 = engine.ReadFile(filePath).ToList();
                int id = 1000;
                properties1.ForEach(p =>
                {
                    p.Id = id + 1;
                    id++;
                });
                return properties1;
            });

            Mapper.Map<IEnumerable<PropertyVm>>(properties).ToList().ForEach(p => propertyList.Add(p));

            await Task.Delay(TimeSpan.FromSeconds(1)).ContinueWith(t => ValidateValues(properties));
        }

        void engine_Progress(object sender, ProgressEventArgs e)
        {
            synchronizationContext.Send(o =>
            {
                ProgressBar1.Value = ((ProgressEventArgs)o).Percent;
                ProgressBar1.Visibility = ProgressBar1.Value >= ProgressBar1.Maximum ? Visibility.Hidden : Visibility.Visible;
            }, e);
        }

        void ValidateValues(IEnumerable<Property> properties)
        {
            Parallel.ForEach(properties, p =>
            {
                var vm = propertyList.First(x => x.Id == p.Id);
                var price = p.PredictPrice();
                if (price == 0)
                {
                    vm.Error = "Imposible predecir valor!";
                }
                var variaton = price * acceptableVariation;
                if (Math.Abs(p.Price - price) > variaton)
                {
                    vm.Error = GetAcceptableRangeString(price);

                }
                synchronizationContext.Send(o => DataGrid1.Items.Refresh(), null);
            });
        }

        static string GetAcceptableRangeString(int value)
        {
            var min = value - (value * acceptableVariation);
            var max = value + (value * acceptableVariation);
            return "Rango aceptable de '" + Math.Ceiling(min) + "' a '" + Math.Floor(max) + "'";
        }
    }
}
