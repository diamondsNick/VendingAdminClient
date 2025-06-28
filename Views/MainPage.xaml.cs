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
using System.Xml.Serialization;
using System.Security.Policy;
using AdminClient.Services;
using AdminClient.Models;
using Newtonsoft.Json.Linq;
using System.Threading;

namespace AdminClient.Views
{
    /// <summary>  
    /// Interaction logic for MainPage.xaml  
    /// </summary>  
    public partial class MainPage : Page
    {
        public static bool isSumGraphType = true;
        public static Sale[]? Sales { get; set; }
        public static User? curUser;

        private CancellationTokenSource _loopCts = new CancellationTokenSource();

        public MainPage()
        {
            InitializeComponent();
            Loaded += MainPage_Loaded;
            Loaded += loadSales;
        }
        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.Windows
                .OfType<MainInterfaceWindow>()
                .FirstOrDefault();
            if (mainWindow != null)
            {
                mainWindow.CurrentPageTitle.Text = "Главная";
            }
            DataContext = this;

            await RunLoopEvery30SecondsAsync(_loopCts.Token);
        }
        private async Task RunLoopEvery30SecondsAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    MachinePieChart();
                    NetworkStabilityChart();
                    loadSales();
                    if(isSumGraphType)
                    {
                        GraphBySum();
                    }

