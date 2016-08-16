using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Linq;

namespace UnileverRehackGetForGTIN.Tests
{
    [TestClass]
    public class ProductsTests
    {
        private readonly string basePath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\..\\_data\\Products\\"));

        private readonly XmlNode _xmlNode;
        public ProductsTests()
        {
            var xml = File.ReadAllText(Path.Combine(basePath, "5000118042553 - Flora ProActiv Light Spread 250g.xml"));
            var doc = new XmlDocument();
            doc.LoadXml(xml);
            _xmlNode = doc.DocumentElement;
        }

        [TestMethod]
        public void ShouldReturnProductGTIN()
        {
            var productCode = _xmlNode.GetGTIN();
            Assert.AreEqual(productCode, "5000118042553");
        }

        [TestMethod]
        public void ShouldReturnProductDescription()
        {
            var description = _xmlNode.GetDescription();
            Assert.AreEqual(description, "Flora ProActiv Light Spread 250g");
        }

        [TestMethod]
        public void ShouldReturnImages()
        {
            var images = _xmlNode.GetImages();
            Assert.AreEqual(images.FirstOrDefault().Url.Substring(0, 60), "http://productimages.brandbank.com/snapshotimagehandler.ashx");
        }
    }
}
