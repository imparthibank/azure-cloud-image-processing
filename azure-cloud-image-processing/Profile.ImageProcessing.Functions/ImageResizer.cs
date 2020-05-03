using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;


namespace Profile.ImageProcessing.Functions
{
    public static class ImageResizer
    {
        [FunctionName("ImageResizer")]
        public static void Run([BlobTrigger("images/{name}", Connection = "AzureWebJobsStorage")] Stream image,
        [Blob("resized-images/{name}_big", FileAccess.Write)] Stream imageBig,
        [Blob("resized-images/{name}_medium", FileAccess.Write)] Stream imageMedium,
        [Blob("resized-images/{name}_small", FileAccess.Write)] Stream imageSmall, string name, ILogger log)
        {
            try
            {
                log.LogInformation($"{name} image Processing started");
                IImageFormat format;

                using (Image<Rgba32> input = Image.Load<Rgba32>(image, out format))
                {
                    ResizeImage(input, imageBig, ImageSize.Big, format);
                }
                log.LogInformation($"The {name} image converted into Big size");
                image.Position = 0;
                using (Image<Rgba32> input = Image.Load<Rgba32>(image, out format))
                {
                    ResizeImage(input, imageMedium, ImageSize.Medium, format);
                }
                log.LogInformation($"The {name} image converted into Medium size");
                image.Position = 0;
                using (Image<Rgba32> input = Image.Load<Rgba32>(image, out format))
                {
                    ResizeImage(input, imageSmall, ImageSize.Small, format);
                }
                log.LogInformation($"The {name} image converted into Small size");
                log.LogInformation($"{name} image Processing Completed");
            }
            catch (Exception ex)
            {
                log.LogError(ex.Message);
            }

        }

        public static void ResizeImage(Image<Rgba32> input, Stream output, ImageSize size, IImageFormat format)
        {
            var dimensions = imageDimensionsTable[size];

            input.Mutate(x => x.Resize(dimensions.Item1, dimensions.Item2));
            input.Save(output, format);
        }

        public enum ImageSize { Big, Small, Medium }

        private static Dictionary<ImageSize, (int, int)> imageDimensionsTable = new Dictionary<ImageSize, (int, int)>() {
        { ImageSize.Big,(190, 190) },
        { ImageSize.Medium,(70, 70) },
        { ImageSize.Small,(30, 30) } };
    }
}
