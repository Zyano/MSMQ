using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace MS_MQ {
    public class QueueContainer {
        public IDictionary<String, MessageQueue> queues { get; private set; }
        private List<QueueHandler> _handlers;
 
        public QueueContainer() {
            queues = new Dictionary<string, MessageQueue>();
            _handlers = new List<QueueHandler>();
        }

        public event EventHandler<QueueHandlerEvent> MessageReceivedEvent; 

        /// <summary>
        /// Attempts to obtain the queue by Path name otherwise it creates the Queue with the specified path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public MessageQueue GetOrCreateQueueByPath(String path) {
            MessageQueue queue;
            queues.TryGetValue(path, out queue);
            if(queue == null) {
                if(MessageQueue.Exists(path)) {
                    queue = new MessageQueue(path);
                }
                else {
                    try {
                        queue = MessageQueue.Create(path);
                        queues.Add(path, queue);
                    }
                    catch(MessageQueueException e) {
                        Debug.WriteLine(e.Message + " - " + e.StackTrace);
                    }
                }
            }
            if(queue != null) {
                QueueHandler qh = new QueueHandler(queue);
                qh.MessageReceivedEvent += QhOnMessageReceivedEvent;
                qh.StartMonitorig();
            }
            return queue;
        }

        private void QhOnMessageReceivedEvent(object sender, QueueHandlerEvent queueHandlerEvent) {
            MessageReceivedEvent.Invoke(this,queueHandlerEvent);
        }

        /// <summary>
        /// Get's the public Queue by name otherwise returns null if the queue wasn't found.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>MessageQueue or Null</returns>
        public MessageQueue GetPublicMessageQueueByName(String name) {
            var qs = MessageQueue.GetPublicQueues();
            MessageQueue mq = qs.FirstOrDefault((queue) => queue.QueueName.Equals(name));
            return mq;
        }

        /// <summary>
        /// Takes a Dictonary with the KEY being the label and Value being the PATH.
        /// returns a dictonary with the queues requested. The dictonary key is the label and the value is the MessageQueue.
        /// </summary>
        /// <param name="queueLabelsPaths"></param>
        /// <returns></returns>
        public IDictionary<String, MessageQueue> CreateQueues(IDictionary<String,String> queueLabelsPaths) {
            IDictionary<String, MessageQueue> queues = new Dictionary<string, MessageQueue>(queueLabelsPaths.Count);
            foreach(KeyValuePair<String,String> qlp in queueLabelsPaths) {
                var q = GetOrCreateQueueByPath(qlp.Value);
                q.Label = qlp.Key;
                queues.Add(q.Label, q);    
            }
            
            return queues;
        }

        /// <summary>
        /// Gets the message from the stream by path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public Message GetMessageFromQueueByPath(String path) {
            MessageQueue mq;
            Message m = null;
            queues.TryGetValue(path, out mq);
            if(mq != null) {
                try {
                    m = mq.Receive();
                }
                catch(MessageQueueException e) {
                    Debug.WriteLine(e.Message + " - " + e.StackTrace);
                }
                
            }
            return m;
        }

    }
}
