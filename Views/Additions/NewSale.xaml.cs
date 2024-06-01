using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using NvvmFinal.ViewModels;
using System.Configuration;
using System.Windows.Forms;
using System.Collections.ObjectModel;


namespace NvvmFinal.Views.Additions
{
    /// <summary>
    /// Interaction logic for NewSale.xaml
    /// </summary>
    public partial class NewSale : Window
    {
        private int customerID;
        private int addressID;
        private int territoryID;
        private int creditID;
        public NewSale(string fullName, int customerID, int addressID, int territoryID, int creditID)
        {

            InitializeComponent();
            LoadData();
            LoadData3();
            TotalCost.DataContext = mainViewModel;
            fullNameBlock.Text = fullName;

            this.customerID = customerID;
            this.addressID = addressID;
            this.territoryID = territoryID;
            this.creditID = creditID;


        }
        private string GetConnectionString()
        {

            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["AdventureWorks2019"];
            if (settings != null)
                return settings.ConnectionString;
            throw new Exception("Connection string for AdventureWorks2019 not found.");
        }
        private void LoadData2(string categoryName)
        {
            productCb.Items.Clear();

            SqlCommand command;
            SqlDataReader dataReader;
            string connectionString = GetConnectionString();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    string query = @"SELECT DISTINCT p.Name FROM Production.Product AS p 
                                    JOIN Production.ProductSubcategory AS ps ON p.ProductSubcategoryID = ps.ProductSubcategoryID 
                                    JOIN Production.ProductCategory AS pc ON ps.ProductCategoryID = pc.ProductCategoryID 
                                     WHERE pc.Name = @Category";
                    command = new SqlCommand(query, con);
                    command.Parameters.AddWithValue("@Category", categoryName);

                    dataReader = command.ExecuteReader();
                    while (dataReader.Read())
                    {
                        productCb.Items.Add(dataReader.GetValue(0));
                    }
                    dataReader.Close();
                    command.Dispose();
                    con.Close();
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"Failed to load data. Error: {ex.Message}");
                }
            }
        }
        private DataTable dataTable;

        private void LoadDataToGridView()
        {
            string selectedName = productCb.SelectedItem.ToString();
            string connectionString = GetConnectionString();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"SELECT DISTINCT ProductID, ProductNumber, ListPrice, Name FROM production.Product 
                             WHERE Name = @ProductName";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@ProductName", selectedName);

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    if (!dataTable.Columns.Contains("Quantity"))
                    {
                        dataTable.Columns.Add("Quantity", typeof(int));
                    }

                    foreach (DataRow row in dataTable.Rows)
                    {
                        row["Quantity"] = quantity.Text;
                    }

                    DataView view = ViewGrid.ItemsSource as DataView;
                    DataTable existingDataTable = view?.Table;

                    if (existingDataTable != null)
                    {
                        existingDataTable.Merge(dataTable, true, MissingSchemaAction.Ignore);
                    }
                    else
                    {
                        ViewGrid.ItemsSource = dataTable.DefaultView;

                    }

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Failed to load data to DataGridView. Error: {ex.Message}");
            }
        }
        private void LoadData()
        {
            SqlCommand command;
            SqlDataReader dataReader;
            string connectionString = GetConnectionString();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    string query = "SELECT DISTINCT Name FROM Production.ProductCategory ";
                    command = new SqlCommand(query, con);

                    dataReader = command.ExecuteReader();
                    while (dataReader.Read())
                    {
                        categoryCB.Items.Add(dataReader.GetValue(0));
                    }
                    dataReader.Close();
                    command.Dispose();
                    con.Close();
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"Failed to load data. Error: {ex.Message}");
                }
            }
        }
        private void categoryCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedCategory = categoryCB.SelectedItem.ToString();
            LoadData2(selectedCategory);
        }


        private void LoadData3()
        {
            SqlCommand command;
            SqlDataReader dataReader;
            string connectionString = GetConnectionString();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    string query = "SELECT DISTINCT Name FROM Purchasing.ShipMethod ";
                    command = new SqlCommand(query, con);

                    dataReader = command.ExecuteReader();
                    while (dataReader.Read())
                    {
                        shippingCb.Items.Add(dataReader.GetValue(0));
                    }
                    dataReader.Close();
                    command.Dispose();
                    con.Close();
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"Failed to load data. Error: {ex.Message}");
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

        private void shippingCb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            string selectedShipMethod = shippingCb.SelectedItem.ToString();
            string connectionString = GetConnectionString();

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string query = "SELECT ShipBase FROM Purchasing.ShipMethod WHERE Name = @ShipMethodName";
                    SqlCommand command = new SqlCommand(query, con);
                    command.Parameters.AddWithValue("@ShipMethodName", selectedShipMethod);

                    object result = command.ExecuteScalar();
                    if (result != null)
                    {
                        shippingCostBlk.Text = result.ToString();
                    }


                    con.Close();
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Failed to load data. Error: {ex.Message}");
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            LoadDataToGridView();
            shippingCostBlk.Text = CqltCost().ToString();
            totalCostBlk.Text = finalPriceCqlt().ToString();
        }

        private decimal initialPriceCqlt()
        {
            decimal totalPrice = 0;

            foreach (var item in ViewGrid.Items)
            {

                if (item is DataRowView dataRowView && !dataRowView.IsNew)
                {
                    if (dataRowView["ListPrice"] != DBNull.Value && dataRowView["Quantity"] != DBNull.Value)
                    {
                        decimal.TryParse(dataRowView["ListPrice"].ToString(), out decimal listPrice);
                        decimal.TryParse(dataRowView["Quantity"].ToString(), out decimal quantity);

                        totalPrice += listPrice * quantity;
                    }
                }
            }
            return totalPrice;
        }
        private decimal CqltCost()
        {
            decimal totalShippingCost = 0;

            string selectedShippingMethod = shippingCb.SelectedItem?.ToString();

            int shipMethodID = GetIDFromName(selectedShippingMethod, "ShipMethodID", "Purchasing.ShipMethod");

            Dictionary<int, decimal> productWeights = new Dictionary<int, decimal>();

            foreach (var item in ViewGrid.Items)
            {
                if (item is DataRowView dataRowView && !dataRowView.IsNew)
                {
                    string productName = dataRowView["Name"].ToString();
                    decimal quantity = Convert.ToDecimal(dataRowView["Quantity"]);
                    //
                    //System.Windows.MessageBox.Show($"Quantity: {quantity}");

                    int productID = GetProductID(productName);
                    decimal productWeight = GetProductWeight(productID);

                    if (!productWeights.ContainsKey(productID))
                    {
                        productWeights.Add(productID, 0);
                    }

                    productWeights[productID] += productWeight * quantity;
                }
            }

            foreach (var weightEntry in productWeights)
            {
                totalShippingCost += CalculateShippingCost(weightEntry.Value, shipMethodID);
            }

            //System.Windows.MessageBox.Show($"Total Shipping Cost: {totalShippingCost}");

            return totalShippingCost;
        }

        private decimal GetProductWeight(int productID)
        {
            decimal productWeight = 0;

            string connectionString = GetConnectionString();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT Weight FROM Production.Product WHERE ProductID = @ProductID";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ProductID", productID);
                object result = command.ExecuteScalar();
                if (result != DBNull.Value)
                {
                    productWeight = Convert.ToDecimal(result);
                    //System.Windows.MessageBox.Show($"Product Weight: {productWeight}");
                }
            }

            return productWeight;
        }

        private decimal CalculateShippingCost(decimal weight, int shipMethodID)
        {
            decimal shippingCost = 0;

            string connectionString = GetConnectionString();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT ShipRate, ShipBase FROM Purchasing.ShipMethod WHERE ShipMethodID = @ShipMethodID";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ShipMethodID", shipMethodID);
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        decimal shipRate = Convert.ToDecimal(reader["ShipRate"]);
                        //System.Windows.MessageBox.Show($"Ship Rate: {shipRate}");
                        decimal shipBase = Convert.ToDecimal(reader["ShipBase"]);
                        //System.Windows.MessageBox.Show($"Ship Base: {shipBase}");
                        shippingCost = (weight * shipRate) + shipBase;
                        //System.Windows.MessageBox.Show($"Total Shipping Cost: {shippingCost}");
                    }
                }
                reader.Close();
            }

            return shippingCost;
        }


        MainViewModel mainViewModel = new MainViewModel();

        private decimal finalPriceCqlt()
        {
            decimal totalPrice = 0;
            decimal tax = 0;
            decimal initialCost = initialPriceCqlt();
            decimal totalShippingCost = CqltCost();


            dataTable = new DataTable();

            if (dataTable != null)
            {
                tax = initialCost * (decimal)(24.0 / 100);
                totalPrice = initialCost + tax + totalShippingCost;

                mainViewModel.TotalCostt = tax;
                return totalPrice;
            }
            else
            {

                return 0;
            }
        }



        private void UpdateStock(int productId, int quantity)
        {
            SqlConnection connection = new SqlConnection(GetConnectionString());

            try
            {
                connection.Open();

                int safetyStockLevel = 0;
                int reorderPoint = 0;
                int currentStock = 0;

                SqlCommand command = new SqlCommand("SELECT SafetyStockLevel FROM production.Product WHERE ProductID = @ProductID", connection);
                command.Parameters.AddWithValue("@ProductID", productId);
                safetyStockLevel = Convert.ToInt32(command.ExecuteScalar());

                command = new SqlCommand("SELECT ReorderPoint FROM production.Product WHERE ProductID = @ProductID", connection);
                command.Parameters.AddWithValue("@ProductID", productId);
                reorderPoint = Convert.ToInt32(command.ExecuteScalar());

                command = new SqlCommand("SELECT * FROM ProductSummary WHERE ProductID = @ProductID;", connection);
                command.Parameters.AddWithValue("@ProductID", productId);
                currentStock = Convert.ToInt32(command.ExecuteScalar());


                if (currentStock - quantity < reorderPoint)
                {
                    SqlCommand ncommand = new SqlCommand("SELECT Name FROM production.Product WHERE ProductID = @ProductID", connection);
                    ncommand.Parameters.AddWithValue("@ProductID", productId);
                    string pname = ncommand.ExecuteScalar().ToString();

                    System.Windows.MessageBox.Show("Το κατάστημα πρέπει να προμηθευτεί το προϊόν: " + pname);
                    return;
                }
                else
                {
                    SqlCommand updateCommand = new SqlCommand("UpdateStock", connection);
                    updateCommand.CommandType = CommandType.StoredProcedure;
                    updateCommand.Parameters.AddWithValue("@ProductID", productId);
                    updateCommand.Parameters.AddWithValue("@Quantity", quantity);
                    updateCommand.ExecuteNonQuery();
                    System.Windows.MessageBox.Show("Η παραγγελία σου πραγματοποίηθηκε ");

                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Σφάλμα κατά την ενημέρωση του αποθέματος: " + ex.Message);
            }
            connection.Close();

        }
        private void InsertSalesOrder(Dictionary<int, (int Quantity, decimal ListPrice)> products)
        {
            string connectionString = GetConnectionString();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    foreach (var product in products)
                    {
                        for (int i = 0; i < product.Value.Quantity; i++)
                        {
                            SqlCommand cmd = new SqlCommand("InsertSalesOrder", connection);
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@ProductID", product.Key);
                            cmd.Parameters.AddWithValue("@Quantity", product.Value.Quantity);
                            cmd.Parameters.AddWithValue("@ListPrice", product.Value.ListPrice);

                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("Σφάλμα κατά την εισαγωγή της παραγγελίας: " + ex.Message);
                }
            }
        }

        private void InsertSalesHeader(int customerID, int addressID, int shipmethod, int territoryID, int creditID, decimal freight, decimal tax)
        {
            string connectionString = GetConnectionString();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();



                    SqlCommand cmd = new SqlCommand("InsertSalesHeader", connection);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CustomerID", customerID);
                    cmd.Parameters.AddWithValue("@AddressID", addressID);
                    cmd.Parameters.AddWithValue("@ShipMethodID", shipmethod);
                    cmd.Parameters.AddWithValue("@TerritoryID", territoryID);
                    cmd.Parameters.AddWithValue("@CreditCardID", creditID);
                    cmd.Parameters.AddWithValue("@freight", freight);
                    cmd.Parameters.AddWithValue("@tax", tax);

                    cmd.ExecuteNonQuery();


                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("Σφάλμα κατά την εισαγωγή της παραγγελίας: " + ex.Message);
                }
            }
        }

        private int GetIDFromName(string name, string columnName, string tableName)
        {
            int id = -1;
            string connectionString = GetConnectionString();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = $"SELECT ShipMethodID FROM Purchasing.ShipMethod WHERE Name = @Name";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Name", name);

                    object result = command.ExecuteScalar();

                    if (result != null)
                    {
                        id = Convert.ToInt32(result);
                    }

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Απέτυχε η ανάκτηση του ShipMethodID. Σφάλμα: {ex.Message}");
            }

            return id;
        }
        private Dictionary<int, (int Quantity, decimal ListPrice)> GetSelectedProductsAndQuantities()
        {
            var productsDetails = new Dictionary<int, (int Quantity, decimal ListPrice)>();

            try
            {
                foreach (var item in ViewGrid.Items)
                {
                    if (item is DataRowView rowView)
                    {
                        int productID = Convert.ToInt32(rowView["ProductID"]);
                        int quantity = Convert.ToInt32(rowView["Quantity"]);
                        decimal listPrice = GetProductListPrice(productID);

                        productsDetails.Add(productID, (Quantity: quantity, ListPrice: listPrice));
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Same Product Exists 2 Times In The Cart\n\nTry Changing The Quantity");
            }

            return productsDetails;
        }

        private decimal GetProductListPrice(int productID)
        {
            decimal listPrice = -1;
            string connectionString = GetConnectionString();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT ListPrice FROM Production.Product WHERE ProductID = @ProductID";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@ProductID", productID);

                    object result = command.ExecuteScalar();

                    if (result != null)
                    {
                        listPrice = Convert.ToDecimal(result);
                    }

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Failed to retrieve ListPrice for ProductID {productID}. Error: {ex.Message}");
            }

            return listPrice;
        }


        private int GetProductID(string productName)
        {
            int productID = -1;

            string connectionString = GetConnectionString();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT ProductID FROM production.Product WHERE Name = @ProductName";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@ProductName", productName);

                    object result = command.ExecuteScalar();

                    if (result != null)
                    {
                        productID = Convert.ToInt32(result);
                    }

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Απέτυχε η ανάκτηση του ProductID. Σφάλμα: {ex.Message}");
            }

            return productID;
        }



        private void order_Click(object sender, RoutedEventArgs e)
        {
            string selectedProduct = productCb.SelectedItem.ToString();
            int productID = GetProductID(selectedProduct);

            if (productID != -1)
            {
                List<UpdateProducts> products = new List<UpdateProducts>();

                foreach (var item in ViewGrid.Items)
                {
                    if (item is DataRowView rowView)
                    {
                        int product = Convert.ToInt32(rowView["ProductID"]);
                        int quantity = Convert.ToInt32(rowView["Quantity"]);
                        products.Add(new UpdateProducts { ProductID = product, Quantity = quantity });
                    }
                }

                foreach (var product in products)
                {
                    UpdateStock(product.ProductID, product.Quantity);
                }

                string shipMethodName = shippingCb.SelectedItem.ToString();
                int shipMethodID = GetIDFromName(shipMethodName, "ShipMethodID", "Purchasing.ShipMethod");

                decimal freight = decimal.Parse(shippingCostBlk.Text);
                decimal tax = mainViewModel.TotalCostt;

                InsertSalesHeader(customerID, addressID, shipMethodID, territoryID, creditID, freight, tax);
                InsertSalesOrder(GetSelectedProductsAndQuantities());

            }
            else
            {
                System.Windows.MessageBox.Show("Το προϊόν δεν βρέθηκε.");
            }

        }


        private void productCb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            quantity.Text = "1";
        }

        private void deleteRow_Click(object sender, RoutedEventArgs e)
        {
            List<DataRowView> rowsToDelete = new List<DataRowView>();

            foreach (var item in ViewGrid.SelectedItems)
            {
                if (item is DataRowView selectedRow)
                {
                    rowsToDelete.Add(selectedRow);
                }
            }
            foreach (var dataRowView in rowsToDelete)
            {
                dataRowView.Row.Delete();
            }
            shippingCostBlk.Text = CqltCost().ToString();
            totalCostBlk.Text = finalPriceCqlt().ToString();
        }


        private void ViewGrid_CurrentCellChanged(object sender, EventArgs e)
        {
            shippingCostBlk.Text = CqltCost().ToString();
            totalCostBlk.Text = finalPriceCqlt().ToString();
        }


    }


}
