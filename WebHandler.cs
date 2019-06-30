#region Initialize references
using Rocket.Core.Logging;
using SDG.Unturned;
using System;
using System.Net;
using System.Xml;
using Steamworks;
#endregion

namespace TourneyCore
{
    public class WebHandler 
    {
        public static string playerdir = "/response/players/player";
        #region CheckData
        public static void CheckData(CSteamID player, ref ESteamRejection? rejection)
        {
            #region variables
        string steamID = string.Empty;
        string communityvisibilitystate = string.Empty;
        string vacBanned = string.Empty;
        string profilestate = string.Empty;
        string Vacdays = string.Empty;
        string playtime = string.Empty;
        int steamprivatekey = 1234567890; // my private steam key 
        ServicePointManager.DefaultConnectionLimit = 3;
            #endregion

            #region webhandler
            try
            {
                
                // webhandling and shyt
                var httpRequest = (HttpWebRequest)WebRequest.Create("http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key="+steamprivatekey+"&steamids=" + player+"&format=xml");
                httpRequest.Proxy = null;
                var httpRequest2 = (HttpWebRequest)WebRequest.Create("http://api.steampowered.com/ISteamUser/GetPlayerBans/v1/?key="+steamprivatekey+"&steamids=" + player+"&format=xml");
                httpRequest2.Proxy = null;
                var httpRequest3 = (HttpWebRequest)WebRequest.Create("http://api.steampowered.com/IPlayerService/GetOwnedGames/v0001/?key="+steamprivatekey+"&format=xml&input_json={%22steamid%22:" + player+",%22appids_filter%22:[304930]}");
                httpRequest3.Proxy = null;
               
                var response = (HttpWebResponse)httpRequest.GetResponse();
                var receiveStream = response.GetResponseStream();
               
                var response2 = (HttpWebResponse)httpRequest2.GetResponse();
                var receiveStream2 = response2.GetResponseStream();
               
                var response3 = (HttpWebResponse)httpRequest3.GetResponse();
                var receiveStream3 = response3.GetResponseStream();
               
                
                XmlDocument doc = new XmlDocument();
                XmlDocument doc2 = new XmlDocument();
                XmlDocument doc3 = new XmlDocument();
                doc.Load(receiveStream);
                receiveStream.Close();
                doc2.Load(receiveStream2);
                receiveStream2.Close();
                doc3.Load(receiveStream3);
                receiveStream3.Close();

                    
                /*Node processing*/
                XmlNodeList nodes = doc.SelectNodes(playerdir);
                XmlNodeList nodes2 = doc2.SelectNodes(playerdir);
                XmlNode nodes3 = doc3.SelectSingleNode("/response/games/message");
                
                foreach (XmlNode node in nodes)
                {

                    steamID = node["steamid"].InnerText;
                    Logger.LogWarning(steamID);
                    communityvisibilitystate = node["communityvisibilitystate"].InnerText;
                    Logger.LogWarning(communityvisibilitystate);
                    profilestate = node["profilestate"].InnerText;
                    Logger.LogWarning(profilestate);
                   
                }
                foreach (XmlNode node in nodes2)
                {

                    vacBanned = node["VACBanned"].InnerText;
                    Vacdays = node["DaysSinceLastBan"].InnerText;
                   

                }
                playtime = nodes3["playtime_forever"].InnerText;
                
                /*Logger.LogWarning("get node");

                steamID = steamIdNode.Value;*/
            }
            catch
            {
                /* Fallback */

                   Logger.LogWarning("Access denied: " + player);
                   rejection =  Init.Instance.GetSteamRejection(1);
                

                
            }
            #endregion

            #region data check
            Logger.LogWarning("[" + steamID + "] Stream Data Handler");
            /*check profile set or not */
            if (profilestate == "0")
            {
                Logger.LogWarning("Access denied: " + player + "[TourneyCore] Profile not set");
                rejection = Init.Instance.GetSteamRejection(0);
                return;
            }
            /* if profile private or friends only */
            if ((int.Parse(communityvisibilitystate) < 3))
            {

                /*reject*/
                Logger.LogWarning("Access denied: " + player +"[TourneyCore] Private profile.");
                rejection = Init.Instance.GetSteamRejection(0);
                return;

            }
            /*if unturned playtime under 200 hours*/
            if (int.Parse(playtime) < 12000)
            {
                /*reject*/
                Logger.LogWarning("Access denied: " + player + "[TourneyCore] Hidden or less than 200h playtime");
                rejection = Init.Instance.GetSteamRejection(0);
                return;
            }
            /* if vac banned */
            if (vacBanned != "false")
            {
                try
                {
                    /*how many days since last vacban*/
                    int d = int.Parse(Vacdays);
                    if (d < 365)
                    {
                        /*if under 1 year reject*/
                        Logger.LogWarning("Access denied: " + player + "[TourneyCore] vac ban under 1 year");
                        rejection = Init.Instance.GetSteamRejection(0);
                        return;

                    }
                    else
                    {
                        /* If days since vac ban > 1 year */
                        Logger.LogWarning("Access Granted:" +player);
                        return;
                    }
                }
                catch (Exception ex) { Logger.LogException(ex); }
            }
            Logger.LogWarning("Granted");
            #endregion
        }
        #region old method not working
        /*public void OnJoinRequested(CSteamID player, ref ESteamRejection? rejectionReason)
        {
            if (FilterData.FilterData.Instance.Configuration.Instance.Whitelists.Any())
            {
                if (FilterData.FilterData.Instance.Configuration.Instance.Whitelists.Contains(player))
                {
                    //if (Configuration.Instance.Logging) { Logger.LogWarning("Access granted: " + player + " // Reason: Whitelist."); }
                    return;
                }
            }
            Logger.LogWarning(" BEFORE");
            string steamID = string.Empty;
            string communityvisibilitystate = string.Empty;
            string vacBanned = string.Empty;
            string profilestate = string.Empty;
            string Vacdays = string.Empty;
            string playtime = string.Empty;
            string playerdir = "/response/players/player";
            try
            {

                var httpRequest = (HttpWebRequest)WebRequest.Create("http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key=&steamids=" + player + "&format=xml");
                var httpRequest2 = (HttpWebRequest)WebRequest.Create("http://api.steampowered.com/ISteamUser/GetPlayerBans/v1/?key=&steamids=" + player + "&format=xml");
                var httpRequest3 = (HttpWebRequest)WebRequest.Create("http://api.steampowered.com/IPlayerService/GetOwnedGames/v0001/?key=&format=xml&input_json={%22steamid%22:" + player + ",%22appids_filter%22:[304930]}");


                var response = (HttpWebResponse)httpRequest.GetResponse();
                var receiveStream = response.GetResponseStream();

                var response2 = (HttpWebResponse)httpRequest2.GetResponse();
                var receiveStream2 = response2.GetResponseStream();

                var response3 = (HttpWebResponse)httpRequest3.GetResponse();
                var receiveStream3 = response3.GetResponseStream();


                XmlDocument doc = new XmlDocument();
                doc.Load(receiveStream);
                receiveStream.Close();
                doc.Save(Console.Out);

                XmlDocument doc2 = new XmlDocument();
                doc2.Load(receiveStream2);
                receiveStream2.Close();

                XmlDocument doc3 = new XmlDocument();
                doc2.Load(receiveStream3);
                receiveStream3.Close();


                XmlNodeList nodes = doc.SelectNodes(playerdir);
                XmlNodeList nodes2 = doc2.SelectNodes(playerdir);
                XmlNode nodes3 = doc3.SelectSingleNode("/response/games/message");

                foreach (XmlNode node in nodes)
                {

                    steamID = node["steamid"].InnerText;
                    Logger.LogWarning(steamID);
                    communityvisibilitystate = node["communityvisibilitystate"].InnerText;
                    Logger.LogWarning(communityvisibilitystate);
                    profilestate = node["profilestate"].InnerText;
                    Logger.LogWarning(profilestate);
                }
                foreach (XmlNode node in nodes2)
                {

                    vacBanned = node["VACBanned"].InnerText;
                    Vacdays = node["DaysSinceLastBan"].InnerText;


                }
                playtime = nodes3["playtime_forever"].InnerText;
                /*Logger.LogWarning("get node");

                steamID = steamIdNode.Value;
            }
            catch
            {
                if (TourneyCore.Init.Instance.Configuration.Instance.BouncerKickOnTimeout)
                {
                    Logger.LogWarning("Access denied: " + player + " // Reason: Timeout.");
                    rejectionReason = ESteamRejection.AUTH_TIMED_OUT;
                }
                else
                {
                    Logger.LogWarning("Access granted: " + player + " // Reason: Timeout.");

                }
                return;
            }


            if (profilestate == "0")
            {
                Logger.LogWarning("Access denied: " + player + " // Reason: No profile.");
                rejectionReason = Init.Instance.GetSteamRejection();
                return;
            }

            if ((communityvisibilitystate == "1"))
            {


                Logger.LogWarning("Access denied: " + player + " (" + steamID + ") // Reason: Private profile.");
                rejectionReason =Init.Instance.GetSteamRejection();
                return;

            }
            if (int.Parse(playtime) < 12000)
            {
                rejectionReason = Init.Instance.GetSteamRejection();
                return;
            }
            if (vacBanned != "false")
            {
                try
                {

                    int d = int.Parse(Vacdays);
                    if (d < 365)
                    {

                        rejectionReason = Init.Instance.GetSteamRejection();
                        return;

                    }
                }
                catch (Exception ex) { Logger.LogException(ex); }
            }
        }*/
        #endregion
        #endregion

    }
}
