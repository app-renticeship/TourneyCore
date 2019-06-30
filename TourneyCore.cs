#region initialize references
using System.Collections.Generic;
using System;
using System.Linq;
using Rocket.Unturned.Permissions;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using Rocket.API.Collections;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Events;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;
using Rocket.Unturned.Skills;
using System.Collections;
#endregion

namespace TourneyCore
{
    public class Init : RocketPlugin<CoreConfig>
    {
        #region Initialize Variables
        public static Init Instance;
        public bool Warmup = false;
        public int buffer;
        public PointsDB PointDB;
        public DatabaseManager Database;
        private Coroutine GG;
        public static Dictionary<Steamworks.CSteamID, int> dicPoints = new Dictionary<Steamworks.CSteamID, int>();
        private DateTime? lastQuery = DateTime.Now;
        #endregion

        #region Initialize Plugin
        protected override void Load()
        {

            
            Instance = this;
            PointDB = new PointsDB();
            Logger.LogWarning("[TourneyCore] Points Database Initialized");

            

            Database = new DatabaseManager();
            Logger.LogWarning("[TourneyCore] GlobanBan Database Initialized");
            #region load events
            UnturnedPlayerEvents.OnPlayerRevive += OnRevive;
            if(LevelManager.levelType == ELevelType.ARENA)
            { 

            UnturnedPlayerEvents.OnPlayerDeath += onDeath;
            StartCoroutine(getArenaState());
            }
            U.Events.OnPlayerConnected += OnPlayerConnected;
            U.Events.OnPlayerDisconnected += OnPlayerDisconnected;

            UnturnedPlayerEvents.OnPlayerUpdateStat += onUpdateStat;
            
            if (Instance.Configuration.Instance.LogIDonjoin)
            {
                Logger.LogWarning("[TourneyCore] Auto Filter On");
            }
            if (Instance.Configuration.Instance.Fallprotect)
            {
                Logger.LogWarning("[TourneyCore] Fallprotect Enabled");
                
                
            }
            else
            {
                Logger.LogWarning("[TourneyCore] Fallprotect Disabled");
            }
            if (Instance.Configuration.Instance.Bouncer)
            {
                Logger.LogWarning("[TourneyCore] Bouncer Enabled");
                UnturnedPermissions.OnJoinRequested += OnJoinRequested;
            }
            else
            {
                Logger.LogWarning("[TourneyCore] Bouncer Disabled");
            }
            Logger.LogWarning("[TourneyCore] Loaded!\n by Dante#2406 for p9nda");
            #endregion
        }

        protected override void Unload()
        {
            #region unload events
            dicPoints.Clear();
            UnturnedPlayerEvents.OnPlayerRevive -= OnRevive;
            UnturnedPlayerEvents.OnPlayerDeath -= onDeath;
            UnturnedPlayerEvents.OnPlayerUpdateStat += onUpdateStat;
            if (Instance.Configuration.Instance.Fallprotect)
            {     
                StopAllCoroutines();
            }
            U.Events.OnPlayerConnected -= OnPlayerConnected;
            U.Events.OnPlayerDisconnected -= OnPlayerDisconnected;
            if (Instance.Configuration.Instance.Bouncer)
            {
                
                UnturnedPermissions.OnJoinRequested -= OnJoinRequested;
            }
            #endregion
            
            Logger.LogWarning("[TourneyCore] Unloaded! \n by Dante#2406 for p9nda");
        }
        #endregion

        #region Check player data
        /* utilize WebHandler class */
        public void OnJoinRequested(CSteamID player, ref ESteamRejection? rejectionReason) 
        {
            try
            {
                if (FilterData.FilterData.Instance.Configuration.Instance.Whitelists.Any())
                {
                    Logger.LogWarning("[TourneyCore] check whitelist");
                    if (FilterData.FilterData.Instance.Configuration.Instance.Whitelists.Contains(player))
                    {
                        //if (Configuration.Instance.Logging) { Logger.LogWarning("Access granted: " + player + " // Reason: Whitelist."); }
                        return;
                    }
                }
                Logger.LogWarning("Check data 1");
                WebHandler.CheckData(player, ref rejectionReason);

                
            }
            catch (Exception)
            {

            }
        }
        #endregion

        #region Skills
        /* give skills */
        public void GiveSkills(UnturnedPlayer p)
        {

            p.SetSkillLevel(UnturnedSkill.Cardio, Configuration.Instance.Cardio);
            p.SetSkillLevel(UnturnedSkill.Exercise, Configuration.Instance.Exercise);
        }
        private void OnRevive(UnturnedPlayer p, Vector3 pos, byte a)
        {
            if (FilterData.FilterData.Instance.Configuration.Instance.Casters.Any())
            {


                foreach (var id in FilterData.FilterData.Instance.Configuration.Instance.Casters)
                {
                    UnturnedPlayer playa = UnturnedPlayer.FromCSteamID(id);
                    if (!playa.Features.GodMode)
                    {
                        playa.Features.GodMode = true;
                    }
                    CommandWindow.input.onInputText("teleport " + id + " wp");


                }


            }
            GiveSkills(p);
        }
        #endregion

