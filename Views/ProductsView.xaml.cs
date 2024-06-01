using NvvmFinal.Views.Additions;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NvvmFinal.Views
{
    /// <summary>
    /// Interaction logic for ProductsView.xaml
    /// </summary>
    public partial class ProductsView : System.Windows.Controls.UserControl
    {
        string str = null;
        public ProductsView()
        {
            InitializeComponent();
            comboBox.SelectedIndex = 0;
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            ProductAddition productAddition = new ProductAddition();
            productAddition.Show();
        }
        private string GetConnectionString()
        {
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["AdventureWorks2019"];
            if (settings != null)
                return settings.ConnectionString;
            throw new Exception("Connection string for AdventureWorks2019 not found.");
        }
        private void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            if (MyDataGrid.SelectedItem != null)
            {
                DataRowView row = (DataRowView)MyDataGrid.SelectedItem;
                string? productNumber = row["ProductNumber"].ToString();

                MessageBoxResult result = System.Windows.MessageBox.Show("Are you sure you want to delete this product?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    string query = "DELETE FROM Production.Product WHERE ProductNumber = @ProductNumber";

                    using (SqlConnection con = new SqlConnection(GetConnectionString()))
                    {
                        try
                        {
                            con.Open();
                            SqlCommand cmd = new SqlCommand(query, con);
                            cmd.Parameters.AddWithValue("@ProductNumber", productNumber);
                            int rowsAffected = cmd.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                System.Windows.MessageBox.Show("Product deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                                LoadData();
                            }
                            else
                            {
                                System.Windows.MessageBox.Show("Failed to delete product.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Windows.MessageBox.Show($"Failed to delete product. Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
            else
            {
                System.Windows.MessageBox.Show("Please select a product to delete.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }


        private void LoadData()
        {
            string query = "SELECT ProductID, Name, StandardCost, ListPrice, SafetyStockLevel, ReorderPoint, SellStartDate FROM Production.Product";

            using (SqlConnection con = new SqlConnection(GetConnectionString()))
            {
                try
                {
                    con.Open();
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

        private void UserControl_Loaded_2(object sender, RoutedEventArgs e)
        {
         
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = txtSearch.Text;

            if (!string.IsNullOrEmpty(searchText))
            {
                string connectionString = GetConnectionString();
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    try
                    {
                        con.Open();
                        string query = string.Empty;

                        if (comboBox.SelectedItem is ComboBoxItem selectedItem)
                        {
                            switch (selectedItem.Content.ToString())
                            {
                                case "All Products":
                                    query = $"SELECT ProductID, Name, StandardCost, ListPrice, SafetyStockLevel, ReorderPoint, SellStartDate FROM Production.Product " +
                                            $"WHERE Name LIKE '{searchText}%' ORDER BY Name";
                                    break;

                                case "Total Inventory":
                                    query = $"SELECT * FROM TotalInventory WHERE Name LIKE '{searchText}%' ORDER BY Name";
                                    break;

                                case "Detailed Inventory":
                                    query = $"SELECT * FROM DetailedInventory WHERE Name LIKE '{searchText}%' ORDER BY Name";
                                    break;
                            }
                        }

                        if (!string.IsNullOrEmpty(query))
                        {
                            SqlCommand cmd = new SqlCommand(query, con);
                            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            MyDataGrid.ItemsSource = dt.DefaultView;
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Windows.Forms.MessageBox.Show($"Failed to load data. Error: {ex.Message}");
                    }
                    
                        con.Close();
                    

                    bool toInt = int.TryParse(txtSearch.Text, out int result);
                    if (toInt)
                    {
                        try
                        {
                            con.Open();
                            string query = string.Empty;

                            if (comboBox.SelectedItem is ComboBoxItem selectedItem)
                            {
                                switch (selectedItem.Content.ToString())
                                {
                                    case "All Products":
                                        query = $"SELECT ProductID, Name, StandardCost, ListPrice, SafetyStockLevel, ReorderPoint, SellStartDate FROM Production.Product " +
                                                $"WHERE ProductID LIKE '{searchText}%' ORDER BY ProductID";
                                        break;

                                    case "Total Inventory":
                                        query = $"SELECT * FROM TotalInventory WHERE ProductID LIKE '{searchText}%' ORDER BY ProductID";
                                        break;

                                    case "Detailed Inventory":
                                        query = $"SELECT * FROM DetailedInventory WHERE ProductID LIKE '{searchText}%' ORDER BY ProductID";
                                        break;
                                }
                            }

                            if (!string.IsNullOrEmpty(query))
                            {
                                SqlCommand cmd = new SqlCommand(query, con);
                                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                                DataTable dt = new DataTable();
                                adapter.Fill(dt);
                                MyDataGrid.ItemsSource = dt.DefaultView;
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Windows.Forms.MessageBox.Show($"Failed to load data. Error: {ex.Message}");
                        }
                       
                            con.Close();
                        
                    }
                }
            }
        }


        public int getProductID()
        {
            int productID = 0;

            if (MyDataGrid.SelectedItem is DataRowView rowView)
            {
                productID = Convert.ToInt32(rowView["ProductID"]);
            }

            return productID;
        }


        private ProductsDetails detailsWindow;
        private void MyDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int productID = getProductID();

            if (detailsWindow != null && detailsWindow.IsLoaded)
            {
                detailsWindow.Close();
            }

            detailsWindow = new ProductsDetails(productID);
            detailsWindow.Show();
        }
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBox.SelectedItem is ComboBoxItem selectedItem && selectedItem.Content.ToString() == "Total Inventory")
            {
                LoadTotalInventory();
            }
            else if (comboBox.SelectedItem is ComboBoxItem selectedItem2 && selectedItem2.Content.ToString() == "Detailed Inventory")
            {
                LoadDetailedInventory();
            }
            else if (comboBox.SelectedItem is ComboBoxItem selectedItem3 && selectedItem3.Content.ToString() == "All Products")
            {
                LoadData();
            }
        }

        private void LoadTotalInventory()
        {
            string connectionString = GetConnectionString();
            string query = @"SELECT * FROM TotalInventory";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlDataAdapter dataAdapter = new SqlDataAdapter(query, connection);
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);
                MyDataGrid.ItemsSource = dataTable.DefaultView;
            }
        }
        private void LoadDetailedInventory()
        {
            string connectionString = GetConnectionString();
            string query = @"SELECT * FROM DetailedInventory";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlDataAdapter dataAdapter = new SqlDataAdapter(query, connection);
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);
                MyDataGrid.ItemsSource = dataTable.DefaultView;
            }
        }


    }

}