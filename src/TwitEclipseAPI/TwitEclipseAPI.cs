// ********************************************
// TwitEclipseAPI - .Net Twitter API Wrapper  *
// Developed by Ryan Alford                   *
// ********************************************

// TwitEclipseAPI.cs
// This class contains the methods used to 
//   connect to Twitter and process the REST API calls

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Xml.Linq;
using System.Web;
using System.Xml;
using System.Collections.Specialized;

namespace TwitterAPI
{
    public class TwitEclipseAPI : OAuthBase
    {
        #region Local Variables and Properties

        private string m_status = string.Empty;
        private string m_userName = string.Empty;
        private string m_password = string.Empty;
        private string m_oAuthConsumerKey = string.Empty;
        private string m_oAuthConsumerSecret = string.Empty;
        private string m_oAuthAccessToken = string.Empty;
        private string m_oAuthAccessTokenSecret = string.Empty;
        private string m_urlShortnerLogin = string.Empty;
        private string m_urlShortnerAPIKey = string.Empty;
        private string m_apiVersion = "2.0.1";

        private string oAuthSignatureMethod = string.Empty;
        private string oAuthSignature = string.Empty;
        private string oAuthTimeStamp = string.Empty;
        private string oAuthNonce = string.Empty;
        private string m_oAuthRequestToken = string.Empty;
        private string m_oAuthRequestTokenSecret = string.Empty;
        private string oAuthUnauthorizedToken = string.Empty;
        
        private RateLimit m_rateLimit;

        #endregion

        #region Public Enums

        public enum UpdateDeliveryDevices
        {
            NONE = 0,
            SMS = 1,
            IM = 2
        }

        public enum ProfileColors
        {
            BackgroundColor = 0,
            TextColor = 1,
            LinkColor = 2,
            SidebarFillColor = 3,
            SidebarBorderColor = 4
        }

        public enum HttpMethod
        {
            GET, 
            POST,
            NONE
        }

        public enum UserTimeLineParameters
        {
            SinceID,
            MaxID,
            Count
        }

        public enum FriendTimeLineParameters
        {
            MaxID,
            SinceID, 
            Count
        }

        public enum UserMentionParameters
        {
            MaxID,
            SinceID,
            Count
        }

        public enum DirectMessageParameters
        {
            ReceivedByScreenName,
            ReceivedBySinceID,
            ReceivedByMaxID,
            ReceivedByCount,
            SentByScreenName,
            SentBySinceID,
            SentByMaxID,
            SentByCount
        }

        #endregion

        #region Public Properties

        public string OAuthAccessToken
        {
            get { return m_oAuthAccessToken; }
            set { m_oAuthAccessToken = value; }
        }

        public string OAuthAccessTokenSecret
        {
            get { return m_oAuthAccessTokenSecret; }
            set { m_oAuthAccessTokenSecret = value; }
        }

        public string OAuthConsumerSecret
        {
            get { return m_oAuthConsumerSecret; }
            set { m_oAuthConsumerSecret = value; }
        }

        public string OAuthConsumerKey
        {
            get { return m_oAuthConsumerKey; }
            set { m_oAuthConsumerKey = value; }
        }

        public RateLimit RateLimit
        {
            get { return m_rateLimit; }
            set { m_rateLimit = value; }
        }

        public string UserName
        {
            get { return m_userName; }
            set { m_userName = value; }
        }

        public string Password
        {
            get { return m_password; }
            set { m_password = value; }
        }

        public string UrlShortnerLogin
        {
            get { return m_urlShortnerLogin; }
            set { m_urlShortnerLogin = value; }
        }

        public string UrlShortnerAPIKey
        {
            get { return m_urlShortnerAPIKey; }
            set { m_urlShortnerAPIKey = value; }
        }

        public string ApiVersion
        {
            get { return m_apiVersion; }
            set { m_apiVersion = value; }
        }

        #endregion

        #region Private Properties

        private bool IsOAuthEnabled
        {
            get { 
                if (string.IsNullOrEmpty(m_oAuthAccessToken) || string.IsNullOrEmpty(m_oAuthAccessTokenSecret))
                    return false;
                else 
                    return true;
            }
        }

        #endregion

        #region Constructor

        public TwitEclipseAPI()
        {

        }

        public TwitEclipseAPI(string userName, string password)
        {
            m_userName = userName;
            m_password = password;

            //if (!VerifyCredentials())
            //    ShowErrorMessage("Login Failed");
            //else
            //    m_isAuthenticated = true;
        }

        #endregion

        #region Private Methods

        #region OAuth methods

        private string OAuthGetToken()
        {
            return OAuthGetToken("");
        }

        private string OAuthWebGetToken(string callbackURL)
        {
            string responseString = string.Empty;

            try
            {
                string outURL = string.Empty;
                string outQueryString = string.Empty;

                OAuthParameters p = new OAuthParameters();
                p.ConsumerKey = m_oAuthConsumerKey;
                p.HttpMethod = HttpMethod.GET;
                p.ConsumerSecret = m_oAuthConsumerSecret;
                p.Nonce = GenerateOAuthNonce();
                p.SignatureType = SignatureTypes.HMACSHA1;
                p.Timestamp = GenerateOAuthTimestamp();
                p.Uri = new Uri("http://api.twitter.com/oauth/request_token");

                string sig = this.GenerateSignature(p, out outURL, out outQueryString);

                outQueryString += "&oauth_signature=" + Common.UrlEncode(sig);

                if (outQueryString.Length > 0)
                    outURL += "?";

                string newURL = outURL + outQueryString + "&" + callbackURL;

                return newURL;
            }
            catch (Exception ex)
            {
                responseString = "***ERROR :: " + ex.Message;
            }

            return responseString;
        }

        private string OAuthGetToken(string PIN)
        {
            string responseString = string.Empty;

            try
            {
                string outURL = string.Empty;
                string outQueryString = string.Empty;

                OAuthParameters p = new OAuthParameters();
                p.ConsumerKey = m_oAuthConsumerKey;
                p.HttpMethod = HttpMethod.GET;
                p.ConsumerSecret = m_oAuthConsumerSecret;
                p.Nonce = GenerateOAuthNonce();
                p.SignatureType = SignatureTypes.HMACSHA1;
                p.Timestamp = GenerateOAuthTimestamp();

                if (!string.IsNullOrEmpty(PIN))
                {
                    p.Token = m_oAuthRequestToken;
                    p.TokenSecret = m_oAuthRequestTokenSecret;
                    p.PIN = PIN;
                    p.Uri = new Uri("http://api.twitter.com/oauth/access_token");
                }
                else
                {
                    p.Uri = new Uri("http://api.twitter.com/oauth/request_token");
                }

                string sig = this.GenerateSignature(p, out outURL, out outQueryString);

                outQueryString += "&oauth_signature=" + Common.UrlEncode(sig);

                if (outQueryString.Length > 0)
                    outURL += "?";

                string newURL = outURL + outQueryString;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(newURL);
                request.Method = "GET";
                request.ServicePoint.Expect100Continue = false;
                request.ContentType = "application/x-www-form-urlencoded";

                using (WebResponse response = request.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        responseString = reader.ReadToEnd();
                    }
                }

                if (!string.IsNullOrEmpty(responseString))
                {
                    Dictionary<string, string> nv = Common.ParseQueryString(responseString);
                    if (nv["oauth_token"] != null)
                    {
                        if (string.IsNullOrEmpty(PIN))
                        {
                            m_oAuthRequestToken = nv["oauth_token"];
                            responseString = "http://api.twitter.com/oauth/authorize?oauth_token=" + m_oAuthRequestToken;
                        }
                        else
                        {
                            m_oAuthAccessToken = nv["oauth_token"];
                            m_userName = nv["screen_name"];
                            responseString = m_oAuthAccessToken;
                        }
                    }

                    if (nv["oauth_token_secret"] != null)
                    {
                        if (string.IsNullOrEmpty(PIN))
                            m_oAuthRequestTokenSecret = nv["oauth_token_secret"];
                        else
                            m_oAuthAccessTokenSecret = nv["oauth_token_secret"];
                    }                    
                }
            }
            catch (Exception ex)
            {
                responseString = "***ERROR :: " + ex.Message;
            }

