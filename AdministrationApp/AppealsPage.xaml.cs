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
    public partial class AppealsPage : Page
    {
        private string connectionString = "Data Source=.;Initial Catalog=AdministrationDB;Integrated Security=True";

        public AppealsPage()
        {
            InitializeComponent();
            LoadAppeals("All");
        }

        private void LoadAppeals(string filter)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM CitizenAppeals";

                    if (filter == "New")
                        query += " WHERE Status = 'Новое'";
                    else if (filter == "InProgress")
                        query += " WHERE Status = 'В работе'";
                    else if (filter == "Completed")
                        query += " WHERE Status = 'Завершено'";

                    query += " ORDER BY AppealDate DESC";

                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    AppealsGrid.ItemsSource = dataTable.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке обращений: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddAppeal_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddEditAppealWindow();
            if (addWindow.ShowDialog() == true)
            {
                LoadAppeals("All");
            }
        }

        private void EditAppeal_Click(object sender, RoutedEventArgs e)
        {
            if (AppealsGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите обращение для редактирования", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DataRowView row = (DataRowView)AppealsGrid.SelectedItem;
            var editWindow = new AddEditAppealWindow();
            if (editWindow.ShowDialog() == true)
            {
                LoadAppeals("All");
            }
        }

        private void DeleteAppeal_Click(object sender, RoutedEventArgs e)
        {
            if (AppealsGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите обращение для удаления", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show("Вы уверены, что хотите удалить выбранное обращение?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                DataRowView row = (DataRowView)AppealsGrid.SelectedItem;
                int appealId = (int)row["AppealID"];

                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        SqlCommand command = new SqlCommand("DELETE FROM CitizenAppeals WHERE AppealID = @AppealID", connection);
                        command.Parameters.AddWithValue("@AppealID", appealId);
                        command.ExecuteNonQuery();
                    }

                    LoadAppeals("All");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении обращения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void RefreshAppeals_Click(object sender, RoutedEventArgs e)
        {
            LoadAppeals("All");
        }

        private void FilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FilterComboBox.SelectedItem != null)
            {
                string filter = ((ComboBoxItem)FilterComboBox.SelectedItem).Tag.ToString();
                LoadAppeals(filter);
            }
        }
    }
}
