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

namespace NvvmFinal.Views.Additions
{
    /// <summary>
    /// Interaction logic for ProductsDetails.xaml
    /// </summary>
    public partial class ProductsDetails : Window
    {
        private int productID;
        public ProductsDetails(int productID)
        {
            InitializeComponent();
            this.productID = productID;

            LoadData(productID);
            FillDetails(productID);
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

        public void LoadData(int productID)
        {
            string connectionString = GetConnectionString();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    string query = "SELECT Name, StandardCost, ListPrice, SellStartDate, SellEndDate " +
                                   "FROM production.product AS sd " +
                                   "WHERE sd.Productid = @productid;";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@productid", productID);
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

        public void FillDetails(int productID)
        {
            string connectionString = GetConnectionString();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    string query = $"select * from productsdetails where productid like '{productID}';";
                    SqlCommand cmd = new SqlCommand(query, con);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        InvoiceBlock.Text = reader["ProductID"].ToString();
                        fullNameBlock.Text = reader["ProductName"].ToString();
                        addressLine1Blk.Text = reader["ProductNumber"].ToString();
                        postalBlk.Text = reader["Type"].ToString();
                        InOBlk.Text = reader["Size"].ToString();
                        CityBlk.Text = reader["CategoryName"].ToString();
                        orderDBlk.Text = reader["Color"].ToString();
                        InvNoBlk.Text = reader["Weight"].ToString();
                        CostBlock.Text = reader["ReorderPoint"].ToString();
                        taxBlk.Text = reader["SafetyStockLevel"].ToString();
                        shipBlk.Text = reader["MakeFlag"].ToString();
                        totalBlk.Text = reader["FinishedGoodsFlag"].ToString();
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
    }
}