using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Store
{
    public enum Job { Worker, Suplier, Buyer }
    public class Person
    {
        public Job job;

        public Person(Job job)
        {
            this.job = job;
        }

        public void DoJob()
        {
            Random rand = new Random();
            switch (job)
            {
                case Job.Worker:
                    Send();
                    break;

                case Job.Suplier:
                    lock (Store.available_products)
                    {
                        StoreItem(Store.available_products.FirstOrDefault(item => item.Value <= 3).Key, rand.Next(5, 10));
                    }
                    break;

                case Job.Buyer:
                    for (int i = 0; i < rand.Next(0, 3); i++)
                    {
                        Order(Store.available_products.ElementAt(rand.Next(0, Store.available_products.Count - 1)).Key, rand.Next(1, 3)); ;
                    }
                    break;
            }
        }
        void StoreItem(string item, int amount)
        {
            if (item != null)
            {
                if (!Store.available_products.ContainsKey(item))
                {
                    Store.available_products.TryAdd(item, amount);
                    Console.WriteLine("Stored" + amount + " " + item);
                }

                else
                {
                    int value = 0;
                    Store.available_products.TryGetValue(item, out value);
                    Store.available_products[item] = value + amount;
                    Console.WriteLine("Added " + amount + " " + item);

                }
            }
        }

        void Order(string item, int amount)
        {
            lock (Store.orders)
            {
                int value = 0;
                Order newOrder = new Order(item, amount);
                Store.orders.Enqueue(newOrder);
                Console.WriteLine("Placed order for " + amount + " " + item);
            }

        }

        void Send()
        {
            lock (Store.available_products) lock (Store.orders)
                {
                    if (Store.orders.Count > 0)
                    {
                        Order order = Store.orders.Dequeue();
                        int value = 0;
                        Store.available_products.TryGetValue(order.item, out value);
                        if (value - order.amount >= 0)
                        {
                            Store.available_products[order.item] = value - order.amount;
                            Console.WriteLine("Shipped " + order.amount + " " + order.item);
                            Store.orderCount++;
                        }

                        else
                        {
                            Console.WriteLine("Not enough " + order.item);
                        }

                    }

                    else
                    {
                        Console.WriteLine("No orders available");
                    }
                }
        }
    }
}
