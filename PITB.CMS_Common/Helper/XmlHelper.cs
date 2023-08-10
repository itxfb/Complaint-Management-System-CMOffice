using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using AutoMapper.Internal;
using PITB.CMS_Common.Helper;

/// <summary>
/// Extension methods for the dynamic object.
/// </summary>
public static class XmlHelper
{
    /// <summary>
    /// Defines the simple types that is directly writeable to XML.
    /// </summary>
    private static readonly Type[] _writeTypes = new[] { typeof(string), typeof(DateTime), typeof(Enum), typeof(decimal), typeof(Guid) };

    /// <summary>
    /// Determines whether [is simple type] [the specified type].
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns>
    /// 	<c>true</c> if [is simple type] [the specified type]; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsSimpleType(this Type type)
    {
        return type.IsPrimitive || _writeTypes.Contains(type);
    }

    /// <summary>
    /// Converts the specified dynamic object to XML.
    /// </summary>
    /// <param name="dynamicObject">The dynamic object.</param>
    /// <returns>Returns an Xml representation of the dynamic object.</returns>
    public static XElement ConvertToXml(dynamic dynamicObject)
    {
        return ConvertToXml(dynamicObject, null);
    }

    /// <summary>
    /// Converts the specified dynamic object to XML.
    /// </summary>
    /// <param name="dynamicObject">The dynamic object.</param>
    /// /// <param name="element">The element name.</param>
    /// <returns>Returns an Xml representation of the dynamic object.</returns>
    public static XElement ConvertToXml(dynamic dynamicObject, string element)
    {
        if (String.IsNullOrWhiteSpace(element))
        {
            element = "object";
        }

        element = XmlConvert.EncodeName(element);
        var ret = new XElement(element);

        Dictionary<string, object> members = new Dictionary<string, object>(dynamicObject);

        var elements = from prop in members
                       let name = XmlConvert.EncodeName(prop.Key)
                       let val = prop.Value.GetType().IsArray ? "array" : prop.Value
                       let value = prop.Value.GetType().IsArray ? GetArrayElement(prop.Key, (Array)prop.Value) 
                       : (prop.Value.GetType().IsSimpleType() ?  new XElement(name, val,  new XAttribute("Server", "Munnay")) 
                       : val.ToXml(name))
                       where value != null
                       select value;

        ret.Add(elements);

        return ret;
    }

    public static string ConvertToXmlString(this Dynamic dynamicObject, string element)
    {
        //XElement xEl = ConvertToXml(dynamicObject, element);
        //XElement xml = XmlHelper.ConvertToXml(dynamicObject, element);
        IList<XElement> listXElements = new List<XElement>();
        List<XElement> xml = (List<XElement>)GetXElement(listXElements,dynamicObject);
        return xml.ToString();
    }


    public static XElement ConvertToXml(this Dynamic dynamicObject, string element)
    {
        if (String.IsNullOrWhiteSpace(element))
        {
            element = "object";
        }

        element = XmlConvert.EncodeName(element);
        
        var ret = new XElement(element);
        //var ret2 = GetXElement(dynamicObject.dictionary);
        Dictionary<string, object> members = new Dictionary<string, object>(dynamicObject.dictionary);

        foreach (KeyValuePair<string, object> prop in members)
        {
            string name = XmlConvert.EncodeName(prop.Key);
            object val = prop.Value.GetType().IsArray ? "array" : prop.Value;

            XElement xEl = null;
            if (prop.Value.GetType().IsArray)
            {
               xEl = GetArrayElement(prop.Key, (Array) prop.Value);

            }
            else 
            {
                if (prop.Value.GetType().IsSimpleType())
                {
                    xEl = new XElement(name, val, new XAttribute("Server", "Munnay"));
                }
                else if (prop.Value.GetType().IsDictionaryType())
                {
                    Dictionary<string, object> dict = (Dictionary<string, object>)prop.Value;
                    XElement el = new XElement("root",dict.Select(kv => new XElement(kv.Key, kv.Value)));
                }
                else
                {
                    xEl = val.ToXml(name);
                }
            }
            
            ret.Add(xEl);
        }


        return ret;
    }

    //public static XElement ToXML(this Dictionary<string, object> dic, string firstNode)
    //{
    //    IList<XElement> xElements = new List<XElement>();

    //    foreach (var item in dic)
    //        xElements.Add(new XElement(item.Key, GetXElement(item.Value)));

    //    XElement root = new XElement(firstNode, xElements.ToArray());

    //    return root;
    //}

    private static object GetXElement(IList<XElement> listXElements, object item, string name = null)
    {
        
