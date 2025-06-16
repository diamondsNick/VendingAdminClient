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

            var mainWindow = Application.Current.Windows
                .OfType<MainInterfaceWindow>()
                .FirstOrDefault();
            if (mainWindow != null)
            {
                mainWindow.CurrentPageTitle.Text = "Главная";
            }
            DataContext = this; // Устанавливаем DataContext для привязки Series  

            double[] values = { 3, 7, 5, 2, 9, 4, 6, 1, 8, 5 };

            // Построение гистограммы  
            var hist = ScottPlot.Statistics.Histogram.WithBinCount(10, values);
            var barPlot = wpfPlot1.Plot.Add.Bars(hist.Bins, hist.Counts);
            
            wpfPlot1.Plot.Title("Пример гистограммы");
            wpfPlot1.Plot.XLabel("Значения");
            wpfPlot1.Plot.YLabel("Количество");

            wpfPlot1.Refresh();
        }
    }
}
