using Microsoft.Extensions.Options;
using OpenRP.Framework.Features.CDN.Entities;
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

        public string GetBase64Link(string subDir, string base64FilePath)
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

        public void PlayBase64(Player player, string subDir, string path)
        {
            string link = GetBase64Link(subDir, path);
            player.PlayAudioStream(link);
        }

        public void PlayBase64(Player player, string subDir, string path, Vector3 position, float range)
        {
            string link = GetBase64Link(subDir, path);
            player.PlayAudioStream(link, position, range);
        }
    }
}
