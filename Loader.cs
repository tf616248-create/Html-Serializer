//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace ConsoleApp1
//{
//    internal class Loader//!!!
//    {
//        public async Task<string> LoadAsync(string url)
//        {
//            using var client = new HttpClient();
//            var response = await client.GetAsync(url);
//            response.EnsureSuccessStatusCode();
//            return await response.Content.ReadAsStringAsync();
//        }
//    }
//}


using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace HtmlTool
{
    /// <summary>
    /// אחראי לבצע קריאה לאתר אינטרנט ולקבל את קוד ה-HTML שלו.
    /// </summary>
    public class Loader
    {
        /// <summary>
        /// טוען את תוכן דף ה-HTML מהכתובת הנתונה.
        /// </summary>
        /// <param name="url">כתובת אינטרנט תקינה (URL).</param>
        /// <returns>מחרוזת HTML.</returns>
        public async Task<string> LoadAsync(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentNullException(nameof(url));

            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var html = await response.Content.ReadAsStringAsync();
                return html;
            }
        }
    }
}

