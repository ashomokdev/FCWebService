using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using ServiceReference1;
using System.Globalization;

public partial class Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void ButtonGetInfo_Click(object sender, EventArgs e)
    {
        string targetPath = GetPathToUploads();
        byte[] byteDocDefinition = null;
        List<byte[]> images = new List<byte[]>();
        if (inputDocumentDefinition.HasFile)
        {
            string filePath = Path.Combine(targetPath, Server.HtmlEncode(inputDocumentDefinition.FileName));
            inputDocumentDefinition.SaveAs(filePath);
            byteDocDefinition = File.ReadAllBytes(filePath);            
        }
        else
        {
            //DefaultDocumentDifinition
        }
        if (inputImages.HasFiles)
        {            
            foreach (System.Web.HttpPostedFile image in inputImages.PostedFiles)
            {
                string pathToImagesDir = Directory.CreateDirectory(Path.Combine(targetPath, Guid.NewGuid().ToString())).FullName;
                
                if (!image.ContentType.Contains("jpeg"))
                {
                    labelImagesWarning.Visible = true;
                    labelImagesWarning.Text = "Warning! " + image.FileName + " not .jpg file. So it not been processed.";
                }
                else
                {
                    string filePath = Path.Combine(pathToImagesDir, Server.HtmlEncode(image.FileName));
                    image.SaveAs(filePath);
                    byte[] byteImage = File.ReadAllBytes(filePath);
                    images.Add(byteImage);
                }
            }           
        }
        else
        {
            //TODO
        }
        //TODO refactoring needed
        ArrayOfBase64Binary arrayImages = new ArrayOfBase64Binary();
        foreach (byte[] image in images)
        {
            arrayImages.Add(image);
        }
        var response = ws.GetResultFromImages(byteDocDefinition, arrayImages);
        if (response != null)
        {
            panelResultTable.Visible = true;
            foreach (var item in response)
            {
                TableRow tRow = new TableRow();
                tableResult.Rows.Add(tRow);
                TableCell tCellNumber = new TableCell();
                tCellNumber.Text = new string(item.Number.ToArray());
                TableCell tCellExpiryDate = new TableCell();
                tCellExpiryDate.Text = new string(item.Date.ToArray());
                tRow.Cells.Add(tCellNumber);
                tRow.Cells.Add(tCellExpiryDate);            
            }
        }
    }

    private string GetPathToUploads()
    {
        string targetPath = Server.MapPath("Uploads");
        if (!System.IO.Directory.Exists(targetPath))
        {
            System.IO.Directory.CreateDirectory(targetPath);
        }
        return targetPath;
    }

    private WebService1SoapClient ws = new WebService1SoapClient();
}