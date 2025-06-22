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
    /// Interaction logic for CompanyPage.xaml
    /// </summary>
    public partial class CompanyPage : Page
    {
        bool isNewCompany = false;
        Company company;
        User curUser;
        string ParentCompanyName = "Нет информации";

        public CompanyPage(Company companyInfo)
        {

            InitializeComponent();

            company = companyInfo;
            DataContext = company;

            GetEssentialStuff();
        }
        private async void GetEssentialStuff()
        {
            curUser = await LoginOperation.GetCurrentUserAsync();

            if(curUser.CompanyID != 0)
            {
                ParentCompanyBox.Text = curUser.Company.Name;
            }
            
            if (company.ID == 0)
            {
                isNewCompany = true;
                ConfigureNewCompany();
            }
            else OutputOldCompany();


        }
        private async void ConfigureNewCompany()
        {
            PageName.Text = "Создание компании";
            company = new Company();
            DataContext = company;
            company.RegistrationDate = DateTime.Now.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);
            company.ParentCompanyID = (long)curUser.CompanyID;
        }
        private async void OutputOldCompany()
        {
           
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            var res = MessageBox.Show("Вы уверены, что хотите отменить создание/редактирование компании?", "Отмена", MessageBoxButton.YesNo, MessageBoxImage.Question);
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
                if (isNewCompany)
                {
                    if (NameBox.Text == null) throw new();
                    else if (AdressBox.Text == null) throw new();
                    else if (PhoneBox.Text == null) throw new();
                    var res = await CompanyService.CreateCompanyAsync(company);
                    if (res is null)
                    {
                        throw new();
                    }
                    MessageBox.Show("Компания успешно сохранена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    if (PageManager.MainFrame.CanGoBack)
                    {
                        PageManager.MainFrame.Navigate(new CompaniesPage());
                    }
                }
                else
                {
                    if (NameBox.Text == null) throw new();
                    else if (AdressBox.Text == null) throw new();
                    else if (PhoneBox.Text == null) throw new();
                    var res = await CompanyService.UpdateCompanyAsync(company.ID, company);
                    if (res is null)
                    {
                        throw new();
                    }
                    MessageBox.Show("Компания успешно сохранена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    if (PageManager.MainFrame.CanGoBack)
                    {
                        PageManager.MainFrame.Navigate(new CompaniesPage());
                    }
                }
            }
            catch
            {
                MessageBox.Show("Произошла ошибка при сохранении компании. Пожалуйста, проверьте введенные данные и повторите попытку.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

    }
}
