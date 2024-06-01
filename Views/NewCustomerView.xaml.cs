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
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using NvvmFinal.Views.Additions;
using System.Windows.Forms;

namespace NvvmFinal.Views
{
    /// <summary>
    /// Interaction logic for NewCustomerView.xaml
    /// </summary>
    public partial class NewCustomerView : System.Windows.Controls.UserControl
    {

        string str = null;
        public NewCustomerView()
        {
            InitializeComponent();
        }
        private void UserControl_Loaded_2(object sender, RoutedEventArgs e)
        {
            LoadData();
        }
        private void addCustomer_Click(object sender, RoutedEventArgs e)
        {
            CustomerAddition customer = new CustomerAddition();
            customer.Show();
        }
        private string GetConnectionString()
        {

            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["AdventureWorks2019"];
            if (settings != null)
                return settings.ConnectionString;
            throw new Exception("Connection string for AdventureWorks2019 not found.");
        }
        private void LoadData()
        {
            string connectionString = GetConnectionString();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    string query = "SELECT * FROM AllCustomers";
                    SqlCommand cmd = new SqlCommand(query, con);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    MyDataGrid.ItemsSource = dt.DefaultView;

                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show($"Failed to load data. Error: {ex.Message}");
                }
            }
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
                        string query = $"SELECT * FROM AllCustomers " +
                            $" WHERE LastName LIKE '{txtSearch.Text}%'" +
                            $" ORDER BY LastName ";
                        SqlCommand cmd = new SqlCommand(query, con);
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        MyDataGrid.ItemsSource = dt.DefaultView;
                        str = txtSearch.Text;

                    }

                    catch (Exception ex)
                    {
                        System.Windows.Forms.MessageBox.Show($"Failed to load data. Error: {ex.Message}");
                    }

                    if (txtSearch.Text.Contains(" "))
                    {
                        int spaceIndex = txtSearch.Text.IndexOf(" ");
                        string lastName = txtSearch.Text.Substring(0, spaceIndex);
                        string firstName = txtSearch.Text.Substring(spaceIndex + 1);
                        string query = $"SELECT * FROM AllCustomers" +
                            $" WHERE LastName LIKE '{lastName}%' AND FirstName Like '{firstName}%'" +
                            $" ORDER BY LastName ";
                        SqlCommand cmd = new SqlCommand(query, con);
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        MyDataGrid.ItemsSource = dt.DefaultView;
                    }
                }
                con.Close();
                bool toInt = int.TryParse(txtSearch.Text, out int result);
                if (toInt)
                {
                    con.Open();
                    string query = $"SELECT * FROM AllCustomers" +
                        $" WHERE CustomerID LIKE '{txtSearch.Text}%'" +
                        $" ORDER BY CustomerID";
                    SqlCommand cmd = new SqlCommand(query, con);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    MyDataGrid.ItemsSource = dt.DefaultView;
                    str = txtSearch.Text;

                }
                con.Close();


            }
        }
        public int getCustomerID()
        {
            int customerID = 0;

            if (MyDataGrid.SelectedItem is DataRowView row)
            {
                customerID = Convert.ToInt32(row["CustomerID"]);
            }
            return customerID;
        }
        private int GetAddressID(int customerID)
        {
            int addressID = -1;

            string connectionString = GetConnectionString();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string query = $"select pa.AddressID as AddressID from person.businessentity as b " +
               $"join person.businessentityaddress as bea on b.businessentityid = bea.businessentityid " +
               $"join person.person as pp on b.businessentityid = pp.businessentityid " +
               $"join person.address as pa on bea.addressid = pa.addressid " +
               $"join sales.customer as sc on pp.businessentityid = sc.personid WHERE CustomerID = {customerID}";

                SqlCommand cmd = new SqlCommand(query, con);
                object address = cmd.ExecuteScalar();

                if (address != null)
                {
                    addressID = Convert.ToInt32(address);
                }

                con.Close();
            }

            return addressID;
        }
        private int GetTerritoryID(int customerID)
        {
            int territoryID = -1;

            string connectionString = GetConnectionString();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string query = $"select TerritoryID from Sales.Customer WHERE CustomerID = {customerID}";

                SqlCommand cmd = new SqlCommand(query, con);
                object territory = cmd.ExecuteScalar();

                if (territory != null)
                {
                    territoryID = Convert.ToInt32(territory);
                }

                con.Close();
            }

            return territoryID;
        }
        private int GetCreditCardID(int customerID)
        {
            int creditID = -1;

            string connectionString = GetConnectionString();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string query = $"select * from sales.PersonCreditCard pc join person.person pp on pc.BusinessEntityID = pp.BusinessEntityID " +
                               $"join sales.customer sc on pp.BusinessEntityID = sc.CustomerID or pp.BusinessEntityID = sc.CustomerID " +
                               $"WHERE CustomerID = {customerID}";

                SqlCommand cmd = new SqlCommand(query, con);
                object credit = cmd.ExecuteScalar();

                if (credit != null)
                {
                    creditID = Convert.ToInt32(credit);
                }

                con.Close();
            }

            return creditID;
        }
        private CustomerDetails detailsCustomerWindow;

        private void MyDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int customerID = getCustomerID();

            // Close the existing window if it's open
            if (detailsCustomerWindow != null && detailsCustomerWindow.IsLoaded)
            {
                detailsCustomerWindow.Close();
            }

            detailsCustomerWindow = new CustomerDetails(customerID);
            detailsCustomerWindow.Show();
        }

        private void addOrder_Click(object sender, RoutedEventArgs e)
        {

            if (MyDataGrid.SelectedItem != null)
            {
                DataRowView dataRowView = MyDataGrid.SelectedItem as DataRowView;
                int customerID = getCustomerID();
                string fullName = null;
                int addressID = GetAddressID(customerID);
                int territoryID = GetTerritoryID(customerID);
                int creditID = GetCreditCardID(customerID);
                if (dataRowView != null)
                {
                    if (dataRowView["FirstName"] != null && dataRowView["LastName"] != null)
                    {
                        string fName = dataRowView["FirstName"].ToString();
                        string lName = dataRowView["LastName"].ToString();

                        fullName = $"{fName} {lName}";
                    }
                }

                if (fullName != null)
                {
                    NewSale newSale = new NewSale(fullName, customerID, addressID, territoryID, creditID);
                    newSale.Show();
                }
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Select a Customer First!");
            }

        }
        private void ExecTrigger()
        {
            int customerID = getCustomerID();
            string connectionString = GetConnectionString();
            using (SqlConnection conn = new SqlConnection(connectionString))

            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("DeleteCustomer", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter("@CustomerID", customerID));

                cmd.ExecuteNonQuery();

            }
        }
        private void delete_Click(object sender, RoutedEventArgs e)
        {

            if (MyDataGrid.SelectedItem is DataRowView dataRowView)
            {
                if (System.Windows.Forms.MessageBox.Show("Are you sure?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    ExecTrigger();
                    dataRowView.Row.Delete();
                    System.Windows.Forms.MessageBox.Show($"Customer {getCustomerID()} Successfully Deleted");

                }
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Select a Customer First");
            }
        }
    }
}
