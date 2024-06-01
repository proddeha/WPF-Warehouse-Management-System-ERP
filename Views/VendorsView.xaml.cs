using NvvmFinal.Views.Additions;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;

namespace NvvmFinal.Views
{
    /// <summary>
    /// Interaction logic for VendorsView.xaml
    /// </summary>
    public partial class VendorsView : System.Windows.Controls.UserControl
    {
        string str = null;
        public VendorsView()
        {
            InitializeComponent();
        }
        private void LoadData()
        {
            string connectionString = GetConnectionString();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    string query = "SELECT * FROM S_Vendors";
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


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
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
            if (string.IsNullOrEmpty(txtSearch.Text))
            {
                LoadData();
                return;
            }
            string connectionString = GetConnectionString();
            using (SqlConnection con = new SqlConnection(connectionString))
            {

                if (!string.IsNullOrEmpty(txtSearch.Text))
                {
                    try
                    {
                        con.Open();
                        string query = $"SELECT * from S_Vendors " +
                            $" WHERE Name LIKE '{txtSearch.Text}%'" +
                            $" ORDER BY Name ";
                        SqlCommand cmd = new SqlCommand(query, con);
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        MyDataGrid.ItemsSource = dt.DefaultView;
                        str = txtSearch.Text;

                    }

                    catch (Exception ex)
                    {
                        System.Windows.Forms.MessageBox.Show($"Failed to load data. Error: {ex.Message}");
                    }

                    if (txtSearch.Text.Contains(" "))
                    {
                        int spaceIndex = txtSearch.Text.IndexOf(" ");
                        string lastName = txtSearch.Text.Substring(0, spaceIndex);
                        string query = $"SELECT * FROM S_Vendors " +
                            $" WHERE Name LIKE '{lastName}%'" +
                            $" ORDER BY Name ";
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
                    string query = $"SELECT * FROM S_Vendors" +
                        $" WHERE BusinessEntityID    LIKE '{txtSearch.Text}%'" +
                        $" ORDER BY BusinessEntityID";
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
        private void MyDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }
        private void UserControl_Loaded_2(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void addVendor_Click(object sender, RoutedEventArgs e)
        {
            VendorAddition vendorAddition = new VendorAddition();
            vendorAddition.Show();

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
                SqlCommand cmd = new SqlCommand("DeleteVendor", conn);
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
                    System.Windows.Forms.MessageBox.Show($"Customer {getCustomerID()} Successfully Deleted");

                }
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Select a Customer First");
            }
        }
    }
}
