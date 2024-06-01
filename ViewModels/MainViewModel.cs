using FontAwesome.Sharp;
using System.Windows.Input;

namespace NvvmFinal.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private ViewModelBase _childViewModel;
        private string _caption;
        private IconChar _icon;
        private decimal _totalCost;

        public ViewModelBase ChildViewModel { get => _childViewModel; set { _childViewModel = value; OnPropertyChanged(nameof(ChildViewModel)); } }
        public string Caption { get => _caption; set { _caption = value; OnPropertyChanged(nameof(Caption)); } }
        public IconChar Icon { get => _icon; set { _icon = value; OnPropertyChanged(nameof(Icon)); } }
        public decimal TotalCostt { get => _totalCost; set { _totalCost = value; OnPropertyChanged(nameof(TotalCostt)); } }

        public ICommand ShowHomeViewCmd { get; }
        public ICommand ShowCustomerViewCmd { get; }
        public ICommand ShowVendorsViewCmd { get; }
        public ICommand ShowProductsViewCmd { get; }
        public ICommand ShowSalesManViewCmd { get; }

        public ICommand ShowInvoicesViewCmd { get; }


        public MainViewModel()
        {
            ShowHomeViewCmd = new ViewModelCommand(ExecShowHomeViewCmd);
            ShowCustomerViewCmd = new ViewModelCommand(ExecCustomerViewCmd);
            ShowVendorsViewCmd = new ViewModelCommand(ExecVendorsViewCmd);
            ShowProductsViewCmd = new ViewModelCommand(ExecProductsViewCmd);
            ShowSalesManViewCmd = new ViewModelCommand(ExecSalesManViewCmd);
            ShowInvoicesViewCmd = new ViewModelCommand(ExecInvoicesViewCmd);

            ExecShowHomeViewCmd(null);
        }

        private void ExecSalesManViewCmd(object obj)
        {
            ChildViewModel = new SalesManVM();
            Caption = "Sales";
            Icon = IconChar.SackDollar;
        }

        private void ExecProductsViewCmd(object obj)
        {
            ChildViewModel = new ProductsVM();
            Caption = "Products";
            Icon = IconChar.Boxes;
        }

        private void ExecVendorsViewCmd(object obj)
        {
            ChildViewModel = new VendorsVM();
            Caption = "Vendors";
            Icon = IconChar.Handshake;
        }

        private void ExecShowHomeViewCmd(object obj)
        {
            ChildViewModel = new HomePageVM();
            Caption = "Dashboard";
            Icon = IconChar.Home;
        }
        private void ExecCustomerViewCmd(object obj)
        {
            ChildViewModel = new CustomerVM();
            Caption = "Customers";
            Icon = IconChar.UserGroup;
        }
        private void ExecInvoicesViewCmd(object obj)
        {
            ChildViewModel = new InvoiceVM();
            Caption = "Invoices";
            Icon = IconChar.UserGroup;
        }

    }


}
