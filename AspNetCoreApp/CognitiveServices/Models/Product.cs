using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CognitiveServices.Models
{
    public class Product
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Brand { get; set; }
        public double Price { get; set; }

        public Product()
        {
        }

        public Product(int id, string name, string brand, double price)
        {
            this.ID = id;
            this.Name = name;
            this.Brand = brand;
            this.Price = price;
        }

        public static List<Product> GetList()
        {
            List<Product> list = new List<Product>() {
                new Product(1,"Xz1","Sony",3999){ },
                new Product(2,"Pro7","Meizu",2499){ },
                new Product(3,"Galaxy S8","Samsung",4399){ },
                new Product(4,"iPhone X","Apple",8399){ },
                new Product(5,"Pixel 2","Google",3699){ },
                new Product(6,"U11","HTC",4099){ },
                new Product(7,"Lumia 1020","Nokia",5399){ }
            };
            return list;
        }

        public static List<String> GetAttr<T>(T model)
        {
            List<String> propList = new List<String>();
            Type types = model.GetType();
            foreach (var p in types.GetProperties())
            {
                propList.Add(p.Name);
            }
            return propList;
        }
    }
}
