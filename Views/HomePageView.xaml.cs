using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using NvvmFinal.ViewModels;
using NvvmFinal.Views.Additions;
//using PdfSharp.Charting;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Data;


namespace NvvmFinal.Views
{
    public partial class HomePageView : System.Windows.Controls.UserControl
    {
        private int selectedProductID;
        public static HomePageView Instance { get; private set; }
        public bool IsSelected { get;set; }
        public HomePageView()
        {
            InitializeComponent();
            Instance = this;
            //cmbYears.Items.Add("ALL");
            dateFrom.Text = "01/01/2014";
            dateUntil.Text = "12/01/2014";
            //LoadCmbYear();
            //cmbYears.SelectedIndex = 2;
            FillPieChart();
            FillCartChart();
            LoadGridData();
            pendingOrdersCount.Text = pendingOrders().ToString();
            MyGridd.ItemsSource = DataRepository.MainTable.DefaultView;
            int selectedProductID = this.selectedProductID;
            calculateTotalCost();
            statusCb.SelectedIndex = 0;
            OpenOrdersLoad();
        }

        private void LoadData()
        {


            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                try
                {
                    connection.Open();
                    string query = $"SELECT[name],[Color],[TotalQuantity],[ReorderPoint]FROM [dbo].[ProductsUnderReorderPoint]";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                    }
                    connection.Close();

                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"Failed to load data. Error: {ex.Message}");
                }
            }
        }
        private string GetConnectionString()
        {

            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["AdventureWorks2019"];
            if (settings != null)
                return settings.ConnectionString;
            throw new Exception("Connection string for AdventureWorks2019 not found.");
        }

        private void FillChart()
        {
            List<double> monthlySales = new List<double>();
            List<string> saleMonths = new List<string>();
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("getSalesPeriod", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@periodFrom", DateTime.Parse(dateFrom.Text)));
                cmd.Parameters.Add(new SqlParameter("@periodUntill", DateTime.Parse(dateUntil.Text)));
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        double sum = Convert.ToDouble(reader.GetDecimal(0));
                        monthlySales.Add(sum);
                        string month = reader.GetString(2);
                        saleMonths.Add(month);
                    }
                }
            }
            MyCartesianChart.Series = new ISeries[]
            {
                new LineSeries<double>
                {
                    Values = monthlySales

                }
            };
            MyCartesianChart.XAxes = new Axis[]
               {
                    new Axis
                    {
                        Labels = saleMonths
                    }
               };
        }

        /*public void LoadCmbYear()
        {

            SqlCommand command;
            SqlDataReader dataReader;
            string connectionString = GetConnectionString();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    string query = "SELECT * FROM allYears ORDER BY SalesYear DESC";
                    command = new SqlCommand(query, con);

                    dataReader = command.ExecuteReader();
                    while (dataReader.Read())
                    {
                        cmbYears.Items.Add(dataReader.GetValue(0));
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
        }*/

        public void FillPieChart()
        {
            SqlCommand command;
            SqlDataReader dataReader;
            string connectionString = GetConnectionString();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                List<int> soldCount = new List<int>();
                List<string> prdctCategory = new List<string>();
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    connection.Open();

                    string query = "SELECT * FROM productsSoldPerCategory";
                    command = new SqlCommand(query, connection);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int sum = reader.GetInt32(0);
                            soldCount.Add(sum);
                            string month = reader.GetString(1);
                            prdctCategory.Add(month);
                        }
                    }

                    myPieChart.Series = new ISeries[]
                    {

                        new PieSeries<int>
                        {
                            Values = new int[] { Convert.ToInt32(soldCount[0]) },
                            Name = prdctCategory[0],
                            Stroke = null,
                            InnerRadius = 20
                        },
                        new PieSeries<int>
                        {
                            Values = new []{Convert.ToInt32(soldCount[1])},
                            Name = prdctCategory[1],
                            Stroke = null,
                            InnerRadius = 20
                        },
                         new PieSeries<int>
                        {
                            Values = new []{Convert.ToInt32(soldCount[2])},
                            Name = prdctCategory[2],
                            Stroke = null,
                            InnerRadius = 20
                        },
                        new PieSeries<int>
                        {
                            Values = new []{Convert.ToInt32(soldCount[3])},
                            Name = prdctCategory[3],
                            Stroke = null,
                            InnerRadius = 20
                        }
                    };
                }
            }
        }
        private void FillCartChart()
        {
            SqlCommand command;
            SqlDataReader dataReader;
            string connectionString = GetConnectionString();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                List<int> soldCount = new List<int>();
                List<string> prdctName = new List<string>();
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    connection.Open();

                    string query = "SELECT TOP (10) [Name],[TotalSold] FROM [dbo].[MostProductsSold] ORDER BY TotalSold DESC";
                    command = new SqlCommand(query, connection);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int sum = reader.GetInt32(1);
                            soldCount.Add(sum);
                            string month = reader.GetString(0);
                            prdctName.Add(month);
                        }
                    }
                    var columnSeries = new ColumnSeries<int>
                    {
                        Values = soldCount

                    };

                    var xAxis = new Axis
                    {
                        Labels = prdctName,
                    };

                    ProductsCartesianChart.Series = new ISeries[] { columnSeries };
                    ProductsCartesianChart.XAxes = new Axis[] { xAxis };

                }
            }
        }
        /*private void cmbYears_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (cmbYears.SelectedIndex != 0)
            {
                int selectedYear = (int)Convert.ToInt64(cmbYears.SelectedItem.ToString());
                
                FillChart(selectedYear);
            }
            else
            {
                SqlCommand command;
                SqlDataReader dataReader;
                string connectionString = GetConnectionString();
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    List<double> monthlySales = new List<double>();
                    List<string> saleMonths = new List<string>();
                    using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                    {
                        connection.Open();

                        string query = "SELECT MonthlySales, SalesMonth FROM AllMonthlySales ORDER BY MonthNumber";
                        command = new SqlCommand(query, connection);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                double sum = Convert.ToDouble(reader.GetDecimal(0));
                                monthlySales.Add(sum);
                                string month = reader.GetString(1);
                                saleMonths.Add(month);
                            }
                        }
                    }
                    MyCartesianChart.Series = new ISeries[]
                    {
                new LineSeries<double>
                {
                    Values = monthlySales

                }
                    };
                    MyCartesianChart.XAxes = new Axis[]
                       {
                    new Axis
                    {
                        Labels = saleMonths
                    }
                       };
                }
            }

        }*/
        private void LoadGridData()
        {
            string connectionString = GetConnectionString();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    string query = "SELECT * FROM ProductsUnderReorderPoint";
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
        public static class DataRepository
        {

            public static DataTable MainTable { get; set; }

            static DataRepository()
            {
                MainTable = new DataTable();
                MainTable.Columns.Add("Vendor", typeof(string));
                MainTable.Columns.Add("ProductID", typeof(string));
                MainTable.Columns.Add("Price", typeof(decimal));
                MainTable.Columns.Add("Quantity", typeof(int));
                MainTable.Columns.Add("ShippingID", typeof(int));
                MainTable.Columns.Add("Tax", typeof(decimal));
                MainTable.Columns.Add("Freight", typeof(decimal));
                MainTable.Columns.Add("VendorID", typeof(int));
                

            }
            

        }
        private void NewOrder_Click(object sender, RoutedEventArgs e)
        {


        }
        public void calculateTotalCost()
        {
            Dispatcher.Invoke(() =>
            {
                decimal sumPrice = 0;
                int sumQuantity = 0;
                decimal price = 0;
                int quantity = 0;
                int sum = 0;

                foreach (var item in MyGridd.Items)
                {
                    if (item is DataRowView dataRowView && !dataRowView.IsNew)
                    {
                        if (dataRowView["Price"] != DBNull.Value && dataRowView["Quantity"] != DBNull.Value)
                        {
                            decimal.TryParse(dataRowView["Price"].ToString(), out price);
                            int.TryParse(dataRowView["Quantity"].ToString(), out quantity);

                            sumPrice += price;
                            sumQuantity += quantity;
                            sum += 1;
                        }
                    }
                    totalItems.Text = sum.ToString();
                }
                totalCostBlk.Text = sumPrice.ToString();
                totalOrderBlk.Text = sumQuantity.ToString();
            });
        }

        private void MyGridd_CurrentCellChanged(object sender, EventArgs e)
        {
            calculateTotalCost();
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
                    System.Windows.MessageBox.Show("Σφάλμα κατά την εισαγωγή της παραγγελίας: " + ex.Message);
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
        private Dictionary<int, int> GetSelectedProductsAndQuantities(string vendorName)
        {
            Dictionary<int, int> productsAndQuantities = new Dictionary<int, int>();
            try
            {

                foreach (var item in MyGridd.Items)
                {

                    if (item is DataRowView rowView)
                    {
                        string readVendor = rowView["Vendor"].ToString();
                        if(vendorName==readVendor)
                        {
                        int productID = Convert.ToInt32(rowView["ProductID"]);
                        int quantity = Convert.ToInt32(rowView["Quantity"]);
                        productsAndQuantities.Add(productID, quantity);
                        }
                    }

                }

            }

            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Same Product Exists 2 Times In The Cart\n\nTry Changing The Quantity");

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
        private void insertHeader(string vendor)
        {
            try
            {
                decimal taxSum = 0;
                int vendorID = 0;
                decimal freightSum = 0;
                int shippingID = 0;

                int productID = selectedProductID;
                if (productID != -1)
                {
                    foreach (var item in MyGridd.Items)
                    {
                        if (item is DataRowView dataRowView && !dataRowView.IsNew)
                        {
                            string readVendor = dataRowView["Vendor"].ToString();
                            if (readVendor==vendor)
                            {
                                int.TryParse(dataRowView["ShippingID"].ToString(), out shippingID);
                                int.TryParse(dataRowView["VendorID"].ToString(), out vendorID);
                                decimal.TryParse(dataRowView["Freight"].ToString(), out decimal freight);
                                decimal.TryParse(dataRowView["Tax"].ToString(), out decimal tax);

                                taxSum += tax;
                                freightSum += freight;
                            }
                        }
                    }

                    PurchaseOrderHeader(vendorID, shippingID, freightSum, taxSum);
                    System.Windows.MessageBox.Show("Η παραγγελία σου πραγματοποίηθηκε ");
                }
                else
                {
                    System.Windows.MessageBox.Show("Το προϊόν δεν βρέθηκε.");
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Προέκυψε σφάλμα: " + ex.Message);
            }
        }
        private void orderBtn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = System.Windows.MessageBox.Show("Are You Sure?", "Complete Order", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                CheckForVendors(true);
            }           
            
        }
        private void archiveOrderBtn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = System.Windows.MessageBox.Show("Are You Sure?", "Archive Order", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                CheckForVendors(false);
            }
        }
        private string selectMaxOrderID()
        {
            string orderID = null;
            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    connection.Open();

                    string query = "SELECT MAX(PurchaseOrderID) FROM Purchasing.PurchaseOrderHeader";

                    SqlCommand command = new SqlCommand(query, connection);
                    command = new SqlCommand(query, connection);
                    using (SqlDataReader reader = command.ExecuteReader())
                    while (reader.Read())
                    {
                       orderID  = reader.GetInt32(0).ToString();
                    }
                    

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Απέτυχε η ανάκτηση του ProductID. Σφάλμα: {ex.Message}");
            }

            return orderID;
        }
        public void CheckForVendors(bool can_execute)
        {
            try
            {

                Dictionary<string, int> vendorCounts = new Dictionary<string, int>();


                foreach (var item in MyGridd.Items)
                {

                    DataRowView row = item as DataRowView;
                    if (row == null)
                    {

                        Debug.WriteLine("Item is not a DataRowView");
                        continue;
                    }

                    string vendorName = row["Vendor"].ToString();
                    if (vendorCounts.ContainsKey(vendorName))
                    {
                        vendorCounts[vendorName]++;
                    }
                    else
                    {
                        vendorCounts[vendorName] = 1;
                    }
                }
                foreach (var vendor in vendorCounts)
                {
                    if (vendor.Value > 1)
                    {
                            
                        insertHeader(vendor.Key);
                        PurchaseOrderDetail(GetSelectedProductsAndQuantities(vendor.Key));
                        changeOrderStatus(can_execute,2,selectMaxOrderID());

                    }
                    else
                    {

                        insertHeader(vendor.Key);
                        PurchaseOrderDetail(GetSelectedProductsAndQuantities(vendor.Key));
                        changeOrderStatus(can_execute,2,selectMaxOrderID());
                    }

                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"An error occurred: {ex.Message}");
                Debug.WriteLine($"Exception: {ex}");
            }
        }
        public void changeOrderStatus(bool execute,int orderStatus,string orderID)
        {
            if (execute)
            { 
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                try
                {
                    connection.Open();

                    SqlCommand cmd = new SqlCommand("ChangingStatus", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@purchaseID", orderID);
                    cmd.Parameters.AddWithValue("@statusid", orderStatus);

                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("Σφάλμα κατά την εισαγωγή της παραγγελίας: " + ex.Message);
                }
            }
            }
        }
        private void OpenOrdersLoad()
        {
            string orderStatus = null;
            if (statusCb.SelectedItem is ComboBoxItem selectedItem)
            {
                orderStatus = selectedItem.Content.ToString();
                
            }
            using (SqlConnection con = new SqlConnection(GetConnectionString()))
            {
                try
                {
                    con.Open();
                    string query = $"SELECT * FROM OpenOrders WHERE OrderStatus LIKE '{orderStatus}' ORDER BY OrderDate DESC";
                    SqlCommand cmd = new SqlCommand(query, con);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                   if (!dt.Columns.Contains("Check"))
                   {
                        dt.Columns.Add(new DataColumn("Check", typeof(bool)));
                   }
                    adapter.Fill(dt);
                    orderGrid.ItemsSource = dt.DefaultView;

                    
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"Failed to load data. Error: {ex.Message}");
                }
            }


        }

        private void viewOrders_Checked(object sender, RoutedEventArgs e)
        {
            orderWidth.Width = new GridLength(1300);
            targetWidth.Width = new GridLength(0);
            rowWidth.Height = new GridLength(320);
            shippingDefaultPanel.Visibility = Visibility.Collapsed;
            viewOrders.Content = "Click For Less Info";
            orderDetailsPanel.Visibility = Visibility.Visible;
        }

        private void viewOrders_Unchecked(object sender, RoutedEventArgs e)
        {
            orderWidth.Width = new GridLength(320);
            targetWidth.Width = new GridLength(320);
            rowWidth.Height = new GridLength(150);
            shippingDefaultPanel.Visibility = Visibility.Visible;
            viewOrders.Content = "Click For More Info";
            orderDetailsPanel.Visibility = Visibility.Collapsed;
        }
        private void buyClick_Checked(object sender, RoutedEventArgs e)
        {
            targetWidth.Width = new GridLength(1300);
            orderWidth.Width = new GridLength(0);
            rowWidth.Height = new GridLength(320);
            MyGridd.Visibility = Visibility.Visible;
            defaultPanel.Visibility = Visibility.Collapsed;
            stackPnl.Visibility = Visibility.Visible;
            buyClick.Content = "Click For Less Info";
            buyClick.Margin = new Thickness(-150, 0, 0, 0);
        }

        private void buyClick_Unchecked(object sender, RoutedEventArgs e)
        {
            targetWidth.Width = new GridLength(320);
            orderWidth.Width = new GridLength(320);
            rowWidth.Height = new GridLength(150);
            MyGridd.Visibility = Visibility.Hidden;
            defaultPanel.Visibility = Visibility.Visible;
            stackPnl.Visibility = Visibility.Hidden;
            buyClick.Content = "Click For More Info";
            buyClick.Margin = new Thickness(0, 0, 0, 5);

        }
        private void ForEachOrderGrid(int orderStatus)
        {
            try
            {
                foreach (var item in orderGrid.Items)
                {
                    if (item is DataRowView row)
                    {
                        if (row["Check"] is bool isChecked && isChecked)
                        {
                            string purchaseOrderID = row["PurchaseOrderID"].ToString();
                            changeOrderStatus(true, orderStatus, purchaseOrderID);
                            //System.Windows.MessageBox.Show($"Row with PurchaseOrderID {purchaseOrderID} is checked.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }

        }

        private void orderGrid_CurrentCellChanged(object sender, EventArgs e)
        {

        }

        private void statusCb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            purchaseID.Text = "";
            OpenOrdersLoad();
            string orderStatus = null;
            if (statusCb.SelectedItem is ComboBoxItem selectedItem)
            {
                orderStatus = selectedItem.Content.ToString();
            }
            if (orderStatus =="Approved")
            {
                completeOrderBtn.Visibility = Visibility.Visible;
                acceptRejectPanel.Visibility = Visibility.Collapsed; 
            }
            else
            {
                completeOrderBtn.Visibility = Visibility.Collapsed;
                acceptRejectPanel.Visibility = Visibility.Visible; 
            }
        }
        private int pendingOrders()
        {
            int pendingOrders = 0;
            using (SqlConnection con = new SqlConnection(GetConnectionString()))
            {
                try
                {
                    con.Open();
                    string query = "select count(purchaseOrderID)as countID from OpenOrders where orderstatus like 'pending'";
                    SqlCommand cmd = new SqlCommand(query, con);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            pendingOrders = Convert.ToInt32(reader.GetInt32(0));
                        }
                    }

                }
                catch(SqlException ex)
                {
                    System.Windows.MessageBox.Show(ex.ToString());
                }
            }
            return pendingOrders;
        }
        private void fillEarnings()
        {
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("earningPeriods", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@periodFrom", DateTime.Parse(dateFrom.Text)));
                cmd.Parameters.Add(new SqlParameter("@periodUntill", DateTime.Parse(dateUntil.Text)));
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        totalEarned.Text = Convert.ToDouble(reader.GetDecimal(0)).ToString("C");

                    }
                }
            }
            fillSpends();
        }
        private void fillSpends()
        {
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("spendPeriods", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@periodFrom", DateTime.Parse(dateFrom.Text)));
                cmd.Parameters.Add(new SqlParameter("@periodUntill", DateTime.Parse(dateUntil.Text)));
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        totalSpent.Text = Convert.ToDouble(reader.GetDecimal(0)).ToString("C");

                    }
                }
            }
        }
        private void rejectOrderBtn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = System.Windows.MessageBox.Show("Are You Sure?", "Reject Order", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                ForEachOrderGrid(3);
                OpenOrdersLoad();
            }
        }

        private void completeOrderBtn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = System.Windows.MessageBox.Show("Are You Sure?", "Complete Order", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                ForEachOrderGrid(4);
                OpenOrdersLoad();
            }
        }
        private void approveOrderBtn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = System.Windows.MessageBox.Show("Are You Sure?", "Approve Order", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                ForEachOrderGrid(2);
                OpenOrdersLoad();
            }
        }
        private void purchaseID_TextChanged(object sender, TextChangedEventArgs e)
        {
            
            using (SqlConnection con = new SqlConnection(GetConnectionString()))
            {
                string orderStatus = null;

                if (statusCb.SelectedItem is ComboBoxItem selectedItem)
                {
                    orderStatus = selectedItem.Content.ToString();
                }
                try
                {
                    con.Open();
                    string query = $"SELECT * FROM OpenOrders WHERE OrderStatus LIKE '{orderStatus}' AND PurchaseOrderID LIKE '{purchaseID.Text}%' ORDER BY OrderDate DESC";
                    SqlCommand cmd = new SqlCommand(query, con);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    
                    if (!dt.Columns.Contains("Check"))
                    {
                        dt.Columns.Add(new DataColumn("Check", typeof(bool)));
                    }
                    adapter.Fill(dt);
                    orderGrid.ItemsSource = dt.DefaultView;


                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show($"Failed to load data. Error: {ex.Message}");
                }

            }
        }

        private void MyDataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            selectedProductID = 0;
            int quantity = 0;
            if (MyDataGrid.SelectedItem is DataRowView row)
            {
                selectedProductID = Convert.ToInt32(row["ProductID"]);
                quantity = Convert.ToInt32(row["SafetyStockLevel"]) - Convert.ToInt32(row["TotalQuantity"]);
            }
            NewOrder newOrder = new NewOrder(selectedProductID, quantity);
            newOrder.Show();
        }

        private void dateUntil_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void chartsSearch_Click(object sender, RoutedEventArgs e)
        {
            if (dateFrom.SelectedDate.HasValue && dateUntil.SelectedDate.HasValue)
            {
                // Get the selected dates
                DateTime dateFromValue = dateFrom.SelectedDate.Value;
                DateTime dateUntilValue = dateUntil.SelectedDate.Value;

                // Compare the dates
                if (dateFromValue < dateUntilValue)
                {
                    FillChart();
                    fillEarnings();
                }
            }
        }
    }
}
