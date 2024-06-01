using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Fonts;
using PdfSharp.Pdf.IO;
using System.Drawing.Text;
using System.IO;

namespace NvvmFinal.Views.Additions
{
    /// <summary>
    /// Interaction logic for Purchase_Details.xaml
    /// </summary>
    public partial class Purchase_Details : Window
    {
        private int productID;
        public Purchase_Details(int productID)
        {
            InitializeComponent();
            this.productID = productID;
            FillDetails(productID);
            LoadData(productID);
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
                    string query = "select Name,ProductNumber,OrderQty,UnitPrice,LineTotal from purchasing.purchaseorderdetail as pod " +
                        "join production.product as pp on pod.productid = pp.productid " +
                        "WHERE pp.ProductID = @ProductID;";


                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@ProductID", productID);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    ViewGrid.ItemsSource = dt.DefaultView;
                    con.Close();


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
                    string query = $"SELECT FirstName, LastName, OrderDate, ShipDate, sm.Name AS Ship_Name, pv.Name AS Vendor_Name, SubTotal, TaxAmt, Freight, TotalDue " +
                                   $"FROM purchasing.PurchaseOrderHeader AS poh " +
                                   $"JOIN purchasing.PurchaseOrderDetail AS pod ON poh.PurchaseOrderID = pod.PurchaseOrderID " +
                                   $"JOIN production.product AS p ON pod.ProductID = p.ProductID " +
                                   $"JOIN person.person AS pp ON poh.EmployeeID = pp.BusinessEntityID " +
                                   $"JOIN Purchasing.vendor AS pv ON poh.VendorID = pv.BusinessEntityID " +
                                   $"JOIN Purchasing.ShipMethod AS sm ON poh.ShipMethodID = sm.ShipMethodID WHERE p.ProductID = '{productID}'";


                    SqlCommand cmd = new SqlCommand(query, con);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        InvoiceBlock.Text = "Purchases";
                        fullNameBlock.Text = reader["FirstName"].ToString() + " " + reader["LastName"].ToString();
                        vendorBlk.Text = reader["Vendor_Name"].ToString();
                        scnBlk.Text = reader["Ship_Name"].ToString();
                        shipdBlk.Text = reader["ShipDate"].ToString();
                        orderDBlk.Text = reader["OrderDate"].ToString();
                        CostBlock.Text = reader["SubTotal"].ToString();
                        taxBlk.Text = reader["TaxAmt"].ToString();
                        shipBlk.Text = reader["Freight"].ToString();
                        totalBlk.Text = reader["TotalDue"].ToString();
                        string fullname = reader["FirstName"].ToString() + " " + reader["LastName"].ToString();
                    }
                    reader.Close();

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to load data. Error: {ex.Message}");
                }
            }

        }
        private IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }
        public class SystemFontResolver : IFontResolver
        {
            public byte[] GetFont(string faceName)
            {

                if (faceName.Equals("Arial", StringComparison.OrdinalIgnoreCase))
                {

                    string fontFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "arial.ttf");


                    if (File.Exists(fontFilePath))
                    {

                        return File.ReadAllBytes(fontFilePath);
                    }
                }

                return new byte[0];
            }

            public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
            {

                if (familyName.Equals("Arial", StringComparison.OrdinalIgnoreCase))
                {

                    return new FontResolverInfo(familyName);
                }


                return null;
            }
        }

        private void print_Click(object sender, RoutedEventArgs e)
        {
            GlobalFontSettings.FontResolver = new SystemFontResolver();
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            PdfDocument pdf = new PdfDocument();
            pdf.Info.Title = "Invoice of Purchases";

            PdfPage pdfPage = pdf.AddPage();
            pdfPage.Width = XUnit.FromInch(8);
            XGraphics graph = XGraphics.FromPdfPage(pdfPage);
            XFont font = new XFont("Arial", 12);

            double pageWidth = pdfPage.Width.Point;
            double availableWidth = pageWidth;

            int startY = 30;
            int offsetX = 30;
            int offsetY = startY;
            int lineHeight = (int)font.GetHeight();
            int lineSpacing = 10;
            int itemCounter = 0;
            int gridItemCounter = 0;

            foreach (TextBlock textBlock in FindVisualChildren<TextBlock>(this))
            {
                if (itemCounter < 20)
                {
                    if (itemCounter % 2 == 0 && itemCounter != 0)
                    {
                        offsetX = 30;
                        offsetY += lineHeight + lineSpacing;
                        lineHeight = (int)font.GetHeight();
                    }
                }
                else
                {

                    if (textBlock == FindVisualChildren<TextBlock>(this).Last())
                    {
                        break;
                    }

                    int remainingItems = itemCounter - 20;
                    int rowIndex = remainingItems / 5;
                    int columnIndex = remainingItems % 5;

                    offsetY = (int)(pdfPage.Height.Point / 2) + (rowIndex * (lineHeight + lineSpacing)) * 2;

                    offsetX = 30 + (int)(columnIndex * ((pageWidth - 10) / 5));


                    lineHeight = (int)font.GetHeight();
                }

                double textBlockWidth = graph.MeasureString(textBlock.Text, font).Width;

                if (offsetX + textBlockWidth > pageWidth)
                {
                    offsetX = 30;
                    offsetY += lineHeight + lineSpacing;
                    lineHeight = (int)font.GetHeight();
                }

                graph.DrawString(textBlock.Text, font, XBrushes.Black,
                    new XRect(offsetX, offsetY, textBlockWidth, pdfPage.Height.Point),
                    XStringFormats.TopLeft);
                offsetX += (int)textBlockWidth;

                offsetX += 5;

                itemCounter++;
            }

            string pdfFilename = @"C:\Users\User\Desktop\Purchase_Invoice.pdf";
            pdf.Save(pdfFilename);

            MessageBox.Show("Invoice printed successfully.");
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