using Rocket.API.Collections;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using Rocket.Core;
using Rocket.Unturned;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System.Net;
using Newtonsoft.Json;
using System.Linq;
using Rocket.API.Serialisation;
using System.Collections.Generic;
using Color = UnityEngine.Color;

namespace automatedPermissions
{
    public class automatedPermissions : RocketPlugin<automatedPermissionsConfig>
    {
        internal automatedPermissions Instance;
        private Color JoinMessageColor;
        private Color LeaveMessageColor;
        public static Dictionary<CSteamID, System.DateTime> times = new Dictionary<CSteamID, System.DateTime>();

        public struct TimesEntry
        {
            public TimesEntry(CSteamID id, System.DateTime time)
            {
                ID = id;
                time_of_join = time;
            }

            public CSteamID ID { get; }
            public System.DateTime time_of_join { get; }
        }

        protected override void Load()
        {
            Instance = this;

            U.Events.OnPlayerConnected += Events_OnPlayerConnected;
            U.Events.OnPlayerDisconnected += Events_OnPlayerDisconnect;

            Instance.Configuration.Save();
        }

        protected override void Unload()
        {
            U.Events.OnPlayerConnected -= Events_OnPlayerConnected;
            U.Events.OnPlayerDisconnected -= Events_OnPlayerDisconnect;
        }

        public override TranslationList DefaultTranslations
        {
            get
            {
                return new TranslationList
                {
                    { "welcome_message", "Welcome {0}! If you keep playing, you will be promoted in {1} Minutes :)" }
                };
            }
        }

        private void Events_OnPlayerConnected(UnturnedPlayer player)
        {

            List<RocketPermissionsGroup> permissionGroups = R.Permissions.GetGroups(player, true);
            Logger.Log("PermissionGroups of player " + player.CSteamID + " = " + System.String.Join<string>(", ", permissionGroups.ToArray().Select(perm => perm.DisplayName)));
            if (permissionGroups.Count == 1)
            {
                string time = string.Format("{0}:{1}", System.Math.Floor((decimal)Instance.Configuration.Instance.intervalUntilPromotionSeconds / 60), (decimal)Instance.Configuration.Instance.intervalUntilPromotionSeconds % 60);
                UnturnedChat.Say(player, Translate("welcome_message", player.CharacterName, time));

                if (!times.ContainsKey(player.CSteamID))
                {
                    times.Add(player.CSteamID, System.DateTime.Now);
                }
                else
                {
                    times[player.CSteamID] = System.DateTime.Now;
                }
            }
        }

        private void Events_OnPlayerDisconnect(UnturnedPlayer player)
        {
            times.Remove(player.CSteamID);
        }


        private void Update()
        {
            foreach (KeyValuePair<CSteamID, System.DateTime> timeEntry in times)
            {
                UnturnedPlayer player = UnturnedPlayer.FromCSteamID(timeEntry.Key);
                double timePlayed = (System.DateTime.Now - timeEntry.Value).TotalSeconds;
                if (timePlayed > Instance.Configuration.Instance.intervalUntilPromotionSeconds)
                {
                    R.Permissions.AddPlayerToGroup("member", player);
                    Logger.Log(string.Format("Player {0} was automatically added to group ${1}!", player.CSteamID, "member"));
                    // foreach (Message m in Instance.Configuration.Instance.Messages)
                    //{
                    //    UnturnedChat.Say(timeEntry.Key, m.Text, UnturnedChat.GetColorFromName(m.Color, Color.green));
                    // }

                    UnturnedChat.Say(timeEntry.Key, "Congratulations! You were automatically added to group member!");
                    UnturnedChat.Say(timeEntry.Key, "Type /p to see your current Permissions :)");
                    times.Remove(timeEntry.Key);
                }
            }
        }

    }

}
