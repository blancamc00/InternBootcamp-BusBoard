namespace BusBoard
{
    class Program
    {
        //static void Main(string[] args)
        //{
         //   Console.WriteLine("Welcome to BusBoard!");
        //}

        static async Task Main(string[] args)
        {
            var u = "https://transportapi.com/v3/uk/bus/stop/0500CCITY436///timetable.json?";
            using var client = new HttpClient();

            var builder = new UriBuilder(u);
            builder.Query = "app_id=7d0ebcfd&app_key=bb3b6e2b09788d7ca770cd28ba1156bf&group=route";
            var url = builder.ToString();
            var result =
                await client.GetAsync(url);
            Console.WriteLine(result);
        }
    }
}
