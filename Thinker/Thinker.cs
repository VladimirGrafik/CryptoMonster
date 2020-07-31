using System;
using System.Collections.Generic;
using System.Linq;
using Components;
using Components.Analytical;
using Components.Db;
using Components.Db.Maps;
using Components.Helpers;
using Components.Markets;
using NHibernate.Linq;
using Thinker.Strategies;

namespace Thinker
{
    public class Thinker
    {
        private readonly MarketApi _marketApi;
        private readonly string _marketName;
        private readonly string _pairConnector;
        private readonly List<CatchPairs> _catchPairs;
        private readonly List<CatchPairs> _filteredCatchPairs;
        private readonly int _uTimeIdNow;

        public Thinker()
        {
            _marketApi = ProHub.MarketApi;
            _marketName = _marketApi.MarketName;
            _pairConnector = _marketApi.PairConnector;
            _catchPairs = GetCatchPairs();
            _filteredCatchPairs = FilteringCatchPairs();
            _uTimeIdNow = UnixTime.Now();
        }

        public void RunThinker()
        {
            foreach (var catchPair in _filteredCatchPairs)
            {
                try
                {
                    var orderBook = _marketApi.GetOrderBook(catchPair.PairName);
                    var orderBookMachine = new OrderBookMachine(orderBook, catchPair, _uTimeIdNow);
                    var runAnalytic = orderBookMachine.RunAnalytic();
                    RunStrategies(orderBookMachine);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        private List<CatchPairs> FilteringCatchPairs()
        {
            var filteredCatchPairs = _catchPairs
                .Where(x => x.MainCurrency == "BTC" && x.LastPrice > 0.00000100M && x.VolumeDay > 10).ToList();
            return filteredCatchPairs;
        }

        private List<CatchPairs> GetCatchPairs()
        {
            using (var cmTh = CmThinking.OpenSession())
            {
                using (var trans = cmTh.BeginTransaction())
                {
                    var uTimeId = cmTh.Query<CatchPairs>().Max(x => x.UTime);
                    if (uTimeId < _uTimeIdNow - 30)
                        throw new Exception("CatchPairs non exist");
                    var catchPairs = cmTh.Query<CatchPairs>().Where(x => x.UTime == uTimeId).ToList();
                    trans.Commit();
                    return catchPairs;
                }
            }
        }

        public void RunStrategies(OrderBookMachine orderBookMachine)
        {
            var decline = new Decline(orderBookMachine);
            var jumpUp = new JumpUp(orderBookMachine);
            var donkey = new Donkey(orderBookMachine);
        }
    }
}
