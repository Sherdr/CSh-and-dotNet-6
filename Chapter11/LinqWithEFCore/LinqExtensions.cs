namespace System.Linq;

    public static class LinqExtensions {
        public static IEnumerable<T> ProcessSequence<T>(this IEnumerable<T> sequence) {
            return sequence;
        }

        public static IQueryable<T> ProcessSequence<T>(this IQueryable<T> sequence) {
            return sequence; 
        }

        public static int? Median(this IEnumerable<int?> sequence) {
            var ordered = sequence.OrderBy(x => x);
            int midPosition = ordered.Count() / 2;
            return ordered.ElementAt(midPosition);
        }

        public static int? Median<T>(this IEnumerable<T> sequence, Func<T, int?> selector) {
            return sequence.Select(selector).Median();
        }

        public static decimal? Median(this IEnumerable<decimal?> sequence) {
            var ordered = sequence.OrderBy(x => x);
            int midPosition = ordered.Count() / 2;
            return ordered.ElementAt(midPosition);
        }

        public static decimal? Median<T>(this IEnumerable<T> sequence, Func<T, decimal?> selector) {
            return sequence.Select(selector).Median();
        }

        public static int? Mode(this IEnumerable<int?> sequence) {
            var grouped = sequence.GroupBy(x => x);
            var ordered = grouped.OrderByDescending(grouped => grouped.Count());
            return ordered.FirstOrDefault()?.Key;
        }

        public static int? Mode<T>(this IEnumerable<T> sequence, Func<T, int?> selector) {
            return sequence.Select(selector).Mode();
        }

        public static decimal? Mode(this IEnumerable<decimal?> sequence) {
            var grouped = sequence.GroupBy(x => x);
            var ordered = grouped.OrderByDescending(grouped => grouped.Count());
            return ordered.FirstOrDefault()?.Key;
        }

        public static decimal? Mode<T>(this IEnumerable<T> sequence, Func<T, decimal?> selector) {
            return sequence.Select(selector).Mode();
        }
}
