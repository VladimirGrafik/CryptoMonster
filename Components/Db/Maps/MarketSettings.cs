using FluentNHibernate.Mapping;

namespace Components.Db.Maps
{
    public class MarketSettings
    {
        public virtual int Id { get; set; }
        public virtual string MarketName { get; set; }
        public virtual int? Enable { get; set; }
        public virtual int? AutoTrade { get; set; }
        public virtual int? StopDump { get; set; }
        public virtual int? StopLoss { get; set; }
        public virtual decimal? PercentStopLoss { get; set; }
        public virtual decimal? BalanceStopLoss { get; set; }
        public virtual int? CatchPairsLimit { get; set; }
        public virtual string OrderByCatchPairs { get; set; }
        public virtual string OrderByCatchPairsDesc { get; set; }
        public virtual int? ApiKey { get; set; }
        public virtual string ApiKey1 { get; set; }
        public virtual string ApiSecret1 { get; set; }
        public virtual string ApiKey2 { get; set; }
        public virtual string ApiSecret2 { get; set; }
    }

    public class MarketSettingsMap : ClassMap<MarketSettings>
    {
        public MarketSettingsMap()
        {
            Table("marketSettings");
            Id(x => x.Id);
            Map(x => x.MarketName);
            Map(x => x.Enable);
            Map(x => x.AutoTrade);
            Map(x => x.StopDump);
            Map(x => x.StopLoss);
            Map(x => x.PercentStopLoss);
            Map(x => x.BalanceStopLoss);
            Map(x => x.CatchPairsLimit);
            Map(x => x.OrderByCatchPairs);
            Map(x => x.OrderByCatchPairsDesc);
            Map(x => x.ApiKey);
            Map(x => x.ApiKey1);
            Map(x => x.ApiSecret1);
            Map(x => x.ApiKey2);
            Map(x => x.ApiSecret2);
        }
    }
}