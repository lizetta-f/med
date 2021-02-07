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
    /// Логика взаимодействия для Tables.xaml
    /// </summary>
    public partial class Tables : Window
    {
        SqlDataAdapter adapter;
        DataTable phonesTable;
        string tableName;

        public Tables(string tableName)
        {
            this.tableName = tableName;
            InitializeComponent();
        }

        public static SqlDbType GetSqlType(Type type)
        {
            if (type == typeof(string))
                return SqlDbType.NVarChar;

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                type = Nullable.GetUnderlyingType(type);

            var param = new SqlParameter("", Activator.CreateInstance(type));
            return param.SqlDbType;
        }

        private void loadTable()
        {
            string sql = "SELECT * FROM " + tableName;
            phonesTable = new DataTable();
            SqlConnection connection = null;
            try
            {
                connection = ((App)Application.Current).GetConnection();
                SqlCommand command = new SqlCommand(sql, connection);
                adapter = new SqlDataAdapter(command);

                // установка команды на добавление для вызова хранимой процедуры

                //connection.Open();
                adapter.Fill(phonesTable);

                string cols = "";

                adapter.InsertCommand = new SqlCommand(tableName + "_ins", connection);
                adapter.InsertCommand.CommandType = CommandType.StoredProcedure;
                

                adapter.UpdateCommand = new SqlCommand(tableName + "_upd", connection);
                adapter.UpdateCommand.CommandType = CommandType.StoredProcedure;

                adapter.DeleteCommand = new SqlCommand(tableName + "_del", connection);
                adapter.DeleteCommand.CommandType = CommandType.StoredProcedure;

                foreach (DataColumn col in phonesTable.DefaultView.Table.Columns)
                {
                    adapter.UpdateCommand.Parameters.Add(new SqlParameter("@"+col.ColumnName, GetSqlType(col.DataType), 100, col.ColumnName));

                    if (col.ColumnName.ToLower() != "id")
                    {
                        adapter.InsertCommand.Parameters.Add(new SqlParameter("@" + col.ColumnName, GetSqlType(col.DataType), 100, col.ColumnName));
                    }
                    else 
                    {
                        SqlParameter parameter = adapter.InsertCommand.Parameters.Add("@" + col.ColumnName, SqlDbType.Int, 0, col.ColumnName);
                        parameter.Direction = ParameterDirection.Output;

                        adapter.DeleteCommand.Parameters.Add(new SqlParameter("@" + col.ColumnName, SqlDbType.Int, 0, col.ColumnName));
                    }

                    //cols = cols + ", " + col.DataType;
                }

                //MessageBox.Show(cols);

                cardsGrid.ItemsSource = phonesTable.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ex.StackTrace);
            }
            finally
            {
                //if (connection != null)
                  //  connection.Close();
            }
        }

        private void UpdateDB()
        {
            //SqlCommandBuilder comandbuilder = new SqlCommandBuilder(adapter);
            adapter.Update(phonesTable);
        }

        private void updateButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateDB();
        }

        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (cardsGrid.SelectedItems != null)
            {
                for (int i = 0; i < cardsGrid.SelectedItems.Count; i++)
                {
                    DataRowView datarowView = cardsGrid.SelectedItems[i] as DataRowView;
                    if (datarowView != null)
                    {
                        DataRow dataRow = (DataRow)datarowView.Row;
                        dataRow.Delete();
                    }
                }
            }
            UpdateDB();
        }

        private void phonesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            ComboBoxItem selectedItem = (ComboBoxItem)comboBox.SelectedItem;
            //tableName = selectedItem.Content.ToString();
            //changeTable();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            loadTable();
        }
    }

}
