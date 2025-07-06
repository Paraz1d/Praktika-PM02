using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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


namespace AdministrationApp
{
    public partial class AddEditUserWindow : Window
    {
        private string connectionString = "Data Source=.;Initial Catalog=AdministrationDB;Integrated Security=True";
        private int? userId;

        public AddEditUserWindow(int? userId = null)
        {
            InitializeComponent();
            this.userId = userId;
            DataContext = new AddEditUserViewModel(userId, connectionString);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = (AddEditUserViewModel)DataContext;
            viewModel.Password = PasswordBox.Password;
            viewModel.ConfirmPassword = ConfirmPasswordBox.Password;

            try
            {
                if (viewModel.SaveUser())
                {
                    DialogResult = true;
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении пользователя: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }

    public class AddEditUserViewModel
    {
        private int? userId;
        private string connectionString;

        public string WindowTitle => userId.HasValue ? "Редактирование пользователя" : "Добавление пользователя";
        public string Username { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
        public List<string> Roles { get; } = new List<string> { "Администратор", "Менеджер", "Пользователь" };
        public bool IsActive { get; set; } = true;

        public AddEditUserViewModel(int? userId, string connectionString)
        {
            this.userId = userId;
            this.connectionString = connectionString;

            if (userId.HasValue)
            {
                LoadUserData();
            }
        }

        private void LoadUserData()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(
                        "SELECT Username, FullName, Role, IsActive FROM Users WHERE UserID = @UserID",
                        connection);

                    command.Parameters.AddWithValue("@UserID", userId.Value);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Username = reader["Username"].ToString();
                            FullName = reader["FullName"].ToString();
                            Role = reader["Role"].ToString();
                            IsActive = (bool)reader["IsActive"];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Не удалось загрузить данные пользователя", ex);
            }
        }

        public bool SaveUser()
        {
            if (string.IsNullOrWhiteSpace(Username))
            {
                MessageBox.Show("Введите логин пользователя", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!userId.HasValue && string.IsNullOrWhiteSpace(Password))
            {
                MessageBox.Show("Введите пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (Password != ConfirmPassword)
            {
                MessageBox.Show("Пароли не совпадают", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(FullName))
            {
                MessageBox.Show("Введите ФИО пользователя", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(Role))
            {
                MessageBox.Show("Выберите роль пользователя", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    if (userId.HasValue)
                    {
                        // Обновление существующего пользователя
                        SqlCommand command;

                        if (!string.IsNullOrEmpty(Password))
                        {
                            command = new SqlCommand(
                                "UPDATE Users SET Username = @Username, Password = @Password, " +
                                "FullName = @FullName, Role = @Role, IsActive = @IsActive " +
                                "WHERE UserID = @UserID", connection);

                            command.Parameters.AddWithValue("@Password", Password);
                        }
                        else
                        {
                            command = new SqlCommand(
                                "UPDATE Users SET Username = @Username, " +
                                "FullName = @FullName, Role = @Role, IsActive = @IsActive " +
                                "WHERE UserID = @UserID", connection);
                        }

                        command.Parameters.AddWithValue("@UserID", userId.Value);
                        AddCommonParameters(command);

                        command.ExecuteNonQuery();
                    }
                    else
                    {
                        // Добавление нового пользователя
                        SqlCommand command = new SqlCommand(
                            "INSERT INTO Users (Username, PasswordHash, FullName, Role, IsActive) " +
                            "VALUES (@Username, @PasswordHash, @FullName, @Role, @IsActive)",
                            connection);

                        command.Parameters.AddWithValue("@Password", Password);
                        AddCommonParameters(command);

                        command.ExecuteNonQuery();
                    }
                }

                return true;
            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
            {
                throw new Exception("Пользователь с таким логином уже существует", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Не удалось сохранить пользователя", ex);
            }
        }

        private void AddCommonParameters(SqlCommand command)
        {
            command.Parameters.AddWithValue("@Username", Username);
            command.Parameters.AddWithValue("@FullName", FullName);
            command.Parameters.AddWithValue("@Role", Role);
            command.Parameters.AddWithValue("@IsActive", IsActive);
        }

      
    }
}