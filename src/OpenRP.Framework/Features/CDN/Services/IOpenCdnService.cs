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
        string? GetLink(string subDir, string filePath);

        /// <summary>
        /// Plays a CDN audio file for the player.
        /// </summary>
        void Play(Player player, string subDir, string path);

        /// <summary>
        /// Plays a CDN audio file for the player from a position, audible within the range.
        /// </summary>
        void Play(Player player, string subDir, string path, Vector3 position, float range);

        void PlayBase64(Player player, string subDir, string path);
        void PlayBase64(Player player, string subDir, string path, Vector3 position, float range);
    }
}
