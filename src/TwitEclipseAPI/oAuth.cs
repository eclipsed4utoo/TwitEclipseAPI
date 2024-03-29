// *****************************************************
// TwitEclipseAPI - .Net Twitter API Wrapper           *
// Developed by Ryan Alford                            *
// The core OAuth code was taken from Shannon Whitley. *
//   http://www.voiceoftech.com/swhitley/?p=681        *
// *****************************************************

using System;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace TwitterAPI
{
    public class OAuthBase
    {

        /// <summary>
        /// Provides a predefined set of algorithms that are supported officially by the protocol
        /// </summary>
        public enum SignatureTypes
        {
            HMACSHA1,
            PLAINTEXT,
            RSASHA1,
            NONE
        }

        /// <summary>
        /// Provides an internal structure to sort the query parameter
        /// </summary>
        protected class QueryParameter
        {
            private string name = null;
            private string value = null;

            public QueryParameter(string name, string value)
            {
                this.name = name;
                this.value = value;
            }

            public string Name
            {
                get { return name; }
            }

            public string Value
            {
                get { return value; }
            }
        }

        /// <summary>
        /// Comparer class used to perform the sorting of the query parameters
        /// </summary>
        protected class QueryParameterComparer : IComparer<QueryParameter>
        {

            #region IComparer<QueryParameter> Members

            public int Compare(QueryParameter x, QueryParameter y)
            {
                if (x.Name == y.Name)
                {
                    return string.Compare(x.Value, y.Value);
                }
                else
                {
                    return string.Compare(x.Name, y.Name);
                }
            }

            #endregion
        }

        protected const string OAuthVersion = "1.0";
        protected const string OAuthParameterPrefix = "oauth_";

        //
        // List of know and used oauth parameters' names
        //        
        protected const string OAuthConsumerKeyKey = "oauth_consumer_key";
        protected const string OAuthConsumerSecretKey = "oauth_consumer_secret";
        protected const string OAuthCallbackKey = "oauth_callback";
        protected const string OAuthVersionKey = "oauth_version";
        protected const string OAuthSignatureMethodKey = "oauth_signature_method";
        protected const string OAuthSignatureKey = "oauth_signature";
        protected const string OAuthTimestampKey = "oauth_timestamp";
        protected const string OAuthNonceKey = "oauth_nonce";
        protected const string OAuthTokenKey = "oauth_token";
        protected const string OAuthTokenSecretKey = "oauth_token_secret";
        protected const string OAuthVerifier = "oauth_verifier";

        protected const string HMACSHA1SignatureType = "HMAC-SHA1";
        protected const string PlainTextSignatureType = "PLAINTEXT";
        protected const string RSASHA1SignatureType = "RSA-SHA1";

        protected string unreservedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";

        /// <summary>
        /// Internal function to cut out all non oauth query string parameters (all parameters not begining with "oauth_")
        /// </summary>
        /// <param name="parameters">The query string part of the Url</param>
        /// <returns>A list of QueryParameter each containing the parameter name and value</returns>
        private List<QueryParameter> GetQueryParameters(string parameters)
        {
            if (parameters.StartsWith("?"))
            {
                parameters = parameters.Remove(0, 1);
            }

            List<QueryParameter> result = new List<QueryParameter>();

            if (!string.IsNullOrEmpty(parameters))
            {
                string[] p = parameters.Split('&');
                foreach (string s in p)
                {
                    if (!string.IsNullOrEmpty(s) && !s.StartsWith(OAuthParameterPrefix))
                    {
                        if (s.IndexOf('=') > -1)
                        {
                            string[] temp = s.Split('=');
                            result.Add(new QueryParameter(temp[0], temp[1]));
                        }
                        else
                        {
                            result.Add(new QueryParameter(s, string.Empty));
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// This is a different Url Encode implementation since the default .NET one outputs the percent encoding in lower case.
        /// While this is not a problem with the percent encoding spec, it is used in upper case throughout OAuth
        /// </summary>
        /// <param name="value">The value to Url encode</param>
        /// <returns>Returns a Url encoded string</returns>
        public string UrlEncode(string value)
        {
            StringBuilder result = new StringBuilder();

            foreach (char symbol in value)
            {
                if (unreservedChars.IndexOf(symbol) != -1)
                {
                    result.Append(symbol);
                }
                else
                {
                    //some symbols produce > 2 char values so the system urlencoder must be used to get the correct data
                    if (String.Format("{0:X2}", (int)symbol).Length > 3)
                    {
                        result.Append(Common.UrlEncode(value.Substring(value.IndexOf(symbol), 1)).ToUpper());
                    }
                    else
                    {
                        result.Append('%' + String.Format("{0:X2}", (int)symbol));
                    }
                }
            }
            return result.ToString();
        }

        /// <summary>
        /// Normalizes the request parameters according to the spec
        /// </summary>
        /// <param name="parameters">The list of parameters already sorted</param>
        /// <returns>a string representing the normalized parameters</returns>
        protected string NormalizeRequestParameters(IList<QueryParameter> parameters)
        {
            StringBuilder sb = new StringBuilder();
            QueryParameter p = null;
            for (int i = 0; i < parameters.Count; i++)
            {
                p = parameters[i];

                if (p.Name != "status" && p.Name != "text")
                    sb.AppendFormat("{0}={1}", p.Name, UrlEncode(p.Value));
                else
                    // status is already encoded
                    sb.AppendFormat("{0}={1}", p.Name, p.Value);

                if (i < parameters.Count - 1)
                {
                    sb.Append("&");
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Generate the signature base that is used to produce the signature
        /// </summary>
        /// <param name="url">The full url that needs to be signed including its non OAuth url parameters</param>
        /// <param name="consumerKey">The consumer key</param>        
        /// <param name="token">The token, if available. If not available pass null or an empty string</param>
        /// <param name="tokenSecret">The token secret, if available. If not available pass null or an empty string</param>
        /// <param name="httpMethod">The http method used. Must be a valid HTTP method verb (POST,GET,PUT, etc)</param>
        /// <param name="signatureType">The signature type. To use the default values use <see cref="OAuthBase.SignatureTypes">OAuthBase.SignatureTypes</see>.</param>
        /// <returns>The signature base</returns>
        private string GenerateSignatureBase(OAuthParameters oauth, out string normalizedUrl, out string normalizedRequestParameters)
        {
            if (oauth.HttpMethod == TwitEclipseAPI.HttpMethod.NONE)
                throw new ArgumentNullException("httpMethod");

            if (oauth.SignatureType == SignatureTypes.NONE)
                throw new ArgumentNullException("signatureType");

            normalizedUrl = null;
            normalizedRequestParameters = null;

            List<QueryParameter> parameters = GetQueryParameters(oauth.Uri.Query);

            if (oauth.HttpMethod == TwitterAPI.TwitEclipseAPI.HttpMethod.GET)
            {
                parameters.Add(new QueryParameter(OAuthVersionKey, oauth.Version));
                parameters.Add(new QueryParameter(OAuthNonceKey, oauth.Nonce));
                parameters.Add(new QueryParameter(OAuthTimestampKey, oauth.Timestamp));
                parameters.Add(new QueryParameter(OAuthSignatureMethodKey, HMACSHA1SignatureType));
                parameters.Add(new QueryParameter(OAuthConsumerKeyKey, oauth.ConsumerKey));

                if (!string.IsNullOrEmpty(oauth.PIN))
                    parameters.Add(new QueryParameter(OAuthVerifier, oauth.PIN));

                if (!string.IsNullOrEmpty(oauth.Token))
                    parameters.Add(new QueryParameter(OAuthTokenKey, oauth.Token));
            }
            else
            {
                parameters.Add(new QueryParameter(OAuthConsumerKeyKey, oauth.ConsumerKey));
                parameters.Add(new QueryParameter(OAuthTokenKey, oauth.Token));
                parameters.Add(new QueryParameter(OAuthNonceKey, oauth.Nonce));
                parameters.Add(new QueryParameter(OAuthTimestampKey, oauth.Timestamp));
                parameters.Add(new QueryParameter(OAuthSignatureMethodKey, HMACSHA1SignatureType));
                parameters.Add(new QueryParameter(OAuthVersionKey, oauth.Version));

                if (!string.IsNullOrEmpty(oauth.Status))
                    parameters.Add(new QueryParameter("status", UrlEncode(oauth.Status)));
            }

            parameters.Sort(new QueryParameterComparer());

            normalizedUrl = string.Format("{0}://{1}", oauth.Uri.Scheme, oauth.Uri.Host);
            if (!((oauth.Uri.Scheme == "http" && oauth.Uri.Port == 80) || (oauth.Uri.Scheme == "https" && oauth.Uri.Port == 443)))
            {
                normalizedUrl += ":" + oauth.Uri.Port;
            }
            normalizedUrl += oauth.Uri.AbsolutePath;
            normalizedRequestParameters = NormalizeRequestParameters(parameters);

            StringBuilder signatureBase = new StringBuilder();
            signatureBase.AppendFormat("{0}&", oauth.HttpMethod.ToString());
            signatureBase.AppendFormat("{0}&", UrlEncode(normalizedUrl));
            signatureBase.AppendFormat("{0}", UrlEncode(normalizedRequestParameters));

            return signatureBase.ToString();
        }

        /// <summary>
        /// Generates a signature using the specified signatureType 
        /// </summary>		
        /// <param name="url">The full url that needs to be signed including its non OAuth url parameters</param>
        /// <param name="consumerKey">The consumer key</param>
        /// <param name="consumerSecret">The consumer seceret</param>
        /// <param name="token">The token, if available. If not available pass null or an empty string</param>
        /// <param name="tokenSecret">The token secret, if available. If not available pass null or an empty string</param>
        /// <param name="httpMethod">The http method used. Must be a valid HTTP method verb (POST,GET,PUT, etc)</param>
        /// <param name="signatureType">The type of signature to use</param>
        /// <returns>A base64 string of the hash value</returns>
        public string GenerateSignature(OAuthParameters oauth, out string normalizedUrl, out string normalizedRequestParameters)
        {
            normalizedUrl = null;
            normalizedRequestParameters = null;

            switch (oauth.SignatureType)
            {
                case SignatureTypes.PLAINTEXT:
                    return Common.UrlEncode(string.Format("{0}&{1}", oauth.ConsumerSecret, oauth.TokenSecret));

                case SignatureTypes.HMACSHA1:
                    string signatureBase = GenerateSignatureBase(oauth, out normalizedUrl, out normalizedRequestParameters);

                    HMACSHA1 hmacsha1 = new HMACSHA1();
                    hmacsha1.Key = Encoding.ASCII.GetBytes(string.Format("{0}&{1}", UrlEncode(oauth.ConsumerSecret), string.IsNullOrEmpty(oauth.TokenSecret) ? "" : UrlEncode(oauth.TokenSecret)));

                    byte[] dataBuffer = System.Text.Encoding.ASCII.GetBytes(signatureBase);
                    byte[] hashBytes = hmacsha1.ComputeHash(dataBuffer);

                    return Convert.ToBase64String(hashBytes);

                case SignatureTypes.RSASHA1:
                    throw new NotImplementedException();

                default:
                    throw new ArgumentException("Unknown signature type", "signatureType");
            }
        }
    }
}