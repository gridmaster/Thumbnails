using System.IO;
using System.Net;

namespace ServiceTest
{
    class Program
    {
        private static void Main(string[] args)
        {
            string uri = string.Concat("http://localhost:8000",
                                       string.Format("/thumbnail/?uri={0}", "www.yahoo.com")); //   file:///D:/Templates/Test4.htm")); // "www.cnn.com")); 

            tryThis(uri);
        }

        public static void tryThis(string uri)
        {  // this works
            HttpWebRequest request = WebRequest.Create(uri) as HttpWebRequest;
            request.Method = "GET";

            // Get response   
            using (WebResponse response = request.GetResponse() as WebResponse)
            {
                using (Stream stream = response.GetResponseStream())
                {
                    byte[] buffer = new byte[response.ContentLength];
                    MemoryStream ms = new MemoryStream();
                    int bytesRead, totalBytesRead = 0;

                    do
                    {
                        bytesRead = stream.Read(buffer, 0, buffer.Length);
                        totalBytesRead += bytesRead;

                        ms.Write(buffer, 0, bytesRead);
                    } while (bytesRead > 0);

                    string path = @"D:\templates\fsx.jpg";
                    if (File.Exists(path))
                        File.Delete(path);

                    var fs = new FileStream(path, FileMode.Create);
                    fs.Write(ms.ToArray(), 0, totalBytesRead);
                    fs.Flush();
                    fs.Close();
                }
            }
        }
    }
}