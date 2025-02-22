using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Features.Actors.Components;
using OpenRP.Framework.Shared.Extensions;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.ActorConversations.Helpers
{
    public class ActorCommandProcessor
    {
        private List<string> _inputCommands;
        private List<ActorProcessedCommand> _outputCommands;
        private List<ServerActor> _actorsParticipating;
        private ServerActor _currentActorSpeaking;
        private List<ActorConversationHistory> _conversationHistory;
        public ActorCommandProcessor(List<string> inputCommands, List<ServerActor> actorsParticipating, List<ActorConversationHistory> conversationHistory)
        {
            _inputCommands = inputCommands;
            _outputCommands = new List<ActorProcessedCommand>();
            _actorsParticipating = actorsParticipating;
            _conversationHistory = conversationHistory;
        }

        public List<ActorProcessedCommand> Process(string inputCommand = null)
        {
            foreach (string command in _inputCommands)
            {
                if (_currentActorSpeaking == null && !command.StartsWith("/switchactor"))
                {
                    if (_conversationHistory.Count > 0)
                    {
                        ActorConversationHistory lastActor = _conversationHistory.OrderByDescending(i => i.SequenceNumber).First();
                        _currentActorSpeaking = lastActor.FromActor;
                    }
                    else
                    {
                        continue;
                    }
                }

                if (ValidateCommandFormat(command, out string commandType, out string content))
                {
                    if (commandType == "/switchactor")
                    {
                        string trimmedContent = content.Split("#")[0].Trim(' ', '[', ']');
                        if (ulong.TryParse(trimmedContent, out ulong value))
                        {
                            _currentActorSpeaking = _actorsParticipating.SingleOrDefault(i => i.GetId() == value);
                        }
                        ActorProcessedCommand processedCommand = new ActorProcessedCommand()
                        {
                            SendChatMessage = false,
                            Actor = _currentActorSpeaking,
                            OriginalCommand = command,
                        };
                        _outputCommands.Add(processedCommand);
                    }
                    else if (commandType == "/say")
                    {
                        ActorProcessedCommand processedCommand = new ActorProcessedCommand()
                        {
                            ChatColor = new Color(245, 245, 245),
                            SendChatMessage = true,
                            Message = $"{_currentActorSpeaking.GetName()} says: {content.Trim('"')}",
                            Actor = _currentActorSpeaking,
                            OriginalCommand = command,
                            TypingTime = CalculateTypingTime(command),
                        };
                        _outputCommands.Add(processedCommand);
                    }
                    else if (commandType == "/me")
                    {
                        ActorProcessedCommand processedCommand = new ActorProcessedCommand()
                        {
                            ChatColor = new Color(246, 165, 250),
                            SendChatMessage = true,
                            Message = $"** {_currentActorSpeaking.GetName()} {content.Trim('"').LowercaseFirstLetter()} **",
                            Actor = _currentActorSpeaking,
                            OriginalCommand = command,
                            TypingTime = CalculateTypingTime(command),
                        };
                        _outputCommands.Add(processedCommand);
                    }
                    else if (commandType == "/do")
                    {
                        ActorProcessedCommand processedCommand = new ActorProcessedCommand()
                        {
                            ChatColor = new Color(246, 165, 250),
                            SendChatMessage = true,
                            Message = $"{content.Trim('"')} (( {_currentActorSpeaking.GetName()} ))",
                            Actor = _currentActorSpeaking,
                            OriginalCommand = command,
                            TypingTime = CalculateTypingTime(command),
                        };
                        _outputCommands.Add(processedCommand);
                    }
                }
            }
            return _outputCommands;
        }

        private bool ValidateCommandFormat(string command, out string commandType, out string content)
        {
            commandType = string.Empty;
            content = string.Empty;

            if (command.StartsWith("/say "))
            {
                commandType = "/say";
                content = command.Substring(5).Trim();
                return !string.IsNullOrEmpty(content);
            }
            else if (command.StartsWith("/me "))
            {
                commandType = "/me";
                content = command.Substring(4).Trim();
                return !string.IsNullOrEmpty(content);
            }
            else if (command.StartsWith("/do "))
            {
                commandType = "/do";
                content = command.Substring(4).Trim();
                return !string.IsNullOrEmpty(content);
            }
            else if (command.StartsWith("/switchactor "))
            {
                commandType = "/switchactor";
                content = command.Substring(13).Trim();
                return !string.IsNullOrEmpty(content);
            }

            return false;
        }

        private int CalculateTypingTime(string text)
        {
            Random random = new Random();
            int typingTimeInMs = random.Next(1300, 3600);
            typingTimeInMs += text.Length * 100;
            return typingTimeInMs;
        }
    }
}
