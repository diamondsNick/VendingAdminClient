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
    /// Interaction logic for CompaniesPage.xaml
    /// </summary>
    public partial class CompaniesPage : Page
    {
        public static int page = 1;
        public static int pageAmount = 0;
        public static int selectedAmount;
        public static int entityAmount = 0;
        public static string HigherCompanyName;
        PagedCompanies? Companies { get; set; }
        public CompaniesPage()
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
                mainWindow.CurrentPageTitle.Text = "Компании";
            }
            foreach (var column in DataGridTable.Columns)
            {
                column.Width = new DataGridLength(1, DataGridLengthUnitType.Auto);
            }

            await LoadCompanyData();
        }

        private async Task LoadCompanyData()
        {
            var curUser = await Services.LoginOperation.GetCurrentUserAsync();


            if (curUser?.CompanyID == null)
            {
                return;
            }

            HigherCompanyName = curUser.Company?.Name ?? "Нет информации";

            UpdateAmountOnBox();

            Companies = await Services.CompanyService.GetPagedCompaniesAsync(selectedAmount, page, (long)curUser.CompanyID);

            if (Companies != null)
            {
                NoDataMessage.Visibility = Visibility.Hidden;
                UpdatePageCounters(Companies);

                foreach (var company in Companies.Companies)
                {
                    company.HighCompanyName = HigherCompanyName;
                }
            }
            else NoDataMessage.Visibility = Visibility.Visible;
            DataGridTable.ItemsSource = Companies?.Companies;

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
            await LoadCompanyData();
        }

        private void UpdatePageCounters(PagedCompanies pr)
        {
            PageNum.Text = page.ToString();
            entityAmount = pr.TotalCount;

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
            else if (page == 1 && selectedAmount > entityAmount)
            {
                MachCountFromTo.Text = $"Записи с 1 до {pr.TotalCount.ToString()} из {entityAmount} записей";
            }
            else
            {
                int entCountOst = 0;
                if (selectedAmount > entityAmount) entCountOst = entityAmount;
                else entCountOst = selectedAmount;
                MachCountFromTo.Text = $"Записи с {page * pageAmount + 1} до {entCountOst} из {entityAmount} записей";
            }

        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            PageManager.MainFrame.Navigate(new CompanyPage(new()));
        }

        private async void PrevPage_Click(object sender, RoutedEventArgs e)
        {
            if (page > 1 && pageAmount > 1)
            {
                page--;
                await LoadCompanyData();
            }
        }

        private async void NextPage_Click(object sender, RoutedEventArgs e)
        {
            if (page >= 1 && pageAmount > 1)
            {
                page++;
                await LoadCompanyData();
            }
        }

        private void FilterBox_GotFocus(object sender, RoutedEventArgs e)
        {
            FilterBox.Text = string.Empty;
        }

        private void FilterBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var tbx = sender as TextBox;
            if (Companies?.Companies == null)
                return;
            if (tbx.Text != "")
            {
                var filtered = Companies.Companies.Where(vm =>
                vm.Name.Contains(tbx.Text, StringComparison.OrdinalIgnoreCase) ||
                vm.Adress.Contains(tbx.Text, StringComparison.OrdinalIgnoreCase) ||
                vm.Phone.Contains(tbx.Text, StringComparison.OrdinalIgnoreCase) ||
                vm.RegistrationDate.Contains(tbx.Text, StringComparison.OrdinalIgnoreCase)
                )
                    .ToList();
                DataGridTable.ItemsSource = filtered;
            }
            else
            {
                DataGridTable.ItemsSource = Companies.Companies;
            }
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Company company)
            {
                PageManager.MainFrame.Navigate(new CompanyPage(company));
            }
        }

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Company selectedCompany)
            {
                var result = MessageBox.Show($"Вы точно хотите удалить {selectedCompany.Name}?", "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        var returnedResult = await CompanyService.DeleteCompanyAsync(selectedCompany.ID);
                        if (returnedResult)
                        {
                            MessageBox.Show("Компания успешно удалена.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show("Не удалось удалить компанию. Возможно, он используется в других операциях.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                    }
                    catch
                    {
                        MessageBox.Show("Не удалось удалить компанию.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    
                }
            }
            await LoadCompanyData();
        }

    }
}
