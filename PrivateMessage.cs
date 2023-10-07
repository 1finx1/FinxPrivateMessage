using Rocket.API.Collections;
using Rocket.Core.Plugins;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using UnityEngine;
using Rocket.API;
using Rocket.Unturned.Chat;
using Steamworks;
using static Rocket.Unturned.Events.UnturnedEvents;
using Rocket.Unturned;

namespace PrivateMessage
{
    public class Plugin : RocketPlugin<WhisperConfig>
    {
        public static Plugin Instance;
        public Dictionary<CSteamID, CSteamID> LastMessageFromPlayer;

        public override TranslationList DefaultTranslations
        {
            get
            {
                return new TranslationList()
                {
                    { "whisper_help", "Send a private message (whisper) to a player." },
                    { "whisper_reply_help", "Reply a message to the last player who sent you a private message." },
                    { "whisper_syntax_error", "Syntax: /whisper <player> <message>" },
                    { "whisper_reply_syntax_error", "Syntax: /reply <message>" },
                    { "whisper_received", "[{0}] whispers: {1}" },
                    { "whisper_sent", "To [{0}]: {1}" },
                    { "whisper_player_not_found", "Couldn't find a player named \"{0}\"" },
                    { "whisper_reply_no_last_player", "No one has sent you a private message, you can't reply to no one." },
                    { "whisper_to_self", "You can't send a private message to yourself." }
                };
            }
        }

        protected override void Load()
        {
            Instance = this;
            LastMessageFromPlayer = new Dictionary<CSteamID, CSteamID>();
            
        }

        protected override void Unload()
        {
            
        }

       

        public void SetPlayerFromLastMessage(UnturnedPlayer fromPlayer, UnturnedPlayer toPlayer)
        {
            if (LastMessageFromPlayer.ContainsKey(toPlayer.CSteamID))
            {
                if (fromPlayer == null)
                {
                    LastMessageFromPlayer.Remove(toPlayer.CSteamID);
                    return;
                }

                LastMessageFromPlayer.Remove(toPlayer.CSteamID);
                LastMessageFromPlayer.Add(toPlayer.CSteamID, fromPlayer.CSteamID);
            }
            else
            {
                if (fromPlayer == null)
                {
                    return;
                }

                LastMessageFromPlayer.Add(toPlayer.CSteamID, fromPlayer.CSteamID);
            }
        }

        public UnturnedPlayer GetPlayerFromLastMessage(UnturnedPlayer player)
        {
            if (LastMessageFromPlayer.TryGetValue(player.CSteamID, out CSteamID lastMessageFromPlayer))
            {
                return UnturnedPlayer.FromCSteamID(lastMessageFromPlayer);
            }

            return null;
        }

        public void WhisperPlayer(UnturnedPlayer fromPlayer, UnturnedPlayer toPlayer, string message)
        {
            string fromPlayerName = fromPlayer == null ? "Server" : fromPlayer.DisplayName;

            if (fromPlayer == null)
            {
                fromPlayerName = "Server";
            }
            else
            {
                fromPlayerName = fromPlayer.DisplayName;
            }

            SetPlayerFromLastMessage(toPlayer, fromPlayer);

            if (fromPlayer != null)
                SetPlayerFromLastMessage(fromPlayer, toPlayer);

            UnturnedChat.Say(toPlayer, string.Format(Translate("whisper_received"), fromPlayerName, message), UnturnedChat.GetColorFromName(Configuration.Instance.Color, Color.magenta));

            if (fromPlayer != null)
                UnturnedChat.Say(fromPlayer, string.Format(Translate("whisper_sent"), toPlayer.DisplayName, message), UnturnedChat.GetColorFromName(Configuration.Instance.Color, Color.magenta));
        }
    }
}
