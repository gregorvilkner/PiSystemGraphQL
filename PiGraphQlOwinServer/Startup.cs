using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using Owin;
using System.IO;
using System.Web.Http;

namespace PiGraphQlOwinServer
{
    // reference on self-hosted OWIN https://braincadet.com/category/c-sharp/
    class Startup
    {

        public void Configuration(IAppBuilder app)
        {


            app.Map("/api", builder =>
            {
                HttpConfiguration config = new HttpConfiguration();
                config.MapHttpAttributeRoutes();
                app.UseWebApi(config);
            });


            if (!Directory.Exists("wwwroot")) Directory.CreateDirectory("wwwroot");

            var physicalFileSystem = new PhysicalFileSystem("./wwwroot");

            // file server options
            var options = new FileServerOptions
            {
                EnableDefaultFiles = true,
                FileSystem = physicalFileSystem, // register file system
                EnableDirectoryBrowsing = false
            };

            options.StaticFileOptions.FileSystem = physicalFileSystem;
            options.StaticFileOptions.ServeUnknownFileTypes = true;
            options.DefaultFilesOptions.DefaultFileNames = new[] { "index.html" };
            app.Use<DefaultFileRewriterMiddleware>(physicalFileSystem);  // middleware to direct non-existing file URLs to index.html
            app.UseFileServer(options);


        }
    }
}
