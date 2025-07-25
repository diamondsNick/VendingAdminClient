﻿using System;
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
    /// Interaction logic for ProductsPage.xaml
    /// </summary>
    public partial class ProductsPage : Page
    {
        public static int page = 1;
        public static int pageAmount = 0;
        public static int selectedAmount;
        public static int entityAmount = 0;
        public static int objOnPage = 0;
        User curUser { get; set; }
        ProductDTO product { get; set; }
        public ProductsPage()
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
                mainWindow.CurrentPageTitle.Text = "Товары";
            }
            foreach (var column in DataGridTable.Columns)
            {
                column.Width = new DataGridLength(1, DataGridLengthUnitType.Auto);
            }

            await LoadProductData();
        }

        private async Task LoadProductData()
        {
            curUser = await LoginOperation.GetCurrentUserAsync();


            if (curUser?.CompanyID == null)
            {
                return;
            }

            UpdateAmountOnBox();

            product = await ProductService.GetProductsAsync(selectedAmount, page);

            if (product != null)
            {
                NoDataMessage.Visibility = Visibility.Hidden;
                UpdatePageCounters(product);
            }
            else NoDataMessage.Visibility = Visibility.Visible;
            DataGridTable.ItemsSource = product.Products;

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
            await LoadProductData();
        }

        private void UpdatePageCounters(ProductDTO pr)
        {
            PageNum.Text = page.ToString();
            entityAmount = pr.TotalCount;
            objOnPage = pr.Products.Count();
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
            PageManager.MainFrame.Navigate(new ProductPage(new()));
        }

        private async void PrevPage_Click(object sender, RoutedEventArgs e)
        {
            if (page > 1 && pageAmount > 1)
            {
                page--;
                await LoadProductData();
            }
        }

        private async void NextPage_Click(object sender, RoutedEventArgs e)
        {
            if (page >= 1 && pageAmount > 1 && page < pageAmount)
            {
                page++;
                await LoadProductData();
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
            if (product.Products == null)
                return;
            if (tbx.Text != "")
            {
                var filtered = product.Products.Where(vm =>
                vm.ID.ToString().Contains(tbx.Text, StringComparison.OrdinalIgnoreCase) ||
                vm.Name.Contains(tbx.Text, StringComparison.OrdinalIgnoreCase) ||
                vm.Description.Contains(tbx.Text, StringComparison.OrdinalIgnoreCase) ||
                vm.AvgSales.ToString().Contains(tbx.Text, StringComparison.OrdinalIgnoreCase)
                )
                    .ToList();
                DataGridTable.ItemsSource = filtered;
            }
            else
            {
                DataGridTable.ItemsSource = product.Products;
            }
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Product product)
            {
                PageManager.MainFrame.Navigate(new ProductPage(product));
            }
        }

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Product product)
            {
                var result = MessageBox.Show($"Вы точно хотите удалить {product.Name}?", "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        var returnedResult = await ProductService.DeleteProductAsync(product.ID);
                        if (returnedResult)
                        {
                            MessageBox.Show("Товар успешно удален.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show("Не удалось удалить Товар. Возможно, он используется в других операциях.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                    }
                    catch
                    {
                        MessageBox.Show("Не удалось удалить Товар.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                }
            }
            await LoadProductData();
        }
    }
}
