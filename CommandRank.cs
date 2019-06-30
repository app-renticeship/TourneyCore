using Rocket.API;
using Logger = Rocket.Core.Logging.Logger;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using UnityEngine;
namespace TourneyCore
{
    public class CommandRank : IRocketCommand
    {
        public string Name
        {
            get { return "rank"; }
        }

        public string Help
        {
            get { return "[Tourney] Display current rank or get user by name"; }
        }

        public string Syntax
        {
            get { return "[<player>]"; }
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

            if (command.Length == 0 && caller is ConsolePlayer == false)
            {
                int playerPoints;
                bool playerExists = Init.dicPoints.TryGetValue(callerPlayer.CSteamID, out playerPoints);
                if (playerExists)
                {
                    UnturnedChat.Say(caller, Init.Instance.Translations.Instance.Translate("rank_self", playerPoints, Init.Instance.PointDB.GetRankBySteamID(callerPlayer.CSteamID.ToString()), Color.cyan));
                }
            }
            else if (command.Length == 1 && (caller is ConsolePlayer || callerPlayer.HasPermission("rank.other")))
            {
                UnturnedPlayer otherPlayer = UnturnedPlayer.FromName(command[0]);
                if (otherPlayer == null)
                {
                    if (caller is ConsolePlayer) { Logger.Log(Init.Instance.Translations.Instance.Translate("general_not_found")); }
                    else { UnturnedChat.Say(caller, Init.Instance.Translations.Instance.Translate("general_not_found"), Color.cyan); }
                }
                else
                {
                    int playerPoints;
                    bool playerExists = Init.dicPoints.TryGetValue(otherPlayer.CSteamID, out playerPoints);
                    if (playerExists)
                    {
                        if (caller is ConsolePlayer) { Logger.Log(Init.Instance.Translations.Instance.Translate("rank_other", playerPoints, Init.Instance.PointDB.GetRankBySteamID(otherPlayer.CSteamID.ToString()), otherPlayer.DisplayName)); }
                        else { UnturnedChat.Say(caller, Init.Instance.Translations.Instance.Translate("rank_other", playerPoints, Init.Instance.PointDB.GetRankBySteamID(otherPlayer.CSteamID.ToString()), otherPlayer.DisplayName), Color.cyan); }
                    }
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