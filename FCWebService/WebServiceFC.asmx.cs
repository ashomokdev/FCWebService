using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Services;

namespace FCWebService
{
    /// <summary>
    /// Simple example of using FCE - getting .jpg of passport and returning recognized info. 
    /// </summary>
    [WebService(Namespace = "http://abbyy/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    public class WebService1 : System.Web.Services.WebService
    {
        [WebMethod(Description = "Uploading images and Document difinition .xml file.")]
        public List<PersonData> GetResultFromImages(byte[] documentDifinition, byte[][] images)
        {
            if (documentDifinition == null)
            {
                documentDifinition = File.ReadAllBytes(pathToDefaultDocumentDifinition);
            }
            List<byte[]> imagesList = images.ToList();
            Request request = new Request(documentDifinition, imagesList);
            Schedule.Instance.AddRequest(request);

            request.requestFinished.WaitOne();
            List<PersonData> result = request.Result;
            return result;
        }
        public static string pathToDefaultDocumentDifinition = Path.Combine(Config.parentDirectory, "DocumentDefinition.xml");
    }
}
