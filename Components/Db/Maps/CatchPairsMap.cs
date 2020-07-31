using FluentNHibernate.Mapping;

namespace Components.Db.Maps
{
    public class CatchPairsMap : ClassMap<CatchPairs>
    {
        public CatchPairsMap()
        {
            Table("catchPairs");
            Id(x => x.Id);
            Map(x => x.PairName);
            Map(x => x.MarketName);
            Map(x => x.DateTime);
            Map(x => x.LastPrice);
            Map(x => x.PriceP15);
            Map(x => x.PriceP24);
            Map(x => x.VolumeDay);
            Map(x => x.VolumeP15);
            Map(x => x.UTime);
//            References(e => e.Store);
        }
    }
}