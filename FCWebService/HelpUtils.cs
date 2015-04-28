using FCEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace FCWebService
{
    public static class HelpUtils
    {
        internal static string ByteArrayToFile(string fileName, byte[] byteArray)
        {
            // Open file for reading
            System.IO.FileStream fileStream =
               new System.IO.FileStream(Path.Combine(GetDataDirectory(), fileName), System.IO.FileMode.Create, System.IO.FileAccess.Write);
            // Writes a block of bytes to this stream using data from
            // a byte array.
            fileStream.Write(byteArray, 0, byteArray.Length);

            fileStream.Close();
            return Path.Combine(GetDataDirectory(), fileName);
        }

        internal static string GetDataDirectory()
        {
            string dataDirectory = AppDomain.CurrentDomain.GetData("DataDirectory") as string;
            if (String.IsNullOrEmpty(dataDirectory))
            {
                dataDirectory = AppDomain.CurrentDomain.BaseDirectory;
            }
            if (!Directory.Exists(dataDirectory))
            {
                Directory.CreateDirectory(dataDirectory);
            }
            return dataDirectory;
        }

        internal static void DoConfigureProcessor(string docDifPath, IFlexiCaptureProcessor processor, IEngine engine)
        {
            IDocumentDefinition documentDefinition = createDocDifinitionFromXML(docDifPath, engine);
            bool checkresult = documentDefinition.Check();

            IStringsCollection errors = documentDefinition.Errors;
            for (int i = 0; i < errors.Count; i++)
            {
                string str = errors[i];
            }

            bool isvalid = documentDefinition.IsValid;

            processor.AddDocumentDefinition(documentDefinition);
            processor.SetForceApplyDocumentDefinition(true);
        }


        internal static IDocumentDefinition createDocDifinitionFromXML(string docDifPath, IEngine engine)
        {
            IDocumentDefinition documentDefinition = engine.CreateDocumentDefinition();

            XDocument xDoc = XDocument.Load(docDifPath);

            var infoGeneral = from x in xDoc.Descendants("_Document_Definition")
                              select new
                              {
                                  templateImageName = x.Descendants("_TemplateImageName").First().Value,
                                  defaultLanguage = xDoc.Descendants("_DefaultLanguage").First().Value,
                                  exportDestinationTypeEnum = xDoc.Descendants("_ExportDestinationTypeEnum").First().Value

                              };
            foreach (var i in infoGeneral)
            {
                documentDefinition.Sections.AddNew("Section").Pages.AddNew(Config.parentDirectory + i.templateImageName);
                documentDefinition.DefaultLanguage = engine.PredefinedLanguages.FindLanguage(i.defaultLanguage);
                documentDefinition.ExportParams = engine.CreateExportParams((ExportDestinationTypeEnum)Enum.Parse(typeof(ExportDestinationTypeEnum), i.exportDestinationTypeEnum));
            }

var infoRegions = from x in xDoc.Descendants("_Region")
                    select new
                    {
                        Name = x.Descendants("_Name").First().Value,
                        X1 = x.Descendants("_X1").First().Value,
                        Y1 = x.Descendants("_Y1").First().Value,
                        X2 = x.Descendants("_X2").First().Value,
                        Y2 = x.Descendants("_Y2").First().Value,
                        BlockTypeEnum = x.Descendants("_BlockTypeEnum").First().Value
                    };

List<Region> regions = new List<Region>();
foreach (var i in infoRegions)
{
    Region region = new Region();
    region.name = i.Name;
    region.x1 = Convert.ToInt32(i.X1);
    region.y1 = Convert.ToInt32(i.Y1);
    region.x2 = Convert.ToInt32(i.X2);
    region.y2 = Convert.ToInt32(i.Y2);
    region.blockTypeEnumElem = (BlockTypeEnum)Enum.Parse(typeof(BlockTypeEnum), i.BlockTypeEnum);
    regions.Add(region);
}

            for (int i = 0; i < regions.Count; i++)
            {
                IRegion currentRegion = engine.CreateRegion();
                currentRegion.AddRect(regions[i].x1, regions[i].y1, regions[i].x2, regions[i].y2);
                documentDefinition.Pages[0].Blocks.AddNew(regions[i].blockTypeEnumElem, currentRegion, regions[i].name);
            }
            return documentDefinition;
        }

        private struct Region
        {
            public string name;
            public int x1;
            public int y1;
            public int x2;
            public int y2;
            public BlockTypeEnum blockTypeEnumElem;
        }
    }
}