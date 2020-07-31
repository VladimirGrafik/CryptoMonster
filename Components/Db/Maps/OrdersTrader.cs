using System;
using FluentNHibernate.Mapping;

namespace Components.Db.Maps
{
    public class OrdersTrader
    {
        public virtual int Id { get; set; }
        public virtual string PairName { get; set; }
        public virtual string MainCurrency { get; set; }
        public virtual string SecondCurrency { get; set; }
        public virtual int? Priority { get; set; }
        public virtual string Characters { get; set; }
        public virtual string TraderType { get; set; }
        public virtual string Strategy { get; set; }
        public virtual int? ModeTrade { get; set; }
        public virtual DateTime? DateStart { get; set; }
        public virtual int? UTimeStart { get; set; }
        public virtual decimal? PriceStart { get; set; }
        public virtual decimal? BPriceStart { get; set; }

        public virtual int? RunBuy { get; set; }
        public virtual int? ForceBuy { get; set; }
        public virtual decimal? PriceBuy { get; set; }
        public virtual decimal? PercentBuy { get; set; }
        public virtual decimal? PricePushBuy { get; set; }
        public virtual decimal? PercentPushBuy { get; set; }
        public virtual decimal? BPriceTradeBuy { get; set; }
        public virtual decimal? BPriceBuy { get; set; }
        public virtual decimal? BPercentBuy { get; set; }
        public virtual decimal? BPricePushBuy { get; set; }
        public virtual decimal? BPercentPushBuy { get; set; }
        public virtual decimal? PercentStartBuy { get; set; }
        public virtual decimal? PercentLimitBuy { get; set; }
        public virtual decimal? PercentEndBuy { get; set; }
        public virtual decimal? PercentPrismMoveBuy { get; set; }
        public virtual decimal? PercentPrismBuy { get; set; }
        public virtual decimal? BalanceLimitBuy { get; set; }

        public virtual int? RunSell { get; set; }
        public virtual int? ForceSell { get; set; }
        public virtual decimal? PriceSell { get; set; }
        public virtual decimal? PercentSell { get; set; }
        public virtual decimal? PricePushSell { get; set; }
        public virtual decimal? PercentPushSell { get; set; }
        public virtual decimal? BPriceTradeSell { get; set; }
        public virtual decimal? BPriceSell { get; set; }
        public virtual decimal? BPercentSell { get; set; }
        public virtual decimal? BPricePushSell { get; set; }
        public virtual decimal? BPercentPushSell { get; set; }
        public virtual decimal? PercentStartSell { get; set; }
        public virtual decimal? PercentLimitSell { get; set; }
        public virtual decimal? PercentEndSell { get; set; }
        public virtual decimal? PercentPrismMoveSell { get; set; }
        public virtual decimal? PercentPrismSell { get; set; }
        public virtual decimal? BalanceLimitSell { get; set; }

        public virtual int? SafeDelete { get; set; }
        public virtual int? ForceDelete { get; set; }
        public virtual int? AutoTrade { get; set; }
        public virtual int? DatePushChange { get; set; }
        public virtual string NumberOrder { get; set; }
        public virtual decimal? Support { get; set; }
        public virtual decimal? Protection { get; set; }
        public virtual decimal? MainBalance { get; set; }
        public virtual decimal? SecondBalance { get; set; }
        public virtual int? PercentSignal { get; set; }
        public virtual decimal? BollLimit { get; set; }
        public virtual int? OrderBookTime { get; set; }
        public virtual string OrderBook { get; set; }
        public virtual string OrderBookSteps { get; set; }
        

        public virtual decimal? PriceP15 { get; set; }
        public virtual decimal? PriceP24 { get; set; }
        public virtual decimal? PricePr24 { get; set; }
        public virtual decimal? PricePr24Height { get; set; }
        public virtual decimal? VolumeDay { get; set; }
        public virtual decimal? VolumeP15 { get; set; }


