TwitEclipseAPI
==============

TwitEclipseAPI is a .Net wrapper for many of the Twitter REST API methods. It uses LINQ-to-XML to parse the data from Twitter, and supports both Web and Desktop OAuth authorization.

Most of the OAuth code was taken from [Shannon Whitley's OAuth] example. I made a number of changes to the code, including adding the PIN workflow support for Desktop OAuth authorization.

TwitEclipseAPI can also be used for mobile devices because of it's small footprint. I was able to completely remove the System.Web.dll reference(which is around 5MB in size) by using only the code from the .dll that I needed.

TwitEclipseAPI also handles shortening of links using bit.ly. Simply get the necessary login and api key from bit.ly, and assign those to their corresponding variables.

Available Twitter API methods:

* Friends Timeline
* User Timeline
* Public Timeline
* Mentions
* Update Status
* Delete Status
* Status Show
* Friends
* Followers
* Direct Messages Received
* Direct Messages Sent
* Direct Messages Delete
* Follow User
* Un-Follow User
* Show Friendship
* Verify Credentials
* Rate Limit
* End Session
* Update Delivery Device
* Update Profile Colors
* Favorites
* Favorites Add
* Favorites Delete
* Block User
* Un-Block User
* Show Block
* Blocking List
* Report Spam
* OAuth Request Token
* OAuth Authorize
* OAuth Authenticate (as of release 0.8 and source code 35930)




  [Shannon Whitley's OAuth]: http://www.voiceoftech.com/swhitley/?p=681
