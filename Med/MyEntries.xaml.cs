using System;
using System.Collections.Generic;
using System.Data;
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
    /// Логика взаимодействия для My.xaml
    /// </summary>
    public partial class MyEntries : Window
    {
        SqlDataAdapter adapter;
        DataTable entriesTable;

        public MyEntries()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            showData();
        }

        private void showData()
        {
            string sql = "SELECT * FROM entries_view";
            entriesTable = new DataTable();
            SqlConnection connection = ((App) Application.Current).GetConnection();
            try
            {
                SqlCommand command = new SqlCommand(sql, connection);
                adapter = new SqlDataAdapter(command);

                // установка команды на добавление для вызова хранимой процедуры
                /*adapter.InsertCommand = new SqlCommand("sp_InsertPhone", connection);
                adapter.InsertCommand.CommandType = CommandType.StoredProcedure;
                adapter.InsertCommand.Parameters.Add(new SqlParameter("@title", SqlDbType.NVarChar, 50, "Title"));
                adapter.InsertCommand.Parameters.Add(new SqlParameter("@company", SqlDbType.NVarChar, 50, "Company"));
                adapter.InsertCommand.Parameters.Add(new SqlParameter("@price", SqlDbType.Int, 0, "Price"));
                SqlParameter parameter = adapter.InsertCommand.Parameters.Add("@Id", SqlDbType.Int, 0, "Id");
                parameter.Direction = ParameterDirection.Output;*/

                adapter.Fill(entriesTable);
                entriesGrid.ItemsSource = entriesTable.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
