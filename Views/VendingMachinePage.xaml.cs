using System;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Channels;
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
using OxyPlot.Axes;

namespace AdminClient.Views
{
    /// <summary>
    /// Interaction logic for VendingMachinePage.xaml
    /// </summary>
    public partial class VendingMachinePage : Page
    {
        bool isNewMachine = false;
        VendingMachine machine;
        Manufacturer[] manufacturers;
        VendingMachineMatrix[] models;
        OperatingMode[] operatingModes;
        MachinePaymentMethod[] machinePaymentMethods;
        List<string> TimeZones = new List<string> { "UTC +2", "UTC +3", "UTC +4", "UTC +5", "UTC +6", "UTC +7", "UTC +8", "UTC +9", "UTC +10", "UTC +11", "UTC +12" };
        bool CoinSlot { get; set; } = false;
        bool BankNotesSlot { get; set; } = false;
        bool NoContactPaymentSlot { get; set; } = false;
        bool QRCode { get; set; } = false;
        public VendingMachinePage(VendingMachine vendingMachine)
        {

            InitializeComponent();

            machine = vendingMachine;
            DataContext = machine;

            GetEssentialStuff();
        }
        private async void GetEssentialStuff()
        {
            manufacturers = await ManufacturerService.GetManufacturersAsync();
            ManufacturerBox.ItemsSource = manufacturers;
            ManufacturerBox.DisplayMemberPath = "Name";
            ManufacturerBox.SelectedValuePath = "ID";

            models = await MachineModelService.GetVendingMachineMatricesAsync();
            ModelBox.ItemsSource = models;
            ModelBox.DisplayMemberPath = "ModelName";
            ModelBox.SelectedValuePath = "ID";

            operatingModes = await OpreratingModeService.GetOperatingModesAsync();
            OperatingModeBox.ItemsSource = operatingModes;
            OperatingModeBox.DisplayMemberPath = "Name";
            OperatingModeBox.SelectedValuePath = "ID";

            
            TimeZoneBox.ItemsSource = TimeZones;

            if (machine.ID == 0)
            {
                isNewMachine = true;
                ConfigureNewMachine();
            }
            else OutputOldMachine();

           
        }
        private async void ConfigureNewMachine()
        {
            PageName.Text = "Создание торгового автомата";
            machine = new VendingMachine();
            DataContext = machine;
            machine.VendingMachineMatrix = new VendingMachineMatrix();
            machine.PlacementDate = DateTime.Now.ToString("dd-MM-yyyy");

            var curUser = await LoginOperation.GetCurrentUserAsync();

            machine.CompanyID = curUser.CompanyID;
        }
        private async void OutputOldMachine()
        {
            if (machine.EndHours != null && machine.StartHours != null)
            {
                HoursBox.Text = machine.StartHours + "-" + machine.EndHours;
            }
            ManufacturerBox.SelectedValue = machine.VendingMachineMatrix.Manufacturer.ID;
            ModelBox.SelectedValue = machine.VendingMachineMatrix.ID;
            OperatingModeBox.SelectedValue = machine.OperatingModeID;
            int indexZone = TimeZones.IndexOf(machine.TimeZone);
            TimeZoneBox.SelectedIndex = indexZone;

            machinePaymentMethods = await MachinePaymentService.GetMachinePaymentMethodesAsync(machine.ID);

            foreach (var machinePaymentMethod in machinePaymentMethods)
            {
                if (machinePaymentMethod.PaymentMethodID == 2)
                {
                    CoinSlot = true;
                    CoinBox.IsChecked = true;
                }
                else if (machinePaymentMethod.PaymentMethodID == 3)
                {
                    BankNotesSlot = true;
                    BankNotesBox.IsChecked = true;
                }
                else if (machinePaymentMethod.PaymentMethodID == 4)
                {
                    NoContactPaymentSlot = true;
                    NoContactBox.IsChecked = true;
                }
                else if (machinePaymentMethod.PaymentMethodID == 1)
                {
                    QRCode = true;
                    QRBox.IsChecked = true;
                }
            }
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            var res = MessageBox.Show("Вы уверены, что хотите отменить создание/редактирование торгового автомата?", "Отмена", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if(res == MessageBoxResult.Yes)
            {
                PageManager.MainFrame.Navigate(new VendingMachinesPage());
            }
        }

        private async void HoursBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            await IsHoursBoxCorrect();
        }

