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

            Instance.Configuration.Save();
        }

        protected override void Unload()
        {
            U.Events.OnPlayerConnected -= Events_OnPlayerConnected;
        }

        public override TranslationList DefaultTranslations
        {
            get
            {
                return new TranslationList
                {
                    { "connect_message", "{0} connected to the server." },
                    { "disconnect_message", "{0} disconnected from the server." },

                    { "connect_message_country", "{0} has connected from {1}." }
                };
            }
        }

        internal Color ParseColor(string color)
        {
            if (color == null)
                return Color.green;
            switch (color.Trim().ToLower())
            {
                case "black":
                    return Color.black;
                case "blue":
                    return Color.blue;
                case "cyan":
                    return Color.cyan;
                case "grey":
                    return Color.grey;
                case "green":
                    return Color.green;
                case "gray":
                    return Color.gray;
                case "magenta":
                    return Color.magenta;
                case "red":
                    return Color.red;
                case "white":
                    return Color.white;
                case "yellow":
                    return Color.yellow;
                case "gold":
                    return new Color(1.0f, 0.843137255f, 0f);
                default:
                    float r;
                    float g;
                    float b;
                    string[] colors = color.Split(',');
                    return (colors.Length == 3 && float.TryParse(colors[0], out r) && float.TryParse(colors[1], out g) && float.TryParse(colors[2], out b) && r >= 0 && r <= 255 && g >= 0 && g <= 255 && b >= 0 && b <= 255) ? new Color(r / 255, g / 255, b / 255) : Color.green;
            }
        }

        private void Events_OnPlayerConnected(UnturnedPlayer player)
        {

            List<RocketPermissionsGroup> permissionGroups = R.Permissions.GetGroups(player, true);
            Logger.Log("PermissionGroups of player " + player.CSteamID + " = " + System.String.Join<RocketPermissionsGroup>(", ", permissionGroups.ToArray()));
            if (permissionGroups.Count == 1)
            {
                if (!times.ContainsKey(player.CSteamID))
                {
                    times.Add(player.CSteamID, System.DateTime.Now);
                } else
                {
                    times[player.CSteamID] = System.DateTime.Now;
                }
            }
        }


        private void Update()
        {
            foreach (KeyValuePair<CSteamID, System.DateTime> timeEntry in times)
            {
                UnturnedPlayer player = UnturnedPlayer.FromCSteamID(timeEntry.Key);
                double timePlayed = (System.DateTime.Now - timeEntry.Value).TotalSeconds;
                if (timePlayed > Instance.Configuration.Instance.intervalUntilPromotion)
                {
                    R.Permissions.AddPlayerToGroup("member", player);
                    UnturnedChat.Say(timeEntry.Key, "Congratulations! You were automatically added to group member!");
                    UnturnedChat.Say(timeEntry.Key, "Type /p to see your current Permissions :)");
                    times.Remove(timeEntry.Key);
                }
            }
        }

    }

}
