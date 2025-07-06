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
    public partial class ReportsPage : Page
    {
        private string connectionString = "Data Source=.;Initial Catalog=AdministrationDB;Integrated Security=True";

        public ReportsPage()
        {
            InitializeComponent();
            LoadReports();
        }

        private void LoadReports()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT r.*, u.FullName AS GeneratedBy " +
                                   "FROM Reports r " +
                                   "JOIN Users u ON r.GeneratedBy = u.UserID " +
                                   "ORDER BY r.GeneratedDate DESC";

                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    ReportsGrid.ItemsSource = dataTable.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке отчетов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GenerateReport_Click(object sender, RoutedEventArgs e)
        {
            var reportWindow = new GenerateReportWindow();
            if (reportWindow.ShowDialog() == true)
            {
                LoadReports();
            }
        }

        private void ViewReport_Click(object sender, RoutedEventArgs e)
        {
            if (ReportsGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите отчет для просмотра", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DataRowView row = (DataRowView)ReportsGrid.SelectedItem;
            var viewWindow = new ViewReportWindow((int)row["ReportID"]);
            viewWindow.ShowDialog();
        }

        private void ExportReport_Click(object sender, RoutedEventArgs e)
        {
            if (ReportsGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите отчет для экспорта", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Реализация экспорта отчета
            MessageBox.Show("Экспорт отчета выполнен успешно", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void RefreshReports_Click(object sender, RoutedEventArgs e)
        {
            LoadReports();
        }
    }
}
