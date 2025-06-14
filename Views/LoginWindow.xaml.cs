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
using AdminClient.Services;

namespace AdminClient.Views
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            //string login = LoginBox.Text;
            //string password = PasswordBox.Text;
            string login = "1231123123";
            string password = "887171";

            //var client = APIHttpClient.Instance;

            LoginOperation operation = new LoginOperation();

            var user = await operation.AuthenticateUserAsync(login, password);

            if (user != null)
            {
                LoginOperation.InitializeCurrentUser(user);

                MainInterfaceWindow mainWindow = new MainInterfaceWindow();

                mainWindow.Show();

                Close();
            }
            if (user == null)
            {
                MessageBox.Show("Неправильно был введен логин или пароль, пожалуйста, перепроверьте данные и повторите попытку.",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
