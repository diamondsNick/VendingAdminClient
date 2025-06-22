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
    public partial class UserPage : Page
    {
        bool isNewUser = false;
        User user;
        User curUser;
        Role[] roles;
        public UserPage(User userInfo)
        {
            InitializeComponent();

            user = userInfo;
            DataContext = user;

            GetEssentialStuff();
        }
        private async void GetEssentialStuff()
        {
            curUser = await LoginOperation.GetCurrentUserAsync();

            if (user.ID == null)
            {
                isNewUser = true;
                PageName.Text = "Создание пользователя";
                ConfigureNewUser();
            }

            roles = await RoleService.GetRolesAsync();
            RoleComboBox.ItemsSource = roles;
            RoleComboBox.DisplayMemberPath = "Name";
            RoleComboBox.SelectedValuePath = "ID";

        }
        private void ConfigureNewUser()
        {
            LoginGrid.Visibility = Visibility.Visible;
            PasswordGrid.Visibility = Visibility.Visible;
            user = new User();
            DataContext = user;
            user.Language = "ru";
            user.RegistrationDate = DateTime.Now.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);
            user.CompanyID = curUser.CompanyID as long?;
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            var res = MessageBox.Show("Вы уверены, что хотите отменить создание/редактирование сотрудника?", "Отмена", MessageBoxButton.YesNo, MessageBoxImage.Question);
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
                if (isNewUser)
                {
                    if (user.FullName == null) throw new();
                    else if (user.RoleID == null) throw new();
                    else if (user.Phone == null) throw new();
                    else if (user.Email == null) throw new();
                    else if (user.Login == null) throw new();
                    else if (user.Password == null) throw new();

                    PagedUsersCreateDTO userDTO = new()
                    {
                        ID = 0,
                        FullName = user.FullName,
                        RoleID = user.RoleID,
                        Phone = user.Phone,
                        Email = user.Email,
                        Login = user.Login,
                        Language = user.Language,
                        Password = user.Password,
                        CompanyID = user.CompanyID,
                        RegistrationDate = user.RegistrationDate
                    };

                    var res = await UserService.CreateUserAsync(userDTO);
                    if (res is null)
                    {
                        throw new();
                    }
                    MessageBox.Show("Пользователь успешно сохранен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    if (PageManager.MainFrame.CanGoBack)
                    {
                        PageManager.MainFrame.Navigate(new UsersPage());
                    }
                }
                else
                {
                    if (user.FullName == null) throw new();
                    else if (user.RoleID == null) throw new();
                    else if (user.Phone == null) throw new();
                    else if (user.Email == null) throw new();

                    PagedUsersCreateDTO userDTO = new()
                    {
                        ID = user.ID,
                        FullName = user.FullName,
                        RoleID = user.RoleID,
                        Phone = user.Phone,
                        Email = user.Email,
                        Login = user.Login,
                        Password = user.Password,
                        CompanyID = user.CompanyID,
                        RegistrationDate = user.RegistrationDate
                    };

                    var res = await UserService.UpdateUserAsync((long)user.ID, userDTO);
                    if (res is null)
                    {
                        throw new();
                    }
                    MessageBox.Show("Пользователь успешно сохранен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    if (PageManager.MainFrame.CanGoBack)
                    {
                        PageManager.MainFrame.Navigate(new UsersPage());
                    }
                }
            }
            catch
            {
                MessageBox.Show("Произошла ошибка при сохранении пользователя. Пожалуйста, проверьте введенные данные и повторите попытку.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private void RoleComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            user.RoleID = (long)RoleComboBox.SelectedValue;
        }
    }
}
