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
using Microsoft.Extensions.Logging;
using OpenRP.Framework.Features.Discord.Helpers;

namespace OpenRP.Framework.Features.Discord.Services
{
    public class DiscordService : IDiscordService
    {
        private DiscordClient _discordClient;
        private readonly IEntityManager _entityManager;
        private readonly DiscordOptions _options;
        private readonly Task? _initializationTask;
        private readonly IServerService _serverService;
        private readonly IDiscordChannelNameQueue _channelNameQueue;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<DiscordService> _logger;

        public DiscordService(IOptions<DiscordOptions> options, IEntityManager entityManager, IServerService serverService, IDiscordChannelNameQueue channelNameQueue, ILoggerFactory loggerFactory, ILogger<DiscordService> logger)
        {
            _entityManager = entityManager;
            _serverService = serverService;
            _channelNameQueue = channelNameQueue;
            _loggerFactory = loggerFactory;
            _logger = logger;
            _options = options.Value;
            if (!String.IsNullOrEmpty(_options.DiscordBotToken))
            {
                _initializationTask = InitializeAsync();
            }
        }

        // Asynchronous initialization method
        private async Task InitializeAsync()
        {
            DiscordConfiguration discordConfiguration = new DiscordConfiguration()
            {
                Token = _options.DiscordBotToken,
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.AllUnprivileged | DiscordIntents.MessageContents,
                // Without this, DSharpPlus only writes to the console, so rate limit warnings never reach the logs.
                LoggerFactory = _loggerFactory
            };

            _discordClient = new DiscordClient(discordConfiguration);

            // Optionally subscribe to events
            _discordClient.MessageCreated += DiscordClient_MessageCreated;

            await _discordClient.ConnectAsync();
        }

        // Method to send a general chat message
        public async Task SendGeneralChatMessage(string text)
        {
            if (!String.IsNullOrEmpty(_options.DiscordBotToken) && _initializationTask != null)
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
        }

        // Method to send a newbie chat message
        public async Task SendSupportChatMessage(string text)
        {
            if (!String.IsNullOrEmpty(_options.DiscordBotToken) && _initializationTask != null)
            {
                // Ensure the client is initialized and connected
                await _initializationTask;

                if (_discordClient == null)
                {
                    throw new InvalidOperationException("Discord client is not initialized.");
                }

                // Attempt to get the channel
                var channel = await _discordClient.GetChannelAsync(_options.SupportChannelId);
                if (channel is null)
                {
                    throw new InvalidOperationException($"Channel with ID {_options.SupportChannelId} not found.");
                }

                // Send the message
                await _discordClient.SendMessageAsync(channel, text.Replace("@", "@​"));
            }
        }

        public async Task SendGlobalOocChatMessage(Player player, string text)
        {
            if (!String.IsNullOrEmpty(_options.DiscordBotToken) && _initializationTask != null)
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
        }

        public async Task SendNewbieChatMessage(Player player, string text)
        {
            if (!String.IsNullOrEmpty(_options.DiscordBotToken) && _initializationTask != null)
            {
                // Ensure the client is initialized and connected
                await _initializationTask;

                if (_discordClient == null)
                {
                    throw new InvalidOperationException("Discord client is not initialized.");
                }

                StringBuilder stringBuilder = new StringBuilder();
                if (!await IsLastMessageFromBotAsync(_options.SupportChannelId))
                {
                    stringBuilder.AppendLine("## [In-Game Newbie Chat]");
                }

                stringBuilder.AppendLine($"**{player.Name.Replace("_", " ")}:** {text}");

                await SendSupportChatMessage(stringBuilder.ToString());
            }
        }

        public async Task<bool> IsLastMessageFromBotAsync(ulong channelId)
        {
            if (!String.IsNullOrEmpty(_options.DiscordBotToken) && _initializationTask != null)
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
                    return IsMessageFromBotAsync(lastMessage);
                }

                // If no messages are found
                Console.WriteLine("No messages found in the channel.");
            }
            return false;
        }

        /// <summary>
        /// Determines if the provided message was sent by the bot.
        /// </summary>
        /// <param name="message">The Discord message to check.</param>
        /// <returns>True if the message was sent by the bot; otherwise, false.</returns>
        private bool IsMessageFromBotAsync(DiscordMessage message)
        {

            if (!String.IsNullOrEmpty(_options.DiscordBotToken) && _initializationTask != null)
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
            return false;
        }

        private Task DiscordClient_MessageCreated(DiscordClient sender, MessageCreateEventArgs e)
        {
            // Discord OOC
            if (e.Channel.Id == _options.GeneralChatChannelId)
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

            if(e.Channel.Id == _options.SupportChannelId)
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

                        string CHAT_ACTION_NEWBIE = String.Format("(( Newbie | {0} [Discord]: {1} ))", nameToUse, e.Message.Content);

                        foreachPlayer.SendClientMessage(Color.LightGreen, CHAT_ACTION_NEWBIE);
                    }
                }
            }
            return System.Threading.Tasks.Task.CompletedTask;
        }

        public void UpdatePlayerCount(bool decrease = false)
        {
            if (!IsConfigured() || _options.PlayerCountChannelId == default)
            {
                return;
            }

            int playerCount = _entityManager.GetComponents<Player>().Count();
            if (decrease)
            {
                playerCount--;
            }
            _channelNameQueue.SetChannelName(_options.PlayerCountChannelId, DiscordChannelNames.GetPlayerCountName(playerCount, _serverService.MaxPlayers), DateTime.UtcNow);
        }

        public void UpdateDateTime(DateTime date, TimeSpan time)
        {
            if (!IsConfigured() || _options.ServerDateTimeChannelId == default)
            {
                return;
            }
            _channelNameQueue.SetChannelName(_options.ServerDateTimeChannelId, DiscordChannelNames.GetServerDateTimeName(date, time), DateTime.UtcNow);
        }

        public async Task SendNextChannelRenameAsync()
        {
            if (!IsConfigured())
            {
                return;
            }

            DiscordChannelRename? rename = _channelNameQueue.TakeNextRename(DateTime.UtcNow);
            if (rename == null)
            {
                return;
            }

            try
            {
                bool renamed = await RenameChannelAsync(rename.ChannelId, rename.Name);
                _channelNameQueue.MarkRenamed(rename, renamed ? DateTime.UtcNow : null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to rename Discord channel {ChannelId} to {Name}; it stays in the queue.", rename.ChannelId, rename.Name);
                _channelNameQueue.MarkRenameFailed(rename);
            }
        }

        private bool IsConfigured()
        {
            return !String.IsNullOrEmpty(_options.DiscordBotToken) && _initializationTask != null;
        }

        /// <summary>
        /// Renames the channel, and returns whether Discord had to be asked (false when it already has the name).
        /// </summary>
        private async Task<bool> RenameChannelAsync(ulong channelId, string name)
        {
            await _initializationTask!;

            DiscordChannel channel = await _discordClient.GetChannelAsync(channelId);
            if (channel == null)
            {
                throw new InvalidOperationException($"Channel with ID {channelId} not found.");
            }
            if (channel.Name == name)
            {
                return false;
            }

            await channel.ModifyAsync(model => model.Name = name);
            return true;
        }
    }
}
