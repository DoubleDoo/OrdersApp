

namespace testkontur.OrderClasses
{
    public class Order
    {
        public string type { get; set; }
        public string orderer { get; set; }
        public string federal { get; set; }
        public string city { get; set; }
        public string date { get; set; }
        public string info { get; set; }
        public string price { get; set; }
        public string link { get; set; }

        public Order(string _link)
        {
            link = _link;
            type = "";
            orderer = "";
            federal = "";
            city = "";
            date = "";
            info = "";
            price = "";
        }

        public override string ToString()
        {
            return "(" + type + ")\n(" + orderer + ")\n(" + federal + ")\n(" + city + ")\n(" + date + ")\n(" + info + ")\n(" + price + ")\n(" + link + ")\n";
        }
        public bool equ(Order obj)
        {
            if (!this.price.Equals("НМЦ не указывается")&&!this.price.Equals("0"))
                return obj.price.Equals(this.price) && obj.date.Equals(this.date);
            else return obj.info.Equals(this.info) && obj.date.Equals(this.date);
        }
    }
}


 
