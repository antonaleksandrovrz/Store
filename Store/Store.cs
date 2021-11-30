using System;
using System.Collections.Generic;
using System.Threading;

namespace Store
{
    public class Order
    {
        public string item;
        public int amount;

        public Order(string itemName,int amount)
        {
            this.item = itemName;
            this.amount = amount;
        }
    }
    class Store
    {
        public static Dictionary<string, int> available_products = new Dictionary<string, int> { { "Shirt" , 25}, { "Pants", 30 }, { "Hat", 10 }, { "Shoes", 5 } };
        public static Queue<Order> orders = new Queue<Order>();
        public static Queue<Person> freeSupplier = new Queue<Person>(3);
        public static Queue<Person> freeWorkers = new Queue<Person>(5);
        public static int orderCount = 0;
        static void Main(string[] args)
        {
            for (int i = 0; i < 5; i++)
            {
                Person temp = new Person(Job.Worker);
                freeWorkers.Enqueue(temp);
            }

            for (int i = 0; i < 5; i++)
            {
                Person temp = new Person(Job.Suplier);
                freeSupplier.Enqueue(temp);
            }

            Thread t = new Thread(Simulate_WorkerJob);
            Thread t2 = new Thread(Simulate_Buyers);
            Thread t3 = new Thread(Simulate_SuplierJob);

            t3.Start();
            t.Start();
            t2.Start();

            t3.Join();
            t.Join();
            t2.Join();

            foreach (var item in Store.available_products)
            {
                    Console.Write(item.Key + " " + item.Value + ", ");
            }
        }

        public static void Simulate_SuplierJob()
        {
            Random rand = new Random();
            do
            {
                Person temp = freeSupplier.Dequeue();
                Thread t = new Thread(new ThreadStart(temp.DoJob));

                t.Start();
                t.Join();
                freeSupplier.Enqueue(temp);
            }
            while (orders.Count > 0);
        }

        public static void Simulate_WorkerJob()
        {
            Random rand = new Random();
            do
            {
                if (freeWorkers.Count > 0)
                {
                    Person temp = freeWorkers.Dequeue();
                    Thread t = new Thread(new ThreadStart(temp.DoJob));

                    t.Start();
                    t.Join();
                    freeWorkers.Enqueue(temp);
                }
            }
            while (Store.orders.Count > 0);
        }

        public static void Simulate_Buyers()
        {
            for (int task = 1; task < 400; task++)
            {
                Person temp = new Person(Job.Buyer);
                Thread t = new Thread(new ThreadStart(temp.DoJob));
                t.Start();
            }
        }
    }
}
