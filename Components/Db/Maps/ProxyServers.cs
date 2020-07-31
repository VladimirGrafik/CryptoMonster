using FluentNHibernate.Mapping;

namespace Components.Db.Maps
{
    public class ProxyServers
    {
        public virtual int Id { get; set; }
        public virtual string Ip { get; set; }
        public virtual int Port { get; set; }
        public virtual int Enable { get; set; }
        public virtual int CountFalse { get; set; }
        public virtual int TimeFalse { get; set; }
        public virtual string Message { get; set; }
    }

    public class ProxyServersMap : ClassMap<ProxyServers>
    {
        public ProxyServersMap()
        {
            Table("proxyServers");
            Id(x => x.Id);
            Map(x => x.Ip);
            Map(x => x.Port);
            Map(x => x.Enable);
            Map(x => x.CountFalse);
            Map(x => x.TimeFalse);
            Map(x => x.Message);
        }
    }
}