            return responseString;
        }

        private string OAuthPostRequest(string url, string postData)
        {
            string responseString = string.Empty;

            WebResponse response = null;
            try
            {
                Uri uri = new Uri(url);

                string outURL = string.Empty;
                string outQueryString = string.Empty;

                OAuthParameters p = new OAuthParameters();
                p.ConsumerKey = m_oAuthConsumerKey;
                p.HttpMethod = HttpMethod.POST;
                p.ConsumerSecret = m_oAuthConsumerSecret;
                p.Nonce = GenerateOAuthNonce();
                p.Status = postData;
                p.SignatureType = SignatureTypes.HMACSHA1;
                p.Timestamp = GenerateOAuthTimestamp();
                p.Token = m_oAuthAccessToken;
                p.TokenSecret = m_oAuthAccessTokenSecret;
                p.Uri = new Uri(url);

                string sig = this.GenerateSignature(p, out outURL, out outQueryString);

                outQueryString += "&oauth_signature=" + Common.UrlEncode(sig);

                if (outQueryString.Length > 0)
                {
                    outURL += "?";
                }

                string newURL = outURL + outQueryString;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(newURL);
                request.Method = "POST";
                request.ServicePoint.Expect100Continue = false;

                request.ContentLength = 0;

                request.ContentType = "application/x-www-form-urlencoded";
                //request.Headers.Add("content-length", postData.Length.ToString());

                response = request.GetResponse();
                
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    responseString = reader.ReadToEnd();
                }

                if (response.Headers["X-RateLimit-Limit"] != null)
                {
                    int limit = int.Parse(response.Headers["X-RateLimit-Limit"]);
                    int remaining = int.Parse(response.Headers["X-RateLimit-Remaining"]);
                    long resetTime = long.Parse(response.Headers["X-RateLimit-Reset"]);

                    m_rateLimit = new RateLimit(limit, remaining, resetTime);
                }
            }
            catch (Exception ex)
            {
                responseString = "***ERROR :: " + ex.Message;
            }