        Type itemType = null;//item.GetType();
        List<PropertyInfo> listPropertyInfo = null; //itemType.GetProperties().ToList();
        if (item != null)
        {
            if (item is Dynamic)
            {
                item = (item as Dynamic).dictionary;
            }
            itemType = item.GetType();

            
            
            if ( item.GetType() == typeof (Dictionary<string, object>)) // if dictionary
            {
                //IList<XElement> xElements = new List<XElement>();
                foreach (var item2 in item as Dictionary<string, object>)
                {
                    listXElements.Add( new XElement(item2.Key,  GetXElement(listXElements,item2.Value)));
                }

                //return listXElements.ToArray();
            }
            if (itemType.IsSimpleType())
            {
                return item;
                PropertyInfo propertyInfo = itemType.GetProperty(name);
                listXElements.Add(new XElement(propertyInfo.Name, propertyInfo.GetValue(item, null), new XAttribute("Server", "Munnay")));
            }
            //else if (item.GetType().IsEnumerableType()) // if list
            //{
            //    //IList<XElement> xElements = new List<XElement>();
            //    //List<object> listObject = 
            //    if (item is Dictionary<string, object>)
            //    {
            //        foreach (Dictionary<string, object> dict in item as List<Dictionary<string, object>>)
            //        {
            //            GetXElement(listXElements, dict);
            //        }
            //    }
            //    else 
            //    {
            //        //Type myType = item.GetType();
            //        //IList<PropertyInfo> props = new List<PropertyInfo>(itemType.GetProperties());
                    
            //        foreach (var obj in (item as IEnumerable))
            //        {
            //            Type objType = obj.GetType();
            //            listPropertyInfo = objType.GetProperties().ToList();
            //            foreach (PropertyInfo prop in listPropertyInfo)
            //            {
            //                object propValue = prop.GetValue(item, null);
            //                listXElements.Add(new XElement(prop.Name, GetXElement(listXElements, propValue, prop.Name)));
            //                // Do something with propValue
            //            }
            //        }
            //    }
                

            //    //return xElements.ToArray();
            //}

           
        }
        return listXElements;

        //return item;
    }



    /// <summary>
    /// Generates an XML string from the dynamic object.
    /// </summary>
    /// <param name="dynamicObject">The dynamic object.</param>
    /// <returns>Returns an XML string.</returns>
    public static string ToXmlString(dynamic dynamicObject)
    {
        XElement xml = XmlHelper.ConvertToXml(dynamicObject);

        return xml.ToString();
    }

    public static string ToXmlString(dynamic dynamicObject, string rootName)
    {
        XElement xml = XmlHelper.ConvertToXml(dynamicObject, rootName);

        return xml.ToString();
    }

    /// <summary>
    /// Converts an anonymous type to an XElement.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <returns>Returns the object as it's XML representation in an XElement.</returns>
    public static XElement ToXml(this object input)
    {
        return input.ToXml(null);
    }

    /// <summary>
    /// Converts an anonymous type to an XElement.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <param name="element">The element name.</param>
    /// <returns>Returns the object as it's XML representation in an XElement.</returns>
    public static XElement ToXml(this object input, string element)
    {
        if (input == null)
        {
            return null;
        }

        if (String.IsNullOrWhiteSpace(element))
        {
            element = "object";
        }

        element = XmlConvert.EncodeName(element);
        var ret = new XElement(element);

        if (input != null)
        {
            var type = input.GetType();
            var props = type.GetProperties();

            object elements = from prop in props
                           let name = XmlConvert.EncodeName(prop.Name)
                           let val = prop.PropertyType.IsArray ? "array" : prop.GetValue(input, null)
                           let value = prop.PropertyType.IsArray ? GetArrayElement(prop, (Array)prop.GetValue(input, null)) : (prop.PropertyType.IsSimpleType() ? new XElement(name, val) : val.ToXml(name))
                           where value != null
                           select value;

            ret.Add(elements);
        }

        return ret;
    }

    /// <summary>
    /// Parses the specified XML string to a dynamic.
    /// </summary>
    /// <param name="xmlString">The XML string.</param>
    /// <returns>A dynamic object.</returns>
    public static dynamic ParseDynamic(this string xmlString)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Gets the array element.
    /// </summary>
    /// <param name="info">The property info.</param>
    /// <param name="input">The input object.</param>
    /// <returns>Returns an XElement with the array collection as child elements.</returns>
    private static XElement GetArrayElement(PropertyInfo info, Array input)
    {
        return GetArrayElement(info.Name, input);
    }

    /// <summary>
    /// Gets the array element.
    /// </summary>
    /// <param name="propertyName">The property name.</param>
    /// <param name="input">The input object.</param>
    /// <returns>Returns an XElement with the array collection as child elements.</returns>
    private static XElement GetArrayElement(string propertyName, Array input)
    {
        var name = XmlConvert.EncodeName(propertyName);

        XElement rootElement = new XElement(name);

        var arrayCount = input.GetLength(0);

        for (int i = 0; i < arrayCount; i++)
        {
            var val = input.GetValue(i);
            XElement childElement = val.GetType().IsSimpleType() ? new XElement(name + "Child", val) : val.ToXml();

            rootElement.Add(childElement);
        }

        return rootElement;
    }
}