                    await Task.Delay(TimeSpan.FromSeconds(30), token);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
            }
        }
        private void loadSales(object sender, RoutedEventArgs e)
        {
            loadSales();
        }

        private async void loadSales()
        {
            curUser = await LoginOperation.GetCurrentUserAsync();
            if(curUser != null && curUser.CompanyID != null)
            {
                Sales = await SalesService.GetCompanySalesAsync(curUser.CompanyID.Value);
                GraphBySum();
            }
            
        }

        private void GraphBySum()
        {
            DateOfGraphic.Text = "Данные по продажам с " + DateTime.Now.AddDays(-10).ToString("dd.MM.yyyy") + " по " + DateTime.Now.ToString("dd.MM.yyyy");
            var model = new PlotModel();

            var categoryAxis = new CategoryAxis
            {
                Position = AxisPosition.Bottom,
                Key = "Category"
            };

            var valueAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                Key = "Value"
            };
            BarItem[] values = new BarItem[10];
            string[] days = new string[10];
            
            DateTime today = DateTime.Now;

            var salesInfo = Sales?
                .GroupBy(s => s.Date.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    TotalAmount = g.Sum(s => s.Cost)
                })
                .ToList();
            bool hasAny = salesInfo?.Any() ?? false;

            for (int i = 9; i>=0; i--)
            {
                DateTime nDate = today.AddDays(-i);
                days[9 - i] = CapitalizeFirstLetter(nDate.ToString("ddd"))
                    + "\n" + nDate.ToString("dd.MM");
                values[9 - i] = new BarItem { Value = 0 };
                if (hasAny && salesInfo != null && salesInfo.Any(e => e.Date.Date == nDate.Date.Date))
                {
                    var amount = salesInfo.First(e => e.Date.Date == nDate.Date.Date).TotalAmount;
                    values[9 - i] = new BarItem { Value = (double)amount };
                }
            }
            categoryAxis.Labels.AddRange(days);
            var barSeries = new BarSeries
            {
                XAxisKey = "Value",
                YAxisKey = "Category",
                ItemsSource = values,
                LabelPlacement = LabelPlacement.Outside,
                LabelFormatString = ""
            };

            barSeries.FillColor = OxyColor.FromAColor(100, OxyColors.DeepSkyBlue);
            barSeries.StrokeThickness = 1;
            barSeries.StrokeColor = OxyColors.CadetBlue;

            model.Axes.Add(categoryAxis);
            model.Axes.Add(valueAxis);
            model.Series.Add(barSeries);

            GraphVending.Model = model;
        }

        private void GraphByQty()
        {
            var model = new PlotModel();

            var categoryAxis = new CategoryAxis
            {
                Position = AxisPosition.Bottom,
                Key = "Category"
            };

            var valueAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                Key = "Value"
            };
            BarItem[] values = new BarItem[10];
            string[] days = new string[10];

            DateTime today = DateTime.Now;

            var salesInfo = Sales?
                .GroupBy(s => s.Date.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    TotalAmount = g.Count()
                })
                .ToList();
            bool hasAny = salesInfo?.Any() ?? false;

            for (int i = 9; i >= 0; i--)
            {
                DateTime nDate = today.AddDays(-i);
                days[9 - i] = CapitalizeFirstLetter(nDate.ToString("ddd"))
                    + "\n" + nDate.ToString("dd.MM");
                values[9 - i] = new BarItem { Value = 0 };
                if (hasAny && salesInfo != null && salesInfo.Any(e => e.Date.Date == nDate.Date.Date))
                {
                    var amount = salesInfo.First(e => e.Date.Date == nDate.Date.Date).TotalAmount;
                    values[9 - i] = new BarItem { Value = (double)amount };
                }
            }
            categoryAxis.Labels.AddRange(days);
            var barSeries = new BarSeries
            {
                XAxisKey = "Value",
                YAxisKey = "Category",
                ItemsSource = values,
                LabelPlacement = LabelPlacement.Base,
                LabelFormatString = ""
            };

            barSeries.FillColor = OxyColor.FromAColor(100, OxyColors.DeepSkyBlue);
            barSeries.StrokeThickness = 1;
            barSeries.StrokeColor = OxyColors.CadetBlue;
            model.Axes.Add(categoryAxis);
            model.Axes.Add(valueAxis);
            model.Series.Add(barSeries);

            GraphVending.Model = model;
        }
        string CapitalizeFirstLetter(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            return char.ToUpper(text[0]) + text.Substring(1);
        }
        private async void NetworkStabilityChart()
        {
            var model = new PlotModel();

            var machines = await VendingMachinesService.GetVendingMachinesAsync();

            double questionedAmount = machines.Count(e => e.StatusID == 2);
            double doesntWork = machines.Count(e => e.StatusID == 1);
            double works = machines.Count(e => e.StatusID == 3);
            string noWork = "Не работает";
            string workingFine = "Работает";
            var unusuableMachines = doesntWork+questionedAmount;
            var rotateAngle =  (works - unusuableMachines)*8;
            Pointer.RenderTransformOrigin = new Point(0.5, 0.88);
            Pointer.RenderTransform = new RotateTransform(rotateAngle);
            double workPres = 0;
            if (unusuableMachines != 0)
            {
                workPres = (works / (works + unusuableMachines)) * 100;
                workPres = Math.Round(workPres);
            }

            ArcText.Text = "Работающих автоматов - " + workPres + "%";

            var pieSeries = new PieSeries
            {
                StrokeThickness = 0,
                InsideLabelPosition = 0.7,
                AngleSpan = 150,
                StartAngle = 195,
                InnerDiameter = 0.4,
                FontSize = 14,
                InsideLabelFormat = "",
                OutsideLabelFormat = ""
            };         
            pieSeries.Slices.Add(new PieSlice(workingFine, works) { Fill = OxyColors.Green });
            pieSeries.Slices.Add(new PieSlice(noWork, unusuableMachines) { Fill = OxyColors.Red });

            pieSeries.TickHorizontalLength = 0;
            pieSeries.TickLabelDistance = 0;
            pieSeries.TickRadialLength = 0;
            model.Series.Add(pieSeries);
            ArcPlot.Model = model;
        }
        private async void MachinePieChart()
        {
            var model = new PlotModel();

            var machines = await VendingMachinesService.GetVendingMachinesAsync();

            double questionedAmount = machines.Count(e => e.StatusID == 2);
            double doesntWork = machines.Count(e => e.StatusID == 1);
            double works = machines.Count(e => e.StatusID == 3);
            string noWork = "Не работает";
            string questioned = "Под вопросом";
            string workingFine = "Работает";

            InsideText.Text = "Нет данных";

            if (works >= questionedAmount && works >= doesntWork) InsideText.Text = workingFine + "\n" + works;
            if (doesntWork >= questionedAmount && doesntWork >= works) InsideText.Text = noWork + "\n" + doesntWork;
            if (questionedAmount >= works && questionedAmount >= doesntWork) InsideText.Text = questioned + "\n" + questionedAmount;

            if (works == 0) works += 0.15;
            if (doesntWork == 0) doesntWork += 0.15;
            if (questionedAmount == 0) questionedAmount += 0.15;


            var pieSeries = new PieSeries
            {
                StrokeThickness = 2.0,
                InsideLabelPosition = 0.7,
                AngleSpan = 360,
                StartAngle = 0,
                InnerDiameter = 0.65,
                FontSize = 14,
                InsideLabelFormat = "",
                OutsideLabelFormat = ""
            };

            pieSeries.Slices.Add(new PieSlice(noWork, doesntWork) { Fill = OxyColors.Red });
            pieSeries.Slices.Add(new PieSlice(workingFine, works) { Fill = OxyColors.Green });
            pieSeries.Slices.Add(new PieSlice(questioned, questionedAmount) { Fill = OxyColors.DarkCyan });
            pieSeries.TickHorizontalLength = 0;
            pieSeries.TickLabelDistance = 0;
            pieSeries.TickRadialLength = 0;
            model.Series.Add(pieSeries);
            MachinePieGraph.Model = model;
        }
        private void SumButton_Click(object sender, RoutedEventArgs e)
        {
            if(isSumGraphType == false)
            {
                SumButtonBackground.Background = new SolidColorBrush(Colors.DeepSkyBlue);
                SumButtonBackground.BorderThickness = new Thickness(0);
                SumButton.Foreground = new SolidColorBrush(Colors.White);

                QtyButtonBackground.Background = new SolidColorBrush(Colors.White);
                QtyButtonBackground.BorderThickness = new Thickness(1);
                QtyButton.Foreground = new SolidColorBrush(Colors.Black);

                GraphBySum();
                isSumGraphType = true;
            }
        }

        private void QtyButton_Click(object sender, RoutedEventArgs e)
        {
            if (isSumGraphType == true)
            {
                QtyButtonBackground.Background = new SolidColorBrush(Colors.DeepSkyBlue);
                QtyButtonBackground.BorderThickness = new Thickness(0);
                QtyButton.Foreground = new SolidColorBrush(Colors.White);

                SumButtonBackground.Background = new SolidColorBrush(Colors.White);
                SumButtonBackground.BorderThickness = new Thickness(1);
                SumButton.Foreground = new SolidColorBrush(Colors.Black);

                GraphByQty();
                isSumGraphType = false;
            }
        }
    }
}