        #region on death
        public void onDeath(UnturnedPlayer player, EDeathCause cause, ELimb limb, CSteamID murderer)
        {
            if (Instance.Configuration.Instance.Points)
            {   
                ArenaPlayer aplayer = new ArenaPlayer(player.SteamPlayer());
                if (LevelManager.arenaPlayers.Contains(aplayer))
                {


                    if (LevelManager.arenaPlayers.Count == 5)
                    {
                        //fifth place
                        UnturnedChat.Say("Tourney>> "+ player + "[5th] +" + Instance.Configuration.Instance.fifthPoint.ToString(), Color.cyan);
                        UpdatePoints(player, Instance.Configuration.Instance.fifthPoint);
                      
                    }
                    else if (LevelManager.arenaPlayers.Count == 4)
                    {
                        UnturnedChat.Say("Tourney>> " + player + "[4th] +" + Instance.Configuration.Instance.fourthpoint.ToString(), Color.cyan);
                        UpdatePoints(player, Instance.Configuration.Instance.fourthpoint);
                    
                    }
                    else if (LevelManager.arenaPlayers.Count == 3)
                    {
                        UnturnedChat.Say("Tourney>> " + player + "[3rd] +" + Instance.Configuration.Instance.thirdpoint.ToString(), Color.cyan);
                        UpdatePoints(player, Instance.Configuration.Instance.thirdpoint);
           
                    }
                    else if (LevelManager.arenaPlayers.Count == 2)
                    {
                        UnturnedChat.Say("Tourney>> " + player + "[2nd] +" + Instance.Configuration.Instance.secondpoint.ToString(), Color.cyan);
                        UpdatePoints(player, Instance.Configuration.Instance.secondpoint);
                        
                    }
                }
            }
        }
        #endregion

        #region on update stat
        private void onUpdateStat(UnturnedPlayer player, EPlayerStat stat)
        {
            if(stat == EPlayerStat.ARENA_WINS)
            {
                UpdatePoints(player, Instance.Configuration.Instance.firstpoint);
                
                UnturnedChat.Say("Tourney>>\n" + player + " Won 1st place and got \n" + Instance.Configuration.Instance.firstpoint + " Point(s)\nTourney>>", Color.cyan);

            }
            if(stat == EPlayerStat.KILLS_PLAYERS)
            {

                if (Instance.Configuration.Instance.GiveKillPoint)
                {
                    UpdatePoints(player, Instance.Configuration.Instance.killpoint);
                }

            }


        }
        #endregion

        #region Check arena state and fall protect and more
        /*check if arena spawn or others */
        public IEnumerator getArenaState()
        {
            while (true)
            {

                if (Provider.isServer)
                {
                    switch (LevelManager.arenaState)
                    {
                        case EArenaState.WARMUP:
                            {  /* using experimental on hurt */

                                if (Instance.Configuration.Instance.Fallprotect)
                                {
                                    Warmup = true;
                                    if (GG != null)
                                        try { StopCoroutine(GG); } catch { }
                                    GG = StartCoroutine(gamer());

                                    /* using experimental on hurt */




                                }
                            }
                            break;
                        case EArenaState.INTERMISSION:
                            {
                                buffer = 5;
                                Warmup = false;

                            }
                            break;
                      /*  case EArenaState.RESTART:
                            {
                                for (int i = 0; i < LevelManager.arenaPlayers.Count; i++)
                                {
                                    ArenaPlayer item = LevelManager.arenaPlayers[i];
                                    UnturnedPlayer item2 = UnturnedPlayer.FromSteamPlayer(item.steamPlayer);
                                    if (!item.hasDied && item.steamPlayer != null && !(item.steamPlayer.player == null))
                                    {
                                        Instance.PointDB.AddPoints(item2.CSteamID.ToString(), Instance.Configuration.Instance.firstpoint);
                                        UnturnedChat.Say("[Tourney]\n"+item+" Got 1st place and won \n"+ Instance.Configuration.Instance.firstpoint+" point(s)!\n[Tourney]", Color.blue);
                                    }
                                }
                            }
                            break; */
                        case EArenaState.SPAWN:
                            {
                                foreach (var id in FilterData.FilterData.Instance.Configuration.Instance.Casters)
                                {
                                    CSteamID L = id;
                                    UnturnedPlayer unPlayer = UnturnedPlayer.FromCSteamID(id);
                                    unPlayer.Features.GodMode = false;
                                    unPlayer.Damage(101, Vector3.up * 101f, EDeathCause.KILL, ELimb.SKULL, L);

                                }


                            }
                            break;
                    }
                }
               

            }


        }
        public IEnumerator gamer()
        {
            Warmup = true;
            UnturnedChat.Say("Tourney>> Fall Disabled for [" + Instance.Configuration.Instance.NoFallDamageTime.ToString() + "s]", Color.cyan);
            yield return new WaitForSeconds(Instance.Configuration.Instance.NoFallDamageTime);


            Warmup = false;
            yield return new WaitForEndOfFrame();
        }
        #endregion

