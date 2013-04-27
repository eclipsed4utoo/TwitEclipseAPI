// ********************************************
// TwitEclipseAPI - .Net Twitter API Wrapper  *
// Developed by Ryan Alford                   *
// ********************************************


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace TwitterAPI
{
    public class User
    {
        #region Private Variables
        private Int64 m_userID = 0;
        private string m_name = string.Empty;
        private Int64 m_friendsCount = 0;
        private DateTime m_createdAt;
        private Int64 m_favoritesCount = 0;
        private string m_UTCOffset = string.Empty;
        private string m_timeZone = string.Empty;
        private string m_profileBackgroundImageURL = string.Empty;
        private bool m_profileBackgroundTile = false;
        private Int64 m_statusesCount = 0;
        private bool m_notifications = false;
        private bool m_following = false;
        private bool m_protected = false;
        private Int64 m_followersCount = 0;
        private string m_profileBackgroundColor = string.Empty;
        private string m_profileTextColor = string.Empty;
        private string m_profileLinkColor = string.Empty;
        private string m_profileSidebarFillColor = string.Empty;
        private string m_profileSidebarBorderColor = string.Empty;
        private string m_screenName = string.Empty;
        private string m_location = string.Empty;
        private string m_description = string.Empty;
        private string m_profileImageURL = string.Empty;
        private string m_url = string.Empty;

        #endregion

        #region Public Properties

        public Int64 UserID
        {
            get { return m_userID; }
            set { m_userID = value; }
        }

        public string Name
        {
            get { return m_name; }
            set { m_name = value; }
        }

        public string ScreenName
        {
            get { return m_screenName; }
            set { m_screenName = value; }
        }

        public string Location
        {
            get { return m_location; }
            set { m_location = value; }
        }

        public string Description
        {
            get { return m_description; }
            set { m_description = value; }
        }

        public string ProfileImageURL
        {
            get { return m_profileImageURL; }
            set { m_profileImageURL = value; }
        }

        public string Url
        {
            get { return m_url; }
            set { m_url = value; }
        }

        public bool Protected
        {
            get { return m_protected; }
            set { m_protected = value; }
        }

        public Int64 FollowersCount
        {
            get { return m_followersCount; }
            set { m_followersCount = value; }
        }

        public string ProfileBackgroundColor
        {
            get { return m_profileBackgroundColor; }
            set { m_profileBackgroundColor = value; }
        }

        public string ProfileTextColor
        {
            get { return m_profileTextColor; }
            set { m_profileTextColor = value; }
        }

        public string ProfileLinkColor
        {
            get { return m_profileLinkColor; }
            set { m_profileLinkColor = value; }
        }

        public string ProfileSidebarFillColor
        {
            get { return m_profileSidebarFillColor; }
            set { m_profileSidebarFillColor = value; }
        }

        public string ProfileSidebarBorderColor
        {
            get { return m_profileSidebarBorderColor; }
            set { m_profileSidebarBorderColor = value; }
        }

        public Int64 FriendsCount
        {
            get { return m_friendsCount; }
            set { m_friendsCount = value; }
        }

        public DateTime CreatedAt
        {
            get { return m_createdAt; }
            set { m_createdAt = value; }
        }

        public Int64 FavoritesCount
        {
            get { return m_favoritesCount; }
            set { m_favoritesCount = value; }
        }

        public string UTCOffset
        {
            get { return m_UTCOffset; }
            set { m_UTCOffset = value; }
        }

        public string TimeZone
        {
            get { return m_timeZone; }
            set { m_timeZone = value; }
        }

        public string ProfileBackgroundImageURL
        {
            get { return m_profileBackgroundImageURL; }
            set { m_profileBackgroundImageURL = value; }
        }

        public bool ProfileBackgroundTile
        {
            get { return m_profileBackgroundTile; }
            set { m_profileBackgroundTile = value; }
        }

        public Int64 StatusesCount
        {
            get { return m_statusesCount; }
            set { m_statusesCount = value; }
        }

        public bool Notifications
        {
            get { return m_notifications; }
            set { m_notifications = value; }
        }

        public bool Following
        {
            get { return m_following; }
            set { m_following = value; }
        }

        #endregion

        #region Constructor

        public User(XElement e, string mode)
        {
            if (string.Compare(mode, "status", true) == 0)
            {
                try
                {
                    m_userID = Int64.Parse(e.Element("user").Element("id").Value);
                    Name = e.Element("user").Element("name").Value;
                    ScreenName = e.Element("user").Element("screen_name").Value;
                    Location = e.Element("user").Element("location").Value;
                    Description = e.Element("user").Element("description").Value;
                    ProfileImageURL = e.Element("user").Element("profile_image_url").Value;
                    Url = e.Element("user").Element("url").Value;
                    Protected = (!string.IsNullOrEmpty(e.Element("user").Element("protected").Value)) ? bool.Parse(e.Element("user").Element("protected").Value) : false;
                    FollowersCount = Int64.Parse(e.Element("user").Element("followers_count").Value);
                    ProfileBackgroundColor = e.Element("user").Element("profile_background_color").Value;
                    ProfileTextColor = e.Element("user").Element("profile_text_color").Value;
                    ProfileLinkColor = e.Element("user").Element("profile_link_color").Value;
                    ProfileSidebarFillColor = e.Element("user").Element("profile_sidebar_fill_color").Value;
                    ProfileSidebarBorderColor = e.Element("user").Element("profile_sidebar_border_color").Value;
                    FriendsCount = Int64.Parse(e.Element("user").Element("friends_count").Value);
                    CreatedAt = Common.ParseDateTime(e.Element("user").Element("created_at").Value);
                    FavoritesCount = Int64.Parse(e.Element("user").Element("favourites_count").Value);
                    UTCOffset = e.Element("user").Element("utc_offset").Value;
                    TimeZone = e.Element("user").Element("time_zone").Value;
                    ProfileBackgroundImageURL = e.Element("user").Element("profile_background_image_url").Value;
                    ProfileBackgroundTile = (!string.IsNullOrEmpty(e.Element("user").Element("profile_background_tile").Value)) ? bool.Parse(e.Element("user").Element("profile_background_tile").Value) : false;
                    StatusesCount = Int64.Parse(e.Element("user").Element("statuses_count").Value);
                    Notifications = (!string.IsNullOrEmpty(e.Element("user").Element("notifications").Value)) ? bool.Parse(e.Element("user").Element("notifications").Value) : false;
                    Following = (!string.IsNullOrEmpty(e.Element("user").Element("following").Value)) ? bool.Parse(e.Element("user").Element("following").Value) : false;
                }
                catch { }
            }
            else if (string.Compare(mode, "user", true) == 0)
            {
                try
                {
                    m_userID = Int64.Parse(e.Element("id").Value);
                    Name = e.Element("name").Value;
                    ScreenName = e.Element("screen_name").Value;
                    Location = e.Element("location").Value;
                    Description = e.Element("description").Value;
                    ProfileImageURL = e.Element("profile_image_url").Value;
                    Url = e.Element("url").Value;
                    Protected = (!string.IsNullOrEmpty(e.Element("protected").Value)) ? bool.Parse(e.Element("protected").Value) : false;
                    FollowersCount = Int64.Parse(e.Element("followers_count").Value);
                    ProfileBackgroundColor = e.Element("profile_background_color").Value;
                    ProfileTextColor = e.Element("profile_text_color").Value;
                    ProfileLinkColor = e.Element("profile_link_color").Value;
                    ProfileSidebarFillColor = e.Element("profile_sidebar_fill_color").Value;
                    ProfileSidebarBorderColor = e.Element("profile_sidebar_border_color").Value;
                    FriendsCount = Int64.Parse(e.Element("friends_count").Value);
                    CreatedAt = Common.ParseDateTime(e.Element("created_at").Value);
                    FavoritesCount = Int64.Parse(e.Element("favourites_count").Value);
                    UTCOffset = e.Element("utc_offset").Value;
                    TimeZone = e.Element("time_zone").Value;
                    ProfileBackgroundImageURL = e.Element("profile_background_image_url").Value;
                    ProfileBackgroundTile = (!string.IsNullOrEmpty(e.Element("profile_background_tile").Value)) ? bool.Parse(e.Element("profile_background_tile").Value) : false;
                    StatusesCount = Int64.Parse(e.Element("statuses_count").Value);
                    Notifications = (!string.IsNullOrEmpty(e.Element("notifications").Value)) ? bool.Parse(e.Element("notifications").Value) : false;
                    Following = (!string.IsNullOrEmpty(e.Element("following").Value)) ? bool.Parse(e.Element("following").Value) : false;
                }
                catch { }
            }
            else if (string.Compare(mode, "sender", true) == 0)
            {
                try
                {
                    m_userID = Int64.Parse(e.Element("sender").Element("id").Value);
                    Name = e.Element("sender").Element("name").Value;
                    ScreenName = e.Element("sender").Element("screen_name").Value;
                    Location = e.Element("sender").Element("location").Value;
                    Description = e.Element("sender").Element("description").Value;
                    ProfileImageURL = e.Element("sender").Element("profile_image_url").Value;
                    Url = e.Element("sender").Element("url").Value;
                    Protected = (!string.IsNullOrEmpty(e.Element("sender").Element("protected").Value)) ? bool.Parse(e.Element("sender").Element("protected").Value) : false;
                    FollowersCount = Int64.Parse(e.Element("sender").Element("followers_count").Value);
                    ProfileBackgroundColor = e.Element("sender").Element("profile_background_color").Value;
                    ProfileTextColor = e.Element("sender").Element("profile_text_color").Value;
                    ProfileLinkColor = e.Element("sender").Element("profile_link_color").Value;
                    ProfileSidebarFillColor = e.Element("sender").Element("profile_sidebar_fill_color").Value;
                    ProfileSidebarBorderColor = e.Element("sender").Element("profile_sidebar_border_color").Value;
                    FriendsCount = Int64.Parse(e.Element("sender").Element("friends_count").Value);
                    CreatedAt = Common.ParseDateTime(e.Element("sender").Element("created_at").Value);
                    FavoritesCount = Int64.Parse(e.Element("sender").Element("favourites_count").Value);
                    UTCOffset = e.Element("sender").Element("utc_offset").Value;
                    TimeZone = e.Element("sender").Element("time_zone").Value;
                    ProfileBackgroundImageURL = e.Element("sender").Element("profile_background_image_url").Value;
                    ProfileBackgroundTile = (!string.IsNullOrEmpty(e.Element("sender").Element("profile_background_tile").Value)) ? bool.Parse(e.Element("sender").Element("profile_background_tile").Value) : false;
                    StatusesCount = Int64.Parse(e.Element("sender").Element("statuses_count").Value);
                    Notifications = (!string.IsNullOrEmpty(e.Element("sender").Element("notifications").Value)) ? bool.Parse(e.Element("sender").Element("notifications").Value) : false;
                    Following = (!string.IsNullOrEmpty(e.Element("sender").Element("following").Value)) ? bool.Parse(e.Element("sender").Element("following").Value) : false;
                }
                catch { }
            }
            else if (string.Compare(mode, "recipient", true) == 0)
            {
                try
                {
                    m_userID = Int64.Parse(e.Element("recipient").Element("id").Value);
                    Name = e.Element("recipient").Element("name").Value;
                    ScreenName = e.Element("recipient").Element("screen_name").Value;
                    Location = e.Element("recipient").Element("location").Value;
                    Description = e.Element("recipient").Element("description").Value;
                    ProfileImageURL = e.Element("recipient").Element("profile_image_url").Value;
                    Url = e.Element("recipient").Element("url").Value;
                    Protected = (!string.IsNullOrEmpty(e.Element("recipient").Element("protected").Value)) ? bool.Parse(e.Element("recipient").Element("protected").Value) : false;
                    FollowersCount = Int64.Parse(e.Element("recipient").Element("followers_count").Value);
                    ProfileBackgroundColor = e.Element("recipient").Element("profile_background_color").Value;
                    ProfileTextColor = e.Element("recipient").Element("profile_text_color").Value;
                    ProfileLinkColor = e.Element("recipient").Element("profile_link_color").Value;
                    ProfileSidebarFillColor = e.Element("recipient").Element("profile_sidebar_fill_color").Value;
                    ProfileSidebarBorderColor = e.Element("recipient").Element("profile_sidebar_border_color").Value;
                    FriendsCount = Int64.Parse(e.Element("recipient").Element("friends_count").Value);
                    CreatedAt = Common.ParseDateTime(e.Element("recipient").Element("created_at").Value);
                    FavoritesCount = Int64.Parse(e.Element("recipient").Element("favourites_count").Value);
                    UTCOffset = e.Element("recipient").Element("utc_offset").Value;
                    TimeZone = e.Element("recipient").Element("time_zone").Value;
                    ProfileBackgroundImageURL = e.Element("recipient").Element("profile_background_image_url").Value;
                    ProfileBackgroundTile = (!string.IsNullOrEmpty(e.Element("recipient").Element("profile_background_tile").Value)) ? bool.Parse(e.Element("recipient").Element("profile_background_tile").Value) : false;
                    StatusesCount = Int64.Parse(e.Element("recipient").Element("statuses_count").Value);
                    Notifications = (!string.IsNullOrEmpty(e.Element("recipient").Element("notifications").Value)) ? bool.Parse(e.Element("recipient").Element("notifications").Value) : false;
                    Following = (!string.IsNullOrEmpty(e.Element("recipient").Element("following").Value)) ? bool.Parse(e.Element("recipient").Element("following").Value) : false;
                }
                catch { }
            }
        }

        #endregion
    }
}
