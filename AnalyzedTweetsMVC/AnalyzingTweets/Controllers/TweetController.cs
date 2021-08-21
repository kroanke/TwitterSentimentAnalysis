using AnalyzingTweets.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace AnalyzingTweets.Controllers
{
    public class TweetController : Controller
    {
        double positive_number = 0;
        double neutral_number = 0;
        double negative_number = 0;
        double total_number = 0;

        private IMongoCollection<Tweet> collection;
        public TweetController()
        {
            var client = new MongoClient("mongodb+srv://dbuser:dbuser@cluster0.gyjbf.mongodb.net/Tweets?retryWrites=true&w=majority");
            IMongoDatabase db = client.GetDatabase("crypto");
            this.collection = db.GetCollection<Tweet>("cryptocurrency");
        }

        // GET: Tweet
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var model = collection.Find(FilterDefinition<Tweet>.Empty).ToList();

            foreach (var m in model)
            {
                if(m.polarity == 1)
                {
                    m.sentimentString = "Positive";
                    positive_number++;
                    total_number++;
                } else if (m.polarity == 0)
                {
                    m.sentimentString = "Neutral";
                    neutral_number++;
                    total_number++;
                } else if (m.polarity == -1)
                {
                    m.sentimentString = "Negative";
                    negative_number++;
                    total_number++;
                }
            }
            Tweet.positive_number = positive_number;
            Tweet.neutral_number = neutral_number;
            Tweet.negative_number = negative_number;
            Tweet.total_number = total_number;

            Tweet.pos_perc = (positive_number / total_number) * 100;
            Tweet.neu_perc = Convert.ToInt32((neutral_number / total_number) * 100);
            Tweet.neg_perc = Convert.ToInt32((negative_number / total_number) * 100);

            int pageSize = 25;
            int pageNumber = (page ?? 1);
            return View(model.ToPagedList(pageNumber, pageSize));
        }
    }
}