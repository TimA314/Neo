using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Neo.Utilities
{
    public static class ImageSanitization
    {
        private static readonly string[] AllowedSchemes = { "http", "https" };
        private const long MaxImageSize = 10 * 1024 * 1024; // 10 MB

        public static async Task<string> SanitizeImageUrl(string imageUrl)
        {
            if (!Uri.TryCreate(imageUrl, UriKind.Absolute, out Uri uriResult))
                return string.Empty;

            if (Array.IndexOf(AllowedSchemes, uriResult.Scheme) < 0)
                return string.Empty;

            return await VerifyImageContentAsync(uriResult);
        }

        private static async Task<string> VerifyImageContentAsync(Uri imageUri)
        {
            using var httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromSeconds(5);

            try
            {
                var response = await httpClient.GetAsync(imageUri, HttpCompletionOption.ResponseHeadersRead);
                if (!response.IsSuccessStatusCode)
                    return string.Empty;

                var contentType = response.Content.Headers.ContentType?.MediaType;
                if (contentType == null || !contentType.StartsWith("image/"))
                    return string.Empty;

                var contentLength = response.Content.Headers.ContentLength;
                if (contentLength == null || contentLength > MaxImageSize)
                    return string.Empty;

                return imageUri.AbsoluteUri;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
