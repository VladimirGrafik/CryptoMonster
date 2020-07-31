using FluentNHibernate.Mapping;

namespace Components.Db.Maps
{
    public class DefaultMarketsMap : ClassMap<DefaultMarkets>
    {
        public DefaultMarketsMap()
        {
            Table("defaultMarkets");
            Id(x => x.Id);
            Map(x => x.MarketName);
            Map(x => x.MarketFullName);
            Map(x => x.Enable);
        }
    }
}