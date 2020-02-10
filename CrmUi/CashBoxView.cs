using CrmBL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CrmUi
{
    class CashBoxView
    {
        CashDesk cashDesk;

        public Label CashDeskName { get; set; }
        public NumericUpDown Price { get; set; }
        public ProgressBar QueueLength { get; set; }
        public Label LeaveCustomerCount { get; set; }

        public CashBoxView(CashDesk cashDesk, int number, int x, int y) // number - сумма в кассе
        {
            this.cashDesk = cashDesk;

            CashDeskName = new Label();
            Price = new NumericUpDown();
            QueueLength = new ProgressBar();
            LeaveCustomerCount = new Label();

            CashDeskName.AutoSize = true;
            CashDeskName.Location = new System.Drawing.Point(x, y);
            CashDeskName.Name = "label" + number;
            CashDeskName.Size = new System.Drawing.Size(35, 13);
            CashDeskName.TabIndex = number;
            CashDeskName.Text = cashDesk.ToString();

            Price.Location = new System.Drawing.Point(x + 70, y);
            Price.Name = "numericUpDown" + number;
            Price.Size = new System.Drawing.Size(120, 20);
            Price.TabIndex = number;
            Price.Maximum = 1000000000000000;

            QueueLength.Location = new System.Drawing.Point(x + 250, y);
            QueueLength.Maximum = cashDesk.MaxQueueLength;
            QueueLength.Name = "progressBar" + number;
            QueueLength.Size = new System.Drawing.Size(100, 23);
            QueueLength.TabIndex = number;
            QueueLength.Value = 0;

            LeaveCustomerCount.AutoSize = true;
            LeaveCustomerCount.Location = new System.Drawing.Point(x + 400, y);
            LeaveCustomerCount.Name = "label2" + number;
            LeaveCustomerCount.Size = new System.Drawing.Size(35, 13);
            LeaveCustomerCount.TabIndex = number;
            LeaveCustomerCount.Text = "";

            cashDesk.CheckClosed += CashDesk_CheckClosed;
        }

        private void CashDesk_CheckClosed(object sender, Check e)
        {
            // синтаксическая конструкция, помогает перекинуть из ассинхронного потока в основной
            Price.BeginInvoke((Action)delegate
            {
                Price.Value += e.Price;
                QueueLength.Value = cashDesk.Count;
                LeaveCustomerCount.Text = cashDesk.ExitCustomer.ToString();
            });
        }
    }
}
