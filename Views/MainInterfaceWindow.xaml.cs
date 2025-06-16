using System;
using System.Collections.Generic;
using System.IO;
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
using AdminClient.Services;

namespace AdminClient.Views
{
    /// <summary>
    /// Interaction logic for MainInterfaceWindow.xaml
    /// </summary>
    public partial class MainInterfaceWindow : Window
    {
        public MainInterfaceWindow()
        {
            InitializeComponent();
            Loaded += MainInterfaceWindow_Loaded;
        }

        private async void MainInterfaceWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var curUser = Services.LoginOperation.GetCurrentUserAsync().Result;
            CompanyNameBox.Text = "";
            if (curUser.Company.Name != null) CompanyNameBox.Text = curUser.Company.Name;

            LanguageImg.Source = new BitmapImage(new Uri($"/Resources/ru.png", UriKind.Relative));
            if (!File.Exists($"/Resources/{curUser.Language}.png"))
            {
                LanguageImg.Source = new BitmapImage(new Uri($"/Resources/{curUser.Language}.png", UriKind.Relative));
            }

            var userInfoArr = curUser.FullName.Split();

            if (userInfoArr.Length > 0)
            {
                UserInfo.Text = userInfoArr[0] + (userInfoArr.Length > 1 ? " " + userInfoArr[1][0] + "." : "")
                    + (userInfoArr.Length > 2 ? " " + userInfoArr[2][0] + "." : "");
            }
            if (curUser.Role != null)
            {
                UserRole.Text = curUser.Role.Name;
            }

            PageManager.MainFrame = MainFrame;
            PageManager.MainFrame.Navigate(new MainPage());
        }

        private void DropdownButton_Click(object sender, RoutedEventArgs e)
        {
            DropdownPopup.IsOpen = !DropdownPopup.IsOpen;
            if (DropdownPopup.IsOpen)
            {
                MenuPic.Source = new BitmapImage(new Uri("/Resources/arrowUP.png", UriKind.Relative));
            }
            else
            {
                MenuPic.Source = new BitmapImage(new Uri("/Resources/arrow.png", UriKind.Relative));
            }
        }
        private void DropdownPopup_Closed(object sender, EventArgs e)
        {
            MenuPic.Source = new BitmapImage(new Uri("/Resources/arrow.png", UriKind.Relative));
        }

        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            PageManager.MainFrame.Navigate(new ProfilePage());
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Вы точно хотите выйти из системы? Все несохраненные изменения будут потеряны!", "Выход из системы", MessageBoxButton.YesNo, MessageBoxImage.Information);

            if (result == MessageBoxResult.Yes)
            {
                LoginWindow loginWindow = new LoginWindow();
                loginWindow.Show();
                this.Close();
            }
        }

        private void MainWindowButton_Click(object sender, RoutedEventArgs e)
        {
            PageManager.MainFrame.Navigate(new MainPage());
        }


        private void MonitorTA_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Administrate_Click(object sender, RoutedEventArgs e)
        {
            if(AdminDropPanel.Visibility == Visibility.Visible) CloseAdminDropdownMenu();
            else OpenAdminDropdownMenu();
        }

        private void OpenAdminDropdownMenu()
        {
            LeftMenuOpen();
            AdminDropPanel.Visibility = Visibility.Visible;
            AdminDropPanel.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1d2127"));
            AdminMenuPic.Source = new BitmapImage(new Uri("/Resources/arrowUP.png", UriKind.Relative));
        }

        private void CloseAdminDropdownMenu()
        {
            AdminDropPanel.Visibility = Visibility.Collapsed;
            AdminMenuPic.Source = new BitmapImage(new Uri("/Resources/arrow.png", UriKind.Relative));
        }

        private void CountingTMC_Click(object sender, RoutedEventArgs e)
        {

        }

        private void VendingAutomats_Click(object sender, RoutedEventArgs e)
        {
            PageManager.MainFrame.Navigate(new VendingMachinesPage());
        }

        private void Companies_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Users_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Modems_Click(object sender, RoutedEventArgs e)
        {

        }

        private void LeftMenuBurger_Click(object sender, RoutedEventArgs e)
        {
            if (LeftMenuColumn.Width.Value > 40)
            {
                LeftMenuClose();
            }
            else
            {
                LeftMenuOpen();
            }
        }
        private void LeftMenuOpen()
        {
            LeftMenuColumn.Width = new GridLength(200);
            CurrentPageTitle.Visibility = Visibility.Visible;
        }
        private void LeftMenuClose()
        {
            LeftMenuColumn.Width = new GridLength(40);
            CurrentPageTitle.Visibility = Visibility.Collapsed;
        }
    }
}
