using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.CDN.Services
{
    public interface IOpenCdnService
    {
        string GetLink(string subDir, string filePath);
    }
}
