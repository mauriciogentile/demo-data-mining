using System.ComponentModel;
using System.Runtime.CompilerServices;
using FileImporter.Annotations;

namespace Demo.FileImporter
{
    public class PropertyVm : INotifyPropertyChanged
    {
        private string _error;
        private bool _hasError;
        public int Id { get; set; }
        public string OperationType { get; set; }
        public string ZoneReputation { get; set; }
        public string PropertyType { get; set; }
        public int Bathrooms { get; set; }
        public int Bedrooms { get; set; }
        public bool HasParking { get; set; }
        public int SquareMts { get; set; }
        public int Age { get; set; }
        public int DistanceToStation { get; set; }
        public int Price { get; set; }

        public bool HasError
        {
            set { _hasError = value; OnPropertyChanged(); }
            get { return _hasError; }
        }

        public string Error
        {
            set
            {
                _error = value; OnPropertyChanged();
                if (!string.IsNullOrEmpty(value))
                {
                    HasError = true;
                }
            }
            get { return _error; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
