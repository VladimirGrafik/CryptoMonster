using FluentNHibernate.Mapping;

namespace Components.Db.Maps
{
    public class SystemSettingsMap : ClassMap<SystemSettings>
    {
        public SystemSettingsMap()
        {
            Table("systemSettings");
            Id(x => x.Id).Column("id");
            Map(x => x.NameSettings).Column("nameSettings");
            Map(x => x.EnableHunter).Column("enableHunter");
            Map(x => x.EableThinker).Column("enableThinker");
            Map(x => x.EnableTrader).Column("enableTrader");
            Map(x => x.StartNewTrade).Column("startNewTrade");
        }
    }
}