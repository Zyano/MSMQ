using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS_MQ {
    public class Stock {
        public String StockName { get; set; }
        public Double LastValue { get; set; }
        public Double Change { get; set; }
        public Double Offer { get; set; }
        public Double Ask { get; set; }
        public String Index { get; set; }
        public DateTime TimeStamp { get; set; }

        public override string ToString() {
            string s = StockName + " - " + Index + " - " + TimeStamp;
            return s;
        }
    }
}
