using Microsoft.Extensions.Options;
using OpenRP.Framework.Features.CDN.Entities;
using SampSharp.Entities.SAMP;
using System;
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

        public OpenCdnService(IOptions<OpenCdnOptions> options)
        {
            _options = options.Value;
            ValidateOptions();
        }

        public string GetLink(string subDir, string filePath)
        {
            string dateHour = DateTime.UtcNow.ToString("yyyy-MM-dd-HH");
            string plainString = dateHour + _options.Password;
            string hash = ComputeSHA256Hash(plainString);
            string encodedFilePath = Uri.EscapeDataString(filePath);
            return $"{_options.BaseUrl}/{hash}/{subDir}/{encodedFilePath}";
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

        private void ValidateOptions()
        {
            if (string.IsNullOrEmpty(_options.Password))
                throw new ArgumentNullException(nameof(OpenCdnOptions.Password));
            if (string.IsNullOrEmpty(_options.BaseUrl))
                throw new ArgumentNullException(nameof(OpenCdnOptions.BaseUrl));
        }

        public void Play(Player player, string subDir, string path)
        {
            string link = GetLink(subDir, path);
            player.PlayAudioStream(link);
        }

        public void Play(Player player, string subDir, string path, Vector3 position, float range)
        {
            string link = GetLink(subDir, path);
            player.PlayAudioStream(link, position, range);
        }
    }
}
