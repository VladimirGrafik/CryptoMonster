using Components.Analytical;
using Components.Db.Maps;

namespace Thinker.Strategies
{
    public class JumpUp : Strategy
    {
        public JumpUp(OrderBookMachine orderBookMachine) : base(orderBookMachine)
        {
        }

        protected override bool RunStrategy(OrderBookMachine orderBookMachine)
        {
            OrdersTrader.Strategy = "JumpUp";
            
            return orderBookMachine.OrderBookState.Bids.WinsCount >
                   orderBookMachine.OrderBookState.Asks.WinsCount &&
                   orderBookMachine.CatchPair.PriceP15 > 5;
        }
    }
}