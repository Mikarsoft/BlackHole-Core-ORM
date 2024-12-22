namespace TestBench
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            TestClass c1 = new TestClass();

            int value = 5;

            var res = c1.ParseExpression(x => (value >= x.NumericValue || (x.Firstname != null && x.IsEnabled))
            && (!x.IsEnabled && x.Firstname == "Mike" && x.LastModified == DateTime.Now));

            string command = res.BuildSqlCommand();
        }
    }
}