        public virtual string BuyOrSell(int? mode, string pair, decimal? rate, decimal? quantity)
        {
            if (mode == 0 || mode == 1) return ProHub.MarketApi.Buy(pair, rate, quantity);
            if (mode == 2 || mode == 3) return ProHub.MarketApi.Sell(pair, rate, quantity);
            throw new Exception("Dont satisfactory mode!");
        }


        public virtual OrdersTrader Run(OrdersTrader obj, int? mode, int? value)
        {
            if (mode == 0 || mode == 1) obj.RunBuy = value;
            if (mode == 2 || mode == 3) obj.RunSell = value;
            return obj;
        }

        public virtual OrdersTrader Price(OrdersTrader obj, int? mode, decimal? value)
        {
            if (mode == 0 || mode == 1) obj.PriceBuy = value;
            if (mode == 2 || mode == 3) obj.PriceSell = value;
            return obj;
        }

        public virtual OrdersTrader Percent(OrdersTrader obj, int? mode, decimal? value)
        {
            if (mode == 0 || mode == 1) obj.PercentBuy = value;
            if (mode == 2 || mode == 3) obj.PercentSell = value;
            return obj;
        }

        public virtual OrdersTrader PricePush(OrdersTrader obj, int? mode, decimal? value)
        {
            if (mode == 0 || mode == 1) obj.PricePushBuy = value;
            if (mode == 2 || mode == 3) obj.PricePushSell = value;
            return obj;
        }


        public virtual OrdersTrader PercentPush(OrdersTrader obj, int? mode, decimal? value)
        {
            if (mode == 0 || mode == 1) obj.PercentPushBuy = value;
            if (mode == 2 || mode == 3) obj.PercentPushSell = value;
            return obj;
        }

        public virtual OrdersTrader BPriceTrede(OrdersTrader obj, int? mode, decimal? value)
        {
            if (mode == 0 || mode == 1) obj.BPriceTradeBuy = value;
            if (mode == 2 || mode == 3) obj.BPriceTradeSell = value;
            return obj;
        }

        public virtual OrdersTrader BPrice(OrdersTrader obj, int? mode, decimal? value)
        {
            if (mode == 0 || mode == 1) obj.BPriceBuy = value;
            if (mode == 2 || mode == 3) obj.BPriceSell = value;
            return obj;
        }

        public virtual OrdersTrader BPercent(OrdersTrader obj, int? mode, decimal? value)
        {
            if (mode == 0 || mode == 1) obj.BPercentBuy = value;
            if (mode == 2 || mode == 3) obj.BPercentSell = value;
            return obj;
        }

        public virtual OrdersTrader BPricePush(OrdersTrader obj, int? mode, decimal? value)
        {
            if (mode == 0 || mode == 1) obj.BPricePushBuy = value;
            if (mode == 2 || mode == 3) obj.BPricePushSell = value;
            return obj;
        }

        public virtual OrdersTrader BPercentPush(OrdersTrader obj, int? mode, decimal? value)
        {
            if (mode == 0 || mode == 1) obj.BPercentPushBuy = value;
            if (mode == 2 || mode == 3) obj.BPercentPushSell = value;
            return obj;
        }

        public virtual OrdersTrader PercentStart(OrdersTrader obj, int? mode, decimal? value)
        {
            if (mode == 0 || mode == 1) obj.PercentStartBuy = value;
            if (mode == 2 || mode == 3) obj.PercentStartSell = value;
            return obj;
        }

        public virtual OrdersTrader PercentLimit(OrdersTrader obj, int? mode, decimal? value)
        {
            if (mode == 0 || mode == 1) obj.PercentLimitBuy = value;
            if (mode == 2 || mode == 3) obj.PercentLimitSell = value;
            return obj;
        }

        public virtual OrdersTrader PercentEnd(OrdersTrader obj, int? mode, decimal? value)
        {
            if (mode == 0 || mode == 1) obj.PercentEndBuy = value;
            if (mode == 2 || mode == 3) obj.PercentEndSell = value;
            return obj;
        }

