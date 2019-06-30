#region Initialize references
using System.Linq;
using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using Steamworks;
using SDG.Unturned;
#endregion

namespace TourneyCore
{
    public class CommandTban : IRocketCommand
    {
        #region Tban command
        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        public string Name => "Tban";

        public string Help => "Tourneyban main command";

        public string Syntax
        {
            get { return "/tban <ban>|<unban>"; }
        }

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>() { "tourneycore.filter" };

        public void Execute(IRocketPlayer caller, params string[] param)
        {

            CSteamID judge = (CSteamID)0;

            if (!(caller is ConsolePlayer))
            {
                judge = ((UnturnedPlayer)caller).CSteamID;

            }
            if (param.Length == 0)
            {
                if (Init.Instance.Configuration.Instance.LogIDonjoin)
                {

                    Init.Instance.Configuration.Save();
                    UnturnedChat.Say(caller, "Players already logged when they joined");
                    return;
                }
                else
                {

                    //if logindonjoin false
                    if (Provider.clients.Count == 0)
                    {
                        UnturnedChat.Say(caller, "No one available");
                        return;
                    }
                    else
                    {

                        List<CSteamID> ids = Provider.clients.Select(x => x.playerID.steamID).ToList();

                       
                        if (!ids.Any())
                        {
                            return;
                        }
                        foreach (var player in ids)
                        {
                            
                            FilterData.FilterData.Instance.Configuration.Instance.Unwhitelisted = ids.Except(FilterData.FilterData.Instance.Configuration.Instance.Whitelists).ToList();
                            FilterData.FilterData.Instance.Configuration.Save();
                            foreach (var playerid in FilterData.FilterData.Instance.Configuration.Instance.Unwhitelisted)
                            {
                                UnturnedChat.Say(caller, playerid.ToString());
                            }

                            UnturnedChat.Say(caller, "IDs saved locally");

                        }

                    }

                }
                return;


            }
            if (param.Length > 0)
            {
                if (param.Length == 1)
                {



                    switch (param[0])
                    {
                        case "debug":

                            if (!FilterData.FilterData.Instance.Configuration.Instance.Unwhitelisted.Any())
                            {
                                UnturnedChat.Say(caller, "Tban list is empty");
                                return;
                            }


                            foreach (var player in FilterData.FilterData.Instance.Configuration.Instance.Unwhitelisted)
                            {

                                UnturnedChat.Say(caller, player.ToString());
                            }


                            break;
                        case "unban":



                            if (!FilterData.FilterData.Instance.Configuration.Instance.Unwhitelisted.Any())
                            {
                                UnturnedChat.Say(caller, "Tban list is empty");
                                return;
                            }



                            foreach (var player in FilterData.FilterData.Instance.Configuration.Instance.Unwhitelisted.ToList())
                            {

                                Init.Instance.Database.UnbanPlayer(player.ToString());

                                SteamBlacklist.unban(player);
                                //CommandWindow.input.onInputText("unban " + player.ToString());
                                UnturnedChat.Say(caller, "Unbanned " + player + " and relieved from Globalban");

                            }

                            break;
                        case "ban":


                            if (!FilterData.FilterData.Instance.Configuration.Instance.Unwhitelisted.Any())
                            {
                                UnturnedChat.Say(caller, "Tban list is empty");
                                return;
                            }
                            else
                            {
                                foreach (var player in FilterData.FilterData.Instance.Configuration.Instance.Unwhitelisted)
                                {

                                    /*ulong? otherPlayerID = player.GetCSteamID;
                                    UnturnedPlayer playern = UnturnedPlayer.FromCSteamID(player);*/



                                    Init.Instance.Database.BanPlayer("Tourneybanned", player.ToString(), "Tourneyban", "[TourneyCore Filter]", 2147483647);

                                    Provider.kick(player, Init.Instance.Translate("Filter_Reason"));
                                    SteamBlacklist.ban(player, Init.Instance.GetIP(player), judge, Init.Instance.Translate("Filter_Reason"), 2147483647);


                                    UnturnedChat.Say("Kicked " + player);
                                    //UnturnedChat.Say(caller, "" + player + " Tourneyban");

                                }
                            }

                            //EArenaState.SPAWN.




                            break;

                        case "enable":
                            if (Init.Instance.Configuration.Instance.LogIDonjoin == true)
                            {
                                UnturnedChat.Say(caller, "Logging on join is already enabled");
                                return;
                            }
                            UnturnedChat.Say(caller, "Enabling logging on join");
                            Init.Instance.Configuration.Instance.LogIDonjoin = true;
                            Init.Instance.Configuration.Save();
                            break;
                        case "disable":
                            if (Init.Instance.Configuration.Instance.LogIDonjoin == false)
                            {
                                UnturnedChat.Say(caller, "Logging on join is already disabled");
                                return;
                            }
                            UnturnedChat.Say(caller, "Disabling logging on join");
                            Init.Instance.Configuration.Instance.LogIDonjoin = false;
                            Init.Instance.Configuration.Save();
                            break;

                        case "clear":
                            if (!FilterData.FilterData.Instance.Configuration.Instance.Unwhitelisted.Any())
                            {
                                UnturnedChat.Say(caller, "List already empty");
                                return;
                            }
                            FilterData.FilterData.Instance.Configuration.Instance.Unwhitelisted.Clear();
                            FilterData.FilterData.Instance.Configuration.Save();
                            UnturnedChat.Say(caller, "Successfully cleared Tban list");
                            break;

                    }
                }
                else
                {
                    UnturnedChat.Say(caller, "Wrong syntax!");
                }

            }





            if (param.Length != 1 && param.Length != 0)
            {
                UnturnedChat.Say(caller, "Wrong syntax");
            }
        }
        #endregion
    }

}