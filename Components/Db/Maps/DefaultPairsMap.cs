using FluentNHibernate.Mapping;

namespace Components.Db.Maps
{
    public class DefaultPairsMap : ClassMap<DefaultPairs>
    {
        public DefaultPairsMap()
        {
            Table("defaultPairs");
            Id(x => x.Id);
            Map(x => x.PairName);
            Map(x => x.MarketName);
            Map(x => x.Enable);
            Map(x => x.FirstCurrency);
            Map(x => x.SecondCurrency);
            Map(x => x.Rating);
        }
    }
}