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

namespace Med
{
    /// <summary>
    /// Логика взаимодействия для Auth.xaml
    /// </summary>
    public partial class Auth : Window
    {
        public Auth()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (login.Text == "" || password.Password == "")
            {
                MessageBox.Show("Заполните все поля");
                return;
            }

            App currentApp = (App) Application.Current;
            SqlConnection conn = currentApp.GetConnection();

            SqlCommand sqlCmd = new SqlCommand("SELECT * FROM users WHERE login=@login and password=@password", conn);
            sqlCmd.Parameters.AddWithValue("@login", login.Text);
            sqlCmd.Parameters.AddWithValue("@password", password.Password);

            using (SqlDataReader reader = sqlCmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    currentApp.UserId = (int) reader["id"];
                    //MessageBox.Show(String.Format("{0}", reader["login"]));
                }
                else {
                    MessageBox.Show("Неверный логин или пароль");
                    return;
                }
            }

            (new MainWindow()).Show();
            this.Close();
        }
    }
}
