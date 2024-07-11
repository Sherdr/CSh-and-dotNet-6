using Microsoft.EntityFrameworkCore;
using Packt.Shared;

namespace LinqWithEFCore {
    internal class Program {
        static void Main(string[] args) {
            FilterAndSort();
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
    }
}
