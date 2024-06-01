using NvvmFinal.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
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
using NvvmFinal.Views;
using System.Security.Cryptography.X509Certificates;


namespace NvvmFinal.Views.Additions

{

    public partial class CustomerDetails : Window
    {
        private string fullname;
        NewSale newSale;
        private int customerID;
        public CustomerDetails(int CustomerID)
        {
            InitializeComponent();
            customerID = CustomerID;
            LoadData(CustomerID);
            FillDetails(CustomerID);
            getCustomerID(CustomerID);

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
        public void LoadData(int CustomerID)
        {
            string connectionString = GetConnectionString();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    string query = $"select ProductId,[Product Name],SalesOrderID,SalesOrderNumber,SalesOrderID,OrderStatus,OrderDate,DueDate,ShipDate from customerDetails as cd WHERE ID like '{CustomerID}';\t";
                    SqlCommand cmd = new SqlCommand(query, con);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    ViewGrid.ItemsSource = dt.DefaultView;

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to load data. Error: {ex.Message}");
                }
            }
        }

        public void FillDetails(int CustomerID)
        {
            string connectionString = GetConnectionString();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    string query = $"select * from customerDetails as cd WHERE ID like '{CustomerID}';";
                    SqlCommand cmd = new SqlCommand(query, con);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {

                        fullNameBlock.Text = reader["FirstName"].ToString() + " " + reader["LastName"].ToString();
                        idBlk.Text = reader["ID"].ToString();
                        addressLine1Blk.Text = reader["AddressLine1"].ToString();
                        postalBlk.Text = reader["PostalCode"].ToString();
                        stateBlk.Text = reader["State"].ToString();
                        CityBlk.Text = reader["City"].ToString();
                        country1Blk.Text = "United States";//reader["State"].ToString();

                        fullname = reader["FirstName"].ToString() + " " + reader["LastName"].ToString();
                    }
                    reader.Close();

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to load data. Error: {ex.Message}");
                }
            }

        }

        private void closeBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();

        }

        private void mnmzBtn_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        private int getCustomerID(int customerID)
        {
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
        private void addOrder_Click(object sender, RoutedEventArgs e)
        {
            newSale = new NewSale(fullname, customerID, GetAddressID(customerID), GetTerritoryID(customerID), GetCreditCardID(customerID));
            newSale.Show();
            this.Close();

        }
    }
}
