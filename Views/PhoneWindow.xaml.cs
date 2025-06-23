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
using System.Windows.Shapes;
using AdminClient.Models;
using AdminClient.Services;

namespace AdminClient.Views
{
    /// <summary>
    /// Interaction logic for PhoneWindow.xaml
    /// </summary>
    public partial class PhoneWindow : Window
    {
        bool isNewSim = false;
        User curUser;
        SimCard sim;

        public PhoneWindow(SimCard simInfo)
        {

            InitializeComponent();

            sim = simInfo;

            DataContext = sim;

            GetEssentialStuff();
        }
        private async void GetEssentialStuff()
        {
            curUser = await LoginOperation.GetCurrentUserAsync();

            if (sim.ID == 0)
            {
                isNewSim = true;
                ConfigureNewModem();
            }
        }
        private async void ConfigureNewModem()
        {
            PageName.Text = "Создание SIM";
            sim = new SimCard();
            DataContext = sim;
            sim.Company = new Company();
            sim.Company = curUser.Company;
            sim.CompanyID = curUser.CompanyID;
            sim.Balance = 0;
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            var res = MessageBox.Show("Вы уверены, что хотите отменить создание/редактирование SIM?", "Отмена", MessageBoxButton.YesNo, MessageBoxImage.Question);
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
                if (isNewSim)
                {
                    if (sim.Number == null) throw new();
                    else if (sim.Vendor == null) throw new();
                    var res = await SIMService.CreateSIMAsync(sim);
                    if (res is null)
                    {
                        throw new();
                    }
                    MessageBox.Show("SIM успешно сохранен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    if (PageManager.MainFrame.CanGoBack)
                    {
                        PageManager.MainFrame.Navigate(new ModemsPage());
                    }
                }
                else
                {

                    if (sim.Number == null) throw new();
                    else if (sim.Vendor == null) throw new();
                    var res = await SIMService.UpdateSIMAsync(sim.ID, sim);
                    if (res == false)
                    {
                        throw new();
                    }
                    MessageBox.Show("SIM успешно сохранен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    if (PageManager.MainFrame.CanGoBack)
                    {
                        PageManager.MainFrame.Navigate(new ModemsPage());
                    }
                }
            }
            catch
            {
                MessageBox.Show("Произошла ошибка при сохранении SIM. Пожалуйста, проверьте введенные данные и повторите попытку.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }
    }
}
