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
                UserInfo.Text = userInfoArr[0] + (userInfoArr.Length > 1 ? " "+userInfoArr[1][0]+"." : "") 
                    + (userInfoArr.Length > 2 ? " " + userInfoArr[2][0] + "." : "");
            }
            if (curUser.Role != null)
            {
                UserRole.Text = curUser.Role.Name;
            }
        }

        private void DropdownButton_Click(object sender, RoutedEventArgs e)
        {
            if(DropdownPanel.Visibility == Visibility.Visible)
            {
                DropdownPanel.Visibility = Visibility.Collapsed;
                MenuPic.Source = new BitmapImage(new Uri("/Resources/arrow.png", UriKind.Relative));
            }
            else if(DropdownPanel.Visibility == Visibility.Collapsed)
            {
                DropdownPanel.Visibility = Visibility.Visible;
                DropdownPanel.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1d2127"));
                MenuPic.Source = new BitmapImage(new Uri("/Resources/arrowUP.png", UriKind.Relative));
            }
            

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("СИГМА", "Сосал?", MessageBoxButton.OKCancel);
        }
    }
}
