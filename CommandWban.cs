#region Initialize variables
using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
#endregion

namespace TourneyCore
{
    public class CommandWban : IRocketCommand
    {
        #region Wban command
        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        public string Name => "wban";

        public string Help => "TourneyFilter Whitelist";

        public string Syntax => "<player>";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>() { "tourneycore.filter" };

        public void Execute(IRocketPlayer caller, params string[] command)
        {
            if (command.Length != 1 && command.Length != 0)
            {
                UnturnedChat.Say(caller, "Wrong arguments, /wban <name or id>");
                return;
            }
            if (command.Length == 1)

            {
                UnturnedPlayer player = UnturnedPlayer.FromName(command[0]);



                if (player == null)
                {
                    UnturnedChat.Say(caller, Init.Instance.Translate("Filter_player_not_found"));
                    return;
                }

                if (FilterData.FilterData.Instance.Configuration.Instance.Whitelists.Contains(player.CSteamID))
                {
                    
                    
                    UnturnedChat.Say(caller, player + Init.Instance.Translate("Filter_WhitelistedRemoved"));
                    FilterData.FilterData.Instance.Configuration.Instance.Unwhitelisted.Add(player.CSteamID);
                    FilterData.FilterData.Instance.Configuration.Instance.Whitelists.Remove(player.CSteamID);
                    FilterData.FilterData.Instance.Configuration.Save();
                }
                else
                {
                    if (FilterData.FilterData.Instance.Configuration.Instance.Unwhitelisted.Contains(player.CSteamID))
                    {
                        
                        FilterData.FilterData.Instance.Configuration.Instance.Unwhitelisted.Remove(player.CSteamID);
                        FilterData.FilterData.Instance.Configuration.Instance.Whitelists.Add(player.CSteamID);
                        FilterData.FilterData.Instance.Configuration.Save();
                        UnturnedChat.Say(caller, player + Init.Instance.Translate("Filter_Whitelisting"));
                        return;
                    }
                    else
                    {




                        FilterData.FilterData.Instance.Configuration.Instance.Whitelists.Add(player.CSteamID);
                        FilterData.FilterData.Instance.Configuration.Save();
                        UnturnedChat.Say(caller, player + Init.Instance.Translate("Filter_Whitelisting"));
                    }
                }

            }
            if (command.Length == 0)
            {
                if (FilterData.FilterData.Instance.Configuration.Instance.Whitelists.Count == 0)
                {
                    UnturnedChat.Say(caller, "Whitelist Empty");
                    return;
                }
                else
                {


                    foreach (var player in FilterData.FilterData.Instance.Configuration.Instance.Whitelists)
                    {

                        UnturnedChat.Say(caller, player.ToString() + " In Whitelist");
                    }
                }

            }


        }
        #endregion
    }
}
