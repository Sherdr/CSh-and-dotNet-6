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

            var query = names.Where(name => name.Length > 4);
            Console.WriteLine("Writing queries.");
            foreach (string name in query) {
                Console.WriteLine(name);
                //names[2] = "Jimmy";
            }
        }
    }
}
