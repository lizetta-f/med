using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Med
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        SqlConnection connection;

        int user_id;
        public int UserId {
            get { return user_id; }
            set { user_id = value; }
        }

        public SqlConnection GetConnection() {

            if (connection == null) {
                string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                connection = new SqlConnection(connectionString);
                connection.Open();
            }

            return connection;
        }

        
    }
}
