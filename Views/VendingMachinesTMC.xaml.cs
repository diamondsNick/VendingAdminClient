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
    public partial class VendingMachinesTMC : Page
    {
        public static int page = 1;
        public static int pageAmount = 0;
        public static int selectedAmount;
        public static int entityAmount = 0;
        public static int objOnPage = 0;
        public PagedMachinesResult? vendingMachines { get; set; }
        public VendingMachinesTMC()
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
                mainWindow.CurrentPageTitle.Text = "Учет ТМЦ";
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
            objOnPage = pr.VendingMachines.Count();
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

        private void DataGridTable_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is DataGrid grid)
            {
                if(grid.SelectedItem != null && grid.SelectedItem is VendingMachine va)
                { 

                    PageManager.MainFrame.Navigate(new VendingMachineTMC(va));
                }
            }
        }
    }
}
