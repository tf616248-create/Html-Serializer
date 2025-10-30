using System;
using System.IO;
using System.Text.Json;

namespace HtmlTool
{
    /// <summary>
    /// מחלקת עזר שמחזיקה את רשימות תגיות ה-HTML.
    /// נטענת מקובצי JSON וממומשת לפי תבנית Singleton.
    /// </summary>
    public class HtmlHelper
    {
        /// <summary>
        /// רשימת כל תגיות ה-HTML הקיימות.
        /// </summary>
        public string[] AllTags { get; private set; }

        /// <summary>
        /// רשימת תגיות שלא דורשות תגית סגירה.
        /// </summary>
        public string[] SelfClosingTags { get; private set; }

        /// <summary>
        /// מופע יחיד של המחלקה (Singleton).
        /// </summary>
        private static HtmlHelper _instance;

        /// <summary>
        /// גישה למופע היחיד של HtmlHelper.
        /// </summary>
        public static HtmlHelper Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new HtmlHelper();
                return _instance;
            }
        }

        /// <summary>
        /// בנאי פרטי שטוען את הנתונים מקבצי JSON.
        /// </summary>
        private HtmlHelper()
        {
            // קריאת הקבצים (בהנחה שהם נמצאים בתיקיית output של הפרויקט)
            string allTagsPath = Path.Combine(AppContext.BaseDirectory, "HtmlTags.json");
            string selfClosingPath = Path.Combine(AppContext.BaseDirectory, "HtmlVoidTags.json");

            if (File.Exists(allTagsPath))
            {
                string json = File.ReadAllText(allTagsPath);
                AllTags = JsonSerializer.Deserialize<string[]>(json);
            }
            else
            {
                AllTags = Array.Empty<string>();
            }

            if (File.Exists(selfClosingPath))
            {
                string json = File.ReadAllText(selfClosingPath);
                SelfClosingTags = JsonSerializer.Deserialize<string[]>(json);
            }
            else
            {
                SelfClosingTags = Array.Empty<string>();
            }
        }
    }
}
