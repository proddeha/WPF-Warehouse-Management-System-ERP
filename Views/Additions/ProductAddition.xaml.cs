using System.Configuration;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Input;

namespace NvvmFinal.Views.Additions
{
    /// <summary>
    /// Interaction logic for ProductAddition.xaml
    /// </summary>
    public partial class ProductAddition : Window
    {
        public ProductAddition()
        {
            InitializeComponent();
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            Close();
        }


        private void MnmzBtn_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }


        private void BtnAddProduct_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string name = txtName.Text;
                string productNumber = txtProductNumber.Text;
                string color = txtColor.Text;
                decimal standardCost = decimal.Parse(txtStandardCost.Text);
                decimal listPrice = decimal.Parse(txtListPrice.Text);
                bool makeFlag = cmbMakeFlag.SelectedIndex == 0;
                bool finishedGoodsFlag = cmbFinishedGoodsFlag.SelectedIndex == 0;
                int safetyStockLevel = int.Parse(txtSafetyStockLevel.Text);
                int reorderPoint = int.Parse(txtReorderPoint.Text);
                int daysToManufacture = int.Parse(txtDaysToManufacture.Text);
                DateTime sellStartDate = DateTime.Parse(txtSellStartDate.Text);
                DateTime modifiedDate = dpModifiedDate.SelectedDate ?? DateTime.Now;


                short? locationID = null;
                if (!string.IsNullOrEmpty(txtLocationID.Text))
                {
                    if (short.TryParse(txtLocationID.Text, out short parsedLocationID))
                    {
                        locationID = parsedLocationID;
                    }
                    else
                    {
                        MessageBox.Show("Invalid input format for Location ID. Please enter a valid number.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("Location ID cannot be empty.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                short? initialQuantity = null;
                if (!string.IsNullOrEmpty(txtInitialStockQuantity.Text))
                {
                    if (short.TryParse(txtInitialStockQuantity.Text, out short parsedInitialQuantity))
                    {
                        initialQuantity = parsedInitialQuantity;
                    }
                    else
                    {
                        MessageBox.Show("Invalid input format for Initial Stock Quantity. Please enter a valid number.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("Initial Stock Quantity cannot be empty.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                byte? bin = null;
                if (!string.IsNullOrEmpty(txtBin.Text))
                {
                    if (byte.TryParse(txtBin.Text, out byte parsedBin))
                    {
                        bin = parsedBin;
                    }
                    else
                    {
                        MessageBox.Show("Invalid input format for Bin. Please enter a valid number.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("Bin cannot be empty.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }


                string shelf = txtShelf.Text;

                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("AddNewProduct", connection))
                    {
                        try
                        {
                            command.CommandType = System.Data.CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("@Name", name);
                            command.Parameters.AddWithValue("@ProductNumber", productNumber);
                            command.Parameters.AddWithValue("@Color", color);
                            command.Parameters.AddWithValue("@StandardCost", standardCost);
                            command.Parameters.AddWithValue("@ListPrice", listPrice);
                            command.Parameters.AddWithValue("@MakeFlag", makeFlag);
                            command.Parameters.AddWithValue("@FinishedGoodsFlag", finishedGoodsFlag);
                            command.Parameters.AddWithValue("@SafetyStockLevel", safetyStockLevel);
                            command.Parameters.AddWithValue("@ReorderPoint", reorderPoint);
                            command.Parameters.AddWithValue("@DaysToManufacture", daysToManufacture);
                            command.Parameters.AddWithValue("@SellStartDate", sellStartDate);
                            command.Parameters.AddWithValue("@ModifiedDate", modifiedDate);
                            command.Parameters.AddWithValue("@LocationID", locationID);
                            command.Parameters.AddWithValue("@InitialStockQuantity", initialQuantity);
                            command.Parameters.AddWithValue("@Bin", bin);
                            command.Parameters.AddWithValue("@Shelf", shelf);


                            command.ExecuteNonQuery();

                            SqlCommand getProductIDCommand = new SqlCommand("SELECT SCOPE_IDENTITY()", connection);
                            object result = getProductIDCommand.ExecuteScalar();
                            if (result != DBNull.Value)
                            {
                                int productID = Convert.ToInt32(result);


                                SqlCommand inventoryCommand = new SqlCommand("INSERT INTO Production.ProductInventory (ProductID, LocationID, Quantity,Bin,Shelf) VALUES (@ProductID, @LocationID, @Quantity,@Bin,@Shelf)", connection);
                                inventoryCommand.Parameters.AddWithValue("@ProductID", productID);
                                inventoryCommand.Parameters.AddWithValue("@LocationID", locationID);
                                inventoryCommand.Parameters.AddWithValue("@Quantity", initialQuantity);
                                inventoryCommand.Parameters.AddWithValue("@Bin", bin);
                                inventoryCommand.Parameters.AddWithValue("@Shelf", shelf);
                                inventoryCommand.ExecuteNonQuery();
                            }
                        }
                        catch (SqlException ex)
                        {
                            MessageBox.Show("Error :" + ex.Message);
                        }

                    }
                }
                MessageBox.Show("New product added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (FormatException)
            {
                MessageBox.Show("Invalid input format. Please ensure all fields are filled correctly.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
    }
}
