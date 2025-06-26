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
    public partial class VendingMachineTMC : Page
    {
        public VendingMachine vendingMachine;
        public VendingAvaliability[] vendingAvaliabilities;
        public int elementAmount;
        public VendingMachineTMC(VendingMachine va)
        {
            InitializeComponent();
            vendingMachine = va;
            Loaded += LoadPageStuff;
        }
        private async void LoadPageStuff(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.Windows
                .OfType<MainInterfaceWindow>()
                .FirstOrDefault();
            if (mainWindow != null)
            {
                mainWindow.CurrentPageTitle.Text = "Заполнение ТМЦ";
            }
            foreach (var column in DataGridTable.Columns)
            {
                column.Width = new DataGridLength(1, DataGridLengthUnitType.Auto);
            }

            await LoadAvaliability();
        }

        private async Task LoadAvaliability()
        {
            var curUser = await LoginOperation.GetCurrentUserAsync();

            if (curUser?.CompanyID == null)
            {
                return;
            }

            vendingAvaliabilities = await VendingAvaliabilityService.GetVendingAvaliabilitiesAsync(vendingMachine.ID);
            var res = await ProductService.GetProductsAsync();
            if(res != null)
            {
                Product[] products = res.Products.ToArray();

                foreach (var vend in vendingAvaliabilities)
                {
                    vend.Product = new();

                    var prodUpd = products.FirstOrDefault(p => p.ID == vend.ProductID);

                    vend.Name = prodUpd?.Name ?? "Неизвестный продукт";
                }
            }

            elementAmount = vendingAvaliabilities.Length;

            if (elementAmount > 0)
            {
                EntityAmountFound.Text = $"Всего найдено {elementAmount} шт";
                NoDataMessage.Visibility = Visibility.Collapsed;
            }
            DataGridTable.ItemsSource = null;
            DataGridTable.ItemsSource = vendingAvaliabilities;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            PageManager.MainFrame.Navigate(new CreateVendingTMC(new VendingAvaliability(), vendingMachine));
        }

        private void FilterBox_GotFocus(object sender, RoutedEventArgs e)
        {
            FilterBox.Text = string.Empty;
        }

        private void FilterBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var tbx = sender as TextBox;
            if (vendingAvaliabilities == null)
                return;
            if (tbx.Text != "")
            {
                var filtered = vendingAvaliabilities.Where(va =>
                va.ID.ToString().Contains(tbx.Text, StringComparison.OrdinalIgnoreCase) ||
                va.Name.Contains(tbx.Text, StringComparison.OrdinalIgnoreCase) ||
                va.Price.ToString().Contains(tbx.Text, StringComparison.OrdinalIgnoreCase) ||
                va.Quantity.ToString().Contains(tbx.Text, StringComparison.OrdinalIgnoreCase)
                )
                    .ToList();
                DataGridTable.ItemsSource = filtered;
            }
            else
            {
                DataGridTable.ItemsSource = vendingAvaliabilities;
            }
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is VendingAvaliability va)
            {
               PageManager.MainFrame.Navigate(new CreateVendingTMC(va, vendingMachine));
            }
        }

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is VendingAvaliability va)
            {
                var result = MessageBox.Show($"Вы точно хотите удалить?", "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        var returnedResult = await ProductService.DeleteProductAsync(va.ID);
                        if (returnedResult)
                        {
                            MessageBox.Show("Запись удалена успешно.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show("Не удалось удалить запись. Возможно, элемент используется в других операциях.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    catch
                    {
                        MessageBox.Show("Не удалось удалить запись.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    await LoadAvaliability();
                }
            }
        }

        private void ProductButton_Click(object sender, RoutedEventArgs e)
        {
            PageManager.MainFrame.Navigate(new ProductsPage());
        }
    }
}
