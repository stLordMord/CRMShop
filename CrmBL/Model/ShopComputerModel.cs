using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmBL.Model
{
    public class ShopComputerModel
    {
        Generator Generator = new Generator();
        Random rnd = new Random();
        public List<CashDesk> CashDesks { get; set; } = new List<CashDesk>();
        public List<Cart> Carts { get; set; } = new List<Cart>();
        public List<Check> Checks { get; set; } = new List<Check>();
        public List<Sell> Sells { get; set; } = new List<Sell>();
        public Queue<Seller> Sellers { get; set; } = new Queue<Seller>();

        public ShopComputerModel()
        {
            var sellers = Generator.GetNewSellers(20);
            Generator.GetNewProducts(1000);
            Generator.GetNewCustomers(100);

            foreach(var seller in sellers)
            {
                Sellers.Enqueue(seller); //очередь касиров (чтобы брать свободных)
            }

            for (int i = 0; i < 3; i++)
            {
                CashDesks.Add(new CashDesk(CashDesks.Count, Sellers.Dequeue())); //создаем кассы
            }
        }

        public void Start()
        {
            var customers = Generator.GetNewCustomers(10);

            var carts = new Queue<Cart>();  // создали корзину

            foreach (var customer in customers) // соединили с покупателем
            {
                var cart = new Cart(customer);
                foreach (var prod in Generator.GetRandomProducts(10, 30)) // нагрузили товар
                {
                    cart.Add(prod);
                }
                carts.Enqueue(cart); // добавляем в общий список
            }

            while (carts.Count > 0)  // расставляем по кассам
            {
                var cash = CashDesks[rnd.Next(CashDesks.Count - 1)]; // выбираем случайную кассу
                cash.Enqueue(carts.Dequeue());
            }

            while (true) // выпускаем по 1 человеку с кассы
            {
                var cash = CashDesks[rnd.Next(CashDesks.Count - 1)]; // выбираем случайную кассу
                var money = cash.Dequeue();
            }
        }
    }
}
