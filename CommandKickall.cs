#region Initialize references
using UnityEngine;
using Rocket.API;
using Rocket.Unturned.Chat;
using System.Collections.Generic;
using System.Linq;
using Steamworks;
using SDG.Unturned;
#endregion

namespace TourneyCore
{
    public class CommandKickall : IRocketCommand
    {
        #region Kickall Command
        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        public string Name => "kickall";

        public string Help => "[TourneyCore] Kick All";

        public string Syntax => "<player>";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>() { "tourneycore.misc" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (Provider.clients.Count == 0)
            {
                UnturnedChat.Say(caller, "No one available");
                return;
            }
            else
            {


                List<CSteamID> rawids = Provider.clients.Select(x => x.playerID.steamID).ToList();
                /* if there is someone whitelisted*/
                if (!FilterData.FilterData.Instance.Configuration.Instance.Whitelists.Any())
                {
                    foreach (var id in rawids)
                    {
                        Provider.kick(id, Init.Instance.Translate("Misc_kickall"));
                        UnturnedChat.Say(caller, "[TourneyCore] Kicked " + id, Color.cyan);

                    }

                }
                else
                {
                    List<CSteamID> filteredids = rawids.Except(FilterData.FilterData.Instance.Configuration.Instance.Whitelists).ToList();
                    foreach (var id in filteredids)
                    {


                        Provider.kick(id, Init.Instance.Translate("Misc_kickall"));
                        UnturnedChat.Say(caller, "[TourneyCore] Kicked " + id,Color.cyan);

                    }
                }
                


            }
        }
        #endregion
    }
}