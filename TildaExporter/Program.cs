using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using TildaExporter.Data;

namespace TildaExporter
{
    class Program
    {
        static readonly string tildaBaseUrl = "http://api.tildacdn.info";
        static readonly string publicKey = "olslwsjnhote1szo4r3o";
        static readonly string secretKey = "k7a91yy3lpo1g03ts1gq";
        static readonly string projectid = "1267295";

        static readonly string projectFolderFullPath = "D:\\Work\\TildaFreshcode\\";
        static readonly string imagesFolderName = "images";
        static readonly string jsFolderName = "js";
        static readonly string cssFolderName = "css";

        static void Main(string[] args)
        {
            Console.WriteLine("Press Enter to start exporting site!");
            Console.ReadLine();
            ExportSite();
        }

        static string GetUrl(string methodName)
        {
            return tildaBaseUrl + "/v1/" + methodName + "/?publickey=" + publicKey + "&secretkey=" + secretKey + "&projectid=" + projectid;
        }

        static void ExportSite()
        {
            CreateFolders();
            Console.WriteLine("Folders was created...");

            Console.WriteLine("Getting base project information...");

            TildaProject project = GetProjectExport();
            List<PageShort> pagesList = GetPagesListRequest();

            CreateHtaccess(project.Htaccess);

            Console.WriteLine();
            Console.WriteLine("Start saving pages...");

            DownloadAndSafeAllPages(pagesList);

            Console.WriteLine();
            Console.WriteLine("Start saving css files...");
            var cssFilesCount = DownloadAndSafeFiles(project.CssFiles, cssFolderName);
            Console.WriteLine("Css files was saved! Total count: " + cssFilesCount);

            Console.WriteLine();
            Console.WriteLine("Start saving js files...");
            var jsFilesCount = DownloadAndSafeFiles(project.JsFiles, jsFolderName);
            Console.WriteLine("Js files was saved!! Total count: " + jsFilesCount);

            Console.WriteLine();
            Console.WriteLine("Start saving base images...");
            var baseImagesCount = DownloadAndSafeFiles(project.Images, imagesFolderName);
            Console.WriteLine("Base images was saved!! Total count: " + baseImagesCount);

            // Wait for closing programm by user
            Console.WriteLine();
            Console.WriteLine("Press enter for exit from programm...");
            Console.ReadLine();
        }

        static void CreateHtaccess(string htaccessData)
        {
            File.WriteAllText(projectFolderFullPath + ".htaccess", htaccessData);
            Console.WriteLine("Htaccess was created...");
        }

        static void CreateFolders()
        {
            Directory.CreateDirectory(projectFolderFullPath);
            Directory.CreateDirectory(projectFolderFullPath + imagesFolderName);
            Directory.CreateDirectory(projectFolderFullPath + jsFolderName);
            Directory.CreateDirectory(projectFolderFullPath + cssFolderName);

            Console.WriteLine("Folders created!");
        }

        static TildaProject GetProjectExport()
        {
            string url = GetUrl("getprojectexport");

            HttpClient client = new HttpClient();
            var result = client.GetAsync(url).Result;

            using (StreamReader sr = new StreamReader(result.Content.ReadAsStreamAsync().Result))
            {
                var jsonText = sr.ReadToEnd();

                var response = JsonConvert.DeserializeObject<TildaResult<TildaProject>>(jsonText);

                Console.WriteLine("Project id: " + response.result.Id);
                Console.WriteLine("With CustomDomain: " + response.result.Customdomain);

                return response.result;
            }
        }

        static List<PageShort> GetPagesListRequest()
        {
            string url = GetUrl("getpageslist");

            HttpClient client = new HttpClient();
            var result = client.GetAsync(url).Result;
            
            using (StreamReader sr = new StreamReader(result.Content.ReadAsStreamAsync().Result))
            {
                Console.WriteLine();
                var jsonText = sr.ReadToEnd();

                var response = JsonConvert.DeserializeObject<TildaResult<List<PageShort>>>(jsonText);

                foreach (var d in response.result)
                {
                    Console.WriteLine("id: " + d.Id + " fileName: " + d.Filename);
                }

                return response.result;
            }
        }

        static void DownloadAndSafeAllPages(List<PageShort> pagesList)
        {
            string url = GetUrl("getpagefullexport");

            foreach(var page in pagesList)
            {
                HttpClient client = new HttpClient();
                var result = client.GetAsync(url + "&pageid=" + page.Id).Result;

                using (StreamReader sr = new StreamReader(result.Content.ReadAsStreamAsync().Result))
                {
                    Console.WriteLine();
                    var jsonText = sr.ReadToEnd();

                    var response = JsonConvert.DeserializeObject<TildaResult<PageFull>>(jsonText);

                    if(response.status.Equals("FOUND"))
                    {
                        File.WriteAllText(projectFolderFullPath + response.result.Filename, response.result.Html);

                        TildaOptimizer.OptimizePage(response.result.Html, projectFolderFullPath + imagesFolderName);


                        DownloadAndSafeFiles(response.result.Images, imagesFolderName);

                        Console.WriteLine("file with id: " + response.result.Id + " was successfuly saved");
                    }
                    
                }
            }
        }

        static int DownloadAndSafeFiles(List<FileExportData> filesToSave, string folderToSave)
        {
            int savedFilesCount = 0;
            foreach (var file in filesToSave)
            {
                try
                {
                    HttpClient client = new HttpClient();
                    var result = client.GetAsync(file.From).Result;

                    using (StreamReader sr = new StreamReader(result.Content.ReadAsStreamAsync().Result))
                    {
                        Console.WriteLine();
                        var text = sr.ReadToEnd();

                        File.WriteAllText(projectFolderFullPath + folderToSave + "\\" + file.To, text);

                        Console.WriteLine("file with name: " + file.To + " was successfuly saved");
                        savedFilesCount++;
                    }
                }
                catch (Exception ex) 
                {
                    Console.Error.Write("Saving file error! Message: " + ex.Message);
                }
            }
            return savedFilesCount;
        }

    }
}
