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
    public class UserStatus
    {
        #region Private variables

        private DateTime m_statusCreatedAt;
        private Int64 m_statusID = 0;
        private string m_text = string.Empty;
        private string m_source = string.Empty;
        private bool m_truncated = false;
        private Int64 m_inReplyToStatusID = 0;
        private Int64 m_inReplyToUserID = 0;
        private bool m_favorited = false;
        private string m_inReplyToScreenName = string.Empty;
        private User m_user;
        private string m_geo = string.Empty;

        #endregion

        #region Public Properties

        public string Geo
        {
            get { return m_geo; }
            set { m_geo = value; }
        }

        public User User
        {
            get { return m_user; }
            set { m_user = value; }
        }

        public DateTime StatusCreatedAt
        {
            get { return m_statusCreatedAt; }
            set { m_statusCreatedAt = value; }
        }

        public Int64 StatusID
        {
            get { return m_statusID; }
            set { m_statusID = value; }
        }

        public string Text
        {
            get { return m_text; }
            set { m_text = value; }
        }

        public string Source
        {
            get { return m_source; }
            set { m_source = value; }
        }

        public bool Truncated
        {
            get { return m_truncated; }
            set { m_truncated = value; }
        }

        public Int64 InReplyToStatusID
        {
            get { return m_inReplyToStatusID; }
            set { m_inReplyToStatusID = value; }
        }

        public Int64 InReplyToUserID
        {
            get { return m_inReplyToUserID; }
            set { m_inReplyToUserID = value; }
        }

        public bool Favorited
        {
            get { return m_favorited; }
            set { m_favorited = value; }
        }

        public string InReplyToScreenName
        {
            get { return m_inReplyToScreenName; }
            set { m_inReplyToScreenName = value; }
        }

        #endregion
    }
}
