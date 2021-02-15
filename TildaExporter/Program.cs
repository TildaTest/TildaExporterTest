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
        static string baseUrl = "http://api.tildacdn.info";
        static readonly string publicKey = "olslwsjnhote1szo4r3o";
        static readonly string secretKey = "k7a91yy3lpo1g03ts1gq";
        static readonly string projectid = "1267295";

        static readonly string projectFolderFullPath = "D:\\Work\\TildaFreshcode1\\";
        static readonly string imagesFolder = "images";
        static readonly string jsFolder = "js";
        static readonly string cssFolder = "css";

        static List<PageShort> pagesList = null;
        static TildaProject project = null;


        static void Main(string[] args)
        {
            CreateFolders();

            Console.WriteLine("Base project information...");
            project = GetProjectExport();

            Console.WriteLine();
            Console.WriteLine("Printing all pages...");

            pagesList = GetPagesListRequest();

            DownloadAndSafeAllPages();

            DownloadAndSafeFiles(project.CssFiles, cssFolder);
            Console.WriteLine("Css files was saved!");

            DownloadAndSafeFiles(project.JsFiles, jsFolder);
            Console.WriteLine("Js files was saved!");

            DownloadAndSafeFiles(project.Images, imagesFolder);
            Console.WriteLine("Images was saved!");

            // Wait for closing programm by user
            Console.WriteLine();
            Console.WriteLine("Press enter for exit from programm...");
            Console.ReadLine();
        }

        static string GetUrl(string methodName)
        {
            return baseUrl + "/v1/" + methodName + "/?publickey=" + publicKey + "&secretkey=" + secretKey + "&projectid=" + projectid;
        }

        static void CreateFolders()
        {
            Directory.CreateDirectory(projectFolderFullPath);
            Directory.CreateDirectory(projectFolderFullPath + imagesFolder);
            Directory.CreateDirectory(projectFolderFullPath + jsFolder);
            Directory.CreateDirectory(projectFolderFullPath + cssFolder);

            Console.WriteLine("Folders created!");
            Console.WriteLine();
        }

        static TildaProject GetProjectExport()
        {
            string url = GetUrl("getprojectexport");

            //Create a query
            HttpClient client = new HttpClient();
            var result = client.GetAsync(url).Result;


            using (StreamReader sr = new StreamReader(result.Content.ReadAsStreamAsync().Result))
            {
                Console.WriteLine();
                var jsonText = sr.ReadToEnd();

                Console.WriteLine(jsonText);

                var response = JsonConvert.DeserializeObject<TildaResult<TildaProject>>(jsonText);

                Console.WriteLine("page id: " + response.result.Id);
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

        static void DownloadAndSafeAllPages()
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

                        Console.WriteLine(response.result.Images);

                        try
                        {
                            DownloadAndSafeFiles(response.result.Images, imagesFolder);
                        }
                        catch (Exception ex) { }

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
            return savedFilesCount;
        }

    }
}
