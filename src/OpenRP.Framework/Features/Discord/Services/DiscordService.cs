using SampSharp.Entities.SAMP;
using SampSharp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using Microsoft.Extensions.Options;
using OpenRP.Framework.Features.CDN.Entities;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using OpenRP.Framework.Features.Discord.Entities;

namespace OpenRP.Framework.Features.Discord.Services
{
    public class DiscordService : IDiscordService
    {
        private DiscordClient _discordClient;
        private readonly IEntityManager _entityManager;
        private readonly DiscordOptions _options;
        private readonly Task _initializationTask;

        // Private constructor to enforce the use of the factory method
        public DiscordService(IOptions<DiscordOptions> options, IEntityManager entityManager)
        {
            _initializationTask = InitializeAsync();
            _entityManager = entityManager;
            _options = options.Value;
            ValidateOptions();
        }
        private void ValidateOptions()
        {
            if (string.IsNullOrEmpty(_options.DiscordBotToken))
                throw new ArgumentNullException(nameof(DiscordOptions.DiscordBotToken));
            if (_options.GeneralChatChannelId == default)
                throw new ArgumentException(nameof(DiscordOptions.GeneralChatChannelId));
        }

        // Asynchronous initialization method
        private async Task InitializeAsync()
        {
            DiscordConfiguration discordConfiguration = new DiscordConfiguration()
            {
                Token = _options.DiscordBotToken,
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.AllUnprivileged | DiscordIntents.MessageContents
            };

            _discordClient = new DiscordClient(discordConfiguration);

            // Optionally subscribe to events
            _discordClient.MessageCreated += DiscordClient_MessageCreated;

            await _discordClient.ConnectAsync();
            // If you need to keep the bot running, consider handling it differently
            // For example, integrate with your application's lifecycle
        }

        // Method to send a general chat message
        public async Task SendGeneralChatMessage(string text)
        {
            // Ensure the client is initialized and connected
            await _initializationTask;

            if (_discordClient == null)
            {
                throw new InvalidOperationException("Discord client is not initialized.");
            }

            // Attempt to get the channel
            var channel = await _discordClient.GetChannelAsync(_options.GeneralChatChannelId);
            if (channel is null)
            {
                throw new InvalidOperationException($"Channel with ID {_options.GeneralChatChannelId} not found.");
            }

            // Send the message
            await _discordClient.SendMessageAsync(channel, text.Replace("@", "@​"));
        }

        public async Task SendGlobalOocChatMessage(Player player, string text)
        {
            // Ensure the client is initialized and connected
            await _initializationTask;

            if (_discordClient == null)
            {
                throw new InvalidOperationException("Discord client is not initialized.");
            }

            StringBuilder stringBuilder = new StringBuilder();
            if (!await IsLastMessageFromBotAsync(_options.GeneralChatChannelId))
            {
                stringBuilder.AppendLine("## [In-Game OOC]");
            }

            stringBuilder.AppendLine($"**{player.Name.Replace("_", " ")}:** {text}");

            await SendGeneralChatMessage(stringBuilder.ToString());
        }

        public async Task<bool> IsLastMessageFromBotAsync(ulong channelId)
        {
            // Retrieve the channel
            var channel = await _discordClient.GetChannelAsync(channelId);
            if (channel == null)
            {
                Console.WriteLine($"Channel with ID {channelId} not found.");
                return false;
            }

            // Fetch the last message in the channel
            var messages = await channel.GetMessagesAsync(1); // Fetch the most recent message
            DiscordMessage lastMessage = messages.FirstOrDefault();

            if (lastMessage != null && !lastMessage.Content.EndsWith("is now playing on the server.") && !lastMessage.Content.EndsWith("is no longer playing on the server."))
            {
                return await IsMessageFromBotAsync(lastMessage);
            }

            // If no messages are found
            Console.WriteLine("No messages found in the channel.");
            return false;
        }

        /// <summary>
        /// Determines if the provided message was sent by the bot.
        /// </summary>
        /// <param name="message">The Discord message to check.</param>
        /// <returns>True if the message was sent by the bot; otherwise, false.</returns>
        private async Task<bool> IsMessageFromBotAsync(DiscordMessage message)
        {
            // Get the bot's user ID
            var botUser = _discordClient.CurrentUser;
            if (botUser == null)
            {
                Console.WriteLine("Bot user information is not available.");
                return false;
            }

            // Compare the message author ID with the bot's user ID
            return message.Author.Id == botUser.Id;
        }

        private Task DiscordClient_MessageCreated(DiscordClient sender, MessageCreateEventArgs e)
        {
            // Discord OOC
            if (e.Channel.Id == 477898678561144843)
            {
                if (!e.Author.IsBot)
                {
                    foreach (Player foreachPlayer in _entityManager.GetComponents<Player>())
                    {
                        string nameToUse = e.Author.Username;

                        if (e.Author is DiscordMember member)
                        {
                            nameToUse = member.DisplayName;
                        }

                        string CHAT_ACTION_OOC = String.Format("(( Discord OOC | {0}: {1} ))", nameToUse, e.Message.Content);

                        foreachPlayer.SendClientMessage(Color.FromString("7289DA", ColorFormat.RGB), CHAT_ACTION_OOC);
                    }
                }
            }
            return System.Threading.Tasks.Task.CompletedTask;
        }
    }

}
