# MarketDataService



This is a web API that provides real-time and historical prices for financial instruments. It connects to the Fintacharts API and is built with Clean Architecture on ASP.NET Core 9.



Main Features



Real-Time Data: Uses a background WebSocket connection to get live prices from the market.

Fast Cache: Saves live prices in memory so the API answers instantly.

Auto-Setup: When you start it for the first time, it automatically creates the database and downloads the necessary data.

Docker Ready: Runs easily using Docker Compose.



How to Run the Project



You only need Docker Desktop installed on your computer.



1\. Open your terminal in the main project folder (where the "docker-compose.yml" file is).

2\. Run this command:

&#x20;  bash

&#x20;  docker-compose up -d --build

3\. Important: On the first run, please wait about 10-20 seconds. The app needs this time to create the database and download the first list of instruments from the API.

