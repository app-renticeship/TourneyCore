#region Initialize References
using UnityEngine;
using Rocket.API;
using Rocket.Unturned.Player;
using Rocket.Unturned.Chat;
using System.Collections.Generic;
using System.Linq;
using Steamworks;
#endregion

namespace TourneyCore
{
    public class CommandCast : IRocketCommand
    {
        #region Cast command
        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        public string Name => "cast";

        public string Help => "[TourneyCore] Toggles cast mode";

        public string Syntax => "<player>";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>() { "tourneycore.misc" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer unPlayer = (UnturnedPlayer)caller;
            CSteamID id = (unPlayer.CSteamID);


            //if vanish and god
            if (unPlayer.Features.GodMode && unPlayer.Features.VanishMode)
            {
                //if list is not empty
                if (FilterData.FilterData.Instance.Configuration.Instance.Casters.Any())
                {

                    //if list contains player
                    if (FilterData.FilterData.Instance.Configuration.Instance.Casters.Contains(id))
                    {
                        FilterData.FilterData.Instance.Configuration.Instance.Casters.Remove(id);
                        FilterData.FilterData.Instance.Configuration.Save();
                        UnturnedChat.Say(caller, "[TourneyCore] Disabled Casting mode!", Color.magenta);
                        unPlayer.Features.GodMode = false;
                        unPlayer.Features.VanishMode = false;
                        return;
                    }
                }
                //if list empty
                else
                {
                    UnturnedChat.Say(caller, "[TourneyCore] Disabled Casting mode!", Color.magenta);
                    unPlayer.Features.GodMode = false;
                    unPlayer.Features.VanishMode = false;
                    return;
                }
            }
            //if not vanish and god or abstract
            else
            {
                //if list empty
                if (!FilterData.FilterData.Instance.Configuration.Instance.Casters.Any())
                {
                    FilterData.FilterData.Instance.Configuration.Instance.Casters.Add(id);
                    FilterData.FilterData.Instance.Configuration.Save();
                    UnturnedChat.Say(caller, "[TourneyCore] Enabled Casting mode!", Color.magenta);
                    unPlayer.Features.GodMode = true;
                    unPlayer.Features.VanishMode = true;
                    return;
                }
                //if list not empty
                else
                {

                    //if contains player
                    if (FilterData.FilterData.Instance.Configuration.Instance.Casters.Contains(id))
                    {
                        UnturnedChat.Say(caller, "[TourneyCore] Enabled Casting mode!", Color.magenta);
                        unPlayer.Features.GodMode = true;
                        unPlayer.Features.VanishMode = true;
                        return;
                    }
                    //if doesn't contain player
                    else
                    {
                        FilterData.FilterData.Instance.Configuration.Instance.Casters.Add(id);
                        FilterData.FilterData.Instance.Configuration.Save();
                        UnturnedChat.Say(caller, "[TourneyCore] Enabled Casting mode!", Color.magenta);
                        unPlayer.Features.GodMode = true;
                        unPlayer.Features.VanishMode = true;
                        return;
                    }
                }
            }






        }
        #endregion
    }
}