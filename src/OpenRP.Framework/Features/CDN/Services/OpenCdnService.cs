using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenRP.Framework.Features.CDN.Entities;
using SampSharp.Entities.SAMP;
using System.Security.Cryptography;
using System.Text;

namespace OpenRP.Framework.Features.CDN.Services
{
    public class OpenCdnService : IOpenCdnService
    {
        // Long enough for clock drift and a slow start; the CDN only checks it when the download begins,
        // so a longer file keeps playing. Short, so a copied link is soon useless.
        private static readonly TimeSpan LinkLifetime = TimeSpan.FromMinutes(10);

        private readonly OpenCdnOptions _options;
        private readonly ILogger<OpenCdnService> _logger;

        public OpenCdnService(IOptions<OpenCdnOptions> options, ILogger<OpenCdnService> logger)
        {
            _options = options.Value;
            _logger = logger;

            if (!IsProperlyConfigured())
            {
                _logger.LogError("OpenCDN is not properly configured and therefore will not do anything.");
            }
        }

        public bool IsProperlyConfigured()
        {
            return !String.IsNullOrEmpty(_options.BaseUrl) && !String.IsNullOrEmpty(_options.Password);
        }

        public string? GetLink(string subDir, string filePath)
        {
            if (!IsProperlyConfigured())
            {
                _logger.LogError("OpenCDN is not properly configured and therefore GetLink was not executed.");
                return null;
            }

            long expires = GetLinkExpiry();
            string signature = ComputeSignature(expires, subDir, filePath);
            return $"{_options.BaseUrl}/{expires}/{signature}/{subDir}/{Uri.EscapeDataString(filePath)}";
        }

        public string? GetBase64Link(string subDir, string base64FilePath)
        {
            return GetLink(subDir, DecodeBase64FilePath(base64FilePath));
        }

        public void Play(Player player, string subDir, string path)
        {
            string? link = GetLink(subDir, path);
            if (link != null)
            {
                player.PlayAudioStream(link);
            }
        }

        public void Play(Player player, string subDir, string path, Vector3 position, float range)
        {
            string? link = GetLink(subDir, path);
            if (link != null)
            {
                player.PlayAudioStream(link, position, range);
            }
        }

        public void PlayBase64(Player player, string subDir, string path)
        {
            Play(player, subDir, DecodeBase64FilePath(path));
        }

        public void PlayBase64(Player player, string subDir, string path, Vector3 position, float range)
        {
            Play(player, subDir, DecodeBase64FilePath(path), position, range);
        }

        private static long GetLinkExpiry()
        {
            return DateTimeOffset.UtcNow.Add(LinkLifetime).ToUnixTimeSeconds();
        }

        // The CDN checks the same HMAC over the unescaped path, so a link can't be pointed at another file or extended.
        private string ComputeSignature(long expires, string subDir, string filePath)
        {
            byte[] key = Encoding.UTF8.GetBytes(_options.Password);
            byte[] message = Encoding.UTF8.GetBytes($"{expires}/{subDir}/{filePath}");
            return Convert.ToHexStringLower(HMACSHA256.HashData(key, message));
        }

        private static string DecodeBase64FilePath(string base64FilePath)
        {
            if (string.IsNullOrWhiteSpace(base64FilePath))
                throw new ArgumentException("File path cannot be null or empty.", nameof(base64FilePath));

            try
            {
                return Encoding.UTF8.GetString(Convert.FromBase64String(base64FilePath));
            }
            catch (FormatException ex)
            {
                throw new ArgumentException("The provided file path is not a valid base64 string.", nameof(base64FilePath), ex);
            }
        }
    }
}
