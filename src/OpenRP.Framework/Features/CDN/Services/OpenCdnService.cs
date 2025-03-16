using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenRP.Framework.Features.CDN.Entities;
using OpenRP.Framework.Features.Inventories.Services;
using SampSharp.Entities.SAMP;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.CDN.Services
{
    public class OpenCdnService : IOpenCdnService
    {
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
            if (!String.IsNullOrEmpty(_options.Password))
            {
                return true;
            }

            if (!String.IsNullOrEmpty(_options.BaseUrl))
            {
                return true;
            }

            return false;
        }

        public string? GetLink(string subDir, string filePath)
        {
            if (IsProperlyConfigured())
            {
                string dateHour = DateTime.UtcNow.ToString("yyyy-MM-dd-HH");
                string plainString = dateHour + _options.Password;
                string hash = ComputeSHA256Hash(plainString);
                string encodedFilePath = Uri.EscapeDataString(filePath);
                return $"{_options.BaseUrl}/{hash}/{subDir}/{encodedFilePath}";
            } else
            {
                _logger.LogError("OpenCDN is not properly configured and therefore GetLink was not executed.");
            }
            return null;
        }

        public string? GetBase64Link(string subDir, string base64FilePath)
        {
            if (IsProperlyConfigured())
            {
                if (string.IsNullOrWhiteSpace(base64FilePath))
                    throw new ArgumentException("File path cannot be null or empty.", nameof(base64FilePath));

                string decodedFilePath;
                try
                {
                    byte[] bytes = Convert.FromBase64String(base64FilePath);
                    decodedFilePath = Encoding.UTF8.GetString(bytes);
                }
                catch (FormatException ex)
                {
                    throw new ArgumentException("The provided file path is not a valid base64 string.", nameof(base64FilePath), ex);
                }

                return GetLink(subDir, decodedFilePath);
            }
            else
            {
                _logger.LogError("OpenCDN is not properly configured and therefore GetBase64Link was not executed.");
            }
            return null;
        }

        private static string ComputeSHA256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                    builder.Append(bytes[i].ToString("x2"));
                return builder.ToString();
            }
        }

        public void Play(Player player, string subDir, string path)
        {
            if (IsProperlyConfigured())
            {
                string link = GetLink(subDir, path);
                player.PlayAudioStream(link);
            }
            else
            {
                _logger.LogError("OpenCDN is not properly configured and therefore Play was not executed.");
            }
        }

        public void Play(Player player, string subDir, string path, Vector3 position, float range)
        {
            if (IsProperlyConfigured())
            {
                string link = GetLink(subDir, path);
                player.PlayAudioStream(link, position, range);
            }
            else
            {
                _logger.LogError("OpenCDN is not properly configured and therefore Play was not executed.");
            }
        }

        public void PlayBase64(Player player, string subDir, string path)
        {
            if (IsProperlyConfigured())
            {
                string link = GetBase64Link(subDir, path);
                player.PlayAudioStream(link);
            }
            else
            {
                _logger.LogError("OpenCDN is not properly configured and therefore Play was not executed.");
            }
        }

        public void PlayBase64(Player player, string subDir, string path, Vector3 position, float range)
        {
            if (IsProperlyConfigured())
            {
                string link = GetBase64Link(subDir, path);
                player.PlayAudioStream(link, position, range);
            }
            else
            {
                _logger.LogError("OpenCDN is not properly configured and therefore Play was not executed.");
            }
        }
    }
}