        private async Task<bool> IsHoursBoxCorrect()
        {
            if (HoursBox.Text.Length >= 11)
            {
                try
                {
                    var hours = HoursBox.Text;
                    if (!HoursBox.Text.Contains('-') || HoursBox.Text.Length > 11)
                    {
                        throw new();
                    }
                    var arrHours = hours.Split('-');

                    machine.StartHours = arrHours[0];
                    machine.EndHours = arrHours[1];
                    return true;
                }
                catch
                {
                    MessageBox.Show("Неправильно указаны часы работы. Пожалуйста, повторите ввод!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
            return false;
        }

        private void ManufacturerBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            var modelRange = models;
            if (ManufacturerBox.SelectedValue != null)
            {
                var selectedManufacturer = manufacturers.FirstOrDefault(m => m.ID == (long)ManufacturerBox.SelectedValue);
                if (selectedManufacturer != null)
                {
                    modelRange = models.Where(m => m.ManufacturerID == selectedManufacturer.ID).ToArray();
                    if (modelRange.Length > 0)
                    {
                        ModelBox.ItemsSource = modelRange;
                        ModelBox.SelectedValue = 0;
                    }
                }
            }
            if (ModelBox.SelectedValue != null)
            {
                machine.VendingMachineMatrix.ID = (long)ModelBox.SelectedValue;
            }
        }

        private void ModelBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ModelBox.SelectedValue != null)
            {
                machine.VendingMachineMatrix.ID = (long)ModelBox.SelectedValue;
            }
        }

        private void OperatingModeBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            machine.OperatingModeID = (long)OperatingModeBox.SelectedValue;
        }

