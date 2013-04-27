// ********************************************
// TwitEclipseAPI - .Net Twitter API Wrapper  *
// Developed by Ryan Alford                   *
// ********************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TwitterAPI
{
    public class OAuthParameters
    {
        private Uri m_uri;
        private string m_consumerKey = string.Empty;
        private string m_consumerSecret = string.Empty;
        private string m_token = string.Empty;
        private string m_tokenSecret = string.Empty;
        private TwitterAPI.TwitEclipseAPI.HttpMethod m_httpMethod = TwitEclipseAPI.HttpMethod.NONE;
        private string m_status = string.Empty;
        private string m_timestamp = string.Empty;
        private string m_nonce = string.Empty;
        private TwitterAPI.OAuthBase.SignatureTypes m_signatureType = OAuthBase.SignatureTypes.NONE;
        private string m_pin = string.Empty;
        private string m_version = "1.0";

        public string ConsumerSecret
        {
            get { return m_consumerSecret; }
            set { m_consumerSecret = value; }
        }

        public string Version
        {
            get { return m_version; }
            set { m_version = value; }
        }

        public string PIN
        {
            get { return m_pin; }
            set { m_pin = value; }
        }

        public TwitterAPI.OAuthBase.SignatureTypes SignatureType
        {
            get { return m_signatureType; }
            set { m_signatureType = value; }
        }

        public string Nonce
        {
            get { return m_nonce; }
            set { m_nonce = value; }
        }

        public string Timestamp
        {
            get { return m_timestamp; }
            set { m_timestamp = value; }
        }

        public string Status
        {
            get { return m_status; }
            set { m_status = value; }
        }

        public TwitterAPI.TwitEclipseAPI.HttpMethod HttpMethod
        {
            get { return m_httpMethod; }
            set { m_httpMethod = value; }
        }

        public string TokenSecret
        {
            get { return m_tokenSecret; }
            set { m_tokenSecret = value; }
        }

        public string Token
        {
            get { return m_token; }
            set { m_token = value; }
        }

        public string ConsumerKey
        {
            get { return m_consumerKey; }
            set { m_consumerKey = value; }
        }

        public Uri Uri
        {
            get { return m_uri; }
            set { m_uri = value; }
        }

    }
}
