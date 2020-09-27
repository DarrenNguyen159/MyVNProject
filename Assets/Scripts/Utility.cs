using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace DN
{
    public class Utility
    {
        public static string Str2Key(string str)
        {
            return "\"" + str + "\":";
        }
    }

    public class JSONTemplate : Dictionary<string, object>
    {
        public JSONTemplate()
        {

        }

        public JSONTemplate AddKeyValue(string key, object value)
        {
            this.Add(key, value);
            return this;
        }

        public JSONTemplate AddChildJSON(string key, JSONTemplate child)
        {
            this.Add(key, child);
            return this;
        }

        public string GetJSON()
        {
            string str = "{";
            int count = 0;
            int total = this.Count;
            foreach (KeyValuePair<string, object> entry in this)
            {
                str += "\"" + entry.Key + "\":";

                Debug.Log("" + entry.Value.GetType().Name + " == " + typeof(JSONTemplate).Name);

                if (entry.Value.GetType().Name == typeof(JSONTemplate).Name)
                {
                    // child object
                    str += ((JSONTemplate)(entry.Value)).GetJSON();
                }
                else if (entry.Value is string)
                {
                    str += "\"" + entry.Value + "\"";
                }
                else
                {
                    str += "" + entry.Value;
                }
                if (count < total - 1)
                {
                    str += ",";
                }
                count += 1;
            }
            str += "}";
            return str;
        }
    }

    public class JSONParserTemplate
    {
        private string str;
        private List<string> entries;

        public Dictionary<string, string> map;

        public JSONParserTemplate(string str)
        {
            this.str = str;
            entries = new List<string>();
            map = new Dictionary<string, string>();
        }

        public void Parse()
        {
            string s = "" + this.str;

            s = s.Trim();

            s = s.Remove(0, s.IndexOf("{", System.StringComparison.Ordinal) + 1);
            s = s.Remove(s.LastIndexOf("}", System.StringComparison.Ordinal));

            Regex regex = new Regex(@"(""[\w]+"":)((""[^""]*"")|((-)?[\d]+((.)[\d]+)?)|({[""\s\S]+}))");
            MatchCollection matchCollection = regex.Matches(s);

            // tách các entries
            foreach (Match match in matchCollection)
            {
                if (match.Value.Length > 0)
                {
                    entries.Add(match.Value);
                }
            }

            // từng entries tách thành key và value
            foreach (string entry in entries)
            {
                string key = entry.Substring(1, entry.IndexOf(":", System.StringComparison.Ordinal) - 2);
                string value = entry.Substring(entry.IndexOf(":", System.StringComparison.Ordinal) + 1, entry.Length - entry.IndexOf(":", System.StringComparison.Ordinal) - 1);
                map.Add(key, value);
            }
        }

        public string GetString(string key)
        {
            string ret;
            map.TryGetValue(key, out ret);
            int startIndex = ret.IndexOf('"');
            if (startIndex != -1)
            {
                ret = ret.Remove(startIndex, 1);
                int lastIndex = ret.LastIndexOf('"');
                ret = ret.Remove(lastIndex, 1);
            }
            return ret;
        }
    }
}
