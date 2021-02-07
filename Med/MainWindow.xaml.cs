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

        private string getFilters() {

            App currentApp = ((App)Application.Current);

            List<string> filters = new List<string>();

            if ((bool)onlyToday.IsChecked) {

                filters.Add("CONVERT(DATE, dbo.entries.time) = CONVERT(DATE, GETDATE())");
            }

            if ((bool)onlyMy.IsChecked)
            {
                filters.Add("dbo.entries.user_id = " + currentApp.UserId.ToString());
            }

            if ((bool)onlyMe.IsChecked)
            {
                filters.Add("dbo.entries.doctor_id = " + currentApp.UserId.ToString());
            }

            if (patientLastName.Text.Length > 0) {
                filters.Add("patients.last_name like '" + patientLastName.Text + "%'");            
            }

            if (patientFirstName.Text.Length > 0)
            {
                filters.Add("patients.first_name like '" + patientFirstName.Text + "%'");
            }

            if (patientMiddleName.Text.Length > 0)
            {
                filters.Add("patients.middle_name like '" + patientMiddleName.Text + "%'");
            }

            if (doctorLastName.Text.Length > 0)
            {
                filters.Add("doctors.last_name like '" + doctorLastName.Text + "%'");
            }

            if (doctorFirstName.Text.Length > 0)
            {
                filters.Add("doctors.first_name like '" + doctorFirstName.Text + "%'");
            }
            
            if (doctorMiddleName.Text.Length > 0)
            {
                filters.Add("doctors.middle_name like '" + doctorMiddleName.Text + "%'");
            }

            return String.Join(" AND ", filters);
        }

        private void showData()
        {
            // Представление не используется из-за проблем с фильтрацией
            string sql = @"SELECT dbo.procedures.Name AS Процедура, dbo.procedures.TimeOnProc AS [Время на процедуру], dbo.entries.time AS [Время записи], dbo.positions.name AS Должность, CONCAT_WS(' ', doctors.last_name, doctors.first_name, 
                         doctors.middle_name) AS Врач, CONCAT_WS(' ', patients.last_name, patients.first_name, patients.middle_name) AS Пациент
                         FROM dbo.entries 
                         INNER JOIN dbo.procedures ON dbo.entries.procedur_id = dbo.procedures.id INNER JOIN
                         dbo.users AS doctors ON dbo.entries.doctor_id = doctors.id INNER JOIN
                         dbo.users AS patients ON dbo.entries.user_id = patients.id INNER JOIN
                         dbo.positions ON doctors.position_id = dbo.positions.id";

            string filter = getFilters();

            if (filter.Length > 0) {

                sql += " WHERE " + filter;
            }

            entriesTable = new DataTable();
            SqlConnection connection = ((App)Application.Current).GetConnection();
            try
            {
                SqlCommand command = new SqlCommand(sql, connection);
                adapter = new SqlDataAdapter(command);
                entriesTable.Rows.Clear();
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
                showData();
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

        private void searchClick(object sender, RoutedEventArgs e)
        {
            showData();
        }
    }
}
