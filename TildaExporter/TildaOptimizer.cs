using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace TildaExporter
{
    public class TildaOptimizer
    {
        public static void OptimizePage(string pageData)
        {
            if (pageData == null)
                return;

            OptimizeImgTags(pageData);



            Console.ReadLine();
        }

        public static void OptimizeImgTags(string pageData)
        {
            MatchCollection allImgTags =  Regex.Matches(pageData, "<img.+?src=[\"'](.+?)[\"'].*?>", RegexOptions.IgnoreCase);
            
            foreach (Match match in allImgTags)
            {
                //match.Value
            }
        }
    }
}
