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
using System.Data.SqlClient;
using System.Data;
using System.Configuration;

namespace Med
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        string connectionString;
        SqlDataAdapter adapter;
        DataTable entriesTable;
        string tableName;

        public MainWindow()
        {
            InitializeComponent();
            // получаем строку подключения из app.config
            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            showData();
        }

        private void showData()
        {
            string sql = "SELECT * FROM entries_view";
            entriesTable = new DataTable();
            SqlConnection connection = ((App)Application.Current).GetConnection();
            try
            {
                SqlCommand command = new SqlCommand(sql, connection);
                adapter = new SqlDataAdapter(command);
                adapter.Fill(entriesTable);
                entriesGrid.ItemsSource = entriesTable.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void addProc_Click(object sender, RoutedEventArgs e) {
            ProcAdd procedureWindow = new ProcAdd();

            if (procedureWindow.ShowDialog() == true)
            {
                string sql = "INSERT INTO entries(procedur_id, doctor_id, user_id, time) VALUES(@proc,@doctor,@user,@date)";
                SqlConnection conn = ((App)Application.Current).GetConnection();
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@proc", procedureWindow.Procedure);
                    cmd.Parameters.AddWithValue("@doctor", procedureWindow.Doctor);
                    cmd.Parameters.AddWithValue("@user", procedureWindow.Patient);
                    cmd.Parameters.AddWithValue("@date", procedureWindow.Date);
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                }
                MessageBox.Show(procedureWindow.Procedure.ToString());
            }
        }


        private void showUsers(object sender, RoutedEventArgs e) {
            Tables tableWindow = new Tables("users");
            tableWindow.Show();
        }

        private void showProcedures(object sender, RoutedEventArgs e)
        {
            Tables tableWindow = new Tables("procedures");
            tableWindow.Show();
        }

        private void showPositions(object sender, RoutedEventArgs e)
        {
            Tables tableWindow = new Tables("positions");
            tableWindow.Show();
        }
        
        private void showEquipment(object sender, RoutedEventArgs e)
        {
            Tables tableWindow = new Tables("equipment");
            tableWindow.Show();
        }
    }
}
