using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GettingStartedClient.ServiceReference1;

namespace GettingStartedClient
{
    class Program
    {
        const string Orange = "Orange";
        const string Apple = "Apple";
        const string Banana = "Banana";
        static void Main(string[] args)
        {
            //Step 1: Create an instance of the WCF proxy.
            FridgeClient client = new FridgeClient();

            Console.WriteLine("Add 3 oranges");
            var total = client.AddFruit(Orange, 3);
            Console.WriteLine($"Total oranges = {total}");

            Console.WriteLine("Add 2 apples");
            total = client.AddFruit(Apple, 2);
            Console.WriteLine($"Total apples = {total}");

            Console.WriteLine("Add 1 banana");
            total = client.AddFruit(Banana, 1);
            Console.WriteLine($"Total bananas = {total}");


            Console.WriteLine("Remove 1 banana");
            total = client.RemoveFruit(Banana, 1);
            Console.WriteLine($"Total bananas = {total}");

            Console.WriteLine("Remove 1 banana (no more left)");
            total = client.RemoveFruit(Banana, 1);
            Console.WriteLine($"Total bananas = {total}");

            Console.WriteLine("Remove 2 oranges (1 left)");
            total = client.RemoveFruit(Orange, 2);
            Console.WriteLine($"Total oranges = {total}");

            Console.WriteLine("Query fruit count");
            total = client.TotalFruit();
            Console.WriteLine($"Total fruit = {total}");
        }
    }
}

