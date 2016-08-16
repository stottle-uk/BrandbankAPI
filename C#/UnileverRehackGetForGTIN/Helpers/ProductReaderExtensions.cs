using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using UnileverRehackGetForGTIN.ExtractData;

namespace UnileverRehackGetForGTIN.Helpers
{
    internal static class ProductsExtensions
    {
        public static XmlNode GetData(this IEnumerable<ProductsCsv> products, Guid credentials)
        {
            var externalCallerHeader = new ExternalCallerHeader { ExternalCallerId = credentials };
            using (var proxy = new DataExtractSoapClient()) //TODO: properly dispose proxy
            {
                return proxy.GetProductDataForGTINs(externalCallerHeader, products.Select(p => p.GTIN).ToArray());
            }
        }

        public static XmlNode SaveImages(this XmlNode xml, string imagesPath)
        {
            foreach (XmlNode xmlNode in xml)
            {
                foreach (var image in xmlNode.GetImages())
                {
                    var path = Path.Combine(imagesPath, string.Format("{0}-T{1}.jpg", xmlNode.GetGTIN(), image.ShotTypeId));
                    var imageData = new WebClient().DownloadData(image.Url);
                    using (var memoryStream = new MemoryStream(imageData))
                    {
                        var imageStream = Image.FromStream(memoryStream);
                        imageStream.Save(path, ImageFormat.Jpeg);
                    }
                }
            }
            return xml;
        }

        public static XmlNode SaveXml(this XmlNode xml, string productsPath)
        {
            foreach (XmlNode xmlNode in xml)
            {
                var path = Path.Combine(productsPath, string.Format("{0} - {1}.xml", xmlNode.GetGTIN(), xmlNode.GetDescription()));
                using (var writer = new XmlTextWriter(path, Encoding.UTF8))
                {
                    writer.Formatting = Formatting.Indented;
                    xmlNode.ParentNode.WriteContentTo(writer);
                }
            }
            return xml;
        }

        public static string GetGTIN(this XmlNode xml)
        {
            var node = XElement.Parse(xml.OuterXml);
            return (from codes
                    in node.Descendants("Identity").Descendants("ProductCodes").Descendants("Code")
                    select codes.Value).FirstOrDefault();
        }

        public static string GetDescription(this XmlNode xml)
        {
            var node = XElement.Parse(xml.OuterXml);
            return (from diagnosticDescription
                    in node.Descendants("Identity").Descendants("DiagnosticDescription")
                    select diagnosticDescription.Value).FirstOrDefault();
        }

        public static IEnumerable<ImageInfo> GetImages(this XmlNode xml)
        {
            var node = XElement.Parse(xml.OuterXml);
            return from image
                   in node.Descendants("Assets").Descendants("Image")
                   select new ImageInfo
                   {
                       Url = image.Descendants("Url").FirstOrDefault().Value,
                       ShotTypeId = image.Attribute("ShotTypeId").Value
                   };
        }
    }
}
