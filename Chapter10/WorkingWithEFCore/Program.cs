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
                Console.WriteLine($"ToQueryString: {categories.ToQueryString()}");
                foreach (Category c in categories) {
                    Console.WriteLine($"{c.CategoryName} has {c.Products.Count} products with a minimum of {stock} units in stock.");
                    foreach (Product p in c.Products) {
                        Console.WriteLine($"\t{p.ProductName} has {p.Stock} units in stock.");
                    }
                }
            }
        }

        static void QueryingProducts() {
            using (Northwind db = new()) {
                Console.WriteLine("Products than cost more than a price, highest at top");
                string? input;
                decimal price;
                do {
                    Console.WriteLine("Enter a product price: ");
                    input = Console.ReadLine();
                } while (!decimal.TryParse(input, out price));
                IQueryable<Product>? products = db.Products?
                    .Where(product => product.Cost > price)
                    .OrderByDescending(product => product.Cost);
                if (products is null) {
                    Console.WriteLine("No products found.");
                    return;
                }
                foreach (Product p in products) {
                    Console.WriteLine($"{p.ProductId}: {p.ProductName} costs {p.Cost:$#,##0.00} and has {p.Stock} in stock.");
                }
            }
        }
    }
}
