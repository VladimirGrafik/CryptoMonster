using System;
using System.Collections.Generic;
using System.Linq;
using Components;
using Components.Db;
using Components.Db.Maps;
using Components.Markets;
using Components.Markets.AdaptersApi;
using NHibernate.Linq;

namespace Trader
{
    public class BalancesManager
    {
        private readonly OrdersTrader _ordersTrader;
        private readonly List<Balances> _balances;
        private static List<MainCurrencies> _mainCurrencies;
        public Balances Main { get; }
        public Balances Second { get; }
        public decimal? ModeBalance { get; }
        public MainCurrencies MainCurrency { get; }

        public BalancesManager(List<Balances> balances, OrdersTrader ordersTrader, List<OrdersTrader> ordersTraders)
        {
            _ordersTrader = ordersTrader;
            _balances = balances;
            _mainCurrencies = _mainCurrencies ?? SetMainCurrencies(ordersTraders);
            Main = _balances.SingleOrDefault(x => x.CurrencyName == ordersTrader.MainCurrency);
            Second = _balances.SingleOrDefault(x => x.CurrencyName == ordersTrader.SecondCurrency);
            ModeBalance = (ordersTrader.ModeTrade == 0 || ordersTrader.ModeTrade == 1) ? Main?.Common :
                (ordersTrader.ModeTrade == 2 || ordersTrader.ModeTrade == 3) ? Second?.Common :
                throw new Exception("Dont satisfactory Mode!");
            MainCurrency = _mainCurrencies.SingleOrDefault(x => x.CurrencyName == _ordersTrader.MainCurrency);
        }

        private List<MainCurrencies> SetMainCurrencies(List<OrdersTrader> ordersTraders)
        {
            var mainCurrencies = new List<MainCurrencies>();
            foreach (var mainCurrency in ProHub.MarketApi.MainCurrencies)
            {
                var ordersTradersMain = ordersTraders.Where(x => x.MainCurrency == mainCurrency.CurrencyName).ToList();
                if (ordersTradersMain.Count == 0) continue;
                var mainBalance = _balances.SingleOrDefault(x => x.CurrencyName == mainCurrency.CurrencyName);
                decimal? sumBalanceBuy = 0;
                decimal? sumBalanceSell = 0;
                var countStarted = 0;
                foreach (var ordersTrader in ordersTradersMain)
                {
                    if (ordersTrader.ModeTrade == 1)
                        sumBalanceBuy += ordersTrader.BalanceLimitBuy;
                    else if (ordersTrader.ModeTrade == 2)
                        if (ordersTrader.BalanceLimitSell != 0 && ordersTrader.PriceSell != 0)
                            sumBalanceSell += ordersTrader.BalanceLimitSell / ordersTrader.PriceSell;
                        else throw new Exception("Dont satisfactory Mode!");
                    countStarted++;
                }
                //!!!!!!!!!!!!!!!!!!!!!!!!!! ChangeCurrency
                mainCurrency.Common = mainBalance?.Common + sumBalanceSell;
                mainCurrency.Available = mainBalance?.Common - sumBalanceBuy;
                mainCurrency.Returning = sumBalanceSell;
                mainCurrency.CountStarted = countStarted;
                mainCurrencies.Add(mainCurrency);
            }
            return mainCurrencies;
        }

        public decimal? GetBalanceLimitBuy()
        {
            var maxFromVolumeDay = (_ordersTrader.VolumeDay != 0) ? _ordersTrader.VolumeDay / 100 : 0;
            decimal? buyBalanceLimit;
            var maxFromCountStarted = MainCurrency?.Available /*/ (3 - MainCurrency?.CountStarted)*/;
            if (MainCurrency?.CountStarted >= 2)
                buyBalanceLimit = (maxFromVolumeDay < MainCurrency.Available)
                    ? maxFromVolumeDay
                    : MainCurrency.Available;
            else
                buyBalanceLimit = (maxFromVolumeDay < MainCurrency?.Available)
                    ? maxFromVolumeDay
                    : maxFromCountStarted;
            using (var cmTr = CmTrading.OpenSession())
            using (var trans = cmTr.BeginTransaction())
                if (buyBalanceLimit > MainCurrency?.MinOrderQuantityLimit)
                {
                    var mainCurrencyOrdersTrader = cmTr.Query<OrdersTrader>()
                        .SingleOrDefault(x => x.PairName == _ordersTrader.PairName);
                    if (mainCurrencyOrdersTrader != null)
                        mainCurrencyOrdersTrader.BalanceLimitBuy = buyBalanceLimit;
                    cmTr.Update(mainCurrencyOrdersTrader);
                    MainCurrency.CountStarted++;
                    MainCurrency.Available -= buyBalanceLimit;
                    MainCurrency.Returning += buyBalanceLimit;
                    trans.Commit();
                    return buyBalanceLimit;
                }
            return null;
        }

        public int? CheckStopLoss()
        {


            return 1;

        }
    }
}