using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Packt.Shared;

namespace WorkingWithEFCore {
    internal class Program {
        static void Main(string[] args) {
            Console.WriteLine($"Using {ProjectConstant.DatabaseProvider} provider.");
            Console.WriteLine($"{DeleteProducts("Bob")} product(s) were deleted.");
        }

        static void QueryingCategories() {
            Console.WriteLine("Categories and how many products they have:");
            using (Northwind db = new()) {
                ILoggerFactory loggerFactory = db.GetService<ILoggerFactory>();
                loggerFactory.AddProvider(new ConsoleLoggerProvider());
                IQueryable<Category>? categories;
                // = db.Categories;
                //.Include(category => category.Products);
                db.ChangeTracker.LazyLoadingEnabled = false;
                Console.WriteLine("Enable eager loading? (Y/N): ");
                bool eagerLoading = Console.ReadKey().Key == ConsoleKey.Y;
                bool explicitLoading = false;
                if (eagerLoading) {
                    categories = db.Categories?
                        .Include(category => category.Products);
                }
                else {
                    categories = db.Categories;
                    Console.WriteLine("Enable explicit loading? (Y/N): ");
                    explicitLoading = Console.ReadKey().Key == ConsoleKey.Y;
                    Console.WriteLine();
                }
                foreach (Category c in categories) {
                    if (explicitLoading) {
                        Console.WriteLine($"Explicitly load products for {c.CategoryName}? (Y/N): ");
                        ConsoleKeyInfo key = Console.ReadKey();
                        if (key.Key == ConsoleKey.Y) {
                            CollectionEntry<Category, Product> products = db.Entry(c)
                                .Collection(cat => cat.Products);
                            if (!products.IsLoaded) {
                                products.Load();
                            }
                        }
                    }
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
                ILoggerFactory loggerFactory = db.GetService<ILoggerFactory>();
                loggerFactory.AddProvider(new ConsoleLoggerProvider());
                Console.WriteLine("Products than cost more than a price, highest at top");
                string? input;
                decimal price;
                do {
                    Console.WriteLine("Enter a product price: ");
                    input = Console.ReadLine();
                } while (!decimal.TryParse(input, out price));
                IQueryable<Product>? products = db.Products?
                    .TagWith("Products filtered by price and sorted.")
                    .Where(product => product.Cost > price)
                    .OrderByDescending(product => product.Cost);
                if (products is null) {
                    Console.WriteLine("No products found.");
                    return;
                }
                foreach (Product p in products) {
                    Console.WriteLine($"{p.ProductId}: \t{p.ProductName} costs {p.Cost:$#,##0.00} and has {p.Stock} in stock.");
                }
            }
        }

        static void QueryingWithLike() {
            using (Northwind db = new()) {
                ILoggerFactory loggerFactory = db.GetService<ILoggerFactory>();
                loggerFactory.AddProvider(new ConsoleLoggerProvider());
                Console.WriteLine("Enter part of a product name: ");
                string? input = Console.ReadLine();
                IQueryable<Product>? products = db.Products?
                    .Where(product => EF.Functions.Like(product.ProductName, $"%{input}%"));
                if (products is null) {
                    Console.WriteLine("No products found.");
                    return;
                }
                foreach (Product p in products) {
                    Console.WriteLine($"{p.ProductName} has {p.Stock} units in stock. Discontinued - {p.Discontinued}.");
                }
            }
        }

        static bool AddProduct(int categoryId, string productName, decimal? price) {
            using (Northwind db = new()) {
                Product product = new() {
                    CategoryId = categoryId,
                    ProductName = productName,
                    Cost = price
                };
                db.Products.Add(product);
                int affected = db.SaveChanges();
                return affected == 1;
            }
        }

        static void ListProducts() {
            using (Northwind db = new()) {
                Console.WriteLine($"{"Id",-3} {"Name",-35} " +
                    $"{"Cost",8} {"Stock",5} {"Disc."}");
                foreach (Product prod in db.Products
                    .OrderByDescending(p => p.Cost)) {
                    Console.WriteLine($"{prod.ProductId:0000} {prod.ProductName,-35} " +
                    $"{prod.Cost,8:$#,##0.00} {prod.Stock,5} {prod.Discontinued}");
                }
            }
        }

        static bool IncreaseProductPrice(string productNameStartWith, decimal amount) {
            using (Northwind db = new()) {
                Product updateProduct = db.Products.First(p => p.ProductName.StartsWith(productNameStartWith));
                updateProduct.Cost += amount;
                int affected = db.SaveChanges();
                return (affected == 1);
            }
        }

        static int DeleteProducts(string productNameStartWith) {
            using (Northwind db = new()) {
                using (IDbContextTransaction t = db.Database.BeginTransaction()) {
                    Console.WriteLine($"Transaction isolation level: {t.GetDbTransaction().IsolationLevel}");
                    IQueryable<Product>? products = db.Products?
                        .Where(p => p.ProductName.StartsWith(productNameStartWith));
                    if (products is null) {
                        Console.WriteLine("No products found.");
                        return 0;
                    }
                    else {
                        db.Products.RemoveRange(products);
                    }
                    int affected = db.SaveChanges();
                    t.Commit();
                    return affected;
                }
            }
        }
    }
}
