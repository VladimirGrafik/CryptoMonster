using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;

namespace Components.Db
{
    public class CmThinking
    {
//        private static string _factoryName;
//        private static string _server;
//        public const string CmTh = "cmThinking";
//        public const string CmTr = "cmTrading";
//        public const string CmSe = "cmSetting";

        public CmThinking()
        {
            InitializeSessionFactory();
        }

        private static ISessionFactory _sessionFactory;

        private static ISessionFactory SessionFactory
        {
            get
            {
                if (_sessionFactory == null)
                    InitializeSessionFactory();
                return _sessionFactory;
            }
        }

        private static void InitializeSessionFactory()
        {
            _sessionFactory = Fluently.Configure().Database(
                    MySQLConfiguration.Standard.ConnectionString(
                        cs => cs.Server("192.168.33.203;port=33066").Database("cmThinking").Username("userm")
                            .Password("PIsfasistP1o2I3+=_-userm")
                    )
                ).Mappings(m => m.FluentMappings.AddFromAssemblyOf<NHelper>())
                .BuildSessionFactory();
        }

        public static ISession OpenSession()
        {
//            GetServer(db);
//            _factoryName = db;
            return SessionFactory.OpenSession();
        }

//        private static void GetServer(string db)
//        {
//            switch (db)
//            {
//                case CmTh:
//                    _server = new string[]
//                        {"cmThinking", "192.168.33.203", "33066", "userm", "PIsfasistP1o2I3+=_-userm", "1"};
//                    break;
//                case CmTr:
//                    _server = new string[]
//                        {"cmTrading", "192.168.33.203", "33066", "userm", "PIsfasistP1o2I3+=_-userm", "2"};
//                    break;
//                case CmSe:
//                    _server = new string[]
//                        {"cmSetting", "192.168.33.203", "33066", "userm", "PIsfasistP1o2I3+=_-userm", "3"};
//                    break;
//                default:
//                    throw new Exception("Не существует введенного сервера!");
//            }
//        }
    }
}