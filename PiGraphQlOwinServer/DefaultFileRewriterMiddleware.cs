using Microsoft.Owin;
using Microsoft.Owin.FileSystems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiGraphQlOwinServer
{
    class DefaultFileRewriterMiddleware : OwinMiddleware
    {
        private readonly IFileSystem _fileSystem;

        public DefaultFileRewriterMiddleware(OwinMiddleware next, IFileSystem fileSystem)
            : base(next)
        {
            _fileSystem = fileSystem;
        }

        public override async Task Invoke(IOwinContext context)
        {
            IFileInfo fileInfo;

            if (!_fileSystem.TryGetFileInfo(context.Request.Path.Value, out fileInfo))
            {
                context.Request.Path = new PathString("/index.html");
            }

            await Next.Invoke(context);
        }

    }
}
