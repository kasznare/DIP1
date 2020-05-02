using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Diploma2.Annotations;
using Diploma2.Model;
using LiveCharts;
using LiveCharts.Wpf;

namespace Diploma2.Views {
    /// <summary>
    /// Interaction logic for CostDiagram.xaml
    /// </summary>
    public partial class CostDiagram : Page, INotifyPropertyChanged {
        public CostDiagram()
        {
            DataContext = this;
            InitializeComponent();
            SeriesCollection = new SeriesCollection();
            LoadData();
           
        }

        public void LoadData()
        {
            //SeriesCollection = new SeriesCollection
            //{
            //    new LineSeries
            //    {
            //        Values = new ChartValues<double> { 3, 5, 7, 4 }
            //    },
            //    //new ColumnSeries
            //    //{
            //    //    Values = new ChartValues<decimal> { 5, 6, 2, 7 }
            //    //}
            //};
            Chart.Series = SeriesCollection;
            //SeriesCollection.Add(new ColumnSeries {
            //        Title = "title",
            //        Values = new ChartValues<double> { 10,10,10,9,8,7,6,5},
            //        Fill = Brushes.Tomato
            //    });
            //    //Labels = new string[] { "0","1", "2", "3" ,"4","5","6","7"};
            //    Labels = new int[] { 0,1,2,3,4,5,6,7};
            //    //Formatter = value => value.ToString(@"F1");
            //    Formatter = value => value;
        }

        public void LoadCosts(ObservableCollection<Cost> costs)
        {
            SeriesCollection = new SeriesCollection
            {
                new LineSeries(costs.Select(i=>i.SummaryCost))
            };
        }
        public int[] Labels { get; set; }
        //public Func<double, string> Formatter { get; set; }
        public Func<double, double> Formatter { get; set; }
        public SeriesCollection SeriesCollection { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
