using System;
using System.Linq;
using System.Web;
using System.IO;
using System.Net;
using System.Text;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Configuration;
using System.Collections.Specialized;

namespace TwitterFeed.Controllers
{
    public class Feed
    {
        public const string oauthVersion = "1.0";
        //public string oauthVersion = ConfigurationManager.AppSettings.Get("oauthVersion");
        public const string oauthSignatureMethod = "HMAC-SHA1";
        public const string consumerKey = "3MylzS4IXxPTx5sy6pXmpbvMj";
        public const string consumerKeySecret = "Okhq3HuXFCCZRGt4ii8eZSbstElkxrYaHyam3YAJu6Hx05TWEu";
        public const string accessToken = "131087058-lr6naKGZplrAEcw2V9J3HoPW03cjzaEvPZZSrPta";
        public const string accessTokenSecret = "DasLJLYfWUv9Vpxx5CwWTOrCw0rf7rsfSZkYg1d48BGW7";

        public Feed()
        {
            //this.ConsumerKey = consumerKey;
            //this.ConsumerKeySecret = consumerKeySecret;
            //this.AccessToken = accessToken;
            //this.AccessTokenSecret = accessTokenSecret;
        }

        //public string ConsumerKey { set; get; }
        //public string ConsumerKeySecret { set; get; }
        //public string AccessToken { set; get; }
        //public string AccessTokenSecret { set; get; }



        public string GetTweets(string screenName, int count)
        {
            try
            {
                var resourceUrl = "https://api.twitter.com/1.1/statuses/user_timeline.json";

                var response = GetResponse(resourceUrl, Method.GET, screenName, count);

                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string GetResponse(string resourceUrl, Method method, string screenName, int count)
        {
            try
            {
                ServicePointManager.Expect100Continue = false;

                var authHeader = CreateHeader(resourceUrl, method, screenName, count);

                var urlParameters = "screen_name=" + Uri.EscapeDataString(screenName) + "&count=" + count;
                resourceUrl += "?" + urlParameters;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(resourceUrl);
                request.Headers.Add("Authorization", authHeader);
                request.Method = method.ToString();
                request.ContentType = "application/x-www-form-urlencoded";


                WebResponse response = request.GetResponse();
                string responseData = new StreamReader(response.GetResponseStream()).ReadToEnd();


                return responseData;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string CreateHeader(string resourceUrl, Method method, string screenName, int count)
        {
            try
            {
                var oauthNonce = CreateOauthNonce();
                var oauthTimestamp = CreateOAuthTimestamp();
                var oauthSignature = CreateOauthSignature(resourceUrl, method, oauthNonce, oauthTimestamp, screenName, count);

                // create the request header
                var headerFormat = "OAuth oauth_nonce=\"{0}\", oauth_signature_method=\"{1}\", " +
                                   "oauth_timestamp=\"{2}\", oauth_consumer_key=\"{3}\", " +
                                   "oauth_token=\"{4}\", oauth_signature=\"{5}\", " +
                                   "oauth_version=\"{6}\"";

                var authHeader = string.Format(headerFormat,
                                        Uri.EscapeDataString(oauthNonce),
                                        Uri.EscapeDataString(oauthSignatureMethod),
                                        Uri.EscapeDataString(oauthTimestamp),
                                        Uri.EscapeDataString(consumerKey),
                                        Uri.EscapeDataString(accessToken),
                                        Uri.EscapeDataString(oauthSignature),
                                        Uri.EscapeDataString(oauthVersion)
                                );


                return authHeader;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string CreateOauthSignature(string resourceUrl, Method method, string oauthNonce, string oauthTimestamp, string screenName, int count)
        {
            try
            {
                // create oauth signature
                var baseFormat = "count={7}&oauth_consumer_key={0}&oauth_nonce={1}&oauth_signature_method={2}" +
                                "&oauth_timestamp={3}&oauth_token={4}&oauth_version={5}&screen_name={6}";

                var baseString = string.Format(baseFormat,
                                            consumerKey,
                                            oauthNonce,
                                            oauthSignatureMethod,
                                            oauthTimestamp,
                                            accessToken,
                                            oauthVersion,
                                             Uri.EscapeDataString(screenName),
                                             Uri.EscapeDataString(count.ToString())
                                            );

                baseString = string.Concat("GET&", Uri.EscapeDataString(resourceUrl), "&", Uri.EscapeDataString(baseString));

                var compositeKey = string.Concat(Uri.EscapeDataString(consumerKeySecret),
                                        "&", Uri.EscapeDataString(accessTokenSecret));

                string oauth_signature;
                using (HMACSHA1 hasher = new HMACSHA1(ASCIIEncoding.ASCII.GetBytes(compositeKey)))
                {
                    oauth_signature = Convert.ToBase64String(
                        hasher.ComputeHash(ASCIIEncoding.ASCII.GetBytes(baseString)));
                }

                return oauth_signature;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static string CreateOAuthTimestamp()
        {
            try
            {
                var timeSpan = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                var timestamp = Convert.ToInt64(timeSpan.TotalSeconds).ToString();

                return timestamp;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string CreateOauthNonce()
        {
            try
            {
                return Convert.ToBase64String(new ASCIIEncoding().GetBytes(DateTime.Now.Ticks.ToString()));
            }
            catch(Exception ex)
            {
                throw ex;
            }
            
        }
    }

    public enum Method
    {
        POST,
        GET
    }
}