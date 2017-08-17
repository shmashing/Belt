using System.Collections.Generic;
using System.Linq;
using Dapper;
using System.Data;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Options;
using BeltExam.Models;
using Microsoft.AspNetCore.Identity;
using System;
 
namespace BeltExam.Factory
{
    public class AuctionFactory : IFactory<User>
    {
        private readonly IOptions<MySqlOptions> MySqlConfig;
        public AuctionFactory(IOptions<MySqlOptions> config)
        {
            MySqlConfig = config;

        }
        internal IDbConnection Connection
        {
            get {
                return new MySqlConnection(MySqlConfig.Value.ConnectionString);
            }
        }

        public void CreateAuction(Auction newAuct){
            using (IDbConnection dbConnection = Connection){
                string query = "INSERT INTO auctions (Product, Description, Bid, EndDate, UserId)" +
                                @"VALUES (@Product, @Description, @Bid, @EndDate, @UserId)";

                dbConnection.Open();
                dbConnection.Execute(query, newAuct);
            }
        }
        public void DeleteAuction(int id){
            using (IDbConnection dbConnection = Connection){
                string query = $"DELETE FROM auctions WHERE (Id = {id})";
                dbConnection.Open();
                dbConnection.Execute(query);
            }            
        }

        public List<Auction> GetAllAuctions(){
            using (IDbConnection dbConnection = Connection){
                string query = "SELECT * FROM auctions ORDER BY EndDate";
                dbConnection.Open();
                var auctions = dbConnection.Query<Auction>(query).ToList();
                foreach (var auction in auctions){
                    query = $"SELECT FirstName FROM users WHERE (Id = {auction.UserId})";
                    auction.User = dbConnection.Query<User>(query).SingleOrDefault();
                    auction.TimeRemaining = auction.EndDate - DateTime.Now;
                }
                return auctions;
            }
        }
        public Auction GetAuctionById(int id){
            using (IDbConnection dbConnection = Connection){
                string query = $"SELECT * FROM auctions WHERE (Id = {id})";
                dbConnection.Open();
                var auction = dbConnection.Query<Auction>(query).SingleOrDefault();
                auction.User = dbConnection.Query<User>($"SELECT FirstName, LastName FROM users WHERE (Id = {auction.UserId})").SingleOrDefault();
                auction.TimeRemaining = auction.EndDate - DateTime.Now;
                return auction;
            }
        }
        public Bid GetAuctionBid(int id){
            using (IDbConnection dbConnection = Connection){
                string query = $"SELECT * FROM bids WHERE bids.AuctionId = {id}";
                dbConnection.Open();
                var bid = dbConnection.Query<Bid>(query).SingleOrDefault();
                if(bid != null){
                    bid.User = dbConnection.Query<User>($"SELECT FirstName, LastName FROM users WHERE (Id = {bid.UserId})").SingleOrDefault();
                }
                return bid;
            }            
        }
        public void MakeNewBid(Bid newBid){
            using (IDbConnection dbConnection = Connection){
                string query = $"DELETE FROM bids WHERE (AuctionId = {newBid.AuctionId})";
                dbConnection.Open();
                dbConnection.Execute(query);

                query = "INSERT INTO bids (AuctionId, UserId, BidAmount) " +
                        @"VALUES (@AuctionId, @UserId, @BidAmount)";

                dbConnection.Execute(query, newBid);

                query = $"UPDATE auctions SET Bid = {newBid.BidAmount} WHERE (Id = {newBid.AuctionId})";
                dbConnection.Execute(query);
            }
        }
        public void FinalizeTransaction(Auction auction){
            using (IDbConnection dbConnection = Connection){
                string query = $"SELECT * FROM bids WHERE bids.AuctionId = {auction.Id}";
                dbConnection.Open();
                var bid = dbConnection.Query<Bid>(query).SingleOrDefault();

                var seller = dbConnection.Query<User>($"SELECT * FROM users WHERE (Id = {auction.UserId})").SingleOrDefault();
                if(seller != null){
                    seller.Wallet += auction.Bid;
                    System.Console.WriteLine(seller.Wallet);
                    query = $"UPDATE users SET Wallet = {seller.Wallet} WHERE (Id = {seller.Id})";
                    dbConnection.Execute(query);
                    }

                var buyer = dbConnection.Query<User>($"SELECT * FROM users WHERE (Id = {bid.UserId})").SingleOrDefault();
                if(buyer != null){
                    buyer.Wallet -= auction.Bid;
                    System.Console.WriteLine(buyer.Wallet);
                    query = $"UPDATE users SET Wallet = {buyer.Wallet} WHERE (Id = {buyer.Id})";
                    dbConnection.Execute(query);
                }
                query = $"DELETE FROM bids WHERE (AuctionId = {bid.AuctionId})";
                dbConnection.Execute(query);

                query = $"DELETE FROM auctions WHERE (Id = {auction.Id})";
                dbConnection.Execute(query);

            }
        }
    }
}