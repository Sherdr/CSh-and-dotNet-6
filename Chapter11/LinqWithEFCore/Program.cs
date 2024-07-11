using Microsoft.EntityFrameworkCore;
using Packt.Shared;

namespace LinqWithEFCore {
    internal class Program {
        static void Main(string[] args) {
            CustomExtensionMethods();
        }

        static void FilterAndSort() {
            Console.WriteLine("Products that cost less than $10:");
            using(Northwind db = new()) {
                DbSet<Product> allProducts = db.Products;
                if(allProducts is null) {
                    Console.WriteLine("No products found.");
                    return;
                }
                IQueryable<Product> processedProducts = allProducts.ProcessSequence();
                IQueryable<Product> filteredProducts = processedProducts.Where(product => product.UnitPrice < 10m);
                IOrderedQueryable<Product> sortedProducts = filteredProducts.OrderByDescending(prod => prod.UnitPrice);
                var projectedProducts = sortedProducts.
                    Select(prod => new {
                        prod.ProductId,
                        prod.ProductName,
                        prod.UnitPrice,
                    });
                foreach (var p in projectedProducts) {
                    Console.WriteLine($"{p.ProductId}: {p.ProductName} costs {p.UnitPrice:$#,##0.00}");
                }
                Console.WriteLine();
            }
        }

        static void JoinCategoriesAndProducts() {
            using (Northwind db = new()) {
                var queryJoin = db.Categories.Join(
                    db.Products,
                    category => category.CategoryId,
                    product => product.CategoryId,
                    (c, p) => new {
                        c.CategoryName,
                        p.ProductName,
                        p.ProductId
                    }).
                OrderBy(cp => cp.CategoryName);
                foreach (var item in queryJoin) {
                    Console.WriteLine($"{item.ProductId}: {item.ProductName} is in {item.CategoryName}");
                }
            }
        }

        static void GroupJoinCategoriesAndProducts() {
            using (Northwind db = new()) {
                var queryGroup = db.Categories.AsEnumerable().GroupJoin(
                    db.Products,
                    category => category.CategoryId,
                    product => product.CategoryId,
                    (c, p) => new {
                        c.CategoryName,
                        Products = p.OrderBy(prod => prod.ProductName)
                    });
                foreach(var cat in queryGroup) {
                    Console.WriteLine($"{cat.CategoryName} has {cat.Products.Count()} products:");
                    foreach(var prod in cat.Products) {
                        Console.WriteLine($"\t{prod.ProductName}");
                    }
                }
            }
        }

        static void AggregateProducts() {
            using(Northwind db = new()) {
                Console.WriteLine($"{"Product count:",          -25} {db.Products.Count(), 10}");
                Console.WriteLine($"{"Highest product price:",  -25} {db.Products.Max(p => p.UnitPrice),10:$#,##0.00}");
                Console.WriteLine($"{"Sum of units in stock:",  -25} {db.Products.Sum(p => p.UnitsInStock), 10:N0}");
                Console.WriteLine($"{"Sum of units on order:",  -25} {db.Products.Sum(p => p.UnitsOnOrder),10:N0}");
                Console.WriteLine($"{"Avarage unit price:",     -25} {db.Products.Average(p => p.UnitPrice),10:$#,##0.00}");
                Console.WriteLine($"{"Value of units in stock:",-25} {db.Products.Sum(p => p.UnitPrice * p.UnitsInStock),10:$#,##0.00}");
            }
        }

        static void CustomExtensionMethods() {
            using(Northwind db = new()) {
                Console.WriteLine($"{"Mean units in stock:",-25} {db.Products.Average(p => p.UnitsInStock),10:N0}");
                Console.WriteLine($"{"Mean units price:",-25} {db.Products.Average(p => p.UnitPrice),10:$#,##0.00}");
                Console.WriteLine($"{"Median units in stock:",-25} {db.Products.Median(p => p.UnitsInStock),10:N0}");
                Console.WriteLine($"{"Median units price:",-25} {db.Products.Median(p => p.UnitPrice),10:$#,##0.00}");
                Console.WriteLine($"{"Mode units in stock:",-25} {db.Products.Mode(p => p.UnitsInStock),10:N0}");
                Console.WriteLine($"{"Mode units price:",-25} {db.Products.Mode(p => p.UnitPrice),10:$#,##0.00}");
            }
        }
    }
}
