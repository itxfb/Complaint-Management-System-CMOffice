using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PITB.CMS_Common.Handler.CustomJsonConverter
{
    public class DynamicJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return true;
            //throw new NotImplementedException();
        }

        private dynamic AddNewDynamic(dynamic d, string key)
        {
            dynamic dictToAdd = new ExpandoObject();
            IDictionary<string, object> dict = (IDictionary<string, object>)d;
            dict.Add(key, dictToAdd);
            //dict.Add(key, value);
            return dictToAdd;
        }

        private dynamic AddDynamicValue(dynamic d, string key, object value)
        {
            IDictionary<string, object> dict = (IDictionary<string, object>)d;
            dict.Add(key, value);
            return dict;
            //return dict[key];
        }

        private dynamic PopulateDynamicRecursive(JToken jToken, dynamic d)
        {
            if (jToken.Type == JTokenType.Object)
            {
                //AddDynamicValue(d, jprop.Name, jprop.Value);
                dynamic dTemp = new ExpandoObject();
                dynamic dToRet = null;
                foreach (JProperty jprop in (JToken)jToken)
                {
                    dToRet = PopulateDynamicRecursive(jprop, dTemp);
                }
                return dToRet;
            }
            else if (jToken.Type == JTokenType.Property)
            {
                return AddDynamicValue(d, ((JProperty)jToken).Name, PopulateDynamicRecursive(((JProperty)jToken).Value, d));
            }
            else if (jToken.Type == JTokenType.Array)
            {
                List<JToken> listJToken = jToken.Values<JToken>().ToList();
                if (listJToken[0].Type == JTokenType.Integer)
                {
                    return listJToken.Select(n => n.Value<int>()).ToList();
                }
                else if (listJToken[0].Type == JTokenType.String)
                {
                    return listJToken.Select(n => n.Value<string>()).ToList();
                }
                else if (listJToken[0].Type == JTokenType.Boolean)
                {
                    return listJToken.Select(n => n.Value<bool>()).ToList();
                }
                else if(listJToken[0].Type == JTokenType.Object)
                {
                    List<dynamic> listD = new List<dynamic>();
                    foreach (JToken jTokenSingle in listJToken)
                    {
                        listD.Add(PopulateDynamicRecursive(jTokenSingle, null)); 
                        //return GetList1(jTokenSingle, d);
                    }
                    return listD;
                }
            }
            else
            {
                //return d;
                if (jToken.Type == JTokenType.Integer)
                {
                    return jToken.Value<int>();
                }
                else if (jToken.Type == JTokenType.String)
                {
                    return jToken.Value<string>();
                }
                else if (jToken.Type == JTokenType.Boolean)
                {
                    return jToken.Value<bool>();
                }
            }
            return d;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jsonObject = JObject.Load(reader);
            return PopulateDynamicRecursive(jsonObject, new ExpandoObject());
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            //base.WriteJson(writer, value, serializer);
            throw new NotImplementedException();
        }
    }
}
