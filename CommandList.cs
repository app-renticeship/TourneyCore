using Rocket.API;
using Logger = Rocket.Core.Logging.Logger;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace TourneyCore

{
    public class CommandList : IRocketCommand
    {
        public string Name
        {
            get { return "list"; }
        }

        public string Help
        {
            get { return "[TourneyCore] Lists points"; }
        }

        public string Syntax
        {
            get { return "[<rank>]"; }
        }

        public List<string> Aliases
        {
            get { return new List<string>(); }
        }

        public AllowedCaller AllowedCaller
        {
            get { return AllowedCaller.Both; }
        }
        public List<string> Permissions => new List<string>() { "tourneycore.points" };

        public void Execute(IRocketPlayer caller, params string[] command)
        {
            
            UnturnedPlayer callerPlayer = null;
            if (caller is ConsolePlayer == false) { callerPlayer = (UnturnedPlayer)caller; }

            if (command.Length == 0)
            {
                string[] rankInfo = Init.Instance.PointDB.GetTopRanks(3);
                if (caller is ConsolePlayer) { Logger.Log(Init.Instance.Translations.Instance.Translate("list_1")); }
                else { UnturnedChat.Say(caller, Init.Instance.Translations.Instance.Translate("list_1"), Color.cyan); }

                if (rankInfo.Length > 0)
                {
                    if (caller is ConsolePlayer) { Logger.Log(Init.Instance.Translations.Instance.Translate("list_2", rankInfo[0], rankInfo[1],   rankInfo[2])); }
                    else { UnturnedChat.Say(caller, Init.Instance.Translations.Instance.Translate("list_2", rankInfo[0], rankInfo[1],   Color.cyan)); }
                }
                if (rankInfo.Length > 3)
                {
                    if (caller is ConsolePlayer) { Logger.Log(Init.Instance.Translations.Instance.Translate("list_3", rankInfo[3], rankInfo[4],  rankInfo[5])); }
                    else { UnturnedChat.Say(caller, Init.Instance.Translations.Instance.Translate("list_3", rankInfo[3], rankInfo[4], Color.cyan)); }
                }
                if (rankInfo.Length > 6)
                {
                    if (caller is ConsolePlayer) { Logger.Log(Init.Instance.Translations.Instance.Translate("list_4", rankInfo[6], rankInfo[7],   rankInfo[8])); }
                    else { UnturnedChat.Say(caller, Init.Instance.Translations.Instance.Translate("list_4", rankInfo[6], rankInfo[7],   rankInfo[8]), Color.cyan); }
                }
            }
            else if (command.Length == 1 && (caller is ConsolePlayer || callerPlayer.HasPermission("list.other")))
            {
                string[] rankInfo = Init.Instance.PointDB.GetAccountByRank(Convert.ToInt32(command[0]));
                if (rankInfo[0] == null)
                {
                    if (caller is ConsolePlayer) { Logger.Log(Init.Instance.Translations.Instance.Translate("list_search_not_found")); }
                    else { UnturnedChat.Say(caller, Init.Instance.Translations.Instance.Translate("list_search_not_found"), Color.cyan); }
                }
                else
                {
                    if (caller is ConsolePlayer) { Logger.Log(Init.Instance.Translations.Instance.Translate("list_search", rankInfo[0], rankInfo[1],   rankInfo[2])); }
                    else { UnturnedChat.Say(caller, Init.Instance.Translations.Instance.Translate("list_search", rankInfo[0], rankInfo[1],   Color.cyan)); }
                }
            }
            else
            {
                if (caller is ConsolePlayer) { Logger.Log(Init.Instance.Translations.Instance.Translate("general_invalid_parameter")); }
                else { UnturnedChat.Say(caller, Init.Instance.Translations.Instance.Translate("general_invalid_parameter"), Color.cyan); }
            }
        }
    }
}