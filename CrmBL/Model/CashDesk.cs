using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmBL.Model
{
    public class CashDesk
    {
        CrmContext db = new CrmContext();

        public int Number { get; set; }
        public Seller Seller { get; set; }
        public Queue<Cart> Queue { get; set; }
        public int MaxQueueLength { get; set; }
        public int ExitCustomer { get; set; }
        public bool IsModel { get; set; }

        public CashDesk(int number, Seller seller)
        {
            Number = number;
            Seller = seller;
            Queue = new Queue<Cart>();
            IsModel = true;
        }

        public void Enqueue(Cart cart)
        {
            if (Queue.Count <= MaxQueueLength)
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
            var card = Queue.Dequeue();

            if (card != null)
            {
                var check = new Check()
                {
                    SellerID = Seller.SellerID,
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

                if (!IsModel)
                {
                    db.SaveChanges();
                }
            }

            return sum;
        }
    }
}
