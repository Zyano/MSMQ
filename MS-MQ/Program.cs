using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace MS_MQ {
    class Program {
        static void Main(string[] args) {
            QueueContainer qh = new QueueContainer();
            qh.MessageReceivedEvent += QhOnMessageReceivedEvent;
            var q = CreateQueues(qh);


            Console.WriteLine("=== Queues created ===");
            foreach(KeyValuePair<string, MessageQueue> messageQueue in q) {
                Console.WriteLine(messageQueue.Key + " - " + messageQueue.Value);
            }
            Console.WriteLine();
            Console.WriteLine("=== Sending message to Queue ===");
            Stock s = new Stock() { Ask = 100.0, Change = 1.52, Index = "Dow Jones", LastValue = 98.48, Offer = 99.5, StockName = "AT&T Inc" ,TimeStamp = DateTime.Now};
            var sendingQ = q.First().Value;
            Console.WriteLine("=== SENDING TO: "+sendingQ.Path + " ===");
            sendingQ.Send(s,"Stock message");
            
            Console.ReadLine();
        }


        private static void QhOnMessageReceivedEvent(object sender, QueueHandlerEvent queueHandlerEvent) {
            Console.WriteLine("New message is ready to be receieved! message ID: " + queueHandlerEvent.MessageId + " on queue path: " + queueHandlerEvent.QueuePath);
        }

        public static IDictionary<String,MessageQueue> CreateQueues(QueueContainer qh) {
            IDictionary<String, String> paths = new Dictionary<string, string>();

            paths.Add("Dow Jones",@".\Private$\StockQueue_Dow_Jones");
            paths.Add("Nasdaq",@".\Private$\StockQueue_Nasdaq");
            paths.Add("S&P 500",@".\Private$\StockQueue_SnP500");
            var q = qh.CreateQueues(paths);
            return q;
        }
    }
}
