using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using System.Web.Script.Serialization;

namespace TrueCryptCrack
{
    internal static class JsonHelpers
    {
        public static dynamic DeserializeJson(string data)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();

            if (data[0] == '[')
            {
                IEnumerable<object> list = serializer.Deserialize<IEnumerable<object>>(data);
                return list;
            }

            IDictionary<string, object> dictionary = serializer.Deserialize<IDictionary<string, object>>(data);
            return DictionaryToExpando(dictionary);
        }

        public static ExpandoObject DictionaryToExpando(this IDictionary<string, object> dictionary)
        {
            var expando = new ExpandoObject();
            var expandoDic = (IDictionary<string, object>)expando;

            // go through the items in the dictionary and copy over the key value pairs)
            foreach (var kvp in dictionary)
            {
                // if the value can also be turned into an ExpandoObject, then do it!
                if (kvp.Value is IDictionary<string, object>)
                {
                    var expandoValue = DictionaryToExpando((IDictionary<string, object>)kvp.Value);
                    expandoDic.Add(kvp.Key, expandoValue);
                }
                else if (kvp.Value is ICollection<object>)
                {
                    // iterate through the collection and convert any string-object dictionaries
                    // along the way into expando objects
                    var itemList = new List<object>();
                    foreach (var item in (ICollection<object>)kvp.Value)
                    {
                        if (item is IDictionary<string, object>)
                        {
                            var expandoItem = DictionaryToExpando((IDictionary<string, object>)item);
                            itemList.Add(expandoItem);
                        }
                        else
                        {
                            itemList.Add(item);
                        }
                    }
                    expandoDic.Add(kvp.Key, itemList);
                }
                else
                {
                    expandoDic.Add(kvp);
                }
            }

            return expando;
        }

        public static string ExpandoToJson(ExpandoObject expando)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            StringBuilder json = new StringBuilder();
            List<string> keyPairs = new List<string>();
            IDictionary<string, object> dictionary = expando as IDictionary<string, object>;
            json.Append("{");

            foreach (KeyValuePair<string, object> pair in dictionary)
            {
                if (pair.Value is ExpandoObject)
                {
                    keyPairs.Add(String.Format(@"""{0}"": {1}", pair.Key, ExpandoToJson((pair.Value as ExpandoObject))));
                }
                else
                {
                    keyPairs.Add(String.Format(@"""{0}"": {1}", pair.Key, serializer.Serialize(pair.Value)));
                }
            }

            json.Append(String.Join(",", keyPairs.ToArray()));
            json.Append("}");

            return json.ToString();
        }
    }
}
