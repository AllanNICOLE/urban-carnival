using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using UC.Core.Services;

namespace UC.Console
{
    class Program
    {
        static CosmosTableService service = new CosmosTableService(ConfigurationManager.ConnectionStrings["cosmosDBkey"].ToString());
        
        static void Main(string[] args)
        {
            System.Console.WriteLine(ConfigurationManager.ConnectionStrings["cosmosDBkey"].ToString());
            System.Console.WriteLine("Urban Carnival - POC Azure Cosmos DB by AllanNICOLE");
            bool loop = true;
            DisplayMenu();
            do
            {
                
                ConsoleKeyInfo c = System.Console.ReadKey();
                switch (c.Key)
                {
                    case ConsoleKey.D1:
                    case ConsoleKey.NumPad1:
                        ListDocuments();
                        break;
                    case ConsoleKey.D2:
                    case ConsoleKey.NumPad2:
                        GetDocument();
                        break;
                    case ConsoleKey.D3:
                    case ConsoleKey.NumPad3:
                        AddDocumentStringOnly();
                        break;
                    case ConsoleKey.Escape:
                        loop = false;
                        break;
                    default:
                        break;
                }
                System.Console.Clear();

            } while (loop);

        }

        static void DisplayMenu()
        {
            System.Console.WriteLine("1. List documents");
            System.Console.WriteLine("2. Get document by GUID");
            System.Console.WriteLine("3. Add document");
            System.Console.WriteLine("ESCAPE to exit program.");
        }

        static void ListDocuments()
        {
            var t = service.GetAllData();
            t.Wait();
           
            System.Console.WriteLine($"Count : {t.Result.Count()}");
            foreach (var item in t.Result)
            {
                System.Console.WriteLine($"{item.RowKey} - {item.Timestamp} : {item.Properties.Count()} properties");
            }
            System.Console.WriteLine("Press enter to continue");
            System.Console.ReadKey();
        }

        static void GetDocument()
        {
            System.Console.Clear();
            System.Console.WriteLine("GUID ?");
            string input = System.Console.ReadLine();
            var t = service.GetDataByGuid(input.Trim());
            t.Wait();
            var item = t.Result;
            System.Console.WriteLine($"{item.RowKey} - created : {item.Timestamp}");
            System.Console.WriteLine("PROPERTIES :");
            foreach (var i in item.Properties)
            {
                System.Console.WriteLine($"{i.Key} - {i.Value.PropertyType} : {i.Value.PropertyAsObject.ToString()}");
            }
            System.Console.WriteLine("Press enter to continue");
            System.Console.ReadKey();
            System.Console.Clear();
        }

        static void AddDocumentStringOnly()
        {
            string id = Guid.NewGuid().ToString();
            System.Console.WriteLine("Quick creation of document. Only string type for now ...");
            System.Console.WriteLine("Your document will have the GUID : "+id);
            System.Console.WriteLine("Please use this format => PROPERTY1:VALUE1/PROPERTYX:VALUEX");
            string input = System.Console.ReadLine();
            List<KeyValuePair<string, object>> properties = new List<KeyValuePair<string, object>>();
            foreach (var item in input.Split("/"))
            {
                var x = item.Split(":");
                properties.Add(new KeyValuePair<string,object>(x[0], x[1]));
            }
            service.AddData(id, properties).Wait();
            System.Console.WriteLine("ADDED !");
            System.Console.WriteLine("Press enter to continue");
            System.Console.ReadKey();
        }
    }
}
