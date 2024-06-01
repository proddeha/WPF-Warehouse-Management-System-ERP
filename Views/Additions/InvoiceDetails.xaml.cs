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
using System.Drawing;

namespace NvvmFinal.Views.Additions
{
    public partial class InvoicesDetails : Window
    {
        private string salesOrderNumber;

        public InvoicesDetails(string salesOrderNumber)
        {
            InitializeComponent();
            this.salesOrderNumber = salesOrderNumber;

            LoadData(salesOrderNumber);
            FillDetails(salesOrderNumber);
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

        public void LoadData(string salesOrderNumber)
        {
            string connectionString = GetConnectionString();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    string query = "SELECT Name, OrderQty, UnitPrice, LineTotal " +
                                   "FROM sales.salesorderdetail AS sd " +
                                   "JOIN sales.salesorderheader AS sh ON sd.SalesOrderID = sh.SalesOrderID " +
                                   "JOIN production.Product AS pp ON sd.ProductID = pp.ProductID " +
                                   "WHERE SalesOrderNumber = @SalesOrderNumber;";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@SalesOrderNumber", salesOrderNumber);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    ViewGrid.ItemsSource = dt.DefaultView;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to load data. Error: {ex.Message}");
                }
            }
        }

        public void FillDetails(string salesOrderNumber)
        {
            string connectionString = GetConnectionString();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    string query = $"select * from Invoice_Details as cd WHERE SalesOrderNumber = '{salesOrderNumber}';";
                    SqlCommand cmd = new SqlCommand(query, con);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        InvoiceBlock.Text = "Sales";
                        fullNameBlock.Text = reader["FirstName"].ToString() + " " + reader["LastName"].ToString();
                        addressLine1Blk.Text = reader["AddressLine1"].ToString();
                        postalBlk.Text = reader["PostalCode"].ToString();
                        InOBlk.Text = reader["DueDate"].ToString();
                        CityBlk.Text = reader["City"].ToString();
                        orderDBlk.Text = reader["OrderDate"].ToString();
                        InvNoBlk.Text = reader["SalesOrderNumber"].ToString();
                        CostBlock.Text = reader["SubTotal"].ToString();
                        taxBlk.Text = reader["TaxAmt"].ToString();
                        shipBlk.Text = reader["Freight"].ToString();
                        totalBlk.Text = reader["TotalDue"].ToString();
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
            pdf.Info.Title = "Invoice of Sales";

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
                if (itemCounter < 24)
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

                    int remainingItems = itemCounter - 24;
                    int rowIndex = remainingItems / 4;
                    int columnIndex = remainingItems % 4;

                    offsetY = (int)(pdfPage.Height.Point / 2) + (rowIndex * (lineHeight + lineSpacing)) * 2;

                    offsetX = 30 + (int)(columnIndex * 150);

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

            string pdfFilename = @"C:\Users\Kwstas\Downloads\Invoice.pdf";
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
