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
    public partial class DocumentsPage : Page
    {
        private string connectionString = "Data Source=.;Initial Catalog=AdministrationDB;Integrated Security=True";

        public DocumentsPage()
        {
            InitializeComponent();
            LoadDocuments("All");
        }

        private void LoadDocuments(string filter)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM Documents";

                    if (filter == "Draft")
                        query += " WHERE Status = 'Черновик'";
                    else if (filter == "Published")
                        query += " WHERE Status = 'Опубликован'";
                    else if (filter == "Archived")
                        query += " WHERE Status = 'Архивный'";

                    query += " ORDER BY CreatedDate DESC";

                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    DocumentsGrid.ItemsSource = dataTable.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке документов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddDocument_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddEditDocumentWindow();
            if (addWindow.ShowDialog() == true)
            {
                LoadDocuments("All");
            }
        }

        private void EditDocument_Click(object sender, RoutedEventArgs e)
        {
            if (DocumentsGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите документ для редактирования", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DataRowView row = (DataRowView)DocumentsGrid.SelectedItem;
            var editWindow = new AddEditDocumentWindow((int)row["DocumentID"]);
            if (editWindow.ShowDialog() == true)
            {
                LoadDocuments("All");
            }
        }

        private void DeleteDocument_Click(object sender, RoutedEventArgs e)
        {
            if (DocumentsGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите документ для удаления", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show("Вы уверены, что хотите удалить выбранный документ?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                DataRowView row = (DataRowView)DocumentsGrid.SelectedItem;
                int documentId = (int)row["DocumentID"];

                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        SqlCommand command = new SqlCommand("DELETE FROM Documents WHERE DocumentID = @DocumentID", connection);
                        command.Parameters.AddWithValue("@DocumentID", documentId);
                        command.ExecuteNonQuery();
                    }

                    LoadDocuments("All");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении документа: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void RefreshDocuments_Click(object sender, RoutedEventArgs e)
        {
            LoadDocuments("All");
        }

        private void FilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FilterComboBox.SelectedItem != null)
            {
                string filter = ((ComboBoxItem)FilterComboBox.SelectedItem).Tag.ToString();
                LoadDocuments(filter);
            }
        }
    }
}