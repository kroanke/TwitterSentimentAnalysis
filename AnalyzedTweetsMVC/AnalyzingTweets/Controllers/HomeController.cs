using AnalyzingTweets.Models;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace AnalyzingTweets.Controllers
{
    public class HomeController : Controller
    {

        private IMongoCollection<Tweet> collection;
        double positive_number = 0;
        double neutral_number = 0;
        double negative_number = 0;
        double total_number = 0;

        int pos_perc = 0;
        int neu_perc = 0;
        int neg_perc = 0;

        int firstWeekPos = 0;
        int firstWeekNeg = 0;
        int firstWeekNeu = 0;

        int secondWeekPos = 0;
        int secondWeekNeg = 0;
        int secondWeekNeu = 0;

        List<DateTime> createdAt = new List<DateTime>();
        List<int> polarity = new List<int>();
        public HomeController()
        {
            var client = new MongoClient("mongodb+srv://dbuser:dbuser@cluster0.gyjbf.mongodb.net/Tweets?retryWrites=true&w=majority");
            IMongoDatabase db = client.GetDatabase("crypto");
            this.collection = db.GetCollection<Tweet>("cryptocurrency");

            var model = collection.Find(FilterDefinition<Tweet>.Empty).ToList();

            foreach (var m in model)
            {
                if (m.polarity == 1)
                {
                    positive_number++;
                    total_number++;
                }
                else if (m.polarity == 0)
                {
                    neutral_number++;
                    total_number++;
                }
                else if (m.polarity == -1)
                {
                    negative_number++;
                    total_number++;
                }
                createdAt.Add(m.created_at);
                polarity.Add(m.polarity);
            }

            pos_perc = Convert.ToInt32(positive_number / total_number * 100);
            neg_perc = Convert.ToInt32(negative_number / total_number * 100);
            neu_perc = Convert.ToInt32(neutral_number / total_number * 100);
        }

        public ActionResult Index()
        {
            List<DataPoint> dataPoints = new List<DataPoint>();

            dataPoints.Add(new DataPoint("Positive", pos_perc));
            dataPoints.Add(new DataPoint("Negative", neg_perc));
            dataPoints.Add(new DataPoint("Neutral", neu_perc));

            ViewBag.DataPoints = JsonConvert.SerializeObject(dataPoints);

            
            
            List<DataPoint> dataPointPos = new List<DataPoint>();
            List<DataPoint> dataPointNeg = new List<DataPoint>();
            List<DataPoint> dataPointNeu = new List<DataPoint>();
            

            
            for (int i = 0; i < createdAt.Count(); i++)
            {
                if(createdAt[i].Day < 9)
                {
                    if (polarity[i] == -1)
                    {
                        firstWeekNeg++;
                    }
                    else if (polarity[i] == 0)
                    {
                        firstWeekNeu++;
                    }
                    else if (polarity[i] == 1)
                    {
                        firstWeekPos++;
                    }
                }

                else if (createdAt[i].Day > 9 && createdAt[i].Day < 16)
                {
                    if (polarity[i] == -1)
                    {
                        secondWeekNeg++;
                    }
                    else if (polarity[i] == 0)
                    {
                        secondWeekNeu++;
                    }
                    else if (polarity[i] == 1)
                    {
                        secondWeekPos++;
                    }
                }


            }

            dataPointNeg.Add(new DataPoint("02/08/2021 - 09/08/2021", firstWeekNeg));
            dataPointNeg.Add(new DataPoint("09/08/2021 - 16/08/2021", secondWeekNeg));


            dataPointNeu.Add(new DataPoint("02/08/2021 - 09/08/2021", firstWeekNeu));
            dataPointNeu.Add(new DataPoint("09/08/2021 - 16/08/2021", secondWeekNeu));


            dataPointPos.Add(new DataPoint("02/08/2021 - 09/08/2021", firstWeekPos));
            dataPointPos.Add(new DataPoint("09/08/2021 - 16/08/2021", secondWeekPos));


            ViewBag.DataPointsPos = JsonConvert.SerializeObject(dataPointPos);
            ViewBag.DataPointsNeg = JsonConvert.SerializeObject(dataPointNeg);
            ViewBag.DataPointsNeu = JsonConvert.SerializeObject(dataPointNeu);



            return View();
        }
        public ActionResult Chart1()
        {
            

            var key = new Chart(width: 300, height: 300)
                .AddTitle("Employee 1")
                .AddSeries(
                    chartType: "Column",
                    name: "Emp1",
                    xValue: new[] { "Positive", "Negative", "Neutral"},
                    yValues: new[] { pos_perc, neg_perc, neu_perc}
                );

            
            return File(key.ToWebImage().GetBytes(), "image/jpeg");
        }
        public ActionResult About()
        {
            

            return View();
        }

        public ActionResult Contact()
        {
            

            return View();
        }
    }
}