using SampSharp.Entities.SAMP;

namespace OpenRP.Framework.Features.CDN.Services
{
    public interface IOpenCdnService
    {
        /// <summary>
        /// A signed link to a CDN file that works for a few minutes, or null when the CDN isn't configured.
        /// </summary>
        string? GetLink(string subDir, string filePath);

        /// <summary>
        /// Plays a CDN audio file for the player.
        /// </summary>
        void Play(Player player, string subDir, string path);

        /// <summary>
        /// Plays a CDN audio file for the player from a position, audible within the range.
        /// </summary>
        void Play(Player player, string subDir, string path, Vector3 position, float range);

        /// <summary>
        /// Plays a CDN audio file for the player, with its path given as base64.
        /// </summary>
        void PlayBase64(Player player, string subDir, string path);

        /// <summary>
        /// Plays a CDN audio file for the player from a position, with its path given as base64.
        /// </summary>
        void PlayBase64(Player player, string subDir, string path, Vector3 position, float range);
    }
}
