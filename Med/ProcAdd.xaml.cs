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
    /// Логика взаимодействия для ProcAdd.xaml
    /// </summary>
    public partial class ProcAdd : Window
    {
        public ProcAdd()
        {
            InitializeComponent();
        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            bool valid = true;

            valid = valid & Procedure > 0;
            valid = valid & Doctor > 0;
            valid = valid & Patient > 0;
            valid = valid & Date != null;

            if (valid)
            {
                this.DialogResult = true;
            }
            else {
                MessageBox.Show("Заполните все поля");
            }
        }

        public int Procedure
        {
            get {
                object value = proceduresBox.SelectedValue;
                if (value == null)
                {
                    return 0;
                }
                else {
                    return (int)value;
                }
            }
        }

        public int Doctor
        {
            get {
                object value = doctorsBox.SelectedValue;
                if (value == null)
                {
                    return 0;
                }
                else
                {
                    return (int)value;
                }
            }
        }

        public int Patient
        {
            get {
                object value = usersBox.SelectedValue;
                if (value == null)
                {
                    return 0;
                }
                else
                {
                    return (int)value;
                }
            }
        }

        public DateTime? Date {

            get { return datePicker1.Value; }

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            App currentApp = ((App)Application.Current);

            SqlConnection conn = ((App)Application.Current).GetConnection();

            SqlDataAdapter ProceduesTableTableAdapter = new SqlDataAdapter("SELECT * FROM procedures", conn);
            DataSet ds = new DataSet();
            ProceduesTableTableAdapter.Fill(ds, "t");

            proceduresBox.ItemsSource = ds.Tables["t"].DefaultView;
            proceduresBox.DisplayMemberPath = "Name";
            proceduresBox.SelectedValuePath = "id";

            SqlDataAdapter DoctorsTableAdapter = new SqlDataAdapter("SELECT * FROM users where position_id is not null and id != @my_id", conn);
            DoctorsTableAdapter.SelectCommand.Parameters.AddWithValue("@my_id", currentApp.UserId);
            DataSet doctorsSet = new DataSet();
            DoctorsTableAdapter.Fill(doctorsSet, "t");

            doctorsBox.ItemsSource = doctorsSet.Tables["t"].DefaultView;
            doctorsBox.SelectedValuePath = "id";

            SqlDataAdapter PatientsTableAdapter = new SqlDataAdapter("SELECT * FROM users", conn);
            DataSet patientsSet = new DataSet();
            PatientsTableAdapter.Fill(patientsSet, "t");

            usersBox.ItemsSource = patientsSet.Tables["t"].DefaultView;
            usersBox.SelectedValuePath = "id";
        }
    }
}
