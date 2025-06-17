using System;
using System.Collections.Generic;
using System.Linq;
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
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot;
using OxyPlot.Wpf;

namespace AdminClient.Views
{
    /// <summary>  
    /// Interaction logic for MainPage.xaml  
    /// </summary>  
    public partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            Loaded += MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.Windows
                .OfType<MainInterfaceWindow>()
                .FirstOrDefault();
            if (mainWindow != null)
            {
                mainWindow.CurrentPageTitle.Text = "Главная";
            }
            DataContext = this;

            var model = new PlotModel { Title = "Продажи по регионам" };

            var categoryAxis = new CategoryAxis
            {
                Position = AxisPosition.Left, // Ось Y
                Key = "Category"
            };
            categoryAxis.Labels.AddRange(new[] { "Москва", "СПб", "Казань", "Краснодар" });


            var valueAxis = new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Key = "Value"
            };


            var barSeries = new BarSeries
            {
                XAxisKey = "Value",
                YAxisKey = "Category",
                ItemsSource = new[]
                {
                    new BarItem { Value = 15 },
                    new BarItem { Value = 22 },
                    new BarItem { Value = 18 },
                    new BarItem { Value = 12 }
                },
                LabelPlacement = LabelPlacement.Inside,
                LabelFormatString = "{0:.0}"
            };


            model.Axes.Add(categoryAxis);
            model.Axes.Add(valueAxis);
            model.Series.Add(barSeries);

            var plotView = new PlotView
            {
                Model = model
            };
            Content = plotView;
        }
    }
}
