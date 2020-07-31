using System;
using Components.Analytical;
using Components.Db.Maps;

namespace Thinker.Strategies
{
    public class Decline : Strategy
    {
        public Decline(OrderBookMachine orderBookMachine) : base(orderBookMachine)
        {
        }

        protected override bool RunStrategy(OrderBookMachine orderBookMachine)
        {
            OrdersTrader.Strategy = "Decline";
            
            return orderBookMachine.OrderBookState.Bids.WinsCount > 30 &&
                   orderBookMachine.OrderBookState.Asks.WinsCount < 10 &&
                   orderBookMachine.CatchPair.PrismPDayHeight < 10 &&
                   orderBookMachine.CatchPair.PriceP15Sma < -2;
        }
    }
}