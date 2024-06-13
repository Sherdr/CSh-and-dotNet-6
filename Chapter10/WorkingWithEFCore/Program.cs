using Microsoft.EntityFrameworkCore;
using Packt.Shared;

namespace WorkingWithEFCore {
    internal class Program {
        static void Main(string[] args) {
            Console.WriteLine($"Using {ProjectConstant.DatabaseProvider} provider.");
            QueryingCategories();
        }
        static void QueryingCategories() {
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
    }
}
