using System;
using System.Collections.Generic;

namespace HtmlTool
{
    /// <summary>
    /// מייצגת תגית אחת של HTML, כולל שם, מזהה, מאפיינים, ילדים והורה.
    /// </summary>
    public class HtmlElement
    {
        /// <summary>
        /// מזהה ייחודי של האלמנט (אם הוגדר בתגית id="...").
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// שם התגית (לדוגמה: div, span, p, a וכו').
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// רשימת מאפיינים של התגית, כמו href, src, title וכו'.
        /// כל פריט הוא זוג (שם, ערך).
        /// </summary>
        public List<(string, string)> Attributes { get; set; } = new List<(string, string)>();

        /// <summary>
        /// רשימת ה-classים שהוגדרו לתגית (אם קיימים).
        /// </summary>
        public List<string> Classes { get; set; } = new List<string>();

        /// <summary>
        /// התוכן הפנימי של התגית (טקסט, במידה ואין תגיות ילדים).
        /// </summary>
        public string InnerHtml { get; set; } = string.Empty;

        /// <summary>
        /// האלמנט ההורה בעץ (אם קיים).
        /// </summary>
        public HtmlElement Parent { get; set; }

        /// <summary>
        /// רשימת הילדים של האלמנט (תגיות שבתוך תגית זו).
        /// </summary>
        public List<HtmlElement> Children { get; set; } = new List<HtmlElement>();

        /// <summary>
        /// הפונקציה מחזירה את כל הצאצאים של האלמנט (מכל הדורות)
        /// באמצעות שימוש בתור (Queue), כדי למנוע ריקורסיה.
        /// </summary>
        public IEnumerable<HtmlElement> Descendants()
        {
            var queue = new Queue<HtmlElement>();
            queue.Enqueue(this);

            while (queue.Count > 0)
            {
                var element = queue.Dequeue();
                yield return element;

                foreach (var child in element.Children)
                {
                    queue.Enqueue(child);
                }
            }
        }

        /// <summary>
        /// הפונקציה מחזירה את כל האבות של האלמנט (מכל הדורות למעלה).
        /// </summary>
        public IEnumerable<HtmlElement> Ancestors()
        {
            var current = this.Parent;
            while (current != null)
            {
                yield return current;
                current = current.Parent;
            }
        }
    }
}
