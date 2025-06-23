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
using AdminClient.Models;
using AdminClient.Services;

namespace AdminClient.Views
{
    /// <summary>
    /// Interaction logic for ModemPage.xaml
    /// </summary>
    public partial class ModemPage : Page
    {
        bool isNewModem = false;
        User curUser;
        Modem modem;

        public ModemPage(Modem modemInfo)
        {

            InitializeComponent();

            modem = modemInfo;

            DataContext = modem;

            GetEssentialStuff();
        }
        private async void GetEssentialStuff()
        {
            curUser = await LoginOperation.GetCurrentUserAsync();

            if (curUser.CompanyID != 0)
            {
                CompanyNameBox.Text = curUser.Company.Name;
            }

            if (modem.ID == 0)
            {
                isNewModem = true;
                ConfigureNewModem();
            }
        }
        private async void ConfigureNewModem()
        {
            PageName.Text = "Создание модема";
            modem = new Modem();
            DataContext = modem;
            modem.Company = new Company();
            modem.Company = curUser.Company;
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            var res = MessageBox.Show("Вы уверены, что хотите отменить создание/редактирование модема?", "Отмена", MessageBoxButton.YesNo, MessageBoxImage.Question);
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
                if (isNewModem)
                {
                    if (modem.SerialNum == null) throw new();
                    else if (modem.Password == null) throw new();
                    else if (modem.Model == null) throw new();
                    else if (modem.Company == null) throw new();
                        var res = await ModemService.CreateModemAsync(modem);
                    if (res is null)
                    {
                        throw new();
                    }
                    MessageBox.Show("Модем успешно сохранен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    if (PageManager.MainFrame.CanGoBack)
                    {
                        PageManager.MainFrame.Navigate(new ModemsPage());
                    }
                }
                else
                {

                    if (modem.SerialNum == null) throw new();
                    else if (modem.Password == null) throw new();
                    else if (modem.Model == null) throw new();
                    var res = await ModemService.UpdateModemAsync(modem.ID, modem);
                    if (res == false)
                    {
                        throw new();
                    }
                    MessageBox.Show("Модем успешно сохранен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    if (PageManager.MainFrame.CanGoBack)
                    {
                        PageManager.MainFrame.Navigate(new ModemsPage());
                    }
                }
            }
            catch
            {
                MessageBox.Show("Произошла ошибка при сохранении модема. Пожалуйста, проверьте введенные данные и повторите попытку.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private void RemoveSim_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AddSim_Click(object sender, RoutedEventArgs e)
        {

        }

        private void RedactSim_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
