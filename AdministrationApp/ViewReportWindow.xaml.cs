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
    public partial class ViewReportWindow : Window
    {
        private string connectionString = "Data Source=.;Initial Catalog=AdministrationDB;Integrated Security=True";

        public ViewReportWindow(int reportId)
        {
            InitializeComponent();
            LoadReport(reportId);
        }

        private void LoadReport(int reportId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(
                        "SELECT r.*, u.FullName AS AuthorName " +
                        "FROM Reports r " +
                        "JOIN Users u ON r.GeneratedBy = u.UserID " +
                        "WHERE r.ReportID = @ReportID",
                        connection);

                    command.Parameters.AddWithValue("@ReportID", reportId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            ReportTitleText.Text = reader["Title"].ToString();
                            ReportInfoText.Text = $"Тип: {reader["ReportType"]} | Автор: {reader["AuthorName"]} | Дата: {reader["GeneratedDate"]}";
                            ReportContentText.Text = reader["Content"].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке отчета: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

