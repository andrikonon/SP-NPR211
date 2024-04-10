using SixLabors.ImageSharp.Formats.Webp;

namespace Helpers;

public class ImageLoader
{
    public static string? SaveImage(Uri uri, string folder)
    {
        try
        {
            using HttpClient client = new();
            using var response = client.GetAsync(uri).Result;

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            string fileName = Guid.NewGuid() + ".webp";

            string path = Path.Combine(folder, fileName);

            using var image = SixLabors.ImageSharp.Image.Load(response.Content.ReadAsByteArrayAsync().Result);
            using var ms = new MemoryStream();
            image.Save(ms, new WebpEncoder());
            File.WriteAllBytes(path, ms.ToArray());
            return fileName;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }

        return null;
    }
}