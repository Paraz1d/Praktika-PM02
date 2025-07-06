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
using System.Data.SqlClient;

namespace AdministrationApp
{
    public partial class AddEditAppealWindow : Window
    {
        private string connectionString = "Data Source=.;Initial Catalog=AdministrationDB;Integrated Security=True";
        private int? appealId;

        public AddEditAppealWindow(int? appealId = null)
        {
            InitializeComponent();
            this.appealId = appealId;
            DataContext = new AddEditAppealViewModel(appealId, connectionString);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = (AddEditAppealViewModel)DataContext;

            try
            {
                if (viewModel.SaveAppeal())
                {
                    DialogResult = true;
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }

    public class AddEditAppealViewModel
    {
        private int? appealId;
        private string connectionString;

        public string WindowTitle => appealId.HasValue ? "Редактирование обращения" : "Добавление обращения";
        public string FullName { get; set; }
        public string ContactInfo { get; set; }
        public string AppealText { get; set; }
        public string Status { get; set; }
        public List<string> Statuses { get; } = new List<string> { "Новое", "В работе", "Завершено" };
        public string ResponsiblePerson { get; set; }
        public string Resolution { get; set; }

        public AddEditAppealViewModel(int? appealId, string connectionString)
        {
            this.appealId = appealId;
            this.connectionString = connectionString;

            if (appealId.HasValue)
            {
                LoadAppealData();
            }
            else
            {
                Status = "Новое";
            }
        }

        private void LoadAppealData()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(
                        "SELECT FullName, ContactInfo, AppealText, Status, ResponsiblePerson, Resolution " +
                        "FROM CitizenAppeals WHERE AppealID = @AppealID", connection);

                    command.Parameters.AddWithValue("@AppealID", appealId.Value);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            FullName = reader["FullName"].ToString();
                            ContactInfo = reader["ContactInfo"].ToString();
                            AppealText = reader["AppealText"].ToString();
                            Status = reader["Status"].ToString();
                            ResponsiblePerson = reader["ResponsiblePerson"].ToString();
                            Resolution = reader["Resolution"].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Не удалось загрузить данные обращения", ex);
            }
        }

        public bool SaveAppeal()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    if (appealId.HasValue)
                    {
                        // Обновление существующего обращения
                        SqlCommand command = new SqlCommand(
                            "UPDATE CitizenAppeals SET FullName = @FullName, ContactInfo = @ContactInfo, " +
                            "AppealText = @AppealText, Status = @Status, ResponsiblePerson = @ResponsiblePerson, " +
                            "Resolution = @Resolution, ResolutionDate = CASE WHEN @Status = 'Завершено' AND Resolution IS NOT NULL THEN GETDATE() ELSE ResolutionDate END " +
                            "WHERE AppealID = @AppealID", connection);

                        command.Parameters.AddWithValue("@AppealID", appealId.Value);
                        AddCommonParameters(command);

                        command.ExecuteNonQuery();
                    }
                    else
                    {
                        // Добавление нового обращения
                        SqlCommand command = new SqlCommand(
                            "INSERT INTO CitizenAppeals (FullName, ContactInfo, AppealText, Status, ResponsiblePerson, Resolution, AppealDate) " +
                            "VALUES (@FullName, @ContactInfo, @AppealText, @Status, @ResponsiblePerson, @Resolution, GETDATE())", connection);

                        AddCommonParameters(command);
                        command.ExecuteNonQuery();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Не удалось сохранить обращение", ex);
            }
        }

        private void AddCommonParameters(SqlCommand command)
        {
            command.Parameters.AddWithValue("@FullName", FullName);
            command.Parameters.AddWithValue("@ContactInfo", ContactInfo);
            command.Parameters.AddWithValue("@AppealText", AppealText);
            command.Parameters.AddWithValue("@Status", Status);
            command.Parameters.AddWithValue("@ResponsiblePerson", ResponsiblePerson ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Resolution", Resolution ?? (object)DBNull.Value);
        }
    }
}