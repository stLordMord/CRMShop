using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmBL.Model
{
    public class CashDesk
    {
        CrmContext db;

        public int Number { get; set; }
        public Seller Seller { get; set; }
        public Queue<Cart> Queue { get; set; }
        public int MaxQueueLength { get; set; }
        public int ExitCustomer { get; set; }
        public bool IsModel { get; set; }
        public int Count => Queue.Count;


        public event EventHandler<Check> CheckClosed;

        public CashDesk(int number, Seller seller, CrmContext db)
        {
            Number = number;
            Seller = seller;
            Queue = new Queue<Cart>();
            IsModel = true;
            MaxQueueLength = 10;
            this.db = db ?? new CrmContext();
        }

        public void Enqueue(Cart cart)
        {
            if (Queue.Count < MaxQueueLength)
            {
                Queue.Enqueue(cart); // поставили в очередь
            }
            else
            {
                ExitCustomer++;
            }
        }

        public decimal Dequeue()
        {
            decimal sum = 0; // сумма покупки
            if (Queue.Count == 0)
            {
                return 0;
            }
            var card = Queue.Dequeue();

            if (card != null)
            {
                var check = new Check()
                {
                    SellerID = Seller.SellerId,
                    Seller = Seller,
                    CustomerId = card.Customer.CustomerId,
                    Customer = card.Customer,
                    Created = DateTime.Now,
                };

                if (!IsModel)
                {
                    db.Checks.Add(check);
                    db.SaveChanges();
                }
                else
                {
                    check.CheckId = 0;
                }

                var sells = new List<Sell>(); //вспомогательный список

                foreach (Product product in card) // достаем из корзины
                {
                    if (product.Count > 0)
                    {
                        var sell = new Sell()
                        {
                            CheckId = check.CheckId,
                            Check = check,
                            ProductId = product.ProductId,
                            Product = product
                        };

                        sells.Add(sell);  // добавляем продажу

                        if (!IsModel)
                        {
                            db.Sells.Add(sell);
                        }

                        product.Count--; // уменьшаем кол-во товара
                        sum += product.Price; // формируем тоговую стоимость
                    }
                }

                check.Price = sum;

                if (!IsModel)
                {
                    db.SaveChanges();
                }

                CheckClosed?.Invoke(this, check); // генерируем событие (? - проверка на null)
            }

            return sum;
        }
        public override string ToString()
        {
            return $"Касса №{Number}";
        }
    }
}
