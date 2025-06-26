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
    /// Interaction logic for UserPage.xaml
    /// </summary>
    public partial class ProductPage : Page
    {
        bool isNewProduct = false;
        Product pr;
        public ProductPage(Product product)
        {
            InitializeComponent();

            pr = product;
            DataContext = pr;
            GetEssentialStuff();
        }
        private async void GetEssentialStuff()
        { 

            if (pr.ID == 0)
            {
                isNewProduct = true;
                ConfigureNewProduct();
            }
        }
        private void ConfigureNewProduct()
        {
            PageName.Text = "Создание товара";
            pr.AvgSales = 0;
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            var res = MessageBox.Show("Вы уверены, что хотите отменить создание/редактирование товара?", "Отмена", MessageBoxButton.YesNo, MessageBoxImage.Question);
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
            try
            {
                if (isNewProduct)
                {
                    if (pr.Name == null) throw new();
                    else if (pr.Description == null) throw new();

                    var res = await ProductService.CreateProductAsync(pr);
                    if (res is null)
                    {
                        throw new();
                    }
                    MessageBox.Show("Товар успешно сохранен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    if (PageManager.MainFrame.CanGoBack)
                    {
                        PageManager.MainFrame.Navigate(new ProductsPage());
                    }
                }
                else
                {
                    if (pr.Name == null) throw new();
                    else if (pr.Description == null) throw new();

                    var res = await ProductService.UpdateProductAsync(pr.ID, pr);
                    if (res is null)
                    {
                        throw new();
                    }
                    MessageBox.Show("Товар успешно сохранен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    if (PageManager.MainFrame.CanGoBack)
                    {
                        PageManager.MainFrame.Navigate(new ProductsPage());
                    }
                }
            }
            catch
            {
                MessageBox.Show("Произошла ошибка при сохранении товара. Пожалуйста, проверьте введенные данные и повторите попытку.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }
    }
}
