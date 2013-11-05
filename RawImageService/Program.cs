using System;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ServiceModel.Description;
using System.IO;
using System.Drawing;

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
            string path = @"D:\Templates\HtmlServiceImage.bmp";

            if( File.Exists(path) )
                File.Delete(path);

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
                        HtmlToThumbnail.WebsiteThumbnail.GetThumbnail(Thumbnail.Uri, Thumbnail.Width,
                                                                      Thumbnail.Hight, Thumbnail.ThumbWidth,
                                                                      Thumbnail.ThumbHight);
                    MemoryStream ms = new MemoryStream();
                    bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    ms.Position = 0;
                    WebOperationContext.Current.OutgoingResponse.ContentType = "image/jpeg";
                    return ms;
                }
                catch (Exception)
                {
                    throw;
                }
                return null;
            }
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