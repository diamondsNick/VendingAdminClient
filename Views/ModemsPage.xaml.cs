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
using AdminClient.DTOs;
using AdminClient.Models;
using AdminClient.Services;

namespace AdminClient.Views
{
    /// <summary>
    /// Interaction logic for ModemsPage.xaml
    /// </summary>
    public partial class ModemsPage : Page
    {
        public static int page = 1;
        public static int pageAmount = 0;
        public static int selectedAmount;
        public static int entityAmount = 0;
        public static int objOnPage = 0;
        PagedModems? Modems { get; set; }
        public ModemsPage()
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
                mainWindow.CurrentPageTitle.Text = "Модемы";
            }
            foreach (var column in DataGridTable.Columns)
            {
                column.Width = new DataGridLength(1, DataGridLengthUnitType.Auto);
            }

            await LoadModemData();
        }

        private async Task LoadModemData()
        {
            var curUser = await LoginOperation.GetCurrentUserAsync();


            if (curUser?.CompanyID == null)
            {
                return;
            }

            UpdateAmountOnBox();

            Modems = await ModemService.GetPagedModemsAsync(selectedAmount, page, (long)curUser.CompanyID);

            if (Modems != null)
            {
                NoDataMessage.Visibility = Visibility.Hidden;
                UpdatePageCounters(Modems);
            }
            else NoDataMessage.Visibility = Visibility.Visible;

            var machines = await VendingMachinesService.GetVendingMachinesPagedAsync((long)curUser.CompanyID, 1500, 1);

            var ModemsArr = Modems.Modems;

            foreach (var Modem in ModemsArr)
            {
                var info = machines.VendingMachines.FirstOrDefault(vm => vm.ModemID == Modem.ID);
                if (info != null)
                {
                    Modem.VendingMachine = new VendingMachine();
                    Modem.VendingMachine.Name = info.Name;
                    Modem.VendingMachine.ID = info.ID;
                    Modem.VendingMachine.VendingMachineMatrix = new VendingMachineMatrix();
                    Modem.VendingMachine.VendingMachineMatrix = info.VendingMachineMatrix;
                    Modem.VendingMachine.VendingMachineMatrix.Manufacturer = new();
                    Modem.VendingMachine.VendingMachineMatrix.Manufacturer = info.VendingMachineMatrix.Manufacturer;
                }
                else
                {
                    Modem.VendingMachine = new VendingMachine();
                    Modem.VendingMachine.Name = "Нет информации";
                    Modem.VendingMachine.ID = 0;
                }
                Modem.Company = new Company();
                Modem.Company.Name = curUser.Company?.Name ?? "Нет информации";
                Modem.Company.ID = (long)curUser.CompanyID;

            }
            DataGridTable.ItemsSource = ModemsArr;
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
            await LoadModemData();
        }

        private void UpdatePageCounters(PagedModems pr)
        {
            PageNum.Text = page.ToString();
            entityAmount = pr.TotalCount;
            objOnPage = pr.Modems.Count();
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
            else if (entityAmount < selectedAmount)
            {
                MachCountFromTo.Text = $"Записи с 1 до {objOnPage} из {entityAmount} записей";
            }
            else
            {
                MachCountFromTo.Text = $"Записи с {page * selectedAmount - selectedAmount + 1} до {selectedAmount * (page - 1) + objOnPage} из {entityAmount} записей";
            }

        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            PageManager.MainFrame.Navigate(new ModemPage(new()));
        }

        private async void PrevPage_Click(object sender, RoutedEventArgs e)
        {
            if (page > 1 && pageAmount > 1)
            {
                page--;
                await LoadModemData();
            }
        }

        private async void NextPage_Click(object sender, RoutedEventArgs e)
        {
            if (page >= 1 && pageAmount > 1)
            {
                page++;
                await LoadModemData();
            }
        }

        private void FilterBox_GotFocus(object sender, RoutedEventArgs e)
        {
            FilterBox.Text = string.Empty;
        }

        private void FilterBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var tbx = sender as TextBox;
            if (Modems?.Modems == null)
                return;
            if (tbx.Text != "")
            {
                var filtered = Modems.Modems.Where(vm =>
                vm.ID.ToString().Contains(tbx.Text, StringComparison.OrdinalIgnoreCase) ||
                vm.Model.Contains(tbx.Text, StringComparison.OrdinalIgnoreCase) ||
                vm.SerialNum.ToString().Contains(tbx.Text, StringComparison.OrdinalIgnoreCase) ||
                vm.Password.Contains(tbx.Text, StringComparison.OrdinalIgnoreCase) ||
                vm.VendingMachineName.Contains(tbx.Text, StringComparison.OrdinalIgnoreCase) ||
                vm.CompanyName.Contains(tbx.Text, StringComparison.OrdinalIgnoreCase)
                )
                    .ToList();
                DataGridTable.ItemsSource = filtered;
            }
            else
            {
                DataGridTable.ItemsSource = Modems.Modems;
            }
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Modem modem)
            {
                PageManager.MainFrame.Navigate(new ModemPage(modem));
            }
        }

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Modem selectedModem)
            {
                var result = MessageBox.Show($"Вы точно хотите удалить {selectedModem.Model}?", "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        var returnedResult = await ModemService.DeleteModemAsync(selectedModem.ID);
                        if (returnedResult)
                        {
                            MessageBox.Show("Модем успешно удален.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                            LoadModemData();
                        }
                        else
                        {
                            MessageBox.Show("Не удалось удалить модем!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                    }
                    catch
                    {
                        MessageBox.Show("Не удалось удалить модем.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                }
            }
            await LoadModemData();
        }
        private async void UnbindModel_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Modem modem && modem.VendingMachine.ID != 0)
            {
                var result = MessageBox.Show($"Вы точно хотите отвязать торговый автомат {modem.VendingMachineName} от модема?", "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        var vendMachine = await VendingMachinesService.GetVendingMachinesAsync();
                        var existingMachine = vendMachine.FirstOrDefault(vm => vm.ID == modem.VendingMachine.ID);

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
                            ModemID = null
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
                    await LoadModemData();
                }
            }
            else if (sender is Button buttonPr && buttonPr.DataContext is Modem modemCl && modemCl.VendingMachine.ID == 0)
            {
                PageManager.MainFrame.Navigate(new LinkModem(modemCl));
            }
        }

    }
}
