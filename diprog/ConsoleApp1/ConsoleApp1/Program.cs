using System.Security.Cryptography.X509Certificates;
using System;
using System.IO;
using System.Linq;

// Define a Product class to represent products in inventory
public class Product
{
    // Properties of a product
    public string Name { get; set; } // Name of the product
    public double Price { get; set; } // Price of the product
    public int Quantity { get; set; } // Quantity of the product
    public int ID { get; set; } // Unique ID of the product

    // Constructor to initialize product properties
    public Product(string name, double price, int quantity, int id)
    {
        Name = name;
        Price = price;
        Quantity = quantity;
        ID = id;
    }

    // Function to display product information
    public virtual void DisplayProductInfo()
    {
        Console.WriteLine($"Product Name: {Name}");
        Console.WriteLine($"Price: P{Price}");
        Console.WriteLine($"Quantity: {Quantity}");
        Console.WriteLine($"ID: {ID}");
    }
}

// Define a finalProduct class that inherits from the Product class to handle products with supplementary attributes.
public class finalProduct : Product
{
    // Additional properties for the finalProduct
    public string Stock { get; set; } // Stock status of the product(Onhand, Warehouse)
    public string Perishable { get; set; } // Indicates whether the product is perishable or not

    // Constructor to initialize additional properties along with base properties
    public finalProduct(string name, double price, int quantity, int id, string stock, string perishable) : base(name, price, quantity, id)
    {
        Stock = stock;
        Perishable = perishable;
    }

    // Override method to display product information including additional properties
    public override void DisplayProductInfo()
    {
        base.DisplayProductInfo();
        Console.WriteLine("Stock: " + Stock);
        Console.WriteLine("Category: " + Perishable);
    }
}

// Define a class to handle the inventory and file operations
class Sys
{
    // Directory leading to the base file location
    private const string filePath = "C:\\Users\\Public\\Documents\\Convenience_Store_Inventory_Management_System.csv";

    // Method to check if CSV file exists, if the CSV file does not exist it creates the file
    public static void csvChecker(string i)
    {
        if (!File.Exists(i))
        {
            try
            {
                using(File.Create(i)) { }
            }
            catch
            {
                Console.WriteLine($"Error creating CSV file");
            }
        }
    }

    // Method to add an item by a new finalProduct to inventory 
    public static void addItem(finalProduct item)
    {
        try
        {

            using (StreamWriter sw = File.AppendText(filePath))
            {
                finalProduct i = new finalProduct(item.Name, item.Price, item.Quantity, item.ID, item.Stock, item.Perishable);
                sw.WriteLine($"{i.Name},{i.Quantity},{i.Price},{i.ID},{i.Stock},{i.Perishable}");
            }
        }
        catch
        {
            Console.WriteLine("Invalid Input");
        }
    }

    // Method to modify details of an item in inventory based on its ID
    public static void editItem(int id)
    {
        try
        {
            string[] lines = File.ReadAllLines(filePath);

            bool idExists = false;
            foreach (string line in lines)
            {
                string[] values = line.Split(',');
                if (values.Length >= 4 && int.Parse(values[3]) == id)
                {
                    idExists = true;
                    break;
                }
            }

            if (!idExists)
            {
                Console.WriteLine("Invalid ID. This item does not exist in the inventory.");
                return;
            }

            while (true)
            {
                Console.Write("Choose what to edit:\n[1] Name\n[2] Quantity\n[3] Price\n[4] Stock\n[5] Category\nEnter choice:");
                int choice = Convert.ToInt32(Console.ReadLine());

                for (int i = 0; i < lines.Length; i++)
                {
                    string[] values = lines[i].Split(',');
                    if (values.Length >= 4 && int.Parse(values[3]) == id)
                    {
                        switch (choice)
                        {
                            case 1:
                                Console.Write("Enter new name:");
                                string n = Console.ReadLine();
                                n = n.ToUpper();
                                lines[i] = $"{n},{values[1]},{values[2]},{id},{values[4]},{values[5]}";
                                break;
                            case 2:
                                Console.Write("Enter new quantity:");
                                int q = Convert.ToInt32(Console.ReadLine());
                                lines[i] = $"{values[0]},{q},{values[2]},{id},{values[4]},{values[5]}";
                                break;
                            case 3:
                                Console.Write("Enter new price:");
                                double p = Convert.ToDouble(Console.ReadLine());
                                lines[i] = $"{values[0]},{values[1]},{p},{id},{values[4]},{values[5]}";
                                break;
                            case 4:
                                Console.Write("Stock([1] Onhand[2] Warehouse):");
                                int s = Convert.ToInt32(Console.ReadLine());
                                string its = "";
                                if (s == 1)
                                {
                                    its = "Onhand";
                                }
                                else if (s == 2)
                                {
                                    its = "Warehouse";
                                }
                                lines[i] = $"{values[0]},{values[1]},{values[2]},{id},{its},{values[5]}";
                                break;
                            case 5:
                                Console.Write("Is it perishable? (true/false):");
                                bool cate = Convert.ToBoolean(Console.ReadLine());
                                string cp = "";
                                if (cate)
                                {
                                    cp = "Perishable";
                                }
                                else if (!cate)
                                {
                                    cp = "Non-perishable";
                                }
                                else
                                {
                                    Console.WriteLine("Invalid Input");
                                    break;
                                }
                                lines[i] = $"{values[0]},{values[1]},{values[2]},{id},{values[4]},{cp}";
                                break;
                            default:
                                Console.WriteLine("Invalid input");
                                return;
                        }
                    }
                }

                File.WriteAllLines(filePath, lines);

                Console.Write("Edit another value of the item?\n[1] Yes\n[0] No\nEnter choice:");
                int c = Convert.ToInt32(Console.ReadLine());
                if (c == 0)
                {
                    break;
                }
            }
        }
        catch
        {
            Console.WriteLine("Invalid Input");
        }
    }

