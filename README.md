# CryptoMonster
Trading bot for trade to the crypto markets with adaptivity for different API.
Three process starting from daemon.
1.Module Hunter get the main information about pairs and put it to the database. (every 1-2 seconds)
2.Module Thincker take data from database table and handeled, filtered best interesting pairs, after that put it in to trade table.(every 5-8 seconds)
3.Module Trader get pairs from trade table and manipulate with buy and sell crypto pairs.(every 0.5-1 seconds)
