using FluentNHibernate.Mapping;

namespace Components.Db.Maps
{
    public class DefaultPairs
    {
        public virtual int Id { get; set; }
        public virtual string PairName { get; set; }
        public virtual string PairLabel { get; set; }
        public virtual string MarketName{ get; set; }
        public virtual int? Enable { get; set; } = 0;
        public virtual string MainCurrency { get; set; }
        public virtual string SecondCurrency { get; set; }
        public virtual int? Rating { get; set; } = 1;

    }
    public class DefaultPairsMap : ClassMap<DefaultPairs>
    {
        public DefaultPairsMap()
        {
            Table("defaultPairs");
            Id(x => x.Id);
            Map(x => x.PairName);
            Map(x => x.PairLabel);
            Map(x => x.MarketName);
            Map(x => x.Enable);
            Map(x => x.MainCurrency);
            Map(x => x.SecondCurrency);
            Map(x => x.Rating);
        }
    }
}