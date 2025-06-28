using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading;
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
using AdminClient.DTOs;
using AdminClient.Models;
using AdminClient.Services;

namespace AdminClient.Views
{
    /// <summary>  
    /// Interaction logic for VendingMachinesPage.xaml  
    /// </summary>  
    public partial class MonitorVendingMachinesPage : Page
    {
        public static int entityAmount = 0;

        List<VendingMachineMonitorDTO> vendingMachineMonitorDTOs = new List<VendingMachineMonitorDTO>();

        List<VendingMachineMonitorDTO> filteredInfo = new List<VendingMachineMonitorDTO>();

        private CancellationTokenSource _loopCts = new CancellationTokenSource();

        public PagedMachinesResult? vendingMachines { get; set; }
        public PagedSimCards pagedSimCards { get; set; }
        public Money[] possiblyStoredMoney { get; set; }
        public PaymentMethod[]? pms { get; set; }
        public string filterWords { get; set; }
        public bool isWorkingStatus { get; set; } = false;
        public bool isNotWorkingStatus { get; set; } = false;
        public bool isUnknownStatus { get; set; } = false;
        public bool isCashFiltered { get; set; } = false;
        public bool isMoneyFiltered { get; set; } = false;
        public bool isCardFiltered { get; set; } = false;
        public bool isQRFiltered { get; set; } = false;
        public MonitorVendingMachinesPage()
        {
            InitializeComponent();
            Loaded += LoadPageStuff;
        }
        private async void LoadPageStuff(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.Windows
                .OfType<MainInterfaceWindow>()
                .FirstOrDefault();
            if (mainWindow != null)
            {
                mainWindow.CurrentPageTitle.Text = "Монитор ТА";
            }
            foreach (var column in DataGridTable.Columns)
            {
                column.Width = new DataGridLength(1, DataGridLengthUnitType.Auto);
            }

            await LoadMachineData();

            await RunLoopEvery30SecondsAsync(_loopCts.Token);
        }
        private async Task RunLoopEvery30SecondsAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    SetFilters();
                    await Task.Delay(TimeSpan.FromSeconds(30), token);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
            }
        }

        private async Task LoadMachineData()
        {
            var curUser = await Services.LoginOperation.GetCurrentUserAsync();

            if (curUser?.CompanyID == null)
            {
                return;
            }

            vendingMachines = 
                await Services.VendingMachinesService.GetVendingMachinesPagedAsync((long)curUser.CompanyID, 1500, 1);
            
            if (vendingMachines != null)
            {
                NoDataMessage.Visibility = Visibility.Collapsed;

                foreach(var ven in vendingMachines.VendingMachines)
                {
                    var vendingMachinePaymentM = await MachinePaymentService.GetMachinePaymentMethodesAsync();

                    EntityAmountFound.Text = $"Всего найдено {vendingMachines.VendingMachines.Count()} шт.";

                    pms = await PaymentMethodService.GetPaymentMethodesAsync();

                    pagedSimCards = await SIMService.GetPagedSimsAsync(1500, 1, curUser.Company.ID, true);

                    possiblyStoredMoney = await MoneyService.GetMoneyAsync();

                    VendingMachineMonitorDTO selMachine = new VendingMachineMonitorDTO();

                    selMachine.vm = ven;

                    selMachine.basicInfo = $"{selMachine.vm.ID} - \u0022{selMachine.vm.Name}\u0022";

                    string modelMachine = "";
                    string modemName = "";
                    string machineAdress = "";

                    if (selMachine.vm.VendingMachineMatrix != null)
                    {
                        modelMachine = selMachine.vm.VendingMachineMatrix.ModelName;
                    }
                    else
                    {
                        modelMachine = "Нет";
                    }

                    if (selMachine.vm.Modem != null)
                    {
                        modemName = selMachine.vm.Modem.Model;
                    }
                    else
                    {
                        modemName = "Нет";
                    }
                    
                    if (selMachine.vm.Adress != null)
                    {
                        machineAdress = selMachine.vm.Adress;
                    }
                    else
                    {
                        machineAdress = "Нет адреса";
                    }
                    selMachine.additionalInfo = $"{modelMachine} ({modemName}) {machineAdress}";

                    MachinePaymentMethod[] paymentMethods = vendingMachinePaymentM.ToArray();

                    var resPaymentMe = paymentMethods.Where(e => e.VendingMachineID == selMachine.vm.ID).ToArray();

                    selMachine.paymentMethods = resPaymentMe;

                    var moneyVM = await MachineMoneyService.GetMachineMoneyAsync(selMachine.vm.ID);

                    selMachine.machineMoney = moneyVM;

                    foreach (var mon in selMachine.machineMoney)
                    {
                        mon.Money = possiblyStoredMoney.FirstOrDefault(m => m.ID == mon.MoneyID);
                    }

                    SimCard foundSim = new();

                    if(pagedSimCards != null && selMachine.vm.Modem != null)
                    {
                        foundSim = pagedSimCards.Sims.FirstOrDefault(s => s.ID == selMachine.vm.Modem.SimCardID);
                    }
                    else
                    {
                        foundSim.Number = "Не известно";
                        foundSim.Vendor = "Не известно";
                        foundSim.Balance = 0;
                    }

                        selMachine.simCard = foundSim;

                    switch (selMachine.vm.StatusID)
                    {
                        case 1:
                            selMachine.StatusColor = Brushes.Red;
                            break;
                        case 2:
                            selMachine.StatusColor = Brushes.Blue;
                            break;
                        case 3:
                            selMachine.StatusColor = Brushes.LawnGreen;
                            break;
                    }

                    decimal sumVal = 0;

                    if(moneyVM != null)
                    {
                        sumVal = selMachine.machineMoney.Sum(m => m.Amount * m.Money.Value);
                    }

                    if(pms != null)
                    {
                        selMachine.pms = pms;
                    }

                    if(selMachine.paymentMethods.Any(pm => pm.PaymentMethodID == 1))
                    {
                        selMachine.IsQR = true;
                    }
                    if (selMachine.paymentMethods.Any(pm => pm.PaymentMethodID == 2))
                    {
                        selMachine.IsMoneyP = true;
                    }
                    if (selMachine.paymentMethods.Any(pm => pm.PaymentMethodID == 3))
                    {
                        selMachine.IsCashP = true;
                    }
                    if (selMachine.paymentMethods.Any(pm => pm.PaymentMethodID == 4))
                    {
                        selMachine.IsCardP = true;
                    }


                    selMachine.Sum = sumVal;

                    vendingMachineMonitorDTOs.Add(selMachine);
                }

                DataGridTable.ItemsSource = vendingMachineMonitorDTOs;
            }
        }

        private void FilterBox_GotFocus(object sender, RoutedEventArgs e)
        {
            FilterBox.Text = string.Empty;
        }

        private void FilterBox_TextChanged(object sender, TextChangedEventArgs e)
        {

            if (!IsInitialized)
                return;

            var tbx = sender as TextBox;
            if (vendingMachineMonitorDTOs == null)
                return;
            filterWords = tbx.Text.Trim();
            SetFilters();
        }

        private void SetFilters()
        {
            if(filterWords != "" && filterWords is not null)
            {
                filteredInfo = vendingMachineMonitorDTOs.Where(vm =>
                vm.vm.ID.ToString().Contains(filterWords, StringComparison.OrdinalIgnoreCase) ||
                vm.basicInfo.Contains(filterWords, StringComparison.OrdinalIgnoreCase) ||
                vm.additionalInfo.Contains(filterWords, StringComparison.OrdinalIgnoreCase) ||
                vm.simCard.Number.Contains(filterWords, StringComparison.OrdinalIgnoreCase) ||
                vm.simCard.Vendor.Contains(filterWords, StringComparison.OrdinalIgnoreCase)
                )
                .ToList();
                if(isNotWorkingStatus)
                {
                    filteredInfo = filteredInfo.Where(vm => vm.vm.StatusID == 1).ToList();
                }
                else if (isWorkingStatus)
                {
                    filteredInfo = filteredInfo.Where(vm => vm.vm.StatusID == 3).ToList();
                }
                else if (isUnknownStatus)
                {
                    filteredInfo = filteredInfo.Where(vm => vm.vm.StatusID == 2).ToList();
                }
                if (isCashFiltered)
                {
                    filteredInfo = filteredInfo.Where(vm => vm.IsCashP == true).ToList();
                }
                if (isMoneyFiltered)
                {
                    filteredInfo = filteredInfo.Where(vm => vm.IsMoneyP == true).ToList();
                }
                if (isCardFiltered)
                {
                    filteredInfo = filteredInfo.Where(vm => vm.IsCardP == true).ToList();
                }
                DataGridTable.ItemsSource = null;
                DataGridTable.ItemsSource = filteredInfo;
            }
            else
            {
                filteredInfo = vendingMachineMonitorDTOs.ToList();

                if (isNotWorkingStatus)
                {
                    filteredInfo = filteredInfo.Where(vm => vm.vm.StatusID == 1).ToList();
                }
                else if (isWorkingStatus)
                {
                    filteredInfo = filteredInfo.Where(vm => vm.vm.StatusID == 3).ToList();
                }
                else if (isUnknownStatus)
                {
                    filteredInfo = filteredInfo.Where(vm => vm.vm.StatusID == 2).ToList();
                }
                if (isCashFiltered)
                {
                    filteredInfo = filteredInfo.Where(vm => vm.IsCashP == true).ToList();
                }
                if (isMoneyFiltered)
                {
                    filteredInfo = filteredInfo.Where(vm => vm.IsMoneyP == true).ToList();
                }
                if (isCardFiltered)
                {
                    filteredInfo = filteredInfo.Where(vm => vm.IsCardP == true).ToList();
                }
                if (isQRFiltered)
                {
                    filteredInfo = filteredInfo.Where(vm => vm.IsQR == true).ToList();
                }
                DataGridTable.ItemsSource = null;
                DataGridTable.ItemsSource = filteredInfo;
            }
            
        }

        private void DataGridTable_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }

        private void WorkingStatus_Click(object sender, RoutedEventArgs e)
        {
            if (!isWorkingStatus)
            {
                isWorkingStatus = true;
                isNotWorkingStatus = false;
                isUnknownStatus = false;
                WorkingStatus.Background = Brushes.DimGray;
                BrokenStatus.Background = Brushes.Transparent;
                UnknownStatus.Background = Brushes.Transparent;
                SetFilters();
            }
            else
            {
                isWorkingStatus = false;
                WorkingStatus.Background = Brushes.Transparent;
                SetFilters();

            }
        }

        private void BrokenStatus_Click(object sender, RoutedEventArgs e)
        {
            if (!isNotWorkingStatus)
            {
                isNotWorkingStatus = true;
                isWorkingStatus = false;
                isUnknownStatus = false;
                BrokenStatus.Background = Brushes.DimGray;
                WorkingStatus.Background = Brushes.Transparent;
                UnknownStatus.Background = Brushes.Transparent;
                SetFilters();
            }
            else
            {
                isNotWorkingStatus = false;
                BrokenStatus.Background = Brushes.Transparent;
                SetFilters();
            }
        }
        private void UnknownStatus_Click(object sender, RoutedEventArgs e)
        {
            if (!isUnknownStatus)
            {
                isUnknownStatus = true;
                isWorkingStatus = false;
                isNotWorkingStatus = false;
                UnknownStatus.Background = Brushes.DimGray;
                WorkingStatus.Background = Brushes.Transparent;
                BrokenStatus.Background = Brushes.Transparent;
                if (filteredInfo.Count > 0)
                {
                    SetFilters();
                }
                else
                {
                    SetFilters();
                }
            }
            else
            {
                isUnknownStatus = false;
                UnknownStatus.Background = Brushes.Transparent;
                if (filteredInfo.Count > 0)
                {
                    SetFilters();
                }
                else
                {
                    SetFilters();
                }
            }
        }

        private void CashFilterButton_Click(object sender, RoutedEventArgs e)
        {
            if(!isCashFiltered)
            {
                isCashFiltered = true;
                CashFilter.Background = Brushes.Gray;
                CashFilterButton.Foreground = Brushes.White;
                SetFilters();
            }
            else
            {
                isCashFiltered = false;
                CashFilter.Background = Brushes.Transparent;
                CashFilterButton.Foreground = Brushes.Gray;
                SetFilters();
            }
        }

        private void MoneyFilterButton_Click(object sender, RoutedEventArgs e)
        {
            if(!isMoneyFiltered)
            {
                isMoneyFiltered = true;
                MoneyFilter.Background = Brushes.Gray;
                MoneyFilterButton.Foreground = Brushes.White;
                SetFilters();
            }
            else
            {
                isMoneyFiltered = false;
                MoneyFilter.Background = Brushes.Transparent;
                MoneyFilterButton.Foreground = Brushes.Gray;
                SetFilters();
            }
        }

        private void CardFilterButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isCardFiltered)
            {
                isCardFiltered = true;
                CardFilter.Background = Brushes.Gray;
                CardFilterButton.Foreground = Brushes.White;
                SetFilters();
            }
            else
            {
                isCardFiltered = false;
                CardFilter.Background = Brushes.Transparent;
                CardFilterButton.Foreground = Brushes.Gray;
                SetFilters();
            }
        }

        private void QRFilterButton_Click(object sender, RoutedEventArgs e)
        {
            if(!isQRFiltered)
            {
                isQRFiltered = true;
                QRFilter.Background = Brushes.Gray;
                QRFilterButton.Foreground = Brushes.White;
                SetFilters();
            }
            else
            {
                isQRFiltered = false;
                QRFilter.Background = Brushes.Transparent;
                QRFilterButton.Foreground = Brushes.Gray;
                SetFilters();
            }
        }
    }
}
