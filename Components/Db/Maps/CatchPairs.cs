using System;
using FluentNHibernate.Mapping;

namespace Components.Db.Maps
{
    public class CatchPairs
    {
        public virtual int Id { get; set; }
        public virtual string PairName { get; set; }
        public virtual string MainCurrency { get; set; }
        public virtual string SecondCurrency { get; set; }
        public virtual DateTime DateTime { get; set; }
        public virtual decimal? LastPrice { get; set; }
        public virtual decimal? PriceP15 { get; set; }
        public virtual decimal? PriceP60 { get; set; }
        public virtual decimal? PricePDay { get; set; }
        public virtual decimal? PriceP15Sma { get; set; }
        public virtual decimal? PriceP60Sma { get; set; }
        public virtual decimal? PriceP15Ema { get; set; }
        public virtual decimal? PriceP60Ema { get; set; }
        public virtual decimal? MaxBidsDay { get; set; }
        public virtual decimal? MinAsksDay { get; set; }
        public virtual decimal? PrismPDay { get; set; }
        public virtual decimal? PrismPDayHeight { get; set; }
        public virtual decimal? VolumeP15 { get; set; }
        public virtual decimal? VolumeDay { get; set; }
        public virtual int? UTime { get; set; }
    }

    public class CatchPairsMap : ClassMap<CatchPairs>
    {
        public CatchPairsMap()
        {
            Table("catchPairs" + ProHub.MarketName);
            Id(x => x.Id);
            Map(x => x.PairName);
            Map(x => x.MainCurrency);
            Map(x => x.SecondCurrency);
            Map(x => x.DateTime);
            Map(x => x.LastPrice);
            Map(x => x.PriceP15);
            Map(x => x.PriceP60);
            Map(x => x.PricePDay);
            Map(x => x.PriceP15Sma);
            Map(x => x.PriceP60Sma);
            Map(x => x.PriceP15Ema);
            Map(x => x.PriceP60Ema);
            Map(x => x.MaxBidsDay);
            Map(x => x.MinAsksDay);
            Map(x => x.PrismPDay);
            Map(x => x.PrismPDayHeight);
            Map(x => x.VolumeP15);
            Map(x => x.VolumeDay);
            Map(x => x.UTime);
        }
    }
}