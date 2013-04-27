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
    public class RateLimit
    {
        #region Private Variables

        private int m_remainingHits = 0;
        private int m_hourlyLimit = 0;
        private string m_resetTime = string.Empty;
        private Int64 m_resetTimeInSeconds = 0;

        #endregion

        #region Public Properties

        public int RemainingHits
        {
            get { return m_remainingHits; }
            set { m_remainingHits = value; }
        }
        
        public int HourlyLimit
        {
            get { return m_hourlyLimit; }
            set { m_hourlyLimit = value; }
        }
        
        public string ResetTime
        {
            get { return m_resetTime; }
            set { m_resetTime = value; }
        }
        
        public Int64 ResetTimeInSeconds
        {
            get { return m_resetTimeInSeconds; }
            set { m_resetTimeInSeconds = value; }
        }

        #endregion

        public RateLimit()
        {

        }

        public RateLimit(int hourlyLimit, int remaining, long resetTime)
        {
            m_hourlyLimit = hourlyLimit;
            m_remainingHits = remaining;
            m_resetTimeInSeconds = resetTime;

            DateTime future = new DateTime(1970, 1, 1).AddSeconds(m_resetTimeInSeconds);
            m_resetTime = future.ToString();
        }
    }
}
