
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HtmlTool
{
    public class HtmlSerializer
    {
        private readonly Regex _tokenRegex = new Regex("<[^>]+>|[^<]+", RegexOptions.Compiled);
        private readonly Regex _tagNameRegex = new Regex(@"^<\/?\s*([a-zA-Z0-9]+)", RegexOptions.Compiled);
        private readonly Regex _attrRegex = new Regex(@"([a-zA-Z0-9\-]+)\s*=\s*(""([^""]*)""|'([^']*)'|([^\s>]+))?", RegexOptions.Compiled);

        /// <summary>
        /// קריאה לדף אינטרנט
        /// </summary>
        public async Task<string> Load(string url)
        {
            HttpClient client = new HttpClient();
            var response = await client.GetAsync(url);
            var html = await response.Content.ReadAsStringAsync();
            return html;
        }

        /// <summary>
        /// המרת מחרוזת Html לעץ של אוביקטים מסוג HtmlElement
        /// </summary>
        public HtmlElement Parse(string html)
        {
            if (html == null)
                throw new ArgumentNullException(nameof(html));

            var tokens = _tokenRegex.Matches(html)
                                    .Select(m => m.Value)
                                    .Select(t => t.Trim())
                                    .Where(t => !string.IsNullOrWhiteSpace(t))
                                    .ToList();

            var root = new HtmlElement { Name = "#document" };
            var current = root;

            foreach (var token in tokens)
            {
                if (token.StartsWith("<"))
                {
                    bool isClosing = token.StartsWith("</");
                    bool isSelfClosing = token.EndsWith("/>");

                    var nameMatch = _tagNameRegex.Match(token);
                    if (!nameMatch.Success)
                        continue;

                    var tagName = nameMatch.Groups[1].Value.ToLowerInvariant();

                    // אם התגית אינה תגית HTML חוקית לפי הרשימה
                    if (!HtmlHelper.Instance.AllTags.Contains(tagName))
                        continue;

                    if (isClosing)
                    {
                        // עלי רמה אחת למעלה בעץ
                        if (current.Parent != null)
                            current = current.Parent;

                        continue;
                    }

                    // יצירת אוביקט HtmlElement
                    var element = new HtmlElement { Name = tagName };

                    // ניתוח attributes
                    int attrStartIndex = nameMatch.Length;
                    string attrText = token.Length > attrStartIndex ? token.Substring(attrStartIndex) : string.Empty;
                    attrText = attrText.Trim().TrimEnd('>').TrimEnd('/').Trim();

                    foreach (Match a in _attrRegex.Matches(attrText))
                    {
                        var key = a.Groups[1].Value;
                        var value = a.Groups[3].Success ? a.Groups[3].Value
                                   : a.Groups[4].Success ? a.Groups[4].Value
                                   : a.Groups[5].Success ? a.Groups[5].Value
                                   : string.Empty;

                        element.Attributes.Add((key, value));

                        if (key.Equals("id", StringComparison.OrdinalIgnoreCase))
                            element.Id = value;

                        if (key.Equals("class", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(value))
                            element.Classes.AddRange(value.Split(' ', StringSplitOptions.RemoveEmptyEntries));
                    }

                    // בדיקה אם התגית סוגרת את עצמה
                    if (HtmlHelper.Instance.SelfClosingTags.Contains(tagName))
                        isSelfClosing = true;

                    // הוספה לעץ
                    element.Parent = current;
                    current.Children.Add(element);

                    if (!isSelfClosing)
                        current = element;
                }
                else
                {
                    // טקסט פנימי
                    var txt = token.Trim();
                    if (!string.IsNullOrEmpty(txt))
                        current.InnerHtml += txt;
                }
            }

            return root;
        }
    }
}
