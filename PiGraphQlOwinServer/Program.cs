﻿using Microsoft.Owin.Hosting;
using PiGraphNetCoreServer.GraphQl;
using System;
using System.Diagnostics;

namespace PiGraphQlOwinServer
{
    class Program
    {
        static void Main(string[] args)
        {

            //https://medium.com/@kirkbackus/owin-access-is-denied-errors-fd9d370aa8d3
            //string baseUrl = "https://*:9090"; // webserver running on port 9090
            string baseUrl = "https://*:9091/"; // webserver running on port 9090
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
