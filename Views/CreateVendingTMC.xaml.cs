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
    /// Interaction logic for CreateVendingTMC.xaml
    /// </summary>
    public partial class CreateVendingTMC : Page
    {
        public bool isNew = true;
        User curUser;
        VendingMachine machine;
        VendingAvaliability va;
        Product[] products;

        public CreateVendingTMC(VendingAvaliability va, VendingMachine vendingMachine)
        {

            InitializeComponent();
            this.va = va;
            machine = vendingMachine;

            GetEssentialStuff();
        }
        private async void GetEssentialStuff()
        {
            curUser = await LoginOperation.GetCurrentUserAsync();

            if (curUser.CompanyID != 0)
            {
                var venAv = await VendingAvaliabilityService.GetVendingAvaliabilitiesAsync(machine.ID);
                var res = await ProductService.GetProductsAsync();
                products = res.Products.ToArray();

                var listedProducts = products.Where(p => !venAv.Any(v => v.ProductID == p.ID 
                        && v.ProductID != va.ProductID)).ToList();

                if(va.ID != 0) 
                {
                    isNew = false;
                    PageName.Text = "Редактирование торговой позиции";
                }

                
                ProductBox.DisplayMemberPath = "Name";
                ProductBox.SelectedValuePath = "ID";
                ProductBox.ItemsSource = listedProducts;
                DataContext = va;
            }
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            var res = MessageBox.Show("Вы уверены, что хотите отменить создание/редактирование?", "Отмена", MessageBoxButton.YesNo, MessageBoxImage.Question);
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
            var result = MessageBox.Show($"Вы точно хотите сохранить?", "Сохранение", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    if (isNew)
                    {
                        va.VendingMachineID = machine.ID;

                        var res = await VendingAvaliabilityService.CreateVendingAvaliabilityAsync(va);

                        if (res != null)
                        {
                            MessageBox.Show("Торговая позиция успешно создана.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                            if (PageManager.MainFrame.CanGoBack)
                            {
                                PageManager.MainFrame.GoBack();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Не удалось создать торговую позицию.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        var res = await VendingAvaliabilityService.UpdateVendingAvaliabilityAsync(va.ID, va);
                        if (res != null)
                        {
                            MessageBox.Show("Торговая позиция успешно изменена.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                            if (PageManager.MainFrame.CanGoBack)
                            {
                                PageManager.MainFrame.GoBack();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Не удалось изменить торговую позицию.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("Не удалось произвести изменения.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