    // Method to remove an item from inventory
    public static void removeItem(int id)
    {
        try
        {
            string[] inv = File.ReadAllLines(filePath);
            bool itemFound = false;

            for (int i = 0; i < inv.Length; i++)
            {
                string[] values = inv[i].Split(',');
                if (values.Length >= 4 && int.Parse(values[3]) == id)
                {
                    itemFound = true;
                    break;
                }
            }

            if (!itemFound)
            {
                Console.WriteLine("Item not found");
                return;
            }

            var uLines = inv.Where(line => !line.Split(',')[3].Equals(id.ToString())).ToArray();
            File.WriteAllLines(filePath, uLines);
            Console.WriteLine("Item Removed");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    // Method to display the inventory
    public static void showInv()
    {
        try
        {
            string[] inv = File.ReadAllLines(filePath);
            if (inv.Length == 0)
            {
                Console.WriteLine("Inventory is empty");
                return;
            }

            int totalStock = 0;
            int totalSpoilage = 0;
            int totalRestock = 0;

            foreach (string line in inv)
            {
                string[] values = line.Split(',');

                if (values.Length >= 6)
                {
                    string name = values[0];
                    int quantity = int.Parse(values[1]);
                    double price = double.Parse(values[2]);
                    int id = int.Parse(values[3]);
                    string stock = values[4];
                    string perishable = values[5];

                    finalProduct p = new finalProduct(name, price, quantity, id, stock, perishable);
                    p.DisplayProductInfo();

                    Console.WriteLine();

                    // Update totals based on stock type
                    switch (stock)
                    {
                        case "Onhand":
                            totalStock += quantity;
                            break;
                        case "Spoilage":
                            totalSpoilage += quantity;
                            break;
                        case "Warehouse":
                            totalRestock += quantity;
                            break;
                        default:
                            break;
                    }
                }
            }

            // Display total stock, spoilage, and restock numbers
            Console.WriteLine($"Total Stock: {totalStock}");
            Console.WriteLine($"Total Spoilage: {totalSpoilage}");
            Console.WriteLine($"Total Restock: {totalRestock}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error displaying inventory: {ex.Message}");
        }
    }

    // Method to check for duplicate item ID
    public static bool dupCheck(int id)
    {
        try
        {
            foreach (string line in File.ReadAllLines(filePath))
            {
                if (!string.IsNullOrEmpty(line))
                {
                    string[] values = line.Split(',');
                    if (values.Length >= 4 && int.TryParse(values[3], out int itemId) && itemId == id)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error checking for duplicate item: {ex.Message}");
            return false;
        }
    }
}

// Abstract class representing a menu
abstract class Menu
{
    public abstract void DisplayMenu();
}

// Class representing the store menu
class StoreMenu : Menu
{
    // Method to display the store menu
    public override void DisplayMenu()
    {   
        //Action Prompts
        Console.WriteLine("\n\tConvenience Store Inventory Management System");
        Console.WriteLine("======================================================");
        Console.WriteLine("[1] Add Item");
        Console.WriteLine("[2] Edit Item");
        Console.WriteLine("[3] Remove Item");
        Console.WriteLine("[4] Check Inventory");
        Console.WriteLine("[0] Exit");
        Console.WriteLine("======================================================");
    }
}

// Class representing the login menu
class LoginMenu : Menu
{   
    //Login Prompt
    public override void DisplayMenu()
    {
        Console.WriteLine("\n\tLogin Menu");
        Console.WriteLine("===============");
        Console.WriteLine("[1] Login");
        Console.WriteLine("[0] Exit");
        Console.WriteLine("===============");
    }
}

// Main program class
class Program
{
    public static void Main(string[] args)
    {
        // Display Menus
        Menu loginMenu = new LoginMenu();
        Menu storeMenu = new StoreMenu();

        // Admin credentials
        const string adminUsername = "Admin";
        const string adminPassword = "AdminPass";

        // Login loop
        bool loggedIn = false;
        while (!loggedIn)
        {
            try
            {
                loginMenu.DisplayMenu();

                Console.Write("Enter your choice: ");
                int loginChoice = Convert.ToInt32(Console.ReadLine());

                // Exit program
                if (loginChoice == 0)
                {
                    Console.WriteLine("\n[ Exiting the program... ]");
                    return;
                }
                // Login option
                else if (loginChoice == 1)
                {
                    Console.Write("\nEnter Username [Admin]: ");
                    string username = Console.ReadLine();
                    Console.Write("Enter Password [AdminPass]: ");
                    string password = Console.ReadLine();

                    // Check username and pass
                    if (username == adminUsername && password == adminPassword)
                    {
                        Console.WriteLine("\n[ Login Successful! ]");
                        loggedIn = true; // Set login to true
                    }
                    else
                    {
                        Console.WriteLine("\n[ Invalid username or password! Please try again. ]");
                    }
                }
                else
                {
                    Console.WriteLine("\n[ Invalid choice! Please enter a valid option. ]");
                }
            }
            catch (FormatException)
            {
                Console.WriteLine("\n[ Invalid input! Please enter a valid number. ]");
            }
            catch (Exception ex)
            {
                Console.WriteLine("\n[ Error: " + ex.Message + " ]");
            }
        }

        while (true)
        {
            string filePath = "C:\\Users\\Public\\Documents\\Convenience_Store_Inventory_Management_System.csv";
            Sys.csvChecker(filePath);
            try
            {
                storeMenu.DisplayMenu();

                Console.Write("Enter your choice: ");
                int choice = Convert.ToInt32(Console.ReadLine());


                switch (choice)
                {
                    case 0:
                        Console.WriteLine("\n[ Exiting the program... ]");
                        return;
                    case 1:
                        Console.WriteLine("\n| Add Item |");
                        int itemId = PromptForItemDetails("ID: ", int.Parse);
                        if (Sys.dupCheck(itemId))
                        {
                            Console.WriteLine("Item already existing!");
                            continue;
                        }
                        string itemName = PromptForItemDetails("Name: ", str => str);
                        double itemPrice = PromptForItemDetails("Price: ", double.Parse);
                        int itemquantity = PromptForItemDetails("Quantity: ", int.Parse);
                        int rawStock = PromptForItemDetails("Stock([1] Onhand [2] Warehouse): ", int.Parse);
                        bool itemPerishable = PromptForItemDetails("Is it perishable? (true/false): ", bool.Parse);
                        string itemStock = "";
                        itemName = itemName.ToUpper();
                        if (rawStock == 1)
                        {
                            itemStock = "Onhand";
                        }
                        else if (rawStock == 2)
                        {
                            itemStock = "Warehouse";
                        }
                        finalProduct item;
                        if (itemPerishable)
                        {
                            string itemCategory = "Perishable";
                            item = new finalProduct(itemName, itemPrice, itemquantity, itemId, itemStock, itemCategory);
                        }
                        else if (!itemPerishable)
                        {
                            string itemCategory = "Non-perishable";
                            item = new finalProduct(itemName, itemPrice, itemquantity, itemId, itemStock, itemCategory);
                        }
                        else
                        {
                            Console.WriteLine("Invalid Input!");
                            break;
                        }

                        Sys.addItem(item);
                        Console.WriteLine("Item added");
                        break;
                    case 2:
                        Console.WriteLine("\n| Edit Item |");
                        int eid = PromptForItemDetails("ID: ", int.Parse);
                        Sys.editItem(eid);
                        break;
                    case 3:
                        Console.WriteLine("\n| Remove Item |");
                        int rid = PromptForItemDetails("ID: ", int.Parse);
                        Sys.removeItem(rid);
                        break;
                    case 4:
                        Console.WriteLine("\n[ Check Inventory ]");
                        Sys.showInv();
                        break;
                    default:
                        Console.WriteLine("\n[ Invalid choice!]");
                        break;
                }
            }
            catch (FormatException)
            {
                Console.WriteLine("\n[ Invalid input!]");
            }
            catch (Exception ex)
            {
                Console.WriteLine("\n[ Error: " + ex.Message + " ]");
            }
        }
    }

    // Method to prompt user for item details and return the input value
    static T PromptForItemDetails<T>(string detail, Func<string, T> converter)
    {
        Console.Write($"Enter {detail}: ");
        string input = Console.ReadLine();
        return converter(input);
    }
}
