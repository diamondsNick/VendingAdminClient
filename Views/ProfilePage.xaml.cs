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
using AdminClient.Services;

namespace AdminClient.Views
{
    /// <summary>
    /// Interaction logic for ProfilePage.xaml
    /// </summary>
    public partial class ProfilePage : Page
    {
        public ProfilePage()
        {
            InitializeComponent();
            Loaded += ProfilePage_Loaded;
        }

        private async void ProfilePage_Loaded(object sender, RoutedEventArgs e)
        { 
            var mainWindow = Application.Current.Windows
                .OfType<MainInterfaceWindow>()
                .FirstOrDefault();

            if (mainWindow != null)
            {
                mainWindow.CurrentPageTitle.Text = "Профиль";
            }

            var user = LoginOperation.GetCurrentUserAsync().Result;

            IDUser.Text = user.ID.Value.ToString();

            var userInfoArr = user.FullName.Split();

            if (userInfoArr.Length > 0)
            {
                UserName.Text = userInfoArr[0] + (userInfoArr.Length > 1 ? " " + userInfoArr[1][0] + "." : "")
                    + (userInfoArr.Length > 2 ? " " + userInfoArr[2][0] + "." : "");
            }
            UserRole.Text = user.Role?.Name ?? "Не указано";
            UserPhone.Text = $"{user.Phone[0]} ({user.Phone.Substring(1, 3)}) {user.Phone.Substring(4, 3)}-{user.Phone.Substring(7, 2)}-{user.Phone.Substring(9, 2)} " ?? "Не указано";
            UserAdress.Text = user.Email ?? "Не указано";
            UserRegistration.Text = user.RegistrationDate;

            if (user.Company != null)
            {
                var company = await CompanyService.GetCompanyInfoAsync((long)user.CompanyID);
                IDCompany.Text = company.ID.ToString();
                CompanyName.Text = company.Name;
                CompanyBalance.Text = company.Finances.ToString("C2"); //Форматируем под русский формат
                CompanyAdress.Text = company.Adress ?? "Не указано";
                CompanyContacts.Text = $"{company.Phone[0]} ({company.Phone.Substring(1, 3)}) {company.Phone.Substring(4, 3)}-{company.Phone.Substring(7, 2)}-{company.Phone.Substring(9, 2)} " ?? "Не указано";
                CompanyRegistration.Text = company.RegistrationDate;
            }
        }
    }
}
