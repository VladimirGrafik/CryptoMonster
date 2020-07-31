using FluentNHibernate.Mapping;

namespace Components.Db.Maps
{
    public class DefaultMarkets
    {
        public virtual int Id { get; set; }
        public virtual string MarketName { get; set; }
        public virtual string MarketFullName { get; set; }
        public virtual int? Enable { get; set; }
    }
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