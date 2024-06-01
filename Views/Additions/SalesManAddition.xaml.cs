using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NvvmFinal.Views.Additions
{

    public partial class SalesManAddition : Window
    {
        public SalesManAddition()
        {
            InitializeComponent();
            LoadGenderTypeData();
            LoadMaritalStatusData();
            LoadRegionData();
            LoadPhoneNumberType();
        }


        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
        private string GetConnectionString()
        {
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["AdventureWorks2019"];
            if (settings != null)
                return settings.ConnectionString;
            throw new Exception("Connection string for AdventureWorks2019 not found.");
        }
        private void LoadGenderTypeData()
        {

            SqlCommand command;
            SqlDataReader dataReader;
            using (SqlConnection con = new SqlConnection(GetConnectionString()))
            {
                try
                {
                    con.Open();
                    string query = "SELECT DISTINCT CASE WHEN Gender = 'M' THEN 'Male'    " +
                                   "WHEN Gender = 'F' THEN 'Female' END AS Gender " +
                                   "FROM HumanResources.Employee ";
                    command = new SqlCommand(query, con);

                    dataReader = command.ExecuteReader();
                    while (dataReader.Read())
                    {
                        cmbGender.Items.Add(dataReader.GetValue(0));
                    }
                    dataReader.Close();
                    command.Dispose();
                    con.Close();
                }

                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to load data. Error: {ex.Message}");
                }
            }
        }
        private void LoadMaritalStatusData()
        {

            SqlCommand command;
            SqlDataReader dataReader;
            using (SqlConnection con = new SqlConnection(GetConnectionString()))
            {
                try
                {
                    con.Open();
                    string query = "SELECT DISTINCT CASE WHEN MaritalStatus = 'M' THEN 'Married'    " +
                                   "WHEN MaritalStatus = 'S' THEN 'Single' END AS MaritalStatus " +
                                   "FROM HumanResources.Employee ";
                    command = new SqlCommand(query, con);

                    dataReader = command.ExecuteReader();
                    while (dataReader.Read())
                    {
                        cmbMaritalStatus.Items.Add(dataReader.GetValue(0));
                    }
                    dataReader.Close();
                    command.Dispose();
                    con.Close();
                }

                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to load data. Error: {ex.Message}");
                }
            }
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void mnmzBtn_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;


        }
        private void LoadRegionData()
        {

            SqlCommand command;
            SqlDataReader dataReader;
            string connectionString = GetConnectionString();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    string query = "SELECT [Name] FROM [dbo].[territoryByName]";
                    command = new SqlCommand(query, con);
                    dataReader = command.ExecuteReader();
                    while (dataReader.Read())
                    {
                        cmbRegionType.Items.Add(dataReader.GetValue(0));
                    }
                    dataReader.Close();
                    command.Dispose();
                    con.Close();
                }

                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to load data. Error: {ex.Message}");
                }
            }
        }
        private void LoadPhoneNumberType()
        {
            SqlCommand command;
            SqlDataReader dataReader;
            string connectionString = GetConnectionString();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    string query = "SELECT [Name] FROM person.PhoneNumberType";
                    command = new SqlCommand(query, con);
                    dataReader = command.ExecuteReader();
                    while (dataReader.Read())
                    {
                        cmbPhoneType.Items.Add(dataReader.GetValue(0));
                    }
                    dataReader.Close();
                    command.Dispose();
                    con.Close();
                }

                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to load data. Error: {ex.Message}");
                }
            }
        }

        private void ExecTrigger()
        {
            using (SqlConnection con = new SqlConnection(GetConnectionString()))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("NewSalesMan", con);
                cmd.CommandType = CommandType.StoredProcedure;

                int businessEntityId = 0;

                // businessEntityId = gridDataReturn(businessEntityId);


                cmd.Parameters.Add(new SqlParameter("@FirstName", txtFname.Text));
                cmd.Parameters.Add(new SqlParameter("@Middle", txtMiddle.Text));
                cmd.Parameters.Add(new SqlParameter("@LastName", txtLname.Text));
                cmd.Parameters.Add(new SqlParameter("@Gender", cmbGender.SelectedItem));
                cmd.Parameters.Add(new SqlParameter("@AddressLine1", txtAddressLine1.Text));
                cmd.Parameters.Add(new SqlParameter("@AddressLine2", txtAddressLine2.Text));
                cmd.Parameters.Add(new SqlParameter("@TerritoryName", cmbRegionType.SelectedItem.ToString()));
                cmd.Parameters.Add(new SqlParameter("@StateProvince", cmbState.SelectedItem.ToString()));
                cmd.Parameters.Add(new SqlParameter("@PhoneNumberType", cmbPhoneType.SelectedItem.ToString()));
                cmd.Parameters.Add(new SqlParameter("@PhoneNumber", txtPNumber.Text));
                cmd.Parameters.Add(new SqlParameter("@City", txtCity.Text));
                cmd.Parameters.Add(new SqlParameter("@Bonus", txtBonus.Text));
                cmd.Parameters.Add(new SqlParameter("@EmailAddress", txtEmail.Text));
                cmd.Parameters.Add(new SqlParameter("@PostalCode", Convert.ToInt32(txtPostal.Text.ToString())));
                cmd.Parameters.Add(new SqlParameter("@NationalIDNumber", txtNational.Text));
                cmd.Parameters.Add(new SqlParameter("@LoginId", txtLogin.Text));
                cmd.Parameters.Add(new SqlParameter("@BirthDate", DateTime.Parse(txtBirth.Text)));
                cmd.Parameters.Add(new SqlParameter("@HireDate", DateTime.Parse(txtHire.Text)));
                cmd.Parameters.Add(new SqlParameter("@MaritalStatus", cmbMaritalStatus.SelectedItem));
                cmd.Parameters.Add(new SqlParameter("@SalesQuota", txtQuota.Text));


                // execute the command
                //SqlDataReader rdr = cmd.ExecuteReader();
                cmd.ExecuteNonQuery();

            }
        }


        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                ExecTrigger();
                MessageBox.Show("Sales Man Successfully Added!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            txtFname.Text = string.Empty;
            txtMiddle.Text = string.Empty;
            txtLname.Text = string.Empty;
            cmbGender.SelectedIndex = -1;
            txtNational.Text = string.Empty;
            txtLogin.Text = string.Empty;
            txtBirth.Text = string.Empty;
            txtHire.Text = string.Empty;
            cmbMaritalStatus.SelectedIndex = -1;
            txtQuota.Text = string.Empty;



        }
        private int getTerId()
        {
            int terId = 0;
            var selectedRegion = (string)cmbRegionType.SelectedItem;
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("selectTerID", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@region", selectedRegion));
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        terId = reader.GetInt32(0);
                    }

                }
                connection.Close();
            }
            return terId;
        }
        private void cmbRegionType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            cmbState.Items.Clear();
            int terId = getTerId();
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("stateFromTerritory", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@territoryID", terId));
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        cmbState.Items.Add(reader.GetString(0));
                    }

                }
                connection.Close();
            }
        }

    }
}