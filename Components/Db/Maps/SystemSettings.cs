using FluentNHibernate.Mapping;

namespace Components.Db.Maps
{
    public class SystemSettings
    {
        public virtual int Id { get; set; }
        public virtual string NameSettings { get; set; }
        public virtual bool EnableHunter { get; set; }
        public virtual bool EnableThinker { get; set; }
        public virtual bool EnableTrader { get; set; }
    }
    public class SystemSettingsMap : ClassMap<SystemSettings>
    {
        public SystemSettingsMap()
        {
            Table("systemSettings");
            Id(x => x.Id).Column("id");
            Map(x => x.NameSettings);
            Map(x => x.EnableHunter);
            Map(x => x.EnableThinker);
            Map(x => x.EnableTrader);
        }
    }
}