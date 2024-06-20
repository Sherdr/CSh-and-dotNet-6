namespace LinqWithObjects {
    internal class Program {
        static void Main(string[] args) {
            string[] names = new[] {
                "Michael", "Pam", "Jim", "Dwight", "Angela", "Kevin", "Toby", "Creed"
            };
            var query1 = names.Where(name => name.EndsWith("m"));
            var query2 = from name in names where name.EndsWith("m") select name;

            string[] result1 = query1.ToArray();
            List<string> result2 = query2.ToList();

            foreach (string name in result1) {
                Console.WriteLine(name);
                names[2] = "Jimmy";
            }
            Console.WriteLine("Deferred execution.");
            foreach (string name in query1) {
                Console.WriteLine(name);
                names[2] = "Jimmy";
            }
        }
    }
}
