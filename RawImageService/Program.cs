using System;
using System.Drawing.Drawing2D;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ServiceModel.Description;
using System.IO;
using System.Drawing;
using HtmlToThumbnail;
using RawImageService.Properties;

namespace RawImageService
{
    [ServiceContract]
    public interface IImageServer
    {
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "/thumbnail/?uri={uri}")]
        Stream GetThumbnail(string uri);
    }

    // implement the service contract
    public class Service : IImageServer
    {
        public Stream GetThumbnail(string uri)
        {
            if (string.IsNullOrEmpty(uri))
            {
                return null;
            }
            else
            {
                if ((uri.IndexOf("file:", System.StringComparison.Ordinal) < 0) &&
                    (uri.IndexOf("http", System.StringComparison.Ordinal) < 0))
                    uri = "http://" + uri;

                Thumbnail.Uri = uri;
                try
                {
                    Bitmap bitmap =
                        WebsiteThumbnail.GetThumbnail(Thumbnail.Uri, Thumbnail.Width,
                                                                      Thumbnail.Hight, Thumbnail.ThumbWidth,
                                                                      Thumbnail.ThumbHight);
                    return CheckForBlankPage(bitmap);
                }
                catch (Exception)
                {
                    throw;
                }
                return null;
            }
        }

        public Stream CheckForBlankPage(Bitmap bitmap)
        {
            MemoryStream ms = new MemoryStream();

            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            ms.Position = 0;

            Bitmap blankImage = Resources.blank_image;

            using (MemoryStream ms2 = new MemoryStream())
            {
                blankImage.Save(ms2, System.Drawing.Imaging.ImageFormat.Jpeg);

                string firstBitmap = Convert.ToBase64String(ms.ToArray());
                String secondBitmap = Convert.ToBase64String(ms2.ToArray());

                if (firstBitmap.Equals(secondBitmap))
                {
                    bitmap = Resources.no_image;
                    Bitmap newImage = new Bitmap(160, 120);
                    using (Graphics gr = Graphics.FromImage(newImage))
                    {
                        gr.SmoothingMode = SmoothingMode.HighQuality;
                        gr.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        gr.PixelOffsetMode = PixelOffsetMode.HighQuality;
                        gr.DrawImage(bitmap, new Rectangle(0, 0, 160, 120));
                    }
                    newImage.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    ms.Position = 0;
                }
            }
            WebOperationContext.Current.OutgoingResponse.ContentType = "image/jpeg";

            return ms;
        }
    }



    class Program
    {
        static void Main(string[] args)
        {
            string baseAddress = "http://" + Environment.MachineName + ":8000/";
            ServiceHost host = new ServiceHost(typeof(Service), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(IImageServer), new WebHttpBinding(), "").Behaviors.Add(new WebHttpBehavior());
            host.Open();
            Console.WriteLine("Service is running");
            Console.Write("Press ENTER to close the host");
            Console.ReadLine();
            host.Close();
        }
    }
}