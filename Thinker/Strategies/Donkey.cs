using Components;
using Components.Analytical;
using Components.Db.Maps;
using Components.Markets;

namespace Thinker.Strategies
{
    public class Donkey : Strategy
    {
        public Donkey(OrderBookMachine orderBookMachine) : base(orderBookMachine)
        {
        }

        protected override bool RunStrategy(OrderBookMachine orderBookMachine)
        {
            OrdersTrader.Strategy = "Donkey";
            
            return orderBookMachine.OrderBookState.Bids.WinsCount > 50 &&
                   orderBookMachine.OrderBookState.Asks.WinsCount < 10 &&
                   orderBookMachine.CatchPair.PrismPDayHeight < 30 || ProHub.MarketName == "BTF";
        }
    }
}