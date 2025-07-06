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
using Microsoft.Win32;


namespace AdministrationApp
{
    public partial class AddEditDocumentWindow : Window
    {
        private readonly DocumentViewModel _viewModel;

        public AddEditDocumentWindow(int? documentId = null)
        {
            InitializeComponent();
            _viewModel = new DocumentViewModel(documentId);
            DataContext = _viewModel;
        }

        private void SaveButton_Click1(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_viewModel.SaveDocument())
                {
                    DialogResult = true;
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении документа: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click1(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void BrowseButton_Click1(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                _viewModel.FilePath = openFileDialog.FileName;
            }
        }
    }

    public class DocumentViewModel
    {
        private readonly int? _documentId;
        private const string ConnectionString = "Data Source=.;Initial Catalog=AdministrationDB;Integrated Security=True";

        public DocumentViewModel(int? documentId)
        {
            _documentId = documentId;

            if (_documentId.HasValue)
            {
                LoadDocumentData();
            }
            else
            {
                Status = "Черновик";
            }
        }

        private void LoadDocumentData()
        {
            try
            {
                using (var connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    var command = new SqlCommand(
                        "SELECT DocumentType, Title, Content, Status, FilePath " +
                        "FROM Documents WHERE DocumentID = @DocumentID",
                        connection);

                    command.Parameters.AddWithValue("@DocumentID", _documentId.Value);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            DocumentType = reader["DocumentType"].ToString();
                            Title = reader["Title"].ToString();
                            Content = reader["Content"].ToString();
                            Status = reader["Status"].ToString();
                            FilePath = reader["FilePath"].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Не удалось загрузить данные документа", ex);
            }
        }

        // Остальные свойства и методы класса
        public string WindowTitle => _documentId.HasValue ? "Редактирование документа" : "Добавление документа";
        public string DocumentType { get; set; }
        public string[] DocumentTypes { get; } = { "Приказ", "Распоряжение", "Письмо", "Отчет", "Протокол" };
        public string Title { get; set; }
        public string Content { get; set; }
        public string Status { get; set; }
        public string[] Statuses { get; } = { "Черновик", "Опубликован", "Архивный" };
        public string FilePath { get; set; }

        
    
public bool SaveDocument()
        {
            if (string.IsNullOrWhiteSpace(DocumentType))
            {
                MessageBox.Show("Выберите тип документа", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(Title))
            {
                MessageBox.Show("Введите название документа", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            try
            {
                using (var connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();

                    if (_documentId.HasValue)
                    {
                        UpdateDocument(connection);
                    }
                    else
                    {
                        InsertDocument(connection);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Не удалось сохранить документ", ex);
            }
        }

        private void UpdateDocument(SqlConnection connection)
        {
            var command = new SqlCommand(
                "UPDATE Documents SET DocumentType = @DocumentType, Title = @Title, " +
                "Content = @Content, Status = @Status, FilePath = @FilePath " +
                "WHERE DocumentID = @DocumentID",
                connection);

            command.Parameters.AddWithValue("@DocumentID", _documentId.Value);
            AddCommonParameters(command);

            command.ExecuteNonQuery();
        }

        private void InsertDocument(SqlConnection connection)
        {
            var command = new SqlCommand(
                "INSERT INTO Documents (DocumentType, Title, Content, Status, FilePath, CreatedDate, Author) " +
                "VALUES (@DocumentType, @Title, @Content, @Status, @FilePath, GETDATE(), @Author)",
                connection);

            command.Parameters.AddWithValue("@Author", App.CurrentUser);
            AddCommonParameters(command);

            command.ExecuteNonQuery();
        }

        private void AddCommonParameters(SqlCommand command)
        {
            command.Parameters.AddWithValue("@DocumentType", DocumentType);
            command.Parameters.AddWithValue("@Title", Title);
            command.Parameters.AddWithValue("@Content", Content ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Status", Status);
            command.Parameters.AddWithValue("@FilePath", FilePath ?? (object)DBNull.Value);
        }
    }
}