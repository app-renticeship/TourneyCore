#region Initialize refernces
using Rocket.API;
using System.Xml.Serialization;
using System.Collections.Generic;
#endregion

namespace TourneyCore
{
    public class CoreConfig : IRocketPluginConfiguration
    {
        #region Variables
        public bool LogIDonjoin;

        public string DatabaseAddress;
        public string DatabaseUsername;
        public string DatabasePassword;
        public string DatabaseName;
        public string DatabaseTableName;
        public string PDatabaseAddress;
        public string PDatabaseUsername;
        public string PDatabasePassword;
        public string PDatabaseName;
        public string PDatabaseTableName;
        public bool GiveKillPoint;
        public int killpoint;
        public int KickInterval = 10;
        public int DatabasePort;
        public int PDatabasePort;
        public bool KickInsteadReject = false;
        public bool Fallprotect;
        public bool Points;
        public int firstpoint;
        public int secondpoint;
        public int thirdpoint;
        public int fourthpoint;
        public int fifthPoint;
        public int NoFallDamageTime;
        public bool Bouncer;
        public int BouncerTimeout;
        public byte Cardio;
        public byte Exercise;
        #endregion
        // write config data etc
        #region Load
        public void LoadDefaults()
        {
            LogIDonjoin = true;
            Points = true;
            Bouncer = true;
            Fallprotect = true;

            NoFallDamageTime = 10;
            GiveKillPoint = true;
            firstpoint = 5;
            secondpoint = 4;
            thirdpoint = 3;
            fourthpoint = 2;
            fifthPoint = 1;
            killpoint = 1;
            Cardio = 3;
            Exercise = 3;

            
            PDatabaseAddress = "localhost";
            PDatabaseUsername = "unturned";
            PDatabasePassword = "password";
            PDatabaseName = "unturned";
            PDatabaseTableName = "points";
            PDatabasePort = 3306;
            
            DatabaseAddress = "localhost";
            DatabaseUsername = "unturned";
            DatabasePassword = "password";
            DatabaseName = "unturned";
            KickInterval = 10;
            DatabaseTableName = "banlist";
            DatabasePort = 3306;


        }
        public class classDatabase
        {
            public string PDatabaseAddress;
            public string PDatabaseUsername;
            public string PDatabasePassword;
            public string PDatabaseName;
            public string PDatabaseTableName;
            public int PDatabasePort;
        }
        #endregion
    }
}
