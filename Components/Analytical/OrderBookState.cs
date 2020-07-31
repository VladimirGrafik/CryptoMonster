namespace Components.Analytical
{
    public class OrderBookState
    {
        public string Winner { get; set; }
        public decimal MarginPercent { get; set; }
        public decimal WallsPosition { get; set; }
        public Double Asks { get; set; }
        public Double Bids { get; set; }


        public class Double
        {
            public decimal First { get; set; }
            public int WinsCount { get; set; }
            public decimal Wall { get; set; }
            public decimal Road { get; set; }
        }
    }
}