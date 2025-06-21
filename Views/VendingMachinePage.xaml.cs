using System;
using System.Collections.Generic;
using System.IO.Packaging;
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
                ConfigureNewMachine();
            }
            else OutputOldMachine();

           
        }
        private async void ConfigureNewMachine()
        {
            if (isNewMachine)
            {   
                isNewMachine = true;
                PageName.Text = "Создание торгового автомата";
            }
            machine.VendingMachineMatrix = new VendingMachineMatrix();
            machine.VendingMachineMatrix.Manufacturer = new Manufacturer();
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
                if (machinePaymentMethod.PaymentMethodID == 3)
                {
                    CoinSlot = true;
                    CoinBox.IsChecked = true;
                }
                else if (machinePaymentMethod.PaymentMethodID == 2)
                {
                    BankNotesSlot = true;
                    BankNotesBox.IsChecked = true;
                }
                else if (machinePaymentMethod.PaymentMethodID == 1)
                {
                    NoContactPaymentSlot = true;
                    NoContactBox.IsChecked = true;
                }
                else if (machinePaymentMethod.PaymentMethodID == 4)
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

        private void HoursBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (HoursBox.Text.Length >= 11)
            {
                try
                {
                    var hours = HoursBox.Text;
                    if(!HoursBox.Text.Contains('-') || HoursBox.Text.Length > 11)
                    {
                        throw new();
                    }
                    var arrHours = hours.Split('-');
                
                    machine.StartHours = arrHours[0].Trim();
                    machine.EndHours = arrHours[1].Trim();
                }
                catch
                {
                    MessageBox.Show("Неправильно указаны часы работы. Пожалуйста, повторите ввод!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
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
            machine.VendingMachineMatrix.ManufacturerID = (long?)ManufacturerBox.SelectedValue;

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

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if(isNewMachine)
            { 
                var result = await VendingMachinesService.CreateVendingMachineAsync(machine);
                if ( result == true)
                {
                    MessageBoxResult messageBoxResult = MessageBox.Show("Торговый автомат успешно создан!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Не удалось создать торговый автомат, пожалуйста, проверьте введенные данные и повторите попытку.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {

            }
        }
    }
}
