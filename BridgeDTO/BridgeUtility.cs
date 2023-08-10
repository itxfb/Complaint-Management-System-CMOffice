using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Text.RegularExpressions;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
//using Bridge.;
//using Bridge.Text.RegularExpressions;
using Bridge;
//using Bridge.Html5;

namespace BridgeDTO
{
    [FileName("Form.js")]
    public class BridgeUtility
    {
        //public static List<string> GetRegexStr(string str)
        //{
        //    //Regex regex = new Regex("<tag1>(.*)</tag1>");
        //    //var v = regex.Match("morenonxmldata<tag1>0002</tag1>morenonxmldata");
        //    //string s = v.Groups[1].ToString();
        //    //List<Match> li
        //    //str = "<div>first html tag</div><div>another tag</div>";
        //    List<string> listStr = new List<string>();
        //    List<int> listIndexes = new List<int>();
        //    //MatchCollection matchCol = Regex.Matches(str, @"<([^>]+)>(.*?)</\1>");
        //    //MatchCollection matchCol = Regex.Matches(str, @"(<[a-z].*?>)|(.*?>)");
        //    MatchCollection matchCol = Regex.Matches(str, @"(<[a-z])");
        //    Group prevGroup = null;
        //    foreach (Match match in matchCol)
        //    {
        //        for (int i=0; i<match.Groups.Count; i++)
        //        {
        //            if (i == 1)
        //            {
        //                Group group = match.Groups[i];
        //                listIndexes.Add(group.Captures[0].Index);
        //                //listStr.Add(str.Substring(group.Captures[0].Index, group.Captures[0].Length));
        //            }
        //        }
        //    }
        //    listIndexes.Add(str.Length);
        //    for (int i = 0; i < listIndexes.Count-1; i++)
        //    {
        //        listStr.Add(str.Substring(listIndexes[i], (listIndexes[i + 1] - listIndexes[i]))); 
        //    }
        //    //foreach (Match match in Regex.Matches(str,@"<([^>]+)>(.*?)</\1>"))
        //    //{
        //    //            Console.WriteLine("{0}={1}",
        //    //                match.Groups[1].Value,
        //    //                match.Groups[2].Value);
        //    //}



        //    //string str = "\"Was? Wo war ich? Ach ja.&lt;pa&gt;\">sdvds dsfsdfs dfdsf dsfds f\"blah blah&lt;pa&gt;\">fasf afadf dsf dsf dsf dsf dsf dsf \"blah blah blah&lt;pa&gt;\">asdasdasdadasd";

        //    return listStr;
        //}
    }
}
