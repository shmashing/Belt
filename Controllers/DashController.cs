using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using BeltExam.Models;
using Microsoft.AspNetCore.Identity;
using BeltExam.Factory;

namespace BeltExam.Controllers
{
    public class DashController : Controller
    {

        private readonly UserFactory userFactory;
        private readonly AuctionFactory auctionFactory;
        public DashController(UserFactory connection, AuctionFactory connection2) {
            userFactory = connection;
            auctionFactory = connection2;
        }

        [HttpGet]
        [Route("home")]
        public IActionResult Home(){
            if(HttpContext.Session.GetInt32("userId")==null){
                return Redirect("/");
            }
            var auctions = auctionFactory.GetAllAuctions();
            foreach(var auction in auctions){
                if(auction.TimeRemaining.Days <= 0 && auction.TimeRemaining.Hours <= 0){
                    auctionFactory.FinalizeTransaction(auction);
                }
            }
            var user = userFactory.GetUserForHtml((int)HttpContext.Session.GetInt32("userId"));
            ViewBag.user = user;
            ViewBag.auctions = auctions;
            return View("Home");
        }
        [HttpGet]
        [Route("show_auction/{id}")]
        public IActionResult ShowAuction(int id){
            if(HttpContext.Session.GetInt32("userId")==null){
                return Redirect("/");
            }
            if(TempData["bidError"] != null){
                ViewBag.bidError = TempData["bidError"];
            }
            var auction = auctionFactory.GetAuctionById(id);
            var bid = auctionFactory.GetAuctionBid(id);
            ViewBag.auction = auction;
            ViewBag.bid = bid;
            return View("ShowAuction");
        }
        [HttpGet]
        [Route("new_auction")]
        public IActionResult AddAuction(){
           if(HttpContext.Session.GetInt32("userId")==null){
                return Redirect("/");
            }
            return View("AddAuction"); 
        }

        [HttpPost]
        [Route("new_auction")]
        public IActionResult MakeAuction(AuctionViewModel newAuct){
            if(HttpContext.Session.GetInt32("userId")==null){
                return Redirect("/");
            }
            if(ModelState.IsValid){
                if(newAuct.EndDate < DateTime.Now){
                    ViewBag.DateError = "End Date must be after today";
                    return View("AddAuction");
                }
                if(newAuct.Bid < 0){
                    ViewBag.BidError = "Starting bid must be greater than 0";
                    return View("AddAuction");
                }
                Auction auction = new Auction{
                    Product = newAuct.Product,
                    Bid = newAuct.Bid,
                    Description = newAuct.Description,
                    EndDate = newAuct.EndDate,
                    UserId = (int) HttpContext.Session.GetInt32("userId"),
                };
                auctionFactory.CreateAuction(auction);
                return RedirectToAction("Home");
            }
            return View("AddAuction");
        }

        [HttpPost]
        [Route("add_bid/{auction_id}")]
        public IActionResult AddBid(int auction_id, float bidAmount){
            if(HttpContext.Session.GetInt32("userId")==null){
                return Redirect("/");
            }
            int userId = (int) HttpContext.Session.GetInt32("userId");
            var user = userFactory.GetUserById(userId);
            Auction auction = auctionFactory.GetAuctionById(auction_id);
            if(bidAmount <= auction.Bid){
                TempData["bidError"] = "Bid must be larger than current bid amount";
                return RedirectToAction("ShowAuction", new {id =auction_id});
            } else if(bidAmount > user.Wallet){
                TempData["bidError"] = "You do not have sufficient funds to make that bid :(";
                return RedirectToAction("ShowAuction", new {id =auction_id});
            }

            Bid newBid = new Bid(){
                BidAmount = bidAmount,
                UserId = userId,
                AuctionId = auction_id,
            };
            auctionFactory.MakeNewBid(newBid);
            return RedirectToAction("ShowAuction", new{id=auction_id});
        }
        [HttpGet]
        [Route("delete/{id}")]
        public IActionResult DeleteAuction(int id){
            if(HttpContext.Session.GetInt32("userId")==null){
                return Redirect("/");
            }
            var auction = auctionFactory.GetAuctionById(id);
            int userId = (int) HttpContext.Session.GetInt32("userId"); 

            if(auction.UserId == userId){
                auctionFactory.DeleteAuction(id);            
            }
            return RedirectToAction("Home");
        }

    }
}