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
    /// Interaction logic for VendorAddition.xaml
    /// </summary>
    public partial class VendorAddition : Window
    {
        public VendorAddition()
        {
            InitializeComponent();
            LoadUnitTypeData();
            LoadProductCategory();
            LoadRegionData();
            LoadContactData();

            LoadCreditData();

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
        private void LoadUnitTypeData()
        {

            SqlCommand command;
            SqlDataReader dataReader;
            using (SqlConnection con = new SqlConnection(GetConnectionString()))
            {
                try
                {
                    con.Open();
                    string query = "SELECT DISTINCT UnitMeasureCode FROM Production.UnitMeasure ";
                    command = new SqlCommand(query, con);

                    dataReader = command.ExecuteReader();
                    while (dataReader.Read())
                    {
                        cmbUnitCode.Items.Add(dataReader.GetValue(0));
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
        private void LoadContactData()
        {

            SqlCommand command;
            SqlDataReader dataReader;
            using (SqlConnection con = new SqlConnection(GetConnectionString()))
            {
                try
                {
                    con.Open();
                    string query = "SELECT DISTINCT(Name) FROM person.businessentitycontact as bec " +
                                   "JOIN person.contacttype as ct on bec.contacttypeid = ct.contacttypeid " +
                                   "WHERE ct.contacttypeid in (2,17,18,19) ";
                    command = new SqlCommand(query, con);

                    dataReader = command.ExecuteReader();
                    while (dataReader.Read())
                    {
                        cmbContact.Items.Add(dataReader.GetValue(0));
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

        private void LoadCreditData()
        {
            SqlCommand command;
            SqlDataReader dataReader;
            string connectionString = GetConnectionString();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    string query = "SELECT DISTINCT(CreditRating) FROM Purchasing.Vendor ";
                    command = new SqlCommand(query, con);

                    dataReader = command.ExecuteReader();
                    while (dataReader.Read())
                    {
                        cmbCredit.Items.Add(dataReader.GetValue(0));
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

        private void LoadProductCategory()
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
                        cmbProdCategory.Items.Add(dataReader.GetValue(0));
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
        private void LoadProduct(string categoryName)
        {
            cmbProd.Items.Clear();

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
                        cmbProd.Items.Add(dataReader.GetValue(0));
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


        private void ExecTrigger()
        {

            using (SqlConnection con = new SqlConnection(GetConnectionString()))
            {
                con.Open();



                SqlCommand cmd = new SqlCommand("NewVendor", con);


                cmd.CommandType = CommandType.StoredProcedure;

                int businessEntityId = 0;

                //businessEntityId = gridDataReturn(businessEntityId);


                cmd.Parameters.Add(new SqlParameter("@account", txtAccount.Text));
                cmd.Parameters.Add(new SqlParameter("@name", txtName.Text));
                cmd.Parameters.Add(new SqlParameter("@UnitCode", cmbUnitCode.SelectedItem));
                cmd.Parameters.Add(new SqlParameter("@creditrating", Convert.ToInt32(cmbCredit.SelectedItem)));
                cmd.Parameters.Add(new SqlParameter("@productname", cmbProd.SelectedItem.ToString()));
                cmd.Parameters.Add(new SqlParameter("@avgtime", Convert.ToInt32(txtAvg.Text.ToString())));
                cmd.Parameters.Add(new SqlParameter("@standardPrice", txtPrice.Text));
                cmd.Parameters.Add(new SqlParameter("@minorder", txtMin.Text));
                cmd.Parameters.Add(new SqlParameter("@maxorder", txtMax.Text));
                cmd.Parameters.Add(new SqlParameter("@AddressLine1", txtAddressLine1.Text));
                cmd.Parameters.Add(new SqlParameter("@AddressLine2", txtAddressLine2.Text));
                cmd.Parameters.Add(new SqlParameter("@TerritoryName", cmbRegionType.SelectedItem.ToString()));
                cmd.Parameters.Add(new SqlParameter("@StateProvince", cmbState.SelectedItem.ToString()));
                cmd.Parameters.Add(new SqlParameter("@City", txtCity.Text));
                cmd.Parameters.Add(new SqlParameter("@PostalCode", Convert.ToInt32(txtPostal.Text.ToString())));
                cmd.Parameters.Add(new SqlParameter("@FirstName", txtFname.Text));
                cmd.Parameters.Add(new SqlParameter("@Middle", txtMiddle.Text));
                cmd.Parameters.Add(new SqlParameter("@LastName", txtLname.Text));
                cmd.Parameters.Add(new SqlParameter("@ContactType", cmbContact.SelectedItem.ToString()));



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
                MessageBox.Show("Vendor Successfully Added!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            /* txtAccount.Text = string.Empty;
             txtName.Text = string.Empty;
             txtCreditRating.Text = string.Empty;
             cmbProd.SelectedIndex = -1;
             txtAvg.Text = string.Empty;
             txtPrice.Text = string.Empty;
             txtMin.Text = string.Empty;
             txtMax.Text = string.Empty;
             cmbUnitCode.SelectedIndex = -1;
            */

        }

        private void cmbProdCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbProdCategory.SelectedItem != null)
            {
                string selectedCategory = cmbProdCategory.SelectedItem.ToString();
                LoadProduct(selectedCategory);
            }
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
        private void cmbRegionType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

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
        }
    }
}
