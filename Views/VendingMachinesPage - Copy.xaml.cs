using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.PortableExecutable;
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
    /// Interaction logic for VendingMachinesPage.xaml  
    /// </summary>  
    public partial class VendingMachinesPage : Page
    {
        public static int page = 1;
        public static int pageAmount = 0;
        public static int selectedAmount;
        public static int entityAmount = 0;
        public PagedMachinesResult? vendingMachines { get; set; }
        public VendingMachinesPage()
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
                mainWindow.CurrentPageTitle.Text = "Торговые автоматы";
            }
            foreach (var column in DataGridTable.Columns)
            {
                column.Width = new DataGridLength(1, DataGridLengthUnitType.Auto);
            }

            await LoadMachineData();
        }

        private async Task LoadMachineData()
        {
            var curUser = await Services.LoginOperation.GetCurrentUserAsync();

            if (curUser?.CompanyID == null)
            {
                return;
            }

            UpdateAmountOnBox();

            vendingMachines = 
                await Services.VendingMachinesService.GetVendingMachinesPagedAsync((long)curUser.CompanyID, selectedAmount, page);
            if (vendingMachines != null)
            {
                NoDataMessage.Visibility = Visibility.Collapsed;
                UpdatePageCounters(vendingMachines);
                DataGridTable.ItemsSource = vendingMachines?.VendingMachines;
            }
        }
        private void UpdateAmountOnBox()
        {

            if (ValueBox.SelectedItem is ComboBoxItem item)
            {
                if (item.Content.ToString() == "Все") selectedAmount = 150000;
                else
                {
                    selectedAmount = Convert.ToInt32(item.Content.ToString());
                }
            }
        }

        private async void ValueBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateAmountOnBox();
            await LoadMachineData();
        }

        private void UpdatePageCounters(PagedMachinesResult pr)
        {
            PageNum.Text = page.ToString();
            entityAmount = pr.TotalCount;

            EntityAmountFound.Text = $"Всего найдено {entityAmount} шт.";


            if (selectedAmount > entityAmount)
            {
                pageAmount = 1;
            }
            else
            {
                pageAmount = (int)Math.Ceiling((double)entityAmount / selectedAmount);
            }

            
            if (pr.TotalCount == 0)
            {
                MachCountFromTo.Text = "Записи с 0 до 0 из 0 записей";
            }
            else if (page == 1 && selectedAmount > entityAmount)
            {
                MachCountFromTo.Text = $"Записи с 1 до {pr.TotalCount.ToString()} из {entityAmount} записей";
            }
            else
            {
                int entCountOst = 0;
                if(selectedAmount > entityAmount) entCountOst = entityAmount;
                else entCountOst = selectedAmount;
                MachCountFromTo.Text = $"Записи с {page * pageAmount + 1} до {entCountOst} из {entityAmount} записей";
            }
                
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            PageManager.MainFrame.Navigate(new VendingMachinePage(new VendingMachine()));
        }

        private async void PrevPage_Click(object sender, RoutedEventArgs e)
        {
            if(page > 1 && pageAmount > 1)
            {
                page--;
                await LoadMachineData();
            }
        }

        private async void NextPage_Click(object sender, RoutedEventArgs e)
        {
            if (page >= 1 && pageAmount > 1)
            {
                page++;
                await LoadMachineData();
            }
        }

        private void FilterBox_GotFocus(object sender, RoutedEventArgs e)
        {
            FilterBox.Text = string.Empty;
        }

        private void FilterBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var tbx = sender as TextBox;
            if (vendingMachines?.VendingMachines == null)
                return;
            if (tbx.Text != "")
            {
                var filtered = vendingMachines.VendingMachines.Where(vm =>
                vm.ID.ToString().Contains(tbx.Text, StringComparison.OrdinalIgnoreCase) ||
                vm.Name.Contains(tbx.Text, StringComparison.OrdinalIgnoreCase) ||
                vm.ModelName.Contains(tbx.Text, StringComparison.OrdinalIgnoreCase) ||
                vm.CompanyName.Contains(tbx.Text, StringComparison.OrdinalIgnoreCase) ||
                vm.ModelID.ToString().Contains(tbx.Text, StringComparison.OrdinalIgnoreCase) ||
                vm.FullAdress.Contains(tbx.Text, StringComparison.OrdinalIgnoreCase) ||
                vm.PlacementDate.Contains(tbx.Text, StringComparison.OrdinalIgnoreCase)
                )
                    .ToList();
                DataGridTable.ItemsSource = filtered;
            }
            else
            {
                DataGridTable.ItemsSource = vendingMachines.VendingMachines;
            }
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is VendingMachine machine)
            {
                PageManager.MainFrame.Navigate(new VendingMachinePage(machine));
            }                
        }

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            if(sender is Button button && button.DataContext is VendingMachine machine)
            {
                var result = MessageBox.Show($"Вы точно хотите удалить торговый автомат {machine.Name}?", "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if(result == MessageBoxResult.Yes)
                {
                    try
                    {
                        var returnedResult = await Services.VendingMachinesService.DeleteVendingMachineAsync(machine.ID);
                        if (returnedResult)
                        {
                            MessageBox.Show("Торговый автомат успешно удален.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show("Не удалось удалить торговый автомат. Возможно, он используется в других операциях.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    catch
                    {
                        MessageBox.Show("Не удалось удалить торговый автомат.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    await LoadMachineData();
                }
            }
        }

        private async void UnbindModel_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is VendingMachine machine && machine.Modem != null)
            {
                var result = MessageBox.Show($"Вы точно хотите отвязать торговый автомат {machine.Name} от модема?", "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        machine.ModemID = null;
                        machine.Modem = null;

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


                        var returnedResult = await Services.VendingMachinesService.UpdateVendingMachineAsync(vendingM.ID, vendingM);
                        if (returnedResult.ID != null)
                        {
                            MessageBox.Show("Торговый автомат успешно отвязан.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show("Не удалось отвязать модем.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    catch
                    {
                        MessageBox.Show("Не удалось отвязать модем.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    await LoadMachineData();
                }
            }
        }

        private void DataGridTable_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
