using System;
using System.IO;
using System.Linq;
using UnileverRehackGetForGTIN.Helpers;

namespace UnileverRehackGetForGTIN
{
    class Program
    {
        private static readonly string _basePath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\..\\_data\\"));
        private static readonly string _productDataFilePath = Path.Combine(_basePath, "Unilever Products for REHACK.csv");
        private static readonly string _productsPath = Path.Combine(_basePath, "Products");
        private static readonly string _imagesPath = Path.Combine(_basePath, "Images");

        static void Main(string[] args)
        {
            var credentials = getCredentials();
            var productReader = new ProductReader(_productsPath, _imagesPath);
            var allProducts = productReader.GetProductsFromCsv(_productDataFilePath);

            var take = 50;
            for (var skip = 0; skip <= allProducts.Count(); skip += take)
            {
                allProducts
                    .Skip(skip)
                    .Take(take)
                    .GetData(credentials)
                    .SaveImages(_imagesPath)
                    .SaveXml(_productsPath);
            }
        }

        private static Guid getCredentials()
        {
            /*
             * Return the Guid from this function or 
             * create a 'Auth.txt' file in the root of the project with the Guid as its contents
             */
            var filePath = Path.Combine(_basePath, "..\\C#\\UnileverRehackGetForGTIN", "Auth.txt");
            return new Guid(File.ReadAllText(filePath));
        }
    }
}