        private void TimeZoneBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            machine.TimeZone = TimeZoneBox.SelectedValue.ToString();
        }

        private async Task<bool> CheckData()
        {
            if (!await IsHoursBoxCorrect())
            {
                return false;
            }
            if (machine.Name.Length == 0) return false;
            else if (machine.VendingMachineMatrix.ID == 0) return false;
            else if (machine.OperatingModeID == 0) return false;
            else if (machine.TimeZone == null) return false;
            else if (machine.Adress.Length == 0) return false;
            else if (machine.PlacementType.Length == 0) return false;
            else if (machine.StartHours.Length == 0) return false;
            else if (machine.EndHours.Length == 0) return false;
            else if (CoinSlot == false && BankNotesSlot == false
                && NoContactPaymentSlot == false && QRCode == false) return false;
            return true;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if(isNewMachine)
            {
                try
                {
                    if(!await CheckData()) throw new();

                    var vendingM = new VendingMachineCreateDTO()
                    {
                        Name = machine.Name,
                        ID = machine.ID,
                        StatusID = 2,
                        OperatingModeID = machine.OperatingModeID,
                        PlacementType = machine.PlacementType,
                        Adress = machine.Adress,
                        StartHours = machine.StartHours,
                        EndHours = machine.EndHours,
                        TimeZone = machine.TimeZone,
                        CompanyID = machine.CompanyID,
                        Coordinates = machine.Coordinates,
                        PlacementDate = machine.PlacementDate,
                        ModelID = machine.VendingMachineMatrix.ID,
                        ModemID = machine.ModemID
                    };


                    var result = await VendingMachinesService.CreateVendingMachineAsync(vendingM);

                    if (result.ID != 0)
                    {
                        if (CoinSlot == true)//3
                        {
                            var res = await MachinePaymentService.CreateMachinePaymentMethodeAsync(result.ID, 3);
                            if (!res)
                            {
                                MessageBox.Show("Не удалось создать слот для монет, пожалуйста, повторите попытку.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                return;
                            }

                        }
                        if (BankNotesSlot == true) //2
                        {
                            var res = await MachinePaymentService.CreateMachinePaymentMethodeAsync(result.ID, 2);
                            if (!res)
                            {
                                MessageBox.Show("Не удалось создать слот для купюр, пожалуйста, повторите попытку.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                return;
                            }
                        }
                        if (NoContactPaymentSlot == true)//1 безнал
                        {
                            var res = await MachinePaymentService.CreateMachinePaymentMethodeAsync(result.ID, 1);
                            if (!res)
                            {
                                MessageBox.Show("Не удалось создать слот для безналичной оплаты, пожалуйста, повторите попытку.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                return;
                            }
                        }
                        if (QRCode == true)//4 код
                        {
                            var res = await MachinePaymentService.CreateMachinePaymentMethodeAsync(result.ID, 4);
                            if (!res)
                            {
                                MessageBox.Show("Не удалось создать QR оплату, пожалуйста, повторите попытку.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                return;
                            }

                        }
                        MessageBox.Show("Торговый автомат успешно создан!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    
                    PageManager.MainFrame.Navigate(new VendingMachinesPage());
                }
                catch
                {
                    MessageBox.Show("Не удалось создать торговый автомат, пожалуйста, проверьте введенные данные и повторите попытку.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                try
                {
                    if (!await CheckData()) throw new();

                    var vendingM = new VendingMachineCreateDTO()
                    {
                        Name = machine.Name,
                        ID = machine.ID,
                        StatusID = 2,
                        OperatingModeID = machine.OperatingModeID,
                        PlacementType = machine.PlacementType,
                        Adress = machine.Adress,
                        StartHours = machine.StartHours,
                        EndHours = machine.EndHours,
                        TimeZone = machine.TimeZone,
                        CompanyID = machine.CompanyID,
                        Coordinates = machine.Coordinates,
                        PlacementDate = machine.PlacementDate,
                        ModelID = machine.VendingMachineMatrix.ID,
                        ModemID = machine.ModemID
                    };


                    var result = await VendingMachinesService.UpdateVendingMachineAsync(vendingM.ID, vendingM);

                    if (result != null)
                    {
                        if (CoinSlot == true && !machinePaymentMethods.Any(e => e.VendingMachineID==result.ID && e.PaymentMethodID == 2))
                        {
                            var res = await MachinePaymentService.CreateMachinePaymentMethodeAsync(result.ID, 3);
                        }
                        if (BankNotesSlot == true && !machinePaymentMethods.Any(e => e.VendingMachineID == result.ID && e.PaymentMethodID ==3))
                        {
                            var res = await MachinePaymentService.CreateMachinePaymentMethodeAsync(result.ID, 2);
                        }
                        if (NoContactPaymentSlot == true && !machinePaymentMethods.Any(e => e.VendingMachineID == result.ID && e.PaymentMethodID == 4))
                        {
                            var res = await MachinePaymentService.CreateMachinePaymentMethodeAsync(result.ID, 1);
                        }
                        if (QRCode == true && !machinePaymentMethods.Any(e => e.VendingMachineID == result.ID && e.PaymentMethodID == 1))
                        {
                            var res = await MachinePaymentService.CreateMachinePaymentMethodeAsync(result.ID, 4);
                        }

                        //Удаление
                        if (CoinSlot == false && machinePaymentMethods.Any(e => e.VendingMachineID == result.ID && e.PaymentMethodID == 2))
                        {
                            var res = await MachinePaymentService.DeleteMachinePaymentMethodeAsync(result.ID, 3);
                        }
                        if (BankNotesSlot == false && machinePaymentMethods.Any(e => e.VendingMachineID == result.ID && e.PaymentMethodID == 3))
                        {
                            var res = await MachinePaymentService.DeleteMachinePaymentMethodeAsync(result.ID, 2);
                        }
                        if (NoContactPaymentSlot == false && machinePaymentMethods.Any(e => e.VendingMachineID == result.ID && e.PaymentMethodID == 4))
                        {
                            var res = await MachinePaymentService.DeleteMachinePaymentMethodeAsync(result.ID, 1);
                        }
                        if (QRCode == false && machinePaymentMethods.Any(e => e.VendingMachineID == result.ID && e.PaymentMethodID == 1))
                        {
                            var res = await MachinePaymentService.DeleteMachinePaymentMethodeAsync(result.ID, 4);
                        }
                        MessageBox.Show("Торговый автомат успешно обновлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        PageManager.MainFrame.Navigate(new VendingMachinesPage());
                    }
                    else
                    {
                        throw new();
                    } 
                }
                catch
                {
                    MessageBox.Show("Не удалось обновить торговый автомат, пожалуйста, проверьте введенные данные и повторите попытку.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void CoinBox_Checked(object sender, RoutedEventArgs e)
        {
            CoinSlot = true;
        }

        private void CoinBox_Unchecked(object sender, RoutedEventArgs e)
        {
            CoinSlot = false;
        }

        private void BankNotesBox_Checked(object sender, RoutedEventArgs e)
        {
            BankNotesSlot = true;
        }

        private void BankNotesBox_Unchecked(object sender, RoutedEventArgs e)
        {
            BankNotesSlot = false;
        }

        private void NoContactBox_Checked(object sender, RoutedEventArgs e)
        {
            NoContactPaymentSlot = true;
        }

        private void NoContactBox_Unchecked(object sender, RoutedEventArgs e)
        {
            NoContactPaymentSlot = false;
        }

        private void QRBox_Checked(object sender, RoutedEventArgs e)
        {
            QRCode = true;
        }

        private void QRBox_Unchecked(object sender, RoutedEventArgs e)
        {
            QRCode = false;
        }

    }
}
