using System.Collections.Generic;
using System.Windows;
using Demo.Data;
using FileHelpers;
using FileHelpers.Options;
using Microsoft.Win32;

namespace FileImporter
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
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

        void ProcessFile(object sender, RoutedEventArgs e)
        {
            var engine = new FileHelperEngine<Property>();
            this.DataContext = engine.ReadFile(TextBox1.Text);
        }
    }
}
