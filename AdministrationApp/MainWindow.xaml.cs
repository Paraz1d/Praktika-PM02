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
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace AdministrationApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ShowLoginDialog();
        }

        private void ShowLoginDialog()
        {
            var loginWindow = new LoginWindow();
            if (loginWindow.ShowDialog() == true)
            {
                UserInfoText.Text = $"Пользователь: {loginWindow.Username}";
                StatusText.Text = "Авторизация успешна";
            }
            else
            {
                Close();
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Appeals_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new AppealsPage());
            StatusText.Text = "Работа с обращениями граждан";
        }

        private void Documents_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new DocumentsPage());
            StatusText.Text = "Работа с документами";
        }

        private void Reports_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ReportsPage());
            StatusText.Text = "Формирование отчетов";
        }

        private void Admin_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new AdminPage());
            StatusText.Text = "Администрирование системы";
        }
    }
}
