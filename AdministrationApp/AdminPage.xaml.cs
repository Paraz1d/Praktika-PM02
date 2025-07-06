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
using System.Data;
using System.Data.SqlClient;


namespace AdministrationApp
{
    public partial class AdminPage : Page
    {
        private string connectionString = "Data Source=.;Initial Catalog=AdministrationDB;Integrated Security=True";

        public AdminPage()
        {
            InitializeComponent();
            LoadUsers();
            LoadSettings();
        }

        private void LoadUsers()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM Users ORDER BY Username";

                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    UsersGrid.ItemsSource = dataTable.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке пользователей: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadSettings()
        {
            // Загрузка настроек системы
            ConnectionStringTextBox.Text = connectionString;
            LoggingCheckBox.IsChecked = true;
            BackupCheckBox.IsChecked = false;
        }

        private void AddUser_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddEditUserWindow();
            if (addWindow.ShowDialog() == true)
            {
                LoadUsers();
            }
        }

        private void EditUser_Click(object sender, RoutedEventArgs e)
        {
            if (UsersGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите пользователя для редактирования", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DataRowView row = (DataRowView)UsersGrid.SelectedItem;
            var editWindow = new AddEditUserWindow((int)row["UserID"]);
            if (editWindow.ShowDialog() == true)
            {
                LoadUsers();
            }
        }

        private void DeleteUser_Click(object sender, RoutedEventArgs e)
        {
            if (UsersGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите пользователя для удаления", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DataRowView row = (DataRowView)UsersGrid.SelectedItem;
            int userId = (int)row["UserID"];
            string username = row["Username"].ToString();

            if (MessageBox.Show($"Вы уверены, что хотите удалить пользователя {username}?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        SqlCommand command = new SqlCommand("DELETE FROM Users WHERE UserID = @UserID", connection);
                        command.Parameters.AddWithValue("@UserID", userId);
                        command.ExecuteNonQuery();
                    }

                    LoadUsers();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении пользователя: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void RefreshUsers_Click(object sender, RoutedEventArgs e)
        {
            LoadUsers();
        }

        private void SaveSettings_Click(object sender, RoutedEventArgs e)
        {
            // Сохранение настроек системы
            MessageBox.Show("Настройки системы сохранены", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
