using System;
using FluentNHibernate.Mapping;

namespace Components.Db.Maps
{
    public class AnalyticHistory
    {
        //CatchPairs
        public virtual int Id { get; set; }
        public virtual string PairName { get; set; }
        public virtual string FirstCurrency { get; set; }
        public virtual string SecondCurrency { get; set; }
        public virtual string MarketName { get; set; }
        public virtual DateTime DateTime { get; set; }
        public virtual decimal? PriceP15 { get; set; }
        public virtual decimal? PriceP24 { get; set; }
        public virtual decimal? LastPrice { get; set; }
        public virtual decimal? MaxBids { get; set; }
        public virtual decimal? MinAsks { get; set; }
        public virtual decimal? HighPercent { get; set; }
        public virtual decimal? VolumeDay { get; set; }
        public virtual decimal? VolumeP15 { get; set; }
        
        //OrderBook
        public virtual string OrderBook { get; set; }
        
        //OrderBookState
        public virtual string Winner { get; set; }
        public virtual decimal? AsksWins { get; set; }
        public virtual decimal? BidsWins { get; set; }
        public virtual decimal? MarginPercent { get; set; }
        public virtual decimal? AsksFirst { get; set; }
        public virtual decimal? BidsFirst { get; set; }
        
        //UTime
        public virtual int UTime { get; set; }
    }
    public class AnalyticHistoryMap : ClassMap<AnalyticHistory>
    {
        public AnalyticHistoryMap()
        {
            Table("analyticHistory");

            //CatchPairs
            Id(x => x.Id);
            Map(x => x.PairName);
            Map(x => x.FirstCurrency);
            Map(x => x.SecondCurrency);
            Map(x => x.MarketName);
            Map(x => x.DateTime);
            Map(x => x.PriceP15);
            Map(x => x.PriceP24);
            Map(x => x.LastPrice);
            Map(x => x.MaxBids);
            Map(x => x.MinAsks);
            Map(x => x.HighPercent);
            Map(x => x.VolumeDay);
            Map(x => x.VolumeP15);

            //OrderBook
            Map(x => x.OrderBook);

            //OrderBookState
            Map(x => x.Winner);
            Map(x => x.AsksWins);
            Map(x => x.BidsWins);
            Map(x => x.MarginPercent);
            Map(x => x.AsksFirst);
            Map(x => x.BidsFirst);

            //UTime
            Map(x => x.UTime);
        }
    }
}