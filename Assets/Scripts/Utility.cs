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
        public Dictionary<string, string> map;

        public JSONParserTemplate(string str)
        {
            this.str = str;
            map = new Dictionary<string, string>();

            Parse();
        }

        public override string ToString()
        {
            return str;
        }

        public void Parse()
        {
            string s = "" + this.str;

            s = Regex.Replace(s, "(\"(?:[^\"\\\\]|\\\\.)*\")|\\s+", "$1");

            // bỏ ngoặc đầu cuối
            if (s.Length == 0) {
                return;
            }
            s = s.Remove(0, s.IndexOf("{", System.StringComparison.Ordinal) + 1);
            s = s.Remove(s.LastIndexOf("}", System.StringComparison.Ordinal));

            // tách các entries
            while (true)
            {
                if (s.IndexOf(":") == -1)
                {
                    break;
                }
                if (s[0] == ',')
                {
                    s = s.Substring(1);
                }

                string key = s.Substring(0, s.IndexOf(":")); // còn dấu ""
                s = s.Remove(0, key.Length + 1); // bỏ từ đầu cho tới :

                // bỏ dấu ""
                key = key.Remove(0, key.IndexOf('"') + 1);
                key = key.Remove(key.LastIndexOf('"'));

                if (s[0] == '{')
                {// Ngay sau : là { thì đây là JSON con
                    int semaphore = 0;
                    int closeBracketIndex = 0;
                    for (int i = 0; i < s.Length; i++)
                    {
                        if (s[i] == '{')
                        {
                            semaphore += 1;
                        }
                        else if (s[i] == '}')
                        {
                            semaphore -= 1;
                            if (semaphore == 0)
                            {// tìm được dấu đóng ngoặc
                                closeBracketIndex = i;
                                break;
                            }
                        }
                    }
                    string value = s.Substring(0, closeBracketIndex + 1);
                    s = s.Remove(0, value.Length);
                    map.Add(key, value);
                }
                else
                {// Đây là value bình thường
                    if (s.IndexOf(",") != -1)
                    {
                        string value = s.Substring(0, s.IndexOf(","));
                        s = s.Substring(value.Length + 1);
                        map.Add(key, value);
                    }
                    else
                    {
                        string value = s;
                        map.Add(key, value);
                    }
                }
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

        public string GetChildJSONString(string key)
        {
            string ret;
            map.TryGetValue(key, out ret);
            return ret;
        }

        public int GetInt(string key)
        {
            string ret;
            map.TryGetValue(key, out ret);
            if (ret == null)
            {
                return -1;
            }
            else
            {
                return System.Int32.Parse(ret);
            }
        }
    }
}
