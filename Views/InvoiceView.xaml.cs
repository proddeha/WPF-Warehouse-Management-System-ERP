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
using NvvmFinal.ViewModels;


namespace NvvmFinal.Views
{
    /// <summary>
    /// Interaction logic for InvoiceView.xaml
    /// </summary>
    public partial class InvoiceView : System.Windows.Controls.UserControl
    {
        string str = null;
        public InvoiceView()
        {
            InitializeComponent();
        }
        private void UserControl_Loaded_2(object sender, RoutedEventArgs e)
        {

        }

        private string GetConnectionString()
        {

            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["AdventureWorks2019"];
            if (settings != null)
                return settings.ConnectionString;
            throw new Exception("Connection string for AdventureWorks2019 not found.");
        }


        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string str = txtSearch.Text;

            if (chkSearchByCustomerID.IsChecked == true)
            {
                chkSearchByEmployeeID.IsChecked = false;
                if (!string.IsNullOrEmpty(str))
                {
                    string connectionString = GetConnectionString();
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        try
                        {
                            con.Open();
                            string query = $"SELECT DISTINCT TOP 1000 soh.CustomerID, pp.LastName, pp.FirstName, soh.SalesOrderNumber,sod.ProductID,TotalDue " +
                                           $"FROM sales.SalesOrderHeader AS soh " +
                                           $"JOIN sales.SalesOrderDetail AS sod ON soh.SalesOrderID = sod.SalesOrderID " +
                                           $"JOIN sales.Customer AS sc ON soh.CustomerID = sc.CustomerID " +
                                           $"JOIN person.Person AS pp ON sc.PersonID = pp.BusinessEntityID " +
                                           $"WHERE soh.SalesOrderNumber LIKE @SearchText " +
                                           $"ORDER BY soh.CustomerID";

                            SqlCommand cmd = new SqlCommand(query, con);
                            cmd.Parameters.AddWithValue("@SearchText", str + "%");
                            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            MyDataGrid.ItemsSource = dt.DefaultView;
                        }
                        catch (Exception ex)
                        {
                            System.Windows.Forms.MessageBox.Show($"Failed to load data. Error: {ex.Message}");
                        }
                        finally
                        {
                            con.Close();
                        }
                    }
                }
            }
            else if (chkSearchByEmployeeID.IsChecked == true)
            {
                chkSearchByCustomerID.IsChecked = false;
                if (!string.IsNullOrEmpty(str))
                {
                    string connectionString = GetConnectionString();
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        try
                        {
                            conn.Open();

                            string query = $"SELECT TOP 7000 poh.PurchaseOrderID, EmployeeID, LastName, FirstName, VendorID, ProductID, OrderQty, SubTotal " +
                                           $"FROM Purchasing.PurchaseOrderHeader AS poh " +
                                           $"JOIN Purchasing.PurchaseOrderDetail AS pod ON poh.PurchaseOrderID = pod.PurchaseOrderID " +
                                           $"JOIN HumanResources.Employee AS e ON poh.EmployeeID = e.BusinessEntityID " +
                                           $"JOIN Person.Person AS pp ON e.BusinessEntityID = pp.BusinessEntityID " +
                                           $"WHERE poh.PurchaseOrderID LIKE @SearchText " +
                                           $"ORDER BY poh.EmployeeID;";

                            SqlCommand cmd = new SqlCommand(query, conn);
                            cmd.Parameters.AddWithValue("@SearchText", str + "%");
                            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            MyDataGrid.ItemsSource = dt.DefaultView;
                        }
                        catch (Exception ex)
                        {
                            System.Windows.Forms.MessageBox.Show($"Failed to load data. Error: {ex.Message}");
                        }
                        finally
                        {
                            conn.Close();
                        }
                    }
                }
                else
                {
                    //System.Windows.MessageBox.Show("Text box is empty");
                }
            }
        }
        public string getSalesOrderNumber()
        {
            string salesOrderNumber = null;

            if (MyDataGrid.SelectedItem is DataRowView rowView)
            {
                salesOrderNumber = rowView["SalesOrderNumber"].ToString();
            }

            return salesOrderNumber;
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



        private InvoicesDetails detailsInvoiceWindow;
        private Purchase_Details detailsPurchaseWindow;

        private void MyDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            
                int productID = getProductID();

                
                if (detailsInvoiceWindow != null && detailsInvoiceWindow.IsLoaded)
                {
                    detailsInvoiceWindow.Close();
                }

                if (detailsPurchaseWindow != null && detailsPurchaseWindow.IsLoaded)
                {
                    detailsPurchaseWindow.Close();
                }

                if (chkSearchByEmployeeID.IsChecked == true)
                {
                    detailsPurchaseWindow = new Purchase_Details(productID);
                    detailsPurchaseWindow.Show();
                }
                else if (chkSearchByCustomerID.IsChecked == true)
                {
                    string salesOrderNumber = getSalesOrderNumber();
                    detailsInvoiceWindow = new InvoicesDetails(salesOrderNumber);
                    detailsInvoiceWindow.Show();
                }           
        }

        private void chkSearchByEmployeeID_Checked(object sender, RoutedEventArgs e)
        {
            chkSearchByCustomerID.IsChecked = false;
        }

        private void chkSearchByCustomerID_Checked(object sender, RoutedEventArgs e)
        {
            chkSearchByEmployeeID.IsChecked= false;
        }
    }
}