            return responseString;
        }

        private string OAuthGetRequest(string url)
        {
            string responseString = string.Empty;

            WebResponse response = null;
            try
            {
                Uri uri = new Uri(url);

                string outURL = string.Empty;
                string outQueryString = string.Empty;

                OAuthParameters p = new OAuthParameters();
                p.ConsumerKey = m_oAuthConsumerKey;
                p.HttpMethod = HttpMethod.GET;
                p.ConsumerSecret = m_oAuthConsumerSecret;
                p.Nonce = GenerateOAuthNonce();
                p.SignatureType = SignatureTypes.HMACSHA1;
                p.Timestamp = GenerateOAuthTimestamp();
                p.Token = m_oAuthAccessToken;
                p.TokenSecret = m_oAuthAccessTokenSecret;
                p.Uri = new Uri(url);

                string sig = this.GenerateSignature(p, out outURL, out outQueryString);

                outQueryString += "&oauth_signature=" + Common.UrlEncode(sig);

                if (outQueryString.Length > 0)
                {
                    outURL += "?";
                }

                string newURL = outURL + outQueryString;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(newURL);
                request.Method = "GET";
                request.ServicePoint.Expect100Continue = false;
                request.ContentType = "application/x-www-form-urlencoded";

                response = request.GetResponse();
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    responseString = reader.ReadToEnd();
                }

                if (response.Headers["X-RateLimit-Limit"] != null)
                {
                    int limit = int.Parse(response.Headers["X-RateLimit-Limit"]);
                    int remaining = int.Parse(response.Headers["X-RateLimit-Remaining"]);
                    long resetTime = long.Parse(response.Headers["X-RateLimit-Reset"]);

                    m_rateLimit = new RateLimit(limit, remaining, resetTime);
                }
            }
            catch (Exception ex)
            {
                responseString = "***ERROR :: " + ex.Message;
            }

            return responseString;
        }

        private string GenerateOAuthNonce()
        {
            return Guid.NewGuid().ToString();
        }

        private string GenerateOAuthTimestamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }

        #endregion

        private bool DetermineRateLimit()
        {
            ////////string response = string.Empty;
            ////////string url = "http://api.twitter.com/1/account/rate_limit_status.xml";

            ////////if (string.IsNullOrEmpty(m_oAuthAccessToken) || string.IsNullOrEmpty(m_oAuthAccessTokenSecret))
            ////////    response = Get(url);
            ////////else
            ////////    response = OAuthGetRequest(url);

            ////////m_rateLimit = GetRateLimit(response);

            ////////return (m_rateLimit.RemainingHits > 0);

            if (m_rateLimit == null)
                return true;

            return (m_rateLimit.RemainingHits > 0);
        }

        private string PerformRequest(string method, string url)
        {
            string responseString = string.Empty;
            WebResponse response = null;
            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                request.Method = method;
                //request.ContentLength = url.Length;
                request.Credentials = new NetworkCredential(m_userName, m_password);
                request.ServicePoint.Expect100Continue = false;

                if (string.Compare(method, "POST", true) == 0)
                {
                    if (!string.IsNullOrEmpty(m_status))
                    {
                        using (Stream stream = request.GetRequestStream())
                        {
                            byte[] data = data = Encoding.ASCII.GetBytes("status=" + m_status);
                            stream.Write(data, 0, data.Length);
                        }
                    }
                }

                response = request.GetResponse();
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    responseString = reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                responseString = "***ERROR :: " + ex.Message;
            }

            return responseString;
        }

        private string Post(string url)
        {
            return PerformRequest("POST", url);
        }

        private string Get(string url)
        {
            return PerformRequest("GET", url);
        }

        private void ShowErrorMessage(string error)
        {
            if (error.Contains("401"))
            {
                throw new Exception("Username or Password is incorrect.");
            }
            else if (error.Contains("403"))
            {
                throw new Exception("Update Limit has been reached");
            }
            else if (error.Contains("404"))
            {
                throw new Exception("Page does not exist");
            }
            else if (error.Contains("406"))
            {
                throw new Exception("Invalid format in Search request");
            }
            else if (error.Contains("500"))
            {
                throw new Exception("Twitter is broken.  Calm down, they will have it up soon.");
            }
            else if (error.Contains("502"))
            {
                throw new Exception("Twitter is broken.  Calm down, they will have it up soon.");
            }
            else if (error.Contains("503"))
            {
                throw new Exception("Twitter is being overloaded request.  Please try again.");
            }
        }

        private List<UserStatus> GetStatusInformation(string response)
        {
            List<UserStatus> users = null;

            if (!response.Contains("ERROR") && !string.IsNullOrEmpty(response))
            {
                XDocument document = XDocument.Parse(response, LoadOptions.None);
                XNamespace georss = "http://www.georss.org/georss";
                var query = from e in document.Root.Descendants("status")
                            select new UserStatus
                            {
                                User = new User(e, "status"),
                                StatusCreatedAt = Common.ParseDateTime(e.Element("created_at").Value),
                                StatusID = Int64.Parse(e.Element("id").Value),
                                Text = e.Element("text").Value,
                                Source = e.Element("source").Value,
                                Truncated = (!string.IsNullOrEmpty(e.Element("truncated").Value)) ? bool.Parse(e.Element("truncated").Value) : false,
                                InReplyToStatusID = (!string.IsNullOrEmpty(e.Element("in_reply_to_status_id").Value)) ? Int64.Parse(e.Element("in_reply_to_status_id").Value) : 0,
                                InReplyToUserID = (!string.IsNullOrEmpty(e.Element("in_reply_to_user_id").Value)) ? Int64.Parse(e.Element("in_reply_to_user_id").Value) : 0,
                                Favorited = (!string.IsNullOrEmpty(e.Element("favorited").Value)) ? bool.Parse(e.Element("favorited").Value) : false,
                                InReplyToScreenName = e.Element("in_reply_to_screen_name").Value,
                                Geo = (!string.IsNullOrEmpty(e.Element("geo").Value)) ? e.Element("geo").Element(georss + "point").Value : string.Empty
                            };

                users = (from u in query
                         where u.Text != ""
                         orderby u.StatusCreatedAt descending
                         select u).ToList();
            }
            else
            {
                ShowErrorMessage(response);
            }

            return users;
        }

        private List<UserStatus> GetUserInformation(string response)
        {
            List<UserStatus> users = null;

            if (!response.Contains("ERROR") && !string.IsNullOrEmpty(response))
            {
                try
                {
                    XDocument document = XDocument.Parse(response, LoadOptions.None);
                    XNamespace georss = "http://www.georss.org/georss";
                    var query = from e in document.Root.Descendants("user")
                                select new UserStatus
                                {
                                    User = new User(e, "user"),
                                    StatusCreatedAt = (e.Element("status") != null) ? (!string.IsNullOrEmpty(e.Element("status").Element("created_at").Value)) ? Common.ParseDateTime(e.Element("status").Element("created_at").Value) : DateTime.MinValue : DateTime.MinValue,
                                    StatusID = (e.Element("status") != null) ? (!string.IsNullOrEmpty(e.Element("status").Element("id").Value)) ? Int64.Parse(e.Element("status").Element("id").Value) : 0 : 0,
                                    Text = (e.Element("status") != null) ? (!string.IsNullOrEmpty(e.Element("status").Element("text").Value)) ? e.Element("status").Element("text").Value : string.Empty : string.Empty,
                                    Source = (e.Element("status") != null) ? (!string.IsNullOrEmpty(e.Element("status").Element("source").Value)) ? e.Element("status").Element("source").Value : string.Empty : string.Empty,
                                    Truncated = (e.Element("status") != null) ? (!string.IsNullOrEmpty(e.Element("status").Element("truncated").Value)) ? bool.Parse(e.Element("status").Element("truncated").Value) : false : false,
                                    InReplyToStatusID = (e.Element("status") != null) ? (!string.IsNullOrEmpty(e.Element("status").Element("in_reply_to_status_id").Value)) ? Int64.Parse(e.Element("status").Element("in_reply_to_status_id").Value) : 0 : 0,
                                    InReplyToUserID = (e.Element("status") != null) ? (!string.IsNullOrEmpty(e.Element("status").Element("in_reply_to_user_id").Value)) ? Int64.Parse(e.Element("status").Element("in_reply_to_user_id").Value) : 0 : 0,
                                    Favorited = (e.Element("status") != null) ? (!string.IsNullOrEmpty(e.Element("status").Element("favorited").Value)) ? bool.Parse(e.Element("status").Element("favorited").Value) : false : false,
                                    InReplyToScreenName = (e.Element("status") != null) ? (!string.IsNullOrEmpty(e.Element("status").Element("in_reply_to_screen_name").Value)) ? e.Element("status").Element("in_reply_to_screen_name").Value : string.Empty : string.Empty,
                                    Geo = (!string.IsNullOrEmpty(e.Element("geo").Value)) ? e.Element("geo").Element(georss + "point").Value : string.Empty
                                };

                    users = (from u in query
                             select u).ToList();
                }
                catch { }
            }
            else
            {
                ShowErrorMessage(response);
            }

            return users;
        }

        private UserStatus GetUserData(string response)
        {
            UserStatus user = new UserStatus();

            if (!response.Contains("ERROR") && !string.IsNullOrEmpty(response))
            {
                try
                {
                    XDocument document = XDocument.Parse(response, LoadOptions.None);
                    XElement element = document.Root;

                    user.User = new User(element, "user");
                    user.StatusCreatedAt = Common.ParseDateTime(element.Element("status").Element("created_at").Value);
                    user.StatusID = Int64.Parse(element.Element("status").Element("id").Value);
                    user.Text = element.Element("status").Element("text").Value;
                    user.Source = element.Element("status").Element("source").Value;
                    user.Truncated = bool.Parse(element.Element("status").Element("truncated").Value);
                    user.InReplyToStatusID = (!string.IsNullOrEmpty(element.Element("status").Element("in_reply_to_status_id").Value)) ? Int64.Parse(element.Element("status").Element("in_reply_to_status_id").Value) : 0;
                    user.InReplyToUserID = (!string.IsNullOrEmpty(element.Element("status").Element("in_reply_to_user_id").Value)) ? Int64.Parse(element.Element("status").Element("in_reply_to_user_id").Value) : 0;
                    user.Favorited = bool.Parse(element.Element("status").Element("favorited").Value);
                    user.InReplyToScreenName = element.Element("status").Element("in_reply_to_screen_name").Value;
                }
                catch { }
            }
            else
            {
                ShowErrorMessage(response);
            }

            return user;
        }

        private List<DirectMessage> GetDirectMessageInformation(string response)
        {
            List<DirectMessage> messages = null;

            if (!response.Contains("ERROR") && !string.IsNullOrEmpty(response))
            {
                try
                {
                    XDocument document = XDocument.Parse(response, LoadOptions.None);

                    var query = from e in document.Root.Descendants("direct_message")
                                select new DirectMessage
                                {
                                    Sender = new User(e, "sender"),
                                    Recipient = new User(e, "recipient"),
                                    ID = (!string.IsNullOrEmpty(e.Element("id").Value)) ? Int64.Parse(e.Element("id").Value) : 0,
                                    SenderID = (!string.IsNullOrEmpty(e.Element("sender_id").Value)) ? Int64.Parse(e.Element("sender_id").Value) : 0,
                                    Text = (!string.IsNullOrEmpty(e.Element("text").Value)) ? e.Element("text").Value : string.Empty,
                                    RecipientID = (!string.IsNullOrEmpty(e.Element("recipient_id").Value)) ? Int64.Parse(e.Element("recipient_id").Value) : 0,
                                    CreatedDate = (!string.IsNullOrEmpty(e.Element("created_at").Value)) ? Common.ParseDateTime(e.Element("created_at").Value) : DateTime.MinValue,
                                    SenderScreenName = (!string.IsNullOrEmpty(e.Element("sender_screen_name").Value)) ? e.Element("sender_screen_name").Value : string.Empty,
                                    RecipientScreenName = (!string.IsNullOrEmpty(e.Element("recipient_screen_name").Value)) ? e.Element("recipient_screen_name").Value : string.Empty
                                };

                    messages = (from u in query
                                select u).ToList();
                }
                catch { }
            }
            else
            {
                ShowErrorMessage(response);
            }

            return messages;
        }

        private RateLimit GetRateLimit(string response)
        {
            RateLimit rateLimit = new RateLimit();

            if (!response.Contains("ERROR"))
            {
                try
                {
                    XDocument document = XDocument.Parse(response, LoadOptions.None);
                    XElement ele = document.Root;

                    rateLimit.HourlyLimit = (ele.Element("hourly-limit") != null) ? int.Parse(ele.Element("hourly-limit").Value) : 0;
                    rateLimit.RemainingHits = (ele.Element("remaining-hits") != null) ? int.Parse(ele.Element("remaining-hits").Value) : 0;
                    rateLimit.ResetTime = (ele.Element("reset-time") != null) ? ele.Element("reset-time").Value : string.Empty;
                    rateLimit.ResetTimeInSeconds = (ele.Element("reset-time-in-seconds") != null) ? Int64.Parse(ele.Element("reset-time-in-seconds").Value) : 0;
                }
                catch { }
            }
            else
            {
                ShowErrorMessage(response);
            }

            return rateLimit;
        }

        private string UploadToShortener(string url)
        {
            if (string.IsNullOrEmpty(m_urlShortnerAPIKey) && string.IsNullOrEmpty(m_urlShortnerLogin))
                return url;

            string newURL = string.Empty;

            string requestURL = string.Format("http://api.bit.ly/shorten?version={0}&longUrl={1}&login={2}&apiKey={3}&format=xml", m_apiVersion, url, m_urlShortnerLogin, m_urlShortnerAPIKey);

            XDocument doc = XDocument.Parse(Get(requestURL), LoadOptions.None);

            newURL = doc.Root.Element("results").Element("nodeKeyVal").Element("shortUrl").Value;

            return newURL;
        }

        private string CheckStatusForURL(string status)
        {
            string[] array = status.Split(new char[] { ' ' });
            string oldURL = string.Empty;
            string newURL = string.Empty;
            string newStatus = status;

            foreach (string s in array)
            {
                if (s.StartsWith("http"))
                {
                    if (s.Length > 25)
                    {
                        oldURL = s;
                        break;
                    }
                }
            }

            if (!string.IsNullOrEmpty(oldURL))
            {
                newURL = UploadToShortener(oldURL);

                if (!string.IsNullOrEmpty(newURL))
                {
                    StringBuilder sb = new StringBuilder();

                    foreach (string s in array)
                    {
                        if (s.StartsWith("http"))
                            sb.Append(newURL + " ");
                        else
                            sb.Append(s + " ");
                    }

                    newStatus = sb.ToString().Trim();
                }
            }

            return newStatus;
        }

        #endregion

        #region Public Methods

        #region Status GET Methods

        /// <summary>
        /// Gets the UserTimeLine of the authenticated user
        /// </summary>
        /// <param name="screenName"></param>
        /// <returns></returns>
        public List<UserStatus> GetUserTimeLine()
        {
            return GetUserTimeLine(m_userName);
        }

        /// <summary>
        /// Gets the UserTimeLine of the specified user
        /// </summary>
        /// <param name="screenName"></param>
        /// <returns></returns>
        public List<UserStatus> GetUserTimeLine(string screenName)
        {
            string response = string.Empty;

            if (DetermineRateLimit())
            {
                string url = string.Format("http://api.twitter.com/1/statuses/user_timeline/{0}.xml", screenName);

                if (!IsOAuthEnabled)
                    response = Get(url);
                else
                    response = OAuthGetRequest(url);
            }

            return GetStatusInformation(response);
        }

        /// <summary>
        /// Gets UserTimeLine of authenticated user using optional parameters
        /// </summary>
        /// <param name="param"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public List<UserStatus> GetUserTimeLine(UserTimeLineParameters param, string value)
        {
            string response = string.Empty;

            if (DetermineRateLimit())
            {
                string url = string.Empty;

                switch (param)
                {
                    case UserTimeLineParameters.SinceID:
                        url = string.Format("http://api.twitter.com/1/statuses/user_timeline.xml?since_id={0}&count=200", value);
                        break;
                    case UserTimeLineParameters.MaxID:
                        url = string.Format("http://api.twitter.com/1/statuses/user_timeline.xml?max_id={0}", value);
                        break;
                    case UserTimeLineParameters.Count:
                        url = string.Format("http://api.twitter.com/1/statuses/user_timeline.xml?count={0}", value);
                        break;
                }

                if (!IsOAuthEnabled)
                    response = Get(url);
                else
                    response = OAuthGetRequest(url);
            }

            return GetStatusInformation(response);
        }

        /// <summary>
        /// Gets last 20 Friend Timeline tweets
        /// </summary>
        /// <returns></returns>
        public List<UserStatus> GetFriendsTimeLine()
        {
            string response = string.Empty;

            if (DetermineRateLimit())
            {
                string url = string.Format("http://api.twitter.com/1/statuses/friends_timeline.xml");

                if (!IsOAuthEnabled)
                    response = Get(url);
                else
                    response = OAuthGetRequest(url);
            }

            return GetStatusInformation(response);
        }

        /// <summary>
        /// Gets Friend Timeline tweets using the last tweet ID and the number to return
        /// </summary>
        /// <param name="sinceID"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public List<UserStatus> GetFriendsTimeLineWithSinceIDAndCount(Int64 sinceID, int count)
        {
            string response = string.Empty;

            if (DetermineRateLimit())
            {
                string url = string.Format("http://api.twitter.com/1/statuses/friends_timeline.xml?since{0}&count={1}", sinceID, count);

                if (!IsOAuthEnabled)
                    response = Get(url);
                else
                    response = OAuthGetRequest(url);
            }

            return GetStatusInformation(response);
        }

        /// <summary>
        /// Gets Friend TimeLine using one of the optional parameters
        /// </summary>
        /// <param name="param"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public List<UserStatus> GetFriendsTimeLine(FriendTimeLineParameters param, string value)
        {
            string response = string.Empty;

            if (DetermineRateLimit())
            {
                string url = string.Empty;

                switch (param)
                {
                    case FriendTimeLineParameters.SinceID:
                        url = string.Format("http://api.twitter.com/1/statuses/friends_timeline.xml?since_id={0}&count=200", value);
                        break;
                    case FriendTimeLineParameters.MaxID:
                        url = string.Format("http://api.twitter.com/1/statuses/friends_timeline.xml?max_id={0}", value);
                        break;
                    case FriendTimeLineParameters.Count:
                        url = string.Format("http://api.twitter.com/1/statuses/friends_timeline.xml?count={0}", value);
                        break;
                }

                if (!IsOAuthEnabled)
                    response = Get(url);
                else
                    response = OAuthGetRequest(url);
            }

            return GetStatusInformation(response);
        }

        /// <summary>
        /// Gets last 20 user mentions of the authenticated user
        /// </summary>
        /// <param name="param"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public List<UserStatus> GetUserMentions()
        {
            string response = string.Empty;

            if (DetermineRateLimit())
            {
                string url = string.Format("http://api.twitter.com/1/statuses/mentions.xml?count=200");

                if (!IsOAuthEnabled)
                    response = Get(url);
                else
                    response = OAuthGetRequest(url);
            }

            return GetStatusInformation(response);
        }

        /// <summary>
        /// Gets user mentions of the authenticated user using one of the optional parameters
        /// </summary>
        /// <param name="param"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public List<UserStatus> GetUserMentions(UserMentionParameters param, string value)
        {
            string response = string.Empty;

            if (DetermineRateLimit())
            {
                string url = string.Empty;

                switch (param)
                {
                    case UserMentionParameters.SinceID:
                        url = string.Format("http://api.twitter.com/1/statuses/mentions.xml?since_id={0}&count=200", value);
                        break;
                    case UserMentionParameters.MaxID:
                        url = string.Format("http://api.twitter.com/1/statuses/mentions.xml?max_id={0}", value);
                        break;
                    case UserMentionParameters.Count:
                        url = string.Format("http://api.twitter.com/1/statuses/mentions.xml?count={0}", value);
                        break;
                }

                if (!IsOAuthEnabled)
                    response = Get(url);
                else
                    response = OAuthGetRequest(url);
            }

            return GetStatusInformation(response);
        }

        /// <summary>
        /// Returns the 20 most recent statuses from non-protected users who have set a custom user icon.
        /// </summary>
        /// <returns>Returns a List of UserStatus</returns>
        public List<UserStatus> GetPublicTimeLine()
        {
            string response = string.Empty;

            if (DetermineRateLimit())
            {
                string url = string.Format("http://api.twitter.com/1/statuses/public_timeline.xml");

                if (!IsOAuthEnabled)
                    response = Get(url);
                else
                    response = OAuthGetRequest(url);
            }

            return GetStatusInformation(response);
        }

        /// <summary>
        /// Gets the current status of the authenticated user
        /// </summary>
        /// <returns></returns>
        public string GetUserCurrentStatus(string userName)
        {
            string response = string.Empty;

            if (DetermineRateLimit())
            {
                UserStatus status = GetUserTimeLine(userName)[0];
                return (status != null) ? status.Text : string.Empty;
            }

            return "";
        }

        #endregion

        #region Status POST Methods

        /// <summary>
        /// Post status for authenticated user
        /// </summary>
        /// <param name="status"></param>
        public bool UpdateUserStatus(string status, bool isRetweet)
        {
            bool successful = false;
            string response = string.Empty;

            if (DetermineRateLimit())
            {
                if (isRetweet)
                    m_status = status;
                else
                    m_status = CheckStatusForURL(status);

                if (m_status.Contains("ERROR"))
                {
                    ShowErrorMessage(m_status);
                    return false;
                }

                if (m_status.Length <= 140)
                {
                    string url = string.Format("http://api.twitter.com/1/statuses/update.xml");

                    if (!IsOAuthEnabled)
                        response = Post(url);
                    else
                        response = OAuthPostRequest(url, m_status);

                    if (response.Contains("status"))
                        successful = true;
                    else
                        successful = false;

                    m_status = string.Empty;
                }
                else
                {
                    successful = false;
                }
            }

            return successful;
        }

        /// <summary>
        /// Deletes tweet
        /// </summary>
        /// <param name="id">ID of tweet.  Tweet must be for autheticated user.</param>
        public void DeleteUserStatus(Int64 id)
        {
            string response = string.Empty;

            if (DetermineRateLimit())
            {
                string url = string.Format("http://api.twitter.com/1/statuses/destroy/{0}.xml", id);

                if (!IsOAuthEnabled)
                    response = Post(url);
                else
                    response = OAuthPostRequest(url, "");
            }

            if (response.Contains("ERROR"))
                ShowErrorMessage(response);
        }

        #endregion

        #region Friend GET Methods

        /// <summary>
        /// Gets extended information of the specified user.  Current status will also be returned
        /// </summary>
        /// <returns></returns>
        public UserStatus GetUser(string userName)
        {
            string response = string.Empty;

            if (DetermineRateLimit())
            {
                string url = string.Format("http://api.twitter.com/1/users/show/{0}.xml", userName);

                if (!IsOAuthEnabled)
                    response = Get(url);
                else
                    response = OAuthGetRequest(url);
            }

            return GetUserData(response);
        }

        /// <summary>
        /// Gets the friends of the authenticated user
        /// </summary>
        /// <returns></returns>
        public List<UserStatus> GetFriends()
        {
            return GetFriends(m_userName);
        }

        /// <summary>
        /// Gets the friends of the specified user
        /// </summary>
        /// <param name="screenName">Either UserID or ScreenName</param>
        /// <returns></returns>
        public List<UserStatus> GetFriends(string screenName)
        {
            string response = string.Empty;

            if (DetermineRateLimit())
            {
                string url = string.Format("http://api.twitter.com/1/statuses/friends/{0}.xml?count=200", screenName);

                if (!IsOAuthEnabled)
                    response = Get(url);
                else
                    response = OAuthGetRequest(url);
            }

            return GetUserInformation(response);
        }

        /// <summary>
        /// Gets the followers of the authenticated user
        /// </summary>
        /// <returns></returns>
        public List<UserStatus> GetFollowers()
        {
            return GetFollowers(m_userName);
        }

        /// <summary>
        /// Gets the followers of the specified user
        /// </summary>
        /// <param name="screenName">Either UserID or ScreenName</param>
        /// <returns></returns>
        public List<UserStatus> GetFollowers(string screenName)
        {
            string response = string.Empty;

            if (DetermineRateLimit())
            {
                string url = string.Format("http://api.twitter.com/1/statuses/followers/{0}.xml", screenName);

                if (!IsOAuthEnabled)
                    response = Get(url);
                else
                    response = OAuthGetRequest(url);
            }

            return GetUserInformation(response);
        }

        #endregion

        #region Direct Message GET Methods

        /// <summary>
        /// Gets received direct messages received for the authenticated user
        /// </summary>
        /// <returns></returns>
        public List<DirectMessage> GetDirectMessagesReceived()
        {
            string response = string.Empty;

            if (DetermineRateLimit())
            {
                string url = "http://api.twitter.com/1/direct_messages.xml";

                if (!IsOAuthEnabled)
                    response = Get(url);
                else
                    response = OAuthGetRequest(url);
            }

            return GetDirectMessageInformation(response);
        }

        /// <summary>
        /// Gets received direct messages sent for the authenticated user
        /// </summary>
        /// <returns></returns>
        public List<DirectMessage> GetDirectMessagesSent()
        {
            string response = string.Empty;

            if (DetermineRateLimit())
            {
                string url = "http://api.twitter.com/1/direct_messages/sent.xml";

                if (!IsOAuthEnabled)
                    response = Get(url);
                else
                    response = OAuthGetRequest(url);
            }

            return GetDirectMessageInformation(response);
        }

        /// <summary>
        /// Gets Direct Messages using one of the optional parameters
        /// </summary>
        /// <param name="param"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public List<DirectMessage> GetDirectMessages(DirectMessageParameters param, string value)
        {
            string response = string.Empty;

            if (DetermineRateLimit())
            {
                string url = string.Empty;

                switch (param)
                {
                    case DirectMessageParameters.ReceivedBySinceID:
                        url = string.Format("http://api.twitter.com/1/direct_messages.xml?since_id={0}&count=200", value);
                        break;
                    case DirectMessageParameters.ReceivedByMaxID:
                        url = string.Format("http://api.twitter.com/1/direct_messages.xml?max_id={0}", value);
                        break;
                    case DirectMessageParameters.ReceivedByCount:
                        url = string.Format("http://api.twitter.com/1/direct_messages.xml?count={0}", value);
                        break;
                    case DirectMessageParameters.SentBySinceID:
                        url = string.Format("http://api.twitter.com/1/direct_messages/sent.xml?since_id={0}", value);
                        break;
                    case DirectMessageParameters.SentByMaxID:
                        url = string.Format("http://api.twitter.com/1/direct_messages/sent.xml?max_id={0}", value);
                        break;
                    case DirectMessageParameters.SentByCount:
                        url = string.Format("http://api.twitter.com/1/direct_messages/sent.xml?count={0}", value);
                        break;
                }

                if (!IsOAuthEnabled)
                    response = Get(url);
                else
                    response = OAuthGetRequest(url);
            }

            return GetDirectMessageInformation(response);
        }

        #endregion

        #region Direct Message POST Methods

        /// <summary>
        /// Sends a Direct Message to the recipient.
        /// </summary>
        /// <param name="recipientScreenName">Screen Name of the recipient</param>
        /// <param name="text">Text of message</param>
        public bool SendDirectMessage(string recipientScreenName, string text)
        {
            string response = string.Empty;
            bool success = false;

            string url = string.Format("http://api.twitter.com/1/direct_messages/new.xml?user={0}&text={1}", recipientScreenName, text);

            if (!IsOAuthEnabled)
                response = Post(url);
            else
                response = OAuthPostRequest(url, "");

            if (response.Contains("direct_message"))
                success = true;
            else
                ShowErrorMessage(response);

            return success;
        }

        /// <summary>
        /// Deletes specified Direct Message
        /// </summary>
        /// <param name="messageID"></param>
        public void DeleteDirectMessage(Int64 messageID)
        {
            string response = string.Empty;

            string url = string.Format("http://api.twitter.com/1/direct_messages/destroy/{0}.xml", messageID);

            if (!IsOAuthEnabled)
                response = Post(url);
            else
                response = OAuthPostRequest(url, "");

            if (response.Contains("ERROR"))
                ShowErrorMessage(response);
        }

        #endregion

        #region Friendship GET Methods

        /// <summary>
        /// Determines if the autheticated user is following the target user.
        /// </summary>
        /// <param name="targetUserName">Screen name of target user</param>
        /// <returns></returns>
        public bool IsFollowingUser(string targetUserName)
        {
            return IsFollowingUser(m_userName, targetUserName);
        }

        /// <summary>
        /// Determines if source user is following the target user.
        /// </summary>
        /// <param name="sourceUserName">Screen name of the source user</param>
        /// <param name="targetUserName">Screen name of the target user</param>
        /// <returns></returns>
        public bool IsFollowingUser(string sourceUserName, string targetUserName)
        {
            bool isFollowingUser = false;

            string response = string.Empty;

            if (DetermineRateLimit())
            {
                string url = string.Format("http://api.twitter.com/1/friendships/show.xml?source_screen_name={0}&target_screen_name={1}", sourceUserName, targetUserName);

                if (!IsOAuthEnabled)
                    response = Get(url);
                else
                    response = OAuthGetRequest(url);

                XDocument document = XDocument.Parse(response, LoadOptions.None);

                var query = from e in document.Root.Descendants("source")
                            select Convert.ToBoolean(e.Element("following").Value);

                foreach (var q in query)
                {
                    isFollowingUser = q;
                }
            }

            return isFollowingUser;
        }

        /// <summary>
        /// Determines if source user is following the target user.
        /// </summary>
        /// <param name="sourceUserID">User ID of the source user</param>
        /// <param name="targetUserID">User ID of the target user</param>
        /// <returns></returns>
        public bool IsFollowingUserByID(string sourceUserID, string targetUserID)
        {
            bool isFollowingUser = false;

            string response = string.Empty;

            if (DetermineRateLimit())
            {
                string url = string.Format("http://api.twitter.com/1/friendships/show.xml?source_id={0}&target_id={1}", sourceUserID, targetUserID);

                if (!IsOAuthEnabled)
                    response = Get(url);
                else
                    response = OAuthGetRequest(url);

                XDocument document = XDocument.Parse(response, LoadOptions.None);

                var query = from e in document.Root.Descendants("source")
                            select Convert.ToBoolean(e.Element("following").Value);

                foreach (var q in query)
                {
                    isFollowingUser = q;
                }
            }

            return isFollowingUser;
        }

        /// <summary>
        /// Determines if the autheticated user is followed by the target user.
        /// </summary>
        /// <param name="targetUserName">Screen name of target user</param>
        /// <returns></returns>
        public bool IsFollowedByUser(string targetUserName)
        {
            return IsFollowedByUser(m_userName, targetUserName);
        }

        /// <summary>
        /// Determines if source user is followed by the target user.
        /// </summary>
        /// <param name="sourceUserName">Screen name of the source user</param>
        /// <param name="targetUserName">Screen name of the target user</param>
        /// <returns></returns>
        public bool IsFollowedByUser(string sourceUserName, string targetUserName)
        {
            bool isFollowedByUser = false;

            string response = string.Empty;

            if (DetermineRateLimit())
            {
                string url = string.Format("http://api.twitter.com/1/friendships/show.xml?source_screen_name={0}&target_screen_name={1}", sourceUserName, targetUserName);

                if (!IsOAuthEnabled)
                    response = Get(url);
                else
                    response = OAuthGetRequest(url);

                XDocument document = XDocument.Parse(response, LoadOptions.None);

                var query = from e in document.Root.Descendants("source")
                            select Convert.ToBoolean(e.Element("followed_by").Value);

                foreach (var q in query)
                {
                    isFollowedByUser = q;
                }
            }

            return isFollowedByUser;
        }

        /// <summary>
        /// Determines if source user is followed by the target user.
        /// </summary>
        /// <param name="sourceUserID">User ID of the source user</param>
        /// <param name="targetUserID">User ID of the target user</param>
        /// <returns></returns>
        public bool IsFollowedByUserByID(string sourceUserID, string targetUserID)
        {
            bool isFollowingUser = false;

            string response = string.Empty;

            if (DetermineRateLimit())
            {
                string url = string.Format("http://api.twitter.com/1/friendships/show.xml?source_id={0}&target_id={1}", sourceUserID, targetUserID);

                if (!IsOAuthEnabled)
                    response = Get(url);
                else
                    response = OAuthGetRequest(url);

                XDocument document = XDocument.Parse(response, LoadOptions.None);

                var query = from e in document.Root.Descendants("source")
                            select Convert.ToBoolean(e.Element("followed_by").Value);

                foreach (var q in query)
                {
                    isFollowingUser = q;
                }
            }

            return isFollowingUser;
        }

        #endregion

        #region Friendship POST Methods

        /// <summary>
        /// Starts following the specified user
        /// </summary>
        /// <param name="userName">Screen name of user to start following</param>
        public void StartFollowingUser(string userName)
        {
            string response = string.Empty;

            string url = string.Format("http://api.twitter.com/1/friendships/create/{0}.xml", userName);

            if (!IsOAuthEnabled)
                response = Post(url);
            else
                response = OAuthPostRequest(url, "");

            if (response.Contains("ERROR"))
                ShowErrorMessage(response);
        }

        /// <summary>
        /// Starts following the specified user
        /// </summary>
        /// <param name="userID">ID of the user to start following</param>
        public void StartFollowingUser(Int64 userID)
        {
            StartFollowingUser(userID.ToString());
        }

        /// <summary>
        /// Stops following specified user
        /// </summary>
        /// <param name="userName">Screen name of user to stop following</param>
        public void StopFollowingUser(string userName)
        {
            string response = string.Empty;

            string url = string.Format("http://api.twitter.com/1/friendships/destroy/{0}.xml", userName);

            if (!IsOAuthEnabled)
                response = Post(url);
            else
                response = OAuthPostRequest(url, "");

            if (response.Contains("ERROR"))
                ShowErrorMessage(response);
        }

        /// <summary>
        /// Stops following specified user
        /// </summary>
        /// <param name="userID">id of user to stop following</param>
        public void StopFollowingUser(Int64 userID)
        {
            StopFollowingUser(userID.ToString());
        }

        #endregion

        #region Account GET Methods

        /// <summary>
        /// Verifies the user's credentials
        /// --- Because this method can be a vector for a brute force dictionary attack to determine a user's password, 
        ///    it is limited to 15 requests per 60 minute period (starting from your first request).---
        /// </summary>
        /// <returns></returns>
        public bool VerifyCredentials()
        {
            string response = string.Empty;

            string url = "http://api.twitter.com/1/account/verify_credentials.xml";

            if (!IsOAuthEnabled)
                response = Get(url);
            else
                response = OAuthGetRequest(url);

            return (response.Contains("user"));
        }

        #endregion

        #region Account POST Methods

        /// <summary>
        /// Ends the session of the currently authenticated user
        /// </summary>
        public bool EndSession()
        {
            string response = string.Empty;

            string url = "http://api.twitter.com/1/account/end_session.xml";

            if (!IsOAuthEnabled)
                response = Post(url);
            else
                response = OAuthPostRequest(url, "");

            return (response.Contains("Logged out"));
        }

        /// <summary>
        /// Sets which device Twitter delivers updates to for the authenticating user.  
        /// Sending none as the device parameter will disable IM or SMS updates.
        /// </summary>
        /// <param name="devices">Enum of devices</param>
        /// <returns></returns>
        public bool UpdateDeliveryDevice(UpdateDeliveryDevices devices)
        {
            string response = string.Empty;

            string url = string.Format("http://api.twitter.com/1/account/update_delivery_device.xml?device={0}", devices.ToString().ToLower());

            if (!IsOAuthEnabled)
                response = Post(url);
            else
                response = OAuthPostRequest(url, "");

            return (response.Contains("user"));
        }

        /// <summary>
        /// Updates the color scheme of the Profile page
        /// </summary>
        /// <param name="profile">Enum of Profile sections</param>
        /// <param name="hexColor">3 or 6 character HEX code</param>
        /// <returns></returns>
        public bool UpdateProfileColors(ProfileColors profile, string hexColor)
        {
            string response = string.Empty;
            string url = string.Empty;

            if (profile == ProfileColors.BackgroundColor)
                url = string.Format("http://api.twitter.com/1/account/update_profile_colors.xml?profile_background_color={0}", hexColor);
            else if (profile == ProfileColors.LinkColor)
                url = string.Format("http://api.twitter.com/1/account/update_profile_colors.xml?profile_link_color={0}", hexColor);
            else if (profile == ProfileColors.SidebarBorderColor)
                url = string.Format("http://api.twitter.com/1/account/update_profile_colors.xml?profile_sidebar_border_color={0}", hexColor);
            else if (profile == ProfileColors.SidebarFillColor)
                url = string.Format("http://api.twitter.com/1/account/update_profile_colors.xml?profile_sidebar_fill_color={0}", hexColor);
            else if (profile == ProfileColors.TextColor)
                url = string.Format("http://api.twitter.com/1/account/update_profile_colors.xml?profile_text_color={0}", hexColor);

            if (!IsOAuthEnabled)
                response = Post(url);
            else
                response = OAuthPostRequest(url, "");

            return (response.Contains("user"));
        }

        #endregion

        #region Favorite GET Methods

        /// <summary>
        /// Gets Favorite tweets for authenticated user
        /// </summary>
        /// <returns></returns>
        public List<UserStatus> GetFavorites()
        {
            return GetFavorites(m_userName);
        }

        /// <summary>
        /// Gets Favorite tweets for specified user
        /// </summary>
        /// <param name="screenName">UserID or ScreenName</param>
        /// <returns></returns>
        public List<UserStatus> GetFavorites(string screenName)
        {
            string response = string.Empty;

            if (DetermineRateLimit())
            {
                string url = string.Format("http://api.twitter.com/1/favorites/{0}.xml", screenName);

                if (!IsOAuthEnabled)
                    response = Get(url);
                else
                    response = OAuthGetRequest(url);
            }

            return GetStatusInformation(response);
        }

        #endregion

        #region Favorite POST Methods

        /// <summary>
        /// Sets status to being favorited
        /// </summary>
        /// <param name="statusID"></param>
        /// <returns></returns>
        public bool SetStatusToFavorite(Int64 statusID)
        {
            string response = string.Empty;

            string url = string.Format("http://api.twitter.com/1/favorites/create/{0}.xml", statusID);

            if (!IsOAuthEnabled)
                response = Post(url);
            else
                response = OAuthPostRequest(url, "");

            return (response.Contains("status"));
        }

        /// <summary>
        /// Sets status to being unfavorited
        /// </summary>
        /// <param name="statusID"></param>
        /// <returns></returns>
        public bool SetStatusToUnfavorite(Int64 statusID)
        {
            string response = string.Empty;

            string url = string.Format("http://api.twitter.com/1/favorites/destroy/{0}.xml", statusID);

            if (!IsOAuthEnabled)
                response = Post(url);
            else
                response = OAuthPostRequest(url, "");

            return (response.Contains("status"));
        }

        #endregion

        #region Notification POST Methods

        /// <summary>
        /// Enables device notifications for updates from the specified user.
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool FollowWithDeviceNotifications(int userID)
        {
            return FollowWithDeviceNotifications(userID.ToString());
        }

        /// <summary>
        /// Enables device notifications for updates from the specified user.
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool FollowWithDeviceNotifications(string screenName)
        {
            string response = string.Empty;

            string url = string.Format("http://api.twitter.com/1/notifications/follow/{0}.xml", screenName);

            if (!IsOAuthEnabled)
                response = Post(url);
            else
                response = OAuthPostRequest(url, "");

            return (response.Contains("user"));
        }

        /// <summary>
        /// Disables device notifications for updates from the specified user.
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool UnfollowDeviceNotifications(int userID)
        {
            return UnfollowDeviceNotifications(userID.ToString());
        }

        /// <summary>
        /// Disables device notifications for updates from the specified user.
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool UnfollowDeviceNotifications(string screenName)
        {
            string response = string.Empty;

            string url = string.Format("http://api.twitter.com/1/notifications/leave/{0}.xml", screenName);

            if (!IsOAuthEnabled)
                response = Post(url);
            else
                response = OAuthPostRequest(url, "");

            return (response.Contains("user"));
        }

        #endregion

        #region Block POST Methods

        /// <summary>
        /// Blocks the user specified
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool BlockUser(Int64 userID)
        {
            return BlockUser(userID.ToString());
        }

        /// <summary>
        /// Blocks the user specified
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool BlockUser(string screenName)
        {
            string response = string.Empty;

            string url = string.Format("http://api.twitter.com/1/blocks/create/{0}.xml", screenName);

            if (!IsOAuthEnabled)
                response = Post(url);
            else
                response = OAuthPostRequest(url, "");

            return (response.Contains("user"));
        }

        /// <summary>
        /// Unblocks the user specified
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool UnblockUser(Int64 userID)
        {
            return UnblockUser(userID.ToString());
        }

        /// <summary>
        /// Unblocks the user specified
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool UnblockUser(string screenName)
        {
            string response = string.Empty;

            string url = string.Format("http://api.twitter.com/1/blocks/destroy/{0}.xml", screenName);

            if (!IsOAuthEnabled)
                response = Post(url);
            else
                response = OAuthPostRequest(url, "");

            return (response.Contains("user"));
        }

        /// <summary>
        /// Checks to see if authenticated user is blocking specified user
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool IsBlocked(Int64 userID)
        {
            return IsBlocked(userID.ToString());
        }

        /// <summary>
        /// Checks to see if authenticated user is blocking specified user
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool IsBlocked(string screenName)
        {
            string response = string.Empty;

            string url = string.Format("http://api.twitter.com/1/blocks/exists/{0}.xml", screenName);

            if (!IsOAuthEnabled)
                response = Post(url);
            else
                response = OAuthPostRequest(url, "");

            return (response.Contains("user"));
        }

        /// <summary>
        /// Gets the list of users the authenticated user is blocking
        /// </summary>
        /// <returns></returns>
        public List<UserStatus> GetBlockedList()
        {
            string response = string.Empty;

            if (DetermineRateLimit())
            {
                string url = "http://api.twitter.com/1/blocks/blocking.xml";

                if (!IsOAuthEnabled)
                    response = Get(url);
                else
                    response = OAuthGetRequest(url);
            }

            return GetUserInformation(response);
        }

        /// <summary>
        /// Reports specified account as spam
        /// </summary>
        /// <param name="screenName">UserID or ScreenName</param>
        /// <returns></returns>
        public bool ReportAsSpam(string screenName)
        {
            string response = string.Empty;

            string url = string.Format("http://api.twitter.com/1/report_spam.xml?screen_name={0}", screenName);

            if (!IsOAuthEnabled)
                response = Post(url);
            else
                response = OAuthPostRequest(url, "");

            return (response.Contains("user"));
        }

        #endregion

        #region OAUTH Methods

        /// <summary>
        /// **** USE FOR DESKTOP AUTHORIZATION ****
        /// Use this method to get the unauthorized request token.
        /// This token is used when the user has NOT authorized your application 
        ///      to use their account.
        /// </summary>
        public string OAuthGetUnauthorizedRequestToken()
        {
            string response = string.Empty;

            response = OAuthGetToken();

            return response;
        }

        /// <summary>
        /// **** USE FOR WEB AUTHORIZATION ****
        /// Use this method to get the access token.
        /// </summary>
        /// <param name="callbackURL">URL to redirect to when user has authorized use</param>
        /// <returns>Full URL for OAuth including callback URL</returns>
        public string OAuthGetWebAccessToken(string callbackURL)
        {
            string response = string.Empty;

            response = OAuthWebGetToken(callbackURL);

            return response;
        }

        /// <summary>
        /// **** USE FOR DESKTOP AUTHORIZATION ****
        /// Use for Desktop Authorization
        /// </summary>
        /// <param name="PIN">PIN from user</param>
        /// <returns></returns>
        public bool OAuthRequestAccessToken(string PIN)
        {
            string response = string.Empty;

            if (!string.IsNullOrEmpty(PIN))
            {
                response = OAuthGetToken(PIN);
            }

            return (!string.IsNullOrEmpty(response));
        }

        #endregion

        #endregion
    }
}