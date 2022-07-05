using System.IO.Compression;

namespace Microsoft.Extensions.Configuration;

public static class JsonConfigurationExtension
{
    public static IConfigurationBuilder AddJsonGzipCompressFile(this IConfigurationBuilder builder, string path, bool isBase64 = true, bool ignoreError = false)
    {
        try
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
        
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));

            if (!File.Exists(path))
                throw new FileNotFoundException("File not found.", path);

            byte[] buffers;
            
            if (isBase64)
            {
                var base64 = File.ReadAllText(path);
                buffers = Convert.FromBase64String(base64);
            }
            else
            {
                buffers = File.ReadAllBytes(path);
            }

            var ms = new MemoryStream();
        
            using var gzip = new GZipStream(new MemoryStream(buffers), CompressionMode.Decompress);
            
            gzip.CopyTo(ms);
            
            ms.Seek(0, SeekOrigin.Begin);
        
            return builder.AddJsonStream(ms);            
        }
        catch
        {
            if (!ignoreError)
                throw;
        }

        return builder;
    }
}