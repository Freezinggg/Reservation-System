using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace Reservation.LoadGenerator
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var url = "http://localhost:5000/api/reservation"; // adjust port if needed

            var client = new HttpClient();

            int totalRequests = 50;

            var tasks = new List<Task<HttpResponseMessage>>();

            var payload = new
            {
                seatCategoryId = "7ec4594a-74a6-4841-afd4-cf440c9278d4",
                quantity = 1
            };

            var json = JsonSerializer.Serialize(payload);
//            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // 🔥 Fire all requests concurrently
            for (int i = 0; i < totalRequests; i++)
            {
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                tasks.Add(client.PostAsync(url, content));
            }

            // ⏳ Wait for all
            var responses = await Task.WhenAll(tasks);

            // 📊 Analyze results
            int success = responses.Count(r => r.StatusCode == System.Net.HttpStatusCode.Created);
            int conflict = responses.Count(r => r.StatusCode == System.Net.HttpStatusCode.Conflict);

            Console.WriteLine($"Total: {totalRequests}");
            Console.WriteLine($"Success (201): {success}");
            Console.WriteLine($"Conflict (409): {conflict}");
            Console.ReadLine();
        }
    }
}
