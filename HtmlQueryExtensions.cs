using System;
using System.Collections.Generic;
using System.Linq;

namespace HtmlTool
{
    /// <summary>
    /// פונקציות הרחבה עבור HtmlElement, לצורך חיפוש אלמנטים לפי Selector.
    /// </summary>
    public static class HtmlQueryExtensions
    {
        /// <summary>
        /// מחפשת אלמנטים בעץ HTML על פי Selector נתון.
        /// </summary>
        /// <param name="element">האלמנט שממנו מתחילים את החיפוש (בדרך כלל root).</param>
        /// <param name="selector">האובייקט Selector שמתאר את הקריטריונים לחיפוש.</param>
        /// <returns>רשימת אלמנטים שתואמים ל-Selector.</returns>
        public static HashSet<HtmlElement> FindBySelector(this HtmlElement element, Selector selector)
        {
            var result = new HashSet<HtmlElement>();
            if (element == null || selector == null)
                return result;

            // קבל את כל הצאצאים של האלמנט (כולל הדורות הבאים)
            var descendants = element.Descendants();

            // מצא את כל האלמנטים שעונים על הקריטריון הנוכחי של ה-selector
            var matched = descendants
                .Where(e => MatchesSelectorLevel(e, selector))
                .ToList();

            if (selector.Child == null)
            {
                // אם זה הסלקטור האחרון, החזר את הרשימה
                foreach (var m in matched)
                    result.Add(m);
            }
            else
            {
                // אם יש סלקטור ילד – המשך לחפש בעומק
                foreach (var m in matched)
                {
                    var subResults = m.FindBySelector(selector.Child);
                    foreach (var sub in subResults)
                        result.Add(sub);
                }
            }

            return result;
        }

        /// <summary>
        /// בודקת אם אלמנט מסוים מתאים לרמה הנוכחית של Selector.
        /// </summary>
        private static bool MatchesSelectorLevel(HtmlElement e, Selector selector)
        {
            if (selector.Tag != null &&
                !string.Equals(e.Name, selector.Tag, StringComparison.OrdinalIgnoreCase))
                return false;

            if (selector.Id != null &&
                !string.Equals(e.Id, selector.Id, StringComparison.OrdinalIgnoreCase))
                return false;

            if (selector.Classes.Any() &&
                !selector.Classes.All(c => e.Classes.Contains(c)))
                return false;

            return true;
        }
    }
}
