using CsvHelper;
using System.Collections.Generic;
using System.IO;

namespace UnileverRehackGetForGTIN
{
    public class ProductReader
    {
        public ProductReader(string productsPath, string imagesPath)
        {
            if (!Directory.Exists(productsPath)) Directory.CreateDirectory(productsPath);
            if (!Directory.Exists(imagesPath)) Directory.CreateDirectory(imagesPath);
        }

        public IEnumerable<ProductsCsv> GetProductsFromCsv(string path)
        {
            using (var reader = File.OpenText(path))
            {
                var csv = new CsvReader(reader);
                while (csv.Read())
                    yield return csv.GetRecord<ProductsCsv>();
            }
        }

    }

    public class ImageInfo
    {
        public string Url { get; set; }
        public string ShotTypeId { get; set; }
    }

    public class ProductsCsv
    {
        public string GTIN { get; set; }
        public string Description { get; set; }
    }
}