        public virtual OrdersTrader PercentPrismMove(OrdersTrader obj, int? mode, decimal? value)
        {
            if (mode == 0 || mode == 1) obj.PercentPrismMoveBuy = value;
            if (mode == 2 || mode == 3) obj.PercentPrismMoveSell = value;
            return obj;
        }

        public virtual OrdersTrader PercentPrism(OrdersTrader obj, int? mode, decimal? value)
        {
            if (mode == 0 || mode == 1) obj.PercentPrismBuy = value;
            if (mode == 2 || mode == 3) obj.PercentPrismSell = value;
            return obj;
        }

        public virtual OrdersTrader BalanceLimit(OrdersTrader obj, int? mode, decimal? value)
        {
            if (mode == 0 || mode == 1) obj.BalanceLimitBuy = value;
            if (mode == 2 || mode == 3) obj.BalanceLimitSell = value;
            return obj;
        }
    }


    public class OrdersTraderMap : ClassMap<OrdersTrader>
    {
        public OrdersTraderMap()
        {
            Table("ordersTrader" + ProHub.MarketName);
            Id(x => x.Id);
            Map(x => x.PairName);
            Map(x => x.Strategy);
            Map(x => x.Priority);
            Map(x => x.Characters);
            Map(x => x.TraderType);
            Map(x => x.ModeTrade);
            Map(x => x.DateStart);
            Map(x => x.UTimeStart);
            Map(x => x.PriceStart);
            Map(x => x.BPriceStart);

            Map(x => x.RunBuy);
            Map(x => x.ForceBuy);
            Map(x => x.PriceBuy);
            Map(x => x.PercentBuy);
            Map(x => x.PricePushBuy);
            Map(x => x.PercentPushBuy);
            Map(x => x.BPriceTradeBuy);
            Map(x => x.BPriceBuy);
            Map(x => x.BPercentBuy);
            Map(x => x.BPricePushBuy);
            Map(x => x.BPercentPushBuy);
            Map(x => x.PercentStartBuy);
            Map(x => x.PercentLimitBuy);
            Map(x => x.PercentEndBuy);
            Map(x => x.PercentPrismMoveBuy);
            Map(x => x.PercentPrismBuy);
            Map(x => x.BalanceLimitBuy);

            Map(x => x.RunSell);
            Map(x => x.ForceSell);
            Map(x => x.PriceSell);
            Map(x => x.PercentSell);
            Map(x => x.PricePushSell);
            Map(x => x.PercentPushSell);
            Map(x => x.BPriceTradeSell);
            Map(x => x.BPriceSell);
            Map(x => x.BPercentSell);
            Map(x => x.BPricePushSell);
            Map(x => x.BPercentPushSell);
            Map(x => x.PercentStartSell);
            Map(x => x.PercentLimitSell);
            Map(x => x.PercentEndSell);
            Map(x => x.PercentPrismMoveSell);
            Map(x => x.PercentPrismSell);
            Map(x => x.BalanceLimitSell);

            Map(x => x.SafeDelete);
            Map(x => x.ForceDelete);
            Map(x => x.AutoTrade);
            Map(x => x.DatePushChange);
            Map(x => x.NumberOrder);
            Map(x => x.MainCurrency);
            Map(x => x.SecondCurrency);
            Map(x => x.Support);
            Map(x => x.Protection);
            Map(x => x.MainBalance);
            Map(x => x.SecondBalance);
            Map(x => x.PercentSignal);
            Map(x => x.BollLimit);
            Map(x => x.OrderBookTime);
            Map(x => x.OrderBook);
            Map(x => x.OrderBookSteps);
            

            Map(x => x.PriceP15);
            Map(x => x.PriceP24);
            Map(x => x.PricePr24);
            Map(x => x.PricePr24Height);
            Map(x => x.VolumeDay);
            Map(x => x.VolumeP15);
        }
    }
}