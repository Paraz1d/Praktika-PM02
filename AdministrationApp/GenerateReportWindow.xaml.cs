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
    public partial class GenerateReportWindow : Window
    {
        private string connectionString = "Data Source=.;Initial Catalog=AdministrationDB;Integrated Security=True";

        public List<string> ReportTypes { get; } = new List<string>
        {
            "Отчет по обращениям граждан",
            "Отчет по документам",
            "Статистика работы"
        };

        public GenerateReportWindow()
        {
            InitializeComponent();
            DataContext = this;
            ReportTypeComboBox.SelectedIndex = 0;
            StartDatePicker.SelectedDate = DateTime.Today.AddMonths(-1);
            EndDatePicker.SelectedDate = DateTime.Today;
        }

        private void GenerateButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TitleTextBox.Text))
            {
                MessageBox.Show("Введите название отчета", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (StartDatePicker.SelectedDate == null || EndDatePicker.SelectedDate == null)
            {
                MessageBox.Show("Выберите период для отчета", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                string reportType = ReportTypeComboBox.SelectedItem.ToString();
                string title = TitleTextBox.Text;
                DateTime startDate = StartDatePicker.SelectedDate.Value;
                DateTime endDate = EndDatePicker.SelectedDate.Value;

                // Генерация отчета (здесь можно добавить реальную логику генерации)
                string reportContent = $"Отчет: {reportType}\nПериод: {startDate:dd.MM.yyyy} - {endDate:dd.MM.yyyy}\nСгенерирован: {DateTime.Now}";

                // Сохранение отчета в базу данных
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(
                        "INSERT INTO Reports (ReportType, Title, GeneratedDate, GeneratedBy, Content, Parameters) " +
                        "VALUES (@ReportType, @Title, GETDATE(), @GeneratedBy, @Content, @Parameters)",
                        connection);

                    command.Parameters.AddWithValue("@ReportType", reportType);
                    command.Parameters.AddWithValue("@Title", title);
                    command.Parameters.AddWithValue("@GeneratedBy", GetCurrentUserId());
                    command.Parameters.AddWithValue("@Content", reportContent);
                    command.Parameters.AddWithValue("@Parameters", $"StartDate={startDate:yyyy-MM-dd};EndDate={endDate:yyyy-MM-dd}");

                    command.ExecuteNonQuery();
                }

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при генерации отчета: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private int GetCurrentUserId()
        {
            // В реальном приложении здесь должна быть логика получения ID текущего пользователя
            return 1; // Заглушка
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}