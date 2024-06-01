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
using static NvvmFinal.Views.HomePageView;

namespace NvvmFinal.Views.Additions
{
    /// <summary>
    /// Interaction logic for NewOrder.xaml
    /// </summary>
    public partial class NewOrder : Window
    {

        private int productID;
        private int quantity;

        public NewOrder(int productID, int quantity)
        {
            InitializeComponent();
            this.productID=productID;
            this.quantity = quantity;
            TotalCost.DataContext = mainViewModel;
            setCategory();
            LoadData3();
            LoadVendors();

        }
        private string GetConnectionString()
        {

            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["AdventureWorks2019"];
            if (settings != null)
                return settings.ConnectionString;
            throw new Exception("Connection string for AdventureWorks2019 not found.");
        }
        private string setProductName()
        {
            string productName = null;
            SqlCommand command;
            SqlDataReader dataReader;
            string connectionString = GetConnectionString();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    string query = $"SELECT p.Name FROM Production.Product AS p where productID = {productID}";
                    command = new SqlCommand(query, con);
                    dataReader = command.ExecuteReader();
                    while (dataReader.Read())
                    {
                        productName = dataReader.GetValue(0).ToString();
                    }
                    productCb.Text = productName;
                    dataReader.Close();
                    command.Dispose();
                    con.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to load data. Error: {ex.Message}");
                }
            }
            return productName;
        }
        private void setCategory()
        {
            string productName = setProductName();
            
            SqlCommand command;
            SqlDataReader dataReader;
            string connectionString = GetConnectionString();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    string query = $@"SELECT DISTINCT ps.Name FROM Production.Product AS p 
                                    JOIN Production.ProductSubcategory AS ps ON p.ProductSubcategoryID = ps.ProductSubcategoryID 
                                    JOIN Production.ProductCategory AS pc ON ps.ProductCategoryID = pc.ProductCategoryID 
                                     WHERE p.Name like  '{productName}'";
                    command = new SqlCommand(query, con);
                    dataReader = command.ExecuteReader();
                    while (dataReader.Read())
                    {
                        categoryCB.Text = dataReader.GetValue(0).ToString();
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
        private void LoadDataToGridView(string vendorName)
        {
            string selectedName = ToString();
            string connectionString = GetConnectionString();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = $@"SELECT pv.Name as VendorName, pp.ProductID ,[StandardPrice],pp.Name as ProductName
                              FROM [Purchasing].[ProductVendor]
                              join Production.Product as pp on ProductVendor.ProductID = pp.ProductID
                              join Purchasing.Vendor as pv on pv.BusinessEntityID = ProductVendor.BusinessEntityID
                              where pp.ProductID = @productID AND pv.name like '{vendorName}'";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@productID", productID);
                    
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    if (!dataTable.Columns.Contains("Quantity"))
                    {
                        dataTable.Columns.Add("Quantity", typeof(int));
                    }

                    foreach (DataRow row in dataTable.Rows)
                    {
                        row["Quantity"] = quantity;  
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
        private void LoadVendors()
        {
            string connectionString = GetConnectionString();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();

                    
                    SqlCommand command = new SqlCommand("vendorNamePrice", con);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@productID", productID);                    
                    SqlDataReader dataReader = command.ExecuteReader();
                    VendorCB.Items.Clear();
                    while (dataReader.Read())
                    {
                        VendorCB.Items.Add(dataReader.GetString(0));
                    }
                    dataReader.Close();
                    command.Dispose();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to load data. Error: {ex.Message}");
                }
                VendorCB.SelectedItem = 0;
            }
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
            shippingCostBlk.Text = CqltCost().ToString();
            totalCostBlk.Text = finalPriceCqlt().ToString();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            decimal cost = Convert.ToDecimal(totalCostBlk.Text);
            foreach (var item in ViewGrid.Items)
            {
                if (item is DataRowView dataRowView && !dataRowView.IsNew)
                {

                    string vendorName = (string)dataRowView["VendorName"];
                    int productNumber = (Int32)dataRowView["ProductID"];
                    int.TryParse(dataRowView["Quantity"].ToString(), out int quantity);
                    string shipMethodName = shippingCb.SelectedItem.ToString();
                    int shipMethodID = GetIDFromName(shipMethodName, "ShipMethodID", "Purchasing.ShipMethod");

                    int VendorID = GetVendorIDFromName(vendorName, "BusinessEntityID", "Purchasing.Vendor");
                    decimal freight = decimal.Parse(shippingCostBlk.Text);
                    decimal tax = mainViewModel.TotalCostt;

                    bool productExists = false;

                    foreach (DataRow existingProduct in DataRepository.MainTable.Rows)
                    {
                        int productId = Convert.ToInt32(existingProduct["ProductID"]);
                        if (productId == productNumber)
                        {

                            existingProduct["Quantity"] = (int)existingProduct["Quantity"] + quantity;
                            productExists = true;
                            break;
                        }
                    }
                    if (!productExists)
                    {
                        
                        DataRepository.MainTable.Rows.Add(vendorName, productNumber, cost, quantity, shipMethodID, tax, freight, VendorID);
                    }
                }
            }

            HomePageView.Instance.calculateTotalCost();
            MessageBox.Show("Product Successfully Added To Cart");
        }

        private decimal initialPriceCqlt()
        {
            decimal totalPrice = 0;

            foreach (var item in ViewGrid.Items)
            {

                if (item is DataRowView dataRowView && !dataRowView.IsNew)
                {
                    if (dataRowView["StandardPrice"] != DBNull.Value && dataRowView["Quantity"] != DBNull.Value)
                    {
                        decimal.TryParse(dataRowView["StandardPrice"].ToString(), out decimal listPrice);
                        decimal.TryParse(dataRowView["Quantity"].ToString(), out decimal quantity);

                        totalPrice += listPrice * quantity;
                    }
                }
            }
            //MessageBox.Show(totalPrice.ToString());
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
                    string productName = dataRowView["ProductName"].ToString();
                    decimal quantity = Convert.ToDecimal(dataRowView["Quantity"]);

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
                        decimal shipBase = Convert.ToDecimal(reader["ShipBase"]);
                        shippingCost = (weight * shipRate) + shipBase;
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


        private decimal VendorPrice(int productID)
        {
            decimal price = 0;
            string connectionString = GetConnectionString();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT StandardPrice FROM Purchasing.ProductVendor WHERE ProductID = @ProductID";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@ProductID", productID);

                    object result = command.ExecuteScalar();

                    if (result != null)
                    {
                        price = Convert.ToDecimal(result);

                    }

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Απέτυχε η ανάκτηση του ProductID. Σφάλμα: {ex.Message}");
            }

            return price;
        }

        private void PurchaseOrderDetail(Dictionary<int, int> products)
        {
            string connectionString = GetConnectionString();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    foreach (var product in products)
                    {
                        int productID = product.Key;
                        int quantity = product.Value;
                        decimal standardPrice = VendorPrice(productID);

                        SqlCommand cmd = new SqlCommand("PurchaseOrderDetail", connection);
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@VendorCost", standardPrice);
                        cmd.Parameters.AddWithValue("@ProductID", productID);
                        cmd.Parameters.AddWithValue("@Quantity", quantity);

                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Σφάλμα κατά την εισαγωγή της παραγγελίας: " + ex.Message);
                }
            }
        }
        private void PurchaseOrderHeader(int vendorID, int shipmethod, decimal freight, decimal tax)
        {
            string connectionString = GetConnectionString();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();



                    SqlCommand cmd = new SqlCommand("PurchaseOrderHeader", connection);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@VendorID", vendorID);
                    cmd.Parameters.AddWithValue("@ShipMethodID", shipmethod);
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
        private int GetVendorIDFromName(string name, string columnName, string tableName)
        {
            int id = -1;
            string connectionString = GetConnectionString();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = $"SELECT BusinessEntityID FROM Purchasing.Vendor WHERE Name = @Name";

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
                System.Windows.MessageBox.Show($"Απέτυχε η ανάκτηση του VendorID. Σφάλμα: {ex.Message}");
            }

            return id;
        }
        private Dictionary<int, int> GetSelectedProductsAndQuantities()
        {
            Dictionary<int, int> productsAndQuantities = new Dictionary<int, int>();
            try
            {
                foreach (var item in ViewGrid.Items)
                {

                    if (item is DataRowView rowView)
                    {

                        string productName = (string)rowView["ProductName"];
                        int quantity = Convert.ToInt32(rowView["Quantity"]);
                        productsAndQuantities.Add(productID, quantity);
                    }

                }

            }

            catch (Exception ex)
            {
                MessageBox.Show("Same Product Exists 2 Times In The Cart\n\nTry Changing The Quantity");

            }
            return productsAndQuantities;
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
            string selectedProduct = ToString();
            if (productID != -1)
            {
                List<UpdateProducts> products = new List<UpdateProducts>();

                foreach (var item in ViewGrid.Items)
                {
                    if (item is DataRowView rowView)
                    {
                        string productName = rowView["ProductName"].ToString();
                        int quantity = Convert.ToInt32(rowView["Quantity"]);

                        int productID = GetProductID(productName);

                        products.Add(new UpdateProducts { ProductID = productID, Quantity = quantity });
                    }
                }
                string shipMethodName = shippingCb.SelectedItem.ToString();
                int shipMethodID = GetIDFromName(shipMethodName, "ShipMethodID", "Purchasing.ShipMethod");
                string VendorName = VendorCB.SelectedItem.ToString();
                int vendorID = GetVendorIDFromName(VendorName, "BusinessEntityID", "Purchasing.Vendor");
                decimal freight = decimal.Parse(shippingCostBlk.Text);
                decimal tax = mainViewModel.TotalCostt;

                PurchaseOrderHeader(vendorID, shipMethodID, freight, tax);
                PurchaseOrderDetail(GetSelectedProductsAndQuantities());
                MessageBox.Show("Product Approved ");
            }
            else
            {
                MessageBox.Show("Το προϊόν δεν βρέθηκε.");
            }

        }


        private void productCb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //quantity.Text = "1";
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


        private void ViewGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {

        }

        private void ViewGrid_CurrentCellChanged(object sender, EventArgs e)
        {
            shippingCostBlk.Text = CqltCost().ToString();
            totalCostBlk.Text = finalPriceCqlt().ToString();
        }

        private void VendorCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string vendorName = VendorCB.SelectedItem.ToString();

            //
            //MessageBox.Show(vendorName);
            LoadDataToGridView(vendorName);
        }
    }


}
