using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using AutoMapper;
using Demo.Data;
using FileHelpers;
using FileHelpers.Events;
using FileImporter;
using Microsoft.Win32;

namespace Demo.FileImporter
{
    public partial class MainWindow
    {
        private readonly SynchronizationContext synchronizationContext;
        private readonly ObservableCollection<PropertyVm> propertyList = new ObservableCollection<PropertyVm>();

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
            ProgressBar1.Maximum = 100;

            var filePath = TextBox1.Text;

            propertyList.Clear();

            var properties = await Task<IList<Property>>.Factory.StartNew(() =>
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

            //DataGrid1.Items.Refresh();

            ProgressBar1.Maximum = properties.Count;
            await Task.Delay(TimeSpan.FromSeconds(1)).ContinueWith(t => ValidateValues(properties));
        }

        void engine_Progress(object sender, ProgressEventArgs e)
        {
            UpdateProgress(e.Percent);
        }

        void ValidateValues(IList<Property> properties)
        {
            int progress = 1;
            const int i = 0;
            Parallel.For(i, properties.Count - 1, p =>
            {
                var vm = propertyList[p];
                var property = properties[p];
                var prediction = property.PredictPrice();
                if (prediction.Value <= 0)
                {
                    vm.Error = "Imposible predecir valor!";
                }
                var acceptableVariation = prediction.StdDev;
                var variaton = Math.Abs(property.Price - prediction.Value);
                if (variaton > acceptableVariation)
                {
                    vm.Error = GetAcceptableRangeString(prediction);
                }
                progress++;
                UpdateProgress(progress);
            });
        }

        static string GetAcceptableRangeString(Prediction prediction)
        {
            var min = (prediction.Value - prediction.StdDev) < 0 ? 0 : (prediction.Value - prediction.StdDev);
            var max = prediction.Value + prediction.StdDev;
            return string.Format("Rango aceptable {0:C0} a {1:C0}", Math.Ceiling((double)min), Math.Floor((double)max));
        }

        void UpdateProgress(double progress)
        {
            synchronizationContext.Send(o =>
            {
                ProgressBar1.Value = (double)o;
                ProgressBar1.Visibility = ProgressBar1.Value >= ProgressBar1.Maximum ? Visibility.Hidden : Visibility.Visible;
            }, progress);
        }
    }
}
