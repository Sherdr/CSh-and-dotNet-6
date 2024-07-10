using Microsoft.EntityFrameworkCore;
using Packt.Shared;

namespace LinqWithEFCore {
    internal class Program {
        static void Main(string[] args) {
            string[] names = new[] { "Michael", "Pam", "Jim", "Dwight", "Angela", "Kevin", "Toby", "Creed"};
            var query1 = (from name in names
                where name.Length > 4
                orderby name.Length, name
                select name);
            var query2 = (from name in names
                where name.Length > 4
                select name).
                Skip(2).
                Take(5);
            Console.WriteLine("Query1:");
            foreach (var name in query1) {
                Console.WriteLine(name);
            }
            Console.WriteLine("\nQuery2:");
            foreach (var name in query2) {
                Console.WriteLine(name);
            }
        }

        static void FilterAndSort() {
            Console.WriteLine("Products that cost less than $10:");
            using(Northwind db = new()) {
                DbSet<Product> allProducts = db.Products;
                IQueryable<Product> filteredProducts = allProducts.Where(prod => prod.UnitPrice < 10m);
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
