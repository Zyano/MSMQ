using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS_MQ {
    public class QueueHandlerEvent : EventArgs {
        public DateTime EventReceivedTime { get; set; }
        public Object MessageObject { get; set; }
        public String QueuePath { get; set; }
        public String MessageId { get; set; }

        public QueueHandlerEvent(Object mObject,String path) {
            MessageObject = mObject;
            EventReceivedTime = DateTime.Now;
            QueuePath = path;
        }
    }
}
