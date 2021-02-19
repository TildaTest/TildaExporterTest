using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace TildaExporter
{
    public class TildaOptimizer
    {
        public static void OptimizePage(string pageData, string fullPathImagesFolder)
        {
            if (pageData == null)
                return;

            int oprimizedImgTagsCount = OptimizeImgTags(ref pageData, fullPathImagesFolder);
            Console.WriteLine("Img tags optimized! Total count: " + oprimizedImgTagsCount);

            int oprimizedScriptsCount = OptimizeScripts(ref pageData);
            Console.WriteLine("Scripts optimized! Total count: " + oprimizedScriptsCount);

            int oprimizedYoutubeVideoCount = OptimizeYoutubeVideo(ref pageData);
            Console.WriteLine("Youtube video optimized! Total count: " + oprimizedYoutubeVideoCount);


            int oprimizedLinksCount = OptimizeLinksLoadingByLazy(ref pageData);
            Console.WriteLine("Links optimized! Total count: " + oprimizedLinksCount);

        }

        // Method adding lazy load property and try to replace images for .webp format
        public static int OptimizeImgTags(ref string pageData, string fullPathImagesFolder)
        {
            int optimizedCound = 0;

            MatchCollection allImgTags =  Regex.Matches(pageData, "<img.+?src=[\"'](.+?)[\"'].*?>", RegexOptions.IgnoreCase);
            
            foreach (Match match in allImgTags)
            {
                var newImgTag = ImgLoadingLazy(match.Value);
                newImgTag = ImgToWebM(newImgTag, fullPathImagesFolder, match.Groups[1].Value);
                if(!newImgTag.Equals(match.Value))
                {
                    pageData = pageData.Replace(match.Value, newImgTag);
                    optimizedCound++;
                }
            }

            return optimizedCound;
        }

        public static string ImgLoadingLazy(string text)
        {
            if (!text.Contains("loading=\"lazy\""))
                return text.Replace("<img ", "<img loading=\"lazy\" ");
            return text;
        }

        public static string ImgToWebM(string text, string fullPathImagesFolder, string imageName)
        {
            string newFileName = "";

            if (imageName.Contains(".webp"))
                return text;

            newFileName = imageName.Replace(".png", ".webp").Replace(".jpg", ".webp");

            var filePath = fullPathImagesFolder + "\\" + newFileName.Replace("images/", "");

            if (!File.Exists(filePath))
            {
                return text;
            }

            text = text.Replace(imageName, newFileName);

            return text;
        }

        public static int OptimizeScripts(ref string pageData)
        {
            int optimizedCound = 0;

            MatchCollection allSciptsTags = Regex.Matches(pageData, "<script.+?src=[\"'](.+?)[\"'].*?</script>", RegexOptions.IgnoreCase);

            foreach (Match matchScript in allSciptsTags)
            {
                var newScriptTag = ScriptDefer(matchScript.Value);

                if (!newScriptTag.Equals(matchScript.Value))
                {
                    pageData = pageData.Remove(matchScript.Index, matchScript.Length).Insert(matchScript.Index, newScriptTag);
                    optimizedCound++;
                }
            }

            return optimizedCound;
        }

        public static string ScriptDefer(string text)
        {
            if (CheckScriptHasNoException(text) && !text.Contains("<script defer src"))
                return text.Replace("<script src", "<script defer src");
            return text;
        }

        public static bool CheckScriptHasNoException(string text)
        {
            if (text.Contains("jquery"))
                return false;
            return true;
        }

        // TODO replace by iframe with lazy load
        public static int OptimizeYoutubeVideo(ref string pageData)
        {
            int optimizedCound = 0;

            MatchCollection allImgTags = Regex.Matches(pageData, "<div id=\"rec225630961\".+?src=[\"'](.+?)[\"'].*?</div>", RegexOptions.IgnoreCase);

            foreach (Match match in allImgTags)
            {
                
            }

            return optimizedCound;
        }

        public static int OptimizeLinksLoadingByLazy(ref string pageData)
        {
            int optimizedCound = 0;

            MatchCollection allLinksTags = Regex.Matches(pageData, "<link rel=\"stylesheet\".+?href=[\"'](.+?)[\"'].*?>", RegexOptions.IgnoreCase);

            foreach (Match match in allLinksTags)
            {
                var linkDefault = "<link rel = \"stylesheet\" href = \""+ match.Groups[1].Value + "\" type = \"text/css\" media = \"print\" onload = \"this.media='all'\">";

                pageData = pageData.Replace(match.Value, linkDefault);

                optimizedCound++;
            }

            return optimizedCound;
        }

    }
}