        #region DEPRECATED
        /* private void playerDamaged(Player player, ref EDeathCause cause, ref ELimb limb, ref CSteamID killer, ref Vector3 direction, ref float damage, ref float times, ref bool canDamage)
         {
             if (cause == EDeathCause.BONES)
             {

             }
             Logger.LogWarning("try candamage false");
             canDamage = false;
             /*if (cause == EDeathCause.BONES)
                 {

                     //if (noFallDamage)
                    // {
                         canDamage = false;
                    // }
                    // else
                    // {
                         //canDamage = true;
                    // }

                 }
                 return;

             Logger.LogWarning("did it");




         }*/
        #endregion

        #region Filter on join
        private void OnPlayerConnected(UnturnedPlayer player)
        {
            if (FilterData.FilterData.Instance.Configuration.Instance.Casters.Any())
            {
                if (FilterData.FilterData.Instance.Configuration.Instance.Casters.Contains(player.CSteamID))
                {
                    UnturnedChat.Say(player.CSteamID, "[TourneyCore] Caster Mode On!", UnityEngine.Color.magenta);
                    player.Features.GodMode = true;
                    player.Features.VanishMode = true;
                    
                    
                }

            }
            GiveSkills(player);
            if (Instance.Configuration.Instance.Points)
            {
                Instance.PointDB.AddUpdatePlayer(player.CSteamID.ToString(), player.DisplayName);
                
                string[] rankInfo = Instance.PointDB.GetAccountBySteamID(player.CSteamID.ToString());
                dicPoints.Add(player.CSteamID, Convert.ToInt32(rankInfo[0]));

            }
            if (Instance.Configuration.Instance.LogIDonjoin)
            {

                if (FilterData.FilterData.Instance.Configuration.Instance.Whitelists.Contains(player.CSteamID))
                {
                    return;
                }
                else
                {
                    CSteamID judge = (CSteamID)0;
                    if (FilterData.FilterData.Instance.Configuration.Instance.Unwhitelisted.Contains(player.CSteamID))
                    {
                        return;
                    }
                    else
                    {


                        FilterData.FilterData.Instance.Configuration.Instance.Unwhitelisted.Add(player.CSteamID);
                        FilterData.FilterData.Instance.Configuration.Save();
                    }
                }



            }
        }
        #endregion

        #region update points
        public void UpdatePoints(UnturnedPlayer player, int points)
        {
            int oldPoints;
            bool playerExists = dicPoints.TryGetValue(player.CSteamID, out oldPoints);

            if (playerExists)
            {
                Instance.PointDB.AddPoints(player.CSteamID.ToString(), points);
                dicPoints[player.CSteamID] += points;



            }
        }
                #endregion

        #region experimental on hurt
                /* EXPERIMENTAL ON HURT EVENT*/

                /*  public void onHurt(Player player, byte damage, Vector3 force, EDeathCause cause, ELimb limb, CSteamID killer)
                  {
                      if (Instance.Configuration.Instance.Fallprotect)
                      {

                          Logger.LogWarning("Fallprotect");
                          if (Warmup)
                          {


                              UnturnedPlayer uplayer = UnturnedPlayer.FromPlayer(player);

                              if (FilterData.FilterData.Instance.Configuration.Instance.Casters.Contains(uplayer.CSteamID))
                              {
                                  if (cause == EDeathCause.BONES)
                                  {
                                      uplayer.Heal(100, false ,false);
                                      player.life.tellBroken(uplayer.CSteamID, false);
                                      uplayer.Bleeding = false;
                                      uplayer.Bleeding = false;
                                      uplayer.Broken = false;
                                      return;
                                  }
                                  return;
                              }
                              else
                              {
                                  if (cause == EDeathCause.BONES)
                                  {


                                      player.life.askHeal(100, true, true);

                                      uplayer.Bleeding = false;
                                      uplayer.Bleeding = false;
                                      uplayer.Broken = false;
                                      player.life.tellBroken(uplayer.CSteamID, false);
                                      uplayer.Infection = 0;
                                      uplayer.Hunger = 0;
                                      uplayer.Thirst = 0; 

                                      Logger.LogWarning("gg32");

                                  }

                              }
                          }
                      }
                      else
                      {
                          return;
                      }

                  }*/
                #endregion

