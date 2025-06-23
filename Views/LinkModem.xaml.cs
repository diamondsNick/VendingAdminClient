using System;
using System.Collections.Generic;
using System.Globalization;
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
using AdminClient.DTOs;
using AdminClient.Models;
using AdminClient.Services;

namespace AdminClient.Views
{
    /// <summary>
    /// Interaction logic for LinkModem.xaml
    /// </summary>
    public partial class LinkModem : Page
    {
        User curUser;
        List<VendingMachine> vendingMachines;
        long selectedMachineID;
        Modem inputedModem;

        public LinkModem(Modem modem)
        {

            InitializeComponent();
            inputedModem = modem;
            DataContext = inputedModem;

            GetEssentialStuff();
        }
        private async void GetEssentialStuff()
        {
            curUser = await LoginOperation.GetCurrentUserAsync();

            if (curUser.CompanyID != 0)
            {
                var vendingMachinesInfo = await VendingMachinesService.GetVendingMachinesPagedAsync((long)curUser.CompanyID, 1500, 1);
                vendingMachines = vendingMachinesInfo.VendingMachines;

                var filtered = vendingMachines.Where(vm => vm.ModemID == null).ToList();

                MachineBox.ItemsSource = filtered;
                MachineBox.DisplayMemberPath = "Name";
                MachineBox.SelectedValuePath = "ID";
            }
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            var res = MessageBox.Show("Вы уверены, что хотите отменить привязку модема?", "Отмена", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res == MessageBoxResult.Yes)
            {
                if (PageManager.MainFrame.CanGoBack)
                {
                    PageManager.MainFrame.GoBack();
                }
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show($"Вы точно хотите связать {inputedModem.Model}?", "Привязка", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var existingMachine = vendingMachines.FirstOrDefault(vm => vm.ID == selectedMachineID);

                    var vendingM = new VendingMachineCreateDTO()
                    {
                        Name = existingMachine.Name,
                        ID = existingMachine.ID,
                        StatusID = 2,
                        OperatingModeID = existingMachine.OperatingModeID,
                        PlacementType = existingMachine.PlacementType,
                        Adress = existingMachine.Adress,
                        StartHours = existingMachine.StartHours,
                        EndHours = existingMachine.EndHours,
                        TimeZone = existingMachine.TimeZone,
                        CompanyID = existingMachine.CompanyID,
                        Coordinates = existingMachine.Coordinates,
                        PlacementDate = existingMachine.PlacementDate,
                        ModelID = existingMachine.VendingMachineMatrix.ID,
                        ModemID = inputedModem.ID
                    };

                    var returnedResult = await Services.VendingMachinesService.UpdateVendingMachineAsync(vendingM.ID, vendingM);
                    if (returnedResult.ID != null)
                    {
                        MessageBox.Show("Торговый автомат успешно привязан.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        if (PageManager.MainFrame.CanGoBack)
                        {
                            PageManager.MainFrame.GoBack();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Не удалось привязать модем.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch
                {
                    MessageBox.Show("Не удалось отвязать модем.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void MachineBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedMachineID = (long)MachineBox.SelectedValue;
        }
    }
}
