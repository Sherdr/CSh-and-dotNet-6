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
                IQueryable<Product> filteredProducts = allProducts.Where(prod => prod.UnitPrice < 10m);
                IOrderedQueryable<Product> sortedProducts = filteredProducts.OrderByDescending(prod => prod.UnitPrice);
                foreach (Product p in sortedProducts) {
                    Console.WriteLine($"{p.ProductId}: {p.ProductName} costs {p.UnitPrice:$#,##0.00}");
                }
                Console.WriteLine();
            }
        }
    }
}
