
using System;
using System.Threading.Tasks;

namespace HtmlTool
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("== HTML Processing Tool ==");

            var loader = new Loader();
            var serializer = new HtmlSerializer();

            // דוגמה: קריאה לדף אמיתי (ניתן לשנות לפי הצורך)
            string url = "http://example.com";
            Console.WriteLine($"טוען את הדף: {url}");
            string html = await loader.LoadAsync(url);

            Console.WriteLine("מבצע סריאליזציה...");
            var document = serializer.Parse(html);

            var query = new HtmlQuery(document);

            Console.WriteLine("מכניס בורר לדוגמה: div");
            var results = query.Find("div");

            Console.WriteLine($"נמצאו {results.Count} תוצאות:");
            foreach (var r in results)
            {
                Console.WriteLine($"< {r.Name} id='{r.Id}' class='{string.Join(" ", r.Classes)}' >");
            }
        }
    }
}
