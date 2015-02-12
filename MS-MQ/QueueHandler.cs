using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Messaging;
using System.Threading;

namespace MS_MQ {
    public class QueueHandler {
        public MessageQueue Queue { get; private set; }
        public bool Monitoring { get; private set; }
        private Thread _monitoringThread;
        public event EventHandler<QueueHandlerEvent> MessageReceivedEvent;
        public ISet<String> Ids;

        public QueueHandler(MessageQueue queue) {
            Queue = queue;
            Ids = new HashSet<string>();
        }

        public void StartMonitorig() {
            Monitoring = true;
            if(_monitoringThread != null) {
                if(_monitoringThread.IsAlive)
                    return;
                _monitoringThread = new Thread(Start) { IsBackground = true };
                _monitoringThread.Start();
            } else {
                _monitoringThread = new Thread(Start) { IsBackground = true };
                _monitoringThread.Start();
            }
        }

        private void Start() {
            while(Monitoring) {
                try {
                    Message m = Queue.Peek();
                    if(m != null) {
                        if(!Ids.Contains(m.Id)) {
                            Ids.Add(m.Id);
                            QueueHandlerEvent qhe = new QueueHandlerEvent(m, Queue.Path) { MessageId = m.Id };
                            MessageReceivedEvent.Invoke(this, qhe);
                        }
                    }
                } catch(MessageQueueException e) {
                    Debug.WriteLine(e.Message + " - " + e.StackTrace);
                } catch(ArgumentException e) {
                    Debug.WriteLine(e.Message + " - " + e.StackTrace);
                }
            }
        }

        public void StopMinitoring() {
            Monitoring = false;
        }
    }
}