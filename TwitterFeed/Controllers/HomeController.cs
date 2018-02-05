using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;

namespace TwitterFeed.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }
       
        [HttpGet] 
        public async Task<ActionResult> GetFeed()
        {
            try
            {
                var feed = new Feed();
                List<object> data = new List<object>();
                var feedData = feed.GetTweets("salesforce", 10);
                JArray jdat = JArray.Parse(feedData);
                
                for (int i = 0; i < jdat.Count(); i++)
                {
                    data.Add(new
                    {
                        name = jdat[i]["user"]["name"].ToString(),
                        screenname = jdat[i]["user"]["screen_name"].ToString(),
                        image = jdat[i]["user"]["profile_image_url_https"].ToString(),
                        text = jdat[i]["text"].ToString(),
                        retweet = jdat[i]["retweet_count"].ToString(),
                        created = jdat[i]["created_at"].ToString(),
                        media = jdat[i]["entities"]["media"] == null ? "" : jdat[i]["entities"]["media"][0]["media_url_https"].ToString()
                    });
                }

                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
