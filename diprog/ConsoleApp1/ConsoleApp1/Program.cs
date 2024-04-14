using System.Security.Cryptography.X509Certificates;
using System;
using System.IO;
using System.Linq;

public class Product
{
    public string Name { get; set; }
    public double Price { get; set; }
    public int Quantity { get; set; }
    public int ID { get; set; }

    public Product(string name, double price, int quantity, int id)
    {
        Name = name;
        Price = price;
        Quantity = quantity;
        ID = id;
    }

    // Display product info
    public virtual void DisplayProductInfo()
    {
        Console.WriteLine($"Product Name: {Name}");
        Console.WriteLine($"Price: P{Price}");
        Console.WriteLine($"Quantity: {Quantity}");
        Console.WriteLine($"ID: {ID}");
    }
}

public class finalProduct : Product
{
    public string Stock { get; set; }
    public string Perishable { get; set; }

    public finalProduct(string name, double price, int quantity, int id, string stock, string perishable) : base(name, price, quantity, id)
    {
        Stock = stock;
        Perishable = perishable;
    }

    public override void DisplayProductInfo()
    {
        base.DisplayProductInfo();
        Console.WriteLine("Stock: " + Stock);
        Console.WriteLine("Category: " + Perishable);
    }
}

class Sys
{
    private const string filePath = "C:\\Users\\Public\\Documents\\Convenience_Store_Inventory_Management_System.csv";

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
                Console.Write("Choose what to edit:\n[1] Name\n[2] Quantity\n[3] Price\nEnter choice:");
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
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error displaying inventory: {ex.Message}");
        }
    }

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

abstract class Menu
{
    public abstract void DisplayMenu();
}

class StoreMenu : Menu
{
    public override void DisplayMenu()
    {   //Action Prompts
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

class LoginMenu : Menu
{   //Login Prompt
    public override void DisplayMenu()
    {
        Console.WriteLine("\n\tLogin Menu");
        Console.WriteLine("===============");
        Console.WriteLine("[1] Login");
        Console.WriteLine("[0] Exit");
        Console.WriteLine("===============");
    }
}

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
                        bool itemPerishable = PromptForItemDetails("Is it perishable? (true/false): ", bool.Parse);
                        finalProduct item;
                        if (itemPerishable)
                        {
                            string itemStock = "Onhand";
                            string itemCategory = "Perishable";
                            item = new finalProduct(itemName, itemPrice, itemquantity, itemId, itemStock, itemCategory);
                        }
                        else if (!itemPerishable)
                        {
                            string itemStock = "Warehouse";
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
