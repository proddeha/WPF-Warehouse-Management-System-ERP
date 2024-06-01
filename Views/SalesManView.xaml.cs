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
using System.Windows.Navigation;
using System.Windows.Shapes;
using NvvmFinal.Views.Additions;
using System.Windows.Forms;
using System.Globalization;

namespace NvvmFinal.Views
{

    public partial class SalesManView : System.Windows.Controls.UserControl
    {
        string? str = null;
        public SalesManView()
        {
            InitializeComponent();

            MyDataGrid.AutoGeneratingColumn += (sender, e) =>
            {
                // Set IsReadOnly to true for all columns except SalesQuota and Bonus
                if (e.PropertyName != "SalesQuota" && e.PropertyName != "Bonus")
                {
                    e.Column.IsReadOnly = true;
                }
            };
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
            using (SqlConnection con = new SqlConnection(GetConnectionString()))
            {
                try
                {
                    con.Open();
                    string query = "SELECT * FROM Pwlites";
                    SqlCommand cmd = new SqlCommand(query, con);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    MyDataGrid.Items.Clear();

                    MyDataGrid.ItemsSource = dt.DefaultView;
                }
                catch (Exception ex)
                {
                    //System.Windows.Forms.MessageBox.Show($"Failed to load data. Error: {ex.Message}");
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }


        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtSearch.Text))
            {
                LoadData();
                return;
            }


            using (SqlConnection con = new SqlConnection(GetConnectionString()))
            {

                if (!string.IsNullOrEmpty(txtSearch.Text))
                {
                    try
                    {
                        con.Open();
                        string query = $"SELECT * from Pwlites " +
                            $" WHERE FirstName LIKE '{txtSearch.Text}%'" +
                            $" ORDER BY FirstName ";
                        SqlCommand cmd = new SqlCommand(query, con);
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        MyDataGrid.ItemsSource = dt.DefaultView;
                        str = txtSearch.Text;

                    }

                    catch (Exception ex)
                    {
                        //System.Windows.Forms.MessageBox.Show($"Failed to load data. Error: {ex.Message}");
                    }

                    if (txtSearch.Text.Contains(" "))
                    {
                        int spaceIndex = txtSearch.Text.IndexOf(" ");
                        string lastName = txtSearch.Text.Substring(0, spaceIndex);
                        string query = $"SELECT * FROM Pwlites " +
                            $" WHERE LastName LIKE '{lastName}%'" +
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
                    string query = $"SELECT * FROM Pwlites" +
                        $" WHERE BusinessEntityID LIKE '{txtSearch.Text}%'" +
                        $" ORDER BY SalesQuota";
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
        private void MyDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                var newValue = ((System.Windows.Controls.TextBox)e.EditingElement).Text;
                var rowIndex = e.Row.GetIndex();
                var columnIndex = e.Column.DisplayIndex;

                int businessEntityID = GetBusinessEntityID(rowIndex);

                UpdateCellValue(businessEntityID, columnIndex, newValue);
            }
        }

        private int GetBusinessEntityID(int rowIndex)
        {
            if (MyDataGrid.Items[rowIndex] is DataRowView row)
            {
                return Convert.ToInt32(row["BusinessEntityID"]);
            }
            return -1;
        }

        private void UpdateCellValue(int businessEntityID, int columnIndex, string newValue)
        {
            string connectionString = GetConnectionString();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("UpdateSalesPerson", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    DataRowView selectedRow = (DataRowView)MyDataGrid.SelectedItem;

                    decimal NewValue = decimal.Parse(newValue, CultureInfo.InvariantCulture);

                    if (MyDataGrid.Columns[columnIndex].Header.ToString() == "SalesQuota")
                    {
                        cmd.Parameters.Add(new SqlParameter("@NewSalesQuota", SqlDbType.Decimal) { Value = NewValue });
                        cmd.Parameters.Add(new SqlParameter("@NewBonus", SqlDbType.Decimal) { Value = selectedRow["Bonus"] });
                        System.Windows.MessageBox.Show("The update of Sales Quota is completed!");
                    }
                    else if (MyDataGrid.Columns[columnIndex].Header.ToString() == "Bonus")
                    {
                        cmd.Parameters.Add(new SqlParameter("@NewSalesQuota", SqlDbType.Decimal) { Value = selectedRow["SalesQuota"] });
                        cmd.Parameters.Add(new SqlParameter("@NewBonus", SqlDbType.Decimal) { Value = NewValue });
                        System.Windows.MessageBox.Show("The update of Bonus is completed!");
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("You cannot update this column!");
                        return; // Exit method if the column is neither SalesQuota nor Bonus
                    }

                    // Add BusinessEntityID parameter to specify the row to be updated
                    cmd.Parameters.Add(new SqlParameter("@BusinessEntityID", SqlDbType.Int) { Value = businessEntityID });

                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("An error occurred: " + ex.Message);
            }
        }








        private void addSalesMan_Click(object sender, RoutedEventArgs e)
        {
            SalesManAddition salesManAddition = new SalesManAddition();
            salesManAddition.Show();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void MyDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }
        public int getCustomerID()
        {
            int customerID = 0;

            if (MyDataGrid.SelectedItem is DataRowView row)
            {
                customerID = Convert.ToInt32(row["BusinessEntityID"]);

            }
            return customerID;
        }

        private void ExecTrigger()
        {
            int customerID = getCustomerID();
            string connectionString = GetConnectionString();
            using (SqlConnection conn = new SqlConnection(connectionString))

            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("DeleteSalesMan", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter("@BusinessEntityID", customerID));

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
                    System.Windows.Forms.MessageBox.Show($"SalesMan {getCustomerID()} Successfully Deleted");

                }
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Select a SalesMan First");
            }
        }
    }
}
