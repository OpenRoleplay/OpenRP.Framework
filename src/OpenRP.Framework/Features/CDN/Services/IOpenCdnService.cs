using SampSharp.Entities.SAMP;
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
        void PlayBase64(Player player, string subDir, string path);
        void PlayBase64(Player player, string subDir, string path, Vector3 position, float range);
    }
}
