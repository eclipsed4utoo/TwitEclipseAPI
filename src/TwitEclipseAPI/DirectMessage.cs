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
    public class DirectMessage
    {
        private User m_sender;
        private User m_recipient;
        private Int64 m_id = 0;
        private Int64 m_senderID = 0;
        private Int64 m_recipientID = 0;
        private string m_text = string.Empty;
        private DateTime m_createdDate = DateTime.MinValue;
        private string m_senderScreenName = string.Empty;
        private string m_recipientScreenName = string.Empty;

        public User Sender
        {
            get { return m_sender; }
            set { m_sender = value; }
        }
        
        public User Recipient
        {
            get { return m_recipient; }
            set { m_recipient = value; }
        }
        
        public Int64 ID
        {
            get { return m_id; }
            set { m_id = value; }
        }
        
        public Int64 SenderID
        {
            get { return m_senderID; }
            set { m_senderID = value; }
        }
        
        public Int64 RecipientID
        {
            get { return m_recipientID; }
            set { m_recipientID = value; }
        }
        
        public string Text
        {
            get { return m_text; }
            set { m_text = value; }
        }
        
        public DateTime CreatedDate
        {
            get { return m_createdDate; }
            set { m_createdDate = value; }
        }
        
        public string SenderScreenName
        {
            get { return m_senderScreenName; }
            set { m_senderScreenName = value; }
        }

        public string RecipientScreenName
        {
            get { return m_recipientScreenName; }
            set { m_recipientScreenName = value; }
        }

    }
}