        #region working fallprot
        public void FixedUpdate()
        {
            if (Warmup)
            {
                List<CSteamID> ids = Provider.clients.Select(x => x.playerID.steamID).ToList();
               
                foreach (var id in ids)
                {
                    UnturnedPlayer playa = UnturnedPlayer.FromCSteamID(id);

                    if (playa.Broken)
                    {
                        playa.Heal(100);
                        playa.Broken = false;

                    }
                        playa.Heal(100);
                        playa.Bleeding = false;
                        playa.Broken = false;
                    
                }

            }
        }
        #endregion

        #region Filter on disconnect
        private void OnPlayerDisconnected(UnturnedPlayer player)
        {
           
                int playerPoints;
                bool playerExists = dicPoints.TryGetValue(player.CSteamID, out playerPoints);
                if (playerExists)
                {
                    dicPoints.Remove(player.CSteamID);
                }
            
            if (Instance.Configuration.Instance.LogIDonjoin)
            {
                CSteamID judge = (CSteamID)0;
                if (!FilterData.FilterData.Instance.Configuration.Instance.Unwhitelisted.Contains(player.CSteamID))
                {
                    return;
                }
                if (FilterData.FilterData.Instance.Configuration.Instance.Unwhitelisted.Contains(player.CSteamID))
                {

                    if (Instance.Database.IsBanned(player.CSteamID.ToString()))
                    {
                        return;
                    }
                    Instance.Database.BanPlayer(player.CharacterName, player.CSteamID.ToString(), "[TourneyCore]", "[TourneyCore Filter]", 2147483647);
                    SteamBlacklist.ban(player.CSteamID, GetIP(player.CSteamID), judge, Translate("Filter_Reason"), SteamBlacklist.PERMANENT);

                }

            }
            else
            {
                return;
            }

        }
        #endregion

        #region Translations
        public override TranslationList DefaultTranslations => new TranslationList()
        {
                {"Misc_kickall","[TourneyCore] Kicked by an Administrator [TourneyCore]" },
                {"Filter_Reason","[TourneyCore] Thanks for Participating in the Qualifier [TourneyCore]" },
                {"Filter_Whitelisting"," Added to whitelist! [TourneyCore]" },
                {"Filter_WhitelistedRemoved"," Removed from whitelist! [TourneyCore]" },
                {"Filter_player_not_found","Player not found! [TourneyCore]" },
            //points
                {"general_not_found","Player not found."},
                {"general_invalid_parameter","Invalid parameter."},
                {"rank_self","[Tourney] Your current rank: {1} with {0} points [{2}]"},
                {"rank_other","[Tourney] {3}'s current rank: {1} with {0} points [{2}]"},
                {"list_1","[Tourney] Top 3:"},
                {"list_2","[Tourney] {1}st: [{2}] {3} ({0} points)"},
                {"list_3","[Tourney] {1}nd: [{2}] {3} ({0} points)"},
                {"list_4","[Tourney] {1}rd: [{2}] {3} ({0} points)"},
                {"list_search","Rank {1}: [{2}] {3} ({0} points)"},
                {"list_search_not_found","Rank not found."},
                {"points_reset_player","Your points have been reset."},
                {"points_reset_caller","{0}'s points have been reset."},
                {"points_set_player","Your points have been set to {0}."},
                {"points_set_caller","{1}'s points have been set to {0}."},
                {"points_add_player","You received {0} points."},
                {"points_add_caller","You sent {0} points to {1}."},
                {"points_remove_player","You lost {0} points."},
                {"points_remove_caller","You removed {0} points from {1}."},
        };
        #endregion

        #region get ip method
        public uint GetIP(CSteamID ID)
        {
            P2PSessionState_t p2PSessionStateT;
            uint ip;
            if (!SteamGameServerNetworking.GetP2PSessionState(ID, out p2PSessionStateT))
            {
                ip = 0;
            }
            else
            {
                ip = p2PSessionStateT.m_nRemoteIP;
            }
            return ip;
        }
        #endregion

        #region rejections
        public ESteamRejection GetSteamRejection(int i)
        {
            var RejectionReason = ESteamRejection.AUTH_VERIFICATION;
            if (i > 0)
            {
                try
                {


                    RejectionReason = ESteamRejection.AUTH_TIMED_OUT;
                }
                catch { }
            }
            else
            {
                try
                {
                    RejectionReason = ESteamRejection.AUTH_NO_STEAM;
                }
                catch { }
            }
            



            return RejectionReason;
        }
        #endregion
    }
}

