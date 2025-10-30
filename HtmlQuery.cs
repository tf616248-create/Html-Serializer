using System;
using System.Collections.Generic;

namespace HtmlTool
{
    /// <summary>
    /// אחראי על חיפוש אלמנטים בעץ HTML לפי בורר (Selector).
    /// </summary>
    public class HtmlQuery
    {
        /// <summary>
        /// האלמנט הראשי (שורש) של עץ ה-HTML.
        /// </summary>
        private readonly HtmlElement _root;

        /// <summary>
        /// יוצר מופע חדש של HtmlQuery על בסיס עץ HTML נתון.
        /// </summary>
        public HtmlQuery(HtmlElement root)
        {
            _root = root ?? throw new ArgumentNullException(nameof(root));
        }

        /// <summary>
        /// מבצע חיפוש של אלמנטים בעץ על פי הבורר הנתון.
        /// </summary>
        /// <param name="selectorText">מחרוזת בורר בסגנון CSS, לדוגמה "div.main" או "#header".</param>
        /// <returns>רשימת אלמנטים שתואמים לבורר.</returns>
        public List<HtmlElement> Find(string selectorText)
        {
            if (string.IsNullOrWhiteSpace(selectorText))
                throw new ArgumentNullException(nameof(selectorText));

            var selector = Selector.Parse(selectorText);
            var result = new List<HtmlElement>();

            // חיפוש באמצעות תור
            var queue = new Queue<HtmlElement>();
            queue.Enqueue(_root);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();

                // בדוק התאמה של האלמנט הנוכחי
                if (selector.Matches(current))
                    result.Add(current);

                // הוסף את הילדים לתור לבדיקה
                foreach (var child in current.Children)
                {
                    queue.Enqueue(child);
                }
            }

            return result;
        }
    }
}
