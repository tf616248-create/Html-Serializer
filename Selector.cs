using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace HtmlTool
{
    /// <summary>
    /// מייצג בורר (selector) פשוט כמו ב-CSS.
    /// מאפשר לבדוק אם אלמנט HTML מתאים לו.
    /// תומך גם בקישור בין רמות (Parent/Child).
    /// </summary>
    public class Selector
    {
        public string Tag { get; private set; }
        public string Id { get; private set; }
        public List<string> Classes { get; private set; }

        /// <summary>
        /// בורר ילד (שלב הבא ברצף ה-selector)
        /// לדוגמה: עבור "div p" – ל-selector של div יהיה child עם tag=p
        /// </summary>
        public Selector Child { get; private set; }

        private Selector()
        {
            Classes = new List<string>();
        }

        /// <summary>
        /// בונה אובייקט Selector ממחרוזת בורר.
        /// תומך גם בבוררים מרובי רמות כמו: "div.main p span"
        /// </summary>
        public static Selector Parse(string selector)
        {
            if (string.IsNullOrWhiteSpace(selector))
                throw new ArgumentNullException(nameof(selector));

            // מפרקים את המחרוזת לפי רווחים
            var parts = selector.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            // בונים שרשרת selector-ים (כל חלק הוא רמה אחת)
            Selector root = null;
            Selector current = null;

            foreach (var part in parts)
            {
                var sel = ParseSingle(part);

                if (root == null)
                {
                    root = sel;
                    current = sel;
                }
                else
                {
                    current.Child = sel;
                    current = sel;
                }
            }

            return root;
        }

        /// <summary>
        /// מפענח רמה אחת של selector (למשל "div.main#header")
        /// </summary>
        private static Selector ParseSingle(string selectorPart)
        {
            var result = new Selector();

            var regex = new Regex(@"^(?<tag>[a-zA-Z0-9]*)?(?<rest>.*)$");
            var match = regex.Match(selectorPart.Trim());
            if (!match.Success)
                return result;

            if (match.Groups["tag"].Success && !string.IsNullOrEmpty(match.Groups["tag"].Value))
                result.Tag = match.Groups["tag"].Value.ToLowerInvariant();

            string rest = match.Groups["rest"].Value;
            var idMatch = Regex.Match(rest, @"#([a-zA-Z0-9\-_]+)");
            if (idMatch.Success)
                result.Id = idMatch.Groups[1].Value;

            foreach (Match cls in Regex.Matches(rest, @"\.([a-zA-Z0-9\-_]+)"))
            {
                result.Classes.Add(cls.Groups[1].Value);
            }

            return result;
        }

        /// <summary>
        /// בודקת אם אלמנט HTML תואם לבורר הזה.
        /// </summary>
        public bool Matches(HtmlElement element)
        {
            if (element == null)
                return false;

            if (!string.IsNullOrEmpty(Tag) &&
                !string.Equals(Tag, element.Name, StringComparison.OrdinalIgnoreCase))
                return false;

            if (!string.IsNullOrEmpty(Id) &&
                !string.Equals(Id, element.Id, StringComparison.OrdinalIgnoreCase))
                return false;

            foreach (var cls in Classes)
            {
                if (!element.Classes.Contains(cls))
                    return false;
            }

            return true;
        }
    }
}
