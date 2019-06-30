#region Initialize references
using Rocket.API;
using Rocket.Unturned.Player;
using Rocket.Unturned.Chat;
using System.Collections.Generic;
#endregion

namespace TourneyCore
{
    public class CommandFall : IRocketCommand
    {
        #region Fall Command
        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        public string Name => "fall";

        public string Help => "[TourneyCore] Toggles negate fall damage";

        public string Syntax => "<player>";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>() { "tourneycore.misc" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer unPlayer = (UnturnedPlayer)caller;




            if (Init.Instance.Configuration.Instance.Fallprotect)
            {
                UnturnedChat.Say(caller, "[TourneyCore] NegateFallDamage disabled");
                Init.Instance.Configuration.Instance.Fallprotect = false;
                Init.Instance.Configuration.Save();

            }
            else
            {
                UnturnedChat.Say(caller, "[TourneyCore] NegateFallDamage enabled");
                Init.Instance.Configuration.Instance.Fallprotect = true;
                Init.Instance.Configuration.Save();
            }






        }
        #endregion
    }
}