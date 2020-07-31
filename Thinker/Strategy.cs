using System;
using Components.Analytical;
using Components.Db;
using Components.Db.Maps;
using Components.Helpers;

namespace Thinker
{
    public abstract class Strategy
    {
        protected OrdersTrader OrdersTrader { get; set; } = new OrdersTrader();

        protected Strategy(OrderBookMachine orderBookMachine)
        {
            if (RunStrategy(orderBookMachine)) WriteOrdersTrader(orderBookMachine);
        }

        protected abstract bool RunStrategy(OrderBookMachine orderBookMachine);

        private bool WriteOrdersTrader(OrderBookMachine orderBookMachine)
        {
            using (var cmTr = CmTrading.OpenSession())
            {
                using (var trans = cmTr.BeginTransaction())
                {
                    OrdersTrader.PairName = orderBookMachine.CatchPair.PairName;
                    OrdersTrader.MainCurrency = orderBookMachine.CatchPair.MainCurrency;
                    OrdersTrader.SecondCurrency = orderBookMachine.CatchPair.SecondCurrency;
                    OrdersTrader.Priority = 7;
                    OrdersTrader.TraderType = "Speeder";
                    OrdersTrader.DateStart = DateTime.Now;
                    OrdersTrader.UTimeStart = UnixTime.Now();

                    OrdersTrader.PriceStart = orderBookMachine.OrderBookState.Bids.First;
                    OrdersTrader.PriceP15 = orderBookMachine.CatchPair.PriceP15;
                    OrdersTrader.PriceP24 = orderBookMachine.CatchPair.PricePDay;
                    OrdersTrader.PricePr24 = orderBookMachine.CatchPair.PrismPDay;
                    OrdersTrader.PricePr24Height = orderBookMachine.CatchPair.PrismPDayHeight;
                    OrdersTrader.VolumeDay = orderBookMachine.CatchPair.VolumeDay;
                    OrdersTrader.VolumeP15 = orderBookMachine.CatchPair.VolumeP15;
                    cmTr.Save(OrdersTrader);
                    trans.Commit();
                }
            }
            return true;
        }
    }
}