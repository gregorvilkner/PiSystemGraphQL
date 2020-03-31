using Microsoft.Owin.Hosting;
using PiGraphNetCoreServer.GraphQl;
using System;
using System.Configuration;
using System.Diagnostics;

namespace PiGraphQlOwinServer
{
    class Program
    {
        static void Main(string[] args)
        {


            //https://medium.com/@kirkbackus/owin-access-is-denied-errors-fd9d370aa8d3
            //string baseUrl = "http://*:9090"; // http webserver running on port 9090
            //string baseUrl = "https://*:9091/"; // https webserver running on port 9091
            
            string baseUrl = ConfigurationManager.AppSettings["baseUrl"];

            using (WebApp.Start<Startup>(baseUrl))
            {
                Console.WriteLine(baseUrl);
                Process.Start(baseUrl); // open the page from the application
                Console.WriteLine("Press Enter to quit.");
                Console.ReadKey();
            }

        }
    }
}
