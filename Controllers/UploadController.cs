using NinjaFit.Api.Support;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;

namespace NinjaFit.Api.Controllers
{
    [RoutePrefix("upload")]
    public class UploadController : ApiController
    {
        [Route("images")]
        [HttpPost]
        public async Task<object> UploadImages()
        {
            if (Request.Content.IsMimeMultipartContent())
            {
                var provider = await Request.Content.ReadAsMultipartAsync<MultipartMemoryStreamProvider>(new MultipartMemoryStreamProvider());

                string tag = string.Empty;
                List<UploadedImage> images = new List<UploadedImage>();

                foreach (HttpContent content in provider.Contents)
                {
                    if (content.Headers.ContentDisposition.Parameters.First().Value == "\"tag\"")
                    {
                        tag = await content.ReadAsStringAsync();
                    }

                    if (content.Headers.ContentType != null)
                    {
                        var image = new UploadedImage();
                        image.Type   = content.Headers.ContentType.MediaType;
                        image.Name   = content.Headers.ContentDisposition.FileName.Replace("\"", "");
                        image.Ext    = Path.GetExtension(image.Name);
                        image.Stream = await content.ReadAsStreamAsync();
                        image.Image  = Image.FromStream(image.Stream);
                        images.Add(image);
                    }
                }

                string basePath = "images",
                       baseUrl  = HostingEnvironment.MapPath($"~/{basePath}"),
                       timeKey  = Utils.NowTimeKey;

                int i       = -1,
                    dateKey = Utils.NowDateKey,
                    pWidth  = Constants.Images.PreviewWidth,
                    pHeight = Constants.Images.PreviewHeight;

                List<UploadedImageResponse> responseImages = new List<UploadedImageResponse>();

                foreach (UploadedImage image in images)
                {
                    string url     = $"{tag}_{dateKey}{timeKey}_{++i}{image.Ext}";
                    string preview = null;

                    image.Image.Save($"{baseUrl}/{url}");

                    if (image.Image.Width > pWidth && image.Image.Height > pHeight)
                    {
                        preview = $"{tag}_{dateKey}{timeKey}_{i}_preview{image.Ext}";

                        image.Preview = ResizeImage(image.Image, Constants.Images.PreviewWidth, Constants.Images.PreviewHeight);
                        image.Preview.Save($"{baseUrl}/{preview}");
                        image.Preview.Dispose();
                    }
                    
                    image.Image .Dispose();
                    image.Stream.Dispose();

                    var imageResponse = new UploadedImageResponse();
                    imageResponse.Url     = $"{basePath}/{url}";
                    imageResponse.Preview = $"{basePath}/{preview}";

                    responseImages.Add(imageResponse);
                }

                return new { Images = responseImages, Success = true };
            }

            return null;
        }

        private static Bitmap ResizeImage(Image image, int minWidth, int minHeight)
        {
            int width, height;
            
            if (image.Width >= image.Height)
            {
                height = minHeight;
                width  = minHeight * image.Width / image.Height;
            }
            else
            {
                width  = minWidth;
                height = minWidth * image.Height / image.Width;
            }

            var destRect  = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap   (width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode    = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode  = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode      = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode    = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        public class UploadedImage
        {
            public string Type    { get; set; }
            public string Name    { get; set; }
            public string Ext     { get; set; }
            public Stream Stream  { get; set; }
            public Image  Image   { get; set; }
            public Image  Preview { get; set; }
        }

        public class UploadedImageResponse
        {
            public string Url     { get; set; }
            public string Preview { get; set; }
        }
    }
}