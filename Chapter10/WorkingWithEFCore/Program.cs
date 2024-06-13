using Microsoft.EntityFrameworkCore;
using Packt.Shared;

namespace WorkingWithEFCore {
    internal class Program {
        static void Main(string[] args) {
            Console.WriteLine($"Using {ProjectConstant.DatabaseProvider} provider.");
            FilteredIncludes();
        }

        static void QueryingCategories() {
            Console.WriteLine("Categories and how many products they have:");
            using (Northwind db = new()) {
                IQueryable<Category>? categories = db.Categories?
                    .Include(category => category.Products);
                if (categories is null) {
                    Console.WriteLine("No categories found.");
                    return;
                }
                foreach (Category c in categories) {
                    Console.WriteLine($"{c.CategoryName} has {c.Products.Count} products.");
                }
            }
        }

        static void FilteredIncludes() {
            using (Northwind db = new()) {
                Console.WriteLine("Enter a minimum for units in stock: ");
                string unitsInStock = Console.ReadLine() ?? "10";
                int stock = int.Parse(unitsInStock);
                IQueryable<Category>? categories = db.Categories?
                    .Include(category => category.Products.Where(product => product.Stock >= stock));
                if (categories is null) {
                    Console.WriteLine("No categories found.");
                    return;
                }
                foreach (Category c in categories) {
                    Console.WriteLine($"{c.CategoryName} has {c.Products.Count} products with a minimum of {stock} units in stock.");
                    foreach (Product p in c.Products) {
                        Console.WriteLine($"\t{p.ProductName} has {p.Stock} units in stock.");
                    }
                }
            }
        }
    }
}
