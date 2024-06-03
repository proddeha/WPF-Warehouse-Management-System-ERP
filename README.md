# Warehouse Managment System(ERP)

## Overview

KVA Systems is a robust desktop application designed to streamline various business operations, utilizing the .NET framework and the Model-View-ViewModel (MVVM) architectural pattern. The application offers a user-friendly interface and comprehensive functionalities to manage customers, products, sales, and vendors efficiently and it is based on 'AdventureWorks2022' Databse.
### Technologies Used :

**Technologies Used**:
1. **.NET Framework**: The core framework used for developing the application, providing a robust environment for building and running Windows applications.
2. **C#**: The primary programming language used for the backend logic and functionality.
3. **XAML**: Used for designing the user interface, allowing for a clear separation of UI design and business logic through the MVVM pattern.
4. **MVVM (Model-View-ViewModel)**: The architectural pattern employed to separate the development of the graphical user interface from the development of the business logic or back-end logic, making the code more maintainable and testable.
5. **Visual Studio**: The integrated development environment (IDE) used for developing, debugging, and managing the project.
6. **SQL Server**: Used as the database management system for storing and managing the application’s data, including customer information, product details, sales records, and vendor information.
7. **WPF (Windows Presentation Foundation)**: Used for building the application’s graphical user interface, providing a rich set of controls, graphics, and data-binding capabilities.
8. **LINQ (Language Integrated Query)**: Used for querying the data in a more readable and concise manner within the C# code.
9. **Git**: Version control system used for tracking changes in the source code, facilitating collaboration among multiple developers.


## Features

- **Login Window:**
  -  Secure login interface to authenticate users and ensure safe access.![image](https://github.com/proddeha/WPF-Warehouse-Management-System-ERP/assets/119131830/e00d46d5-86f0-4f86-a52c-7e2387417966)

- **Home Page:**
- ![image](https://github.com/proddeha/WPF-Warehouse-Management-System-ERP/assets/119131830/58aa1b7e-e7cc-4b59-b6fb-609eb20ad4ca) 
- Serves as the main dashboard, providing an overview and easy navigation to other modules of the application.
- **Home Page Features :**
  - **Stats** And **Charts** :  Cartesian Chart about each Month's sales,A total of Earnings and Spendings , Piechart about quantities of each category that we sold , And last a Chart about the Top Selling Products
  - **Open Orders** : Indicates the number of pending Purchased orders that need attention. We can either accept or reject the order as well as complete(update the stock) the orders that are approved. ![image](https://github.com/proddeha/WPF-Warehouse-Management-System-ERP/assets/119131830/c20a1dd2-6cfb-4633-ba3b-d192bd8f06e4)
  - **Data Grid** about the products that are under the **Safety Stock Level**
      - On double click it opens a new window that allows you to order a product thats under the **Safety Stock Level** , you have the option to select the Vendor of that product as well as the Shipping line you want the product to be shipped at you . After that there's the option to immediatly Approve the product or you can place it in the Cart ![image](https://github.com/proddeha/WPF-Warehouse-Management-System-ERP/assets/119131830/5a8d2d2d-623f-4743-9b58-7fcb2a0a12eb)
  - **Cart :**
    - Here are the product orders that we are about to make . We have 2 options, we either Approve the order or we Archive order meaning it will be displayed as 'Pending' ![image](https://github.com/proddeha/WPF-Warehouse-Management-System-ERP/assets/119131830/b013f129-9f38-46f6-8881-43ae7383b964)
- **Customers:**
- ![image](https://github.com/proddeha/WPF-Warehouse-Management-System-ERP/assets/119131830/1d323ef9-ff44-4328-81ed-a963bf5e83ef)
  - Tools for **adding a customer** and **viewing customer information** but also we can make a sale to that customer.
  - We can dynamically search a customer by either his ID or Hist Last Name and Fist Name

  - By clicking the 'Add new Customer' button it brings up this new page here where we can fill each customers informations ![image](https://github.com/proddeha/WPF-Warehouse-Management-System-ERP/assets/119131830/4fe6dd83-25c3-4acb-8753-e6214e98d126)
  - And by either clicking the 'Add new Sale' after you've selected the customer in the DataGrid Or by double-clicking our desired customer , this page appears and we can select the product that our custemer purchased and make a sale by clicking the 'Complete Order' Button ![image](https://github.com/proddeha/WPF-Warehouse-Management-System-ERP/assets/119131830/8f5ee4a3-ba76-4350-a352-7c1b94895fa7)
  - And last if we just double click on the customer we can see all of his information as well as his purchases from us and ofcourse we can also click the 'Add new Sale' to bring up the Sales window as shown above ![image](https://github.com/proddeha/WPF-Warehouse-Management-System-ERP/assets/119131830/24db65fd-a9c9-4d83-a73e-558c5f84637a)
- **Sales Man:**
  - The 'SalesMan' is the page that is responsible to manage and track the performance of the sales team. This interface provides a comprehensive view of each salesperson's details, including their sales quotas, bonuses, and current status. It also includes functionality to add new salespeople and delete existing ones.
  - ![image](https://github.com/proddeha/WPF-Warehouse-Management-System-ERP/assets/119131830/6fe59e3f-e840-4751-b4e7-cb237ba9c51d)
- **Vendors:**
  - The 'Vendors' is the page that is responsible to manage and track vendor information. This interface provides a detailed view of each vendor's details, including their product associations, account numbers, standard prices, and active status. It also includes functionality to add new vendors and delete existing ones.
  - ![image](https://github.com/proddeha/WPF-Warehouse-Management-System-ERP/assets/119131830/fbb0cc50-832c-4bd6-9dfc-436375bfd207)

- **Products:**
  - The 'Products' is the page that is responsible to manage and track the details of all products within the business. This interface provides a comprehensive view of each product, including its ID, name, costs, prices, and stock levels. It also includes functionality to add new products and delete existing ones.![image](https://github.com/proddeha/WPF-Warehouse-Management-System-ERP/assets/119131830/7f086517-3458-4f6e-a5fc-ffdc2bbe0f0d)

- **Invoices:**
  - The 'Invoices' is the page that is responsible to manage and track all invoices related to sales and purchases within the business. This interface provides a comprehensive view of each invoice, including details such as purchase order ID, employee ID, vendor ID, product ID, and the names of the employees and vendors involved. It also includes functionality to filter between sales invoices and purchase invoices ![image](https://github.com/proddeha/WPF-Warehouse-Management-System-ERP/assets/119131830/044990a2-56c5-47bf-81f0-3a38976bc6d1)

