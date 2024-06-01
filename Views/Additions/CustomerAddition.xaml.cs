using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NvvmFinal.Views.Additions
{
    /// <summary>
    /// Interaction logic for CustomerAddition.xaml
    /// </summary>
    public partial class CustomerAddition : Window
    {
        public CustomerAddition()
        {
            InitializeComponent();
            LoadPersonTypeData();
            LoadAddressData();
            LoadRegionData();
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
        private void LoadPersonTypeData()
        {

            SqlCommand command;
            SqlDataReader dataReader;
            string connectionString = GetConnectionString();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    string query = "SELECT DISTINCT PersonType FROM Person.Person ";
                    command = new SqlCommand(query, con);

                    dataReader = command.ExecuteReader();
                    while (dataReader.Read())
                    {
                        cmbPersonType.Items.Add(dataReader.GetValue(0));
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
        private void LoadAddressData()
        {

            SqlCommand command;
            SqlDataReader dataReader;
            string connectionString = GetConnectionString();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    string query = "SELECT [Name] FROM [Person].[AddressType]";
                    command = new SqlCommand(query, con);

                    dataReader = command.ExecuteReader();
                    while (dataReader.Read())
                    {
                        cmbAdressType.Items.Add(dataReader.GetValue(0));
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


        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtSearch.Text))
            {
                LoadData();
                return;
            }
            string connectionString = GetConnectionString();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                if (!string.IsNullOrEmpty(txtSearch.Text))
                {
                    try
                    {
                        con.Open();
                        string query = $"SELECT [BusinessEntityID],[Name] FROM [Sales].[Store]" +
                            $" WHERE Name LIKE '{txtSearch.Text}%'" +
                            $" ORDER BY Name ";
                        SqlCommand cmd = new SqlCommand(query, con);
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        storeGrid.ItemsSource = dt.DefaultView;


                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Failed to load data. Error: {ex.Message}");
                    }
                }
                con.Close();
                bool toInt = int.TryParse(txtSearch.Text, out int result);
                if (toInt)
                {
                    try
                    {
                        con.Open();
                        string query = $"SELECT [BusinessEntityID],[Name] FROM [Sales].[Store]" +
                            $" WHERE BusinessEntityID LIKE '{txtSearch.Text}%'" +
                            $" ORDER BY BusinessEntityID ";
                        SqlCommand cmd = new SqlCommand(query, con);
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        storeGrid.ItemsSource = dt.DefaultView;


                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Failed to load data. Error: {ex.Message}");
                    }
                }
                con.Close();


            }
        }
        private void LoadData()
        {

            var terID = getTerId();
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                try
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand("storeByTerritory", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@territoryID", terID));
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        storeGrid.ItemsSource = dt.DefaultView;
                    }
                    connection.Close();

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to load data. Error: {ex.Message}");
                }
            }
        }


        private void ExecTrigger()
        {
            string connectionString = GetConnectionString();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("InsertingCustomer", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                int businessEntityId = 0;
                businessEntityId = gridDataReturn(businessEntityId);

                cmd.Parameters.Add(new SqlParameter("@FirstName", txtFirstName.Text));
                cmd.Parameters.Add(new SqlParameter("@LastName", txtLastName.Text));
                cmd.Parameters.Add(new SqlParameter("@PersonType", cmbPersonType.SelectedItem.ToString()));
                cmd.Parameters.Add(new SqlParameter("@AddressLine1", txtAddressLine1.Text));
                cmd.Parameters.Add(new SqlParameter("@AddressLine2", txtAddressLine2.Text));
                cmd.Parameters.Add(new SqlParameter("@AddressType", 2/*Convert.ToInt32(cmbAdressType.SelectedItem.ToString()))*/));
                cmd.Parameters.Add(new SqlParameter("@PostalCode", Convert.ToInt32(txtPostal.Text.ToString())));
                cmd.Parameters.Add(new SqlParameter("@TerritoryName", cmbRegionType.SelectedItem.ToString()));
                cmd.Parameters.Add(new SqlParameter("@StateProvince", cmbState.SelectedItem.ToString()));
                cmd.Parameters.Add(new SqlParameter("@City", txtCity.Text));
                cmd.Parameters.Add(new SqlParameter("@StoreID", businessEntityId));

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
                MessageBox.Show("Customer Successfully Added!");
                txtFirstName.Text = string.Empty;
                txtLastName.Text = string.Empty;
                txtAddressLine1.Text = string.Empty;
                txtAddressLine2.Text = string.Empty;
                txtPostal.Text = string.Empty;
                cmbRegionType.SelectedIndex = -1;
                cmbState.SelectedIndex = -1;
                txtCity.Text = string.Empty;
                cmbPersonType.SelectedIndex = -1;
                cmbAdressType.SelectedIndex = -1;
                storeGrid.UnselectAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private int gridDataReturn(int businessEntityId)
        {
            businessEntityId = 0;
            if (storeGrid.SelectedItem != null)
            {

                DataRowView rowView = (DataRowView)storeGrid.SelectedItem;


                businessEntityId = (int)rowView["BusinessEntityID"];




            }
            return businessEntityId;
        }

        private void txtFirstName_TextChanged(object sender, TextChangedEventArgs e)
        {

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
        private int getStatesOnTerId()
        {
            return 0;
        }
        private void cmbRegionType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            storeGrid.ItemsSource = null;
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

            LoadData();
        }

        private void cmbState_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}

