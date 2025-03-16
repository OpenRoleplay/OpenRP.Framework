using OpenRP.Framework.Shared.ServerEvents.Attributes;
using OpenRP.Framework.Shared.ServerEvents.Entities.EventArgs;
using OpenRP.Framework.Shared.ServerEvents.Entities;
using OpenRP.Framework.Shared.ServerEvents.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CacheManager.Core.Logging;
using Microsoft.Extensions.Logging;

namespace OpenRP.Framework.Shared.ServerEvents.Services
{
    public class ServerEventDispatcher : IServerEventDispatcher
    {
        private readonly IServerEventAggregator _serverEventAggregator;
        private readonly Dictionary<string, List<MethodInfo>> _eventHandlers = new Dictionary<string, List<MethodInfo>>();
        private readonly List<object> _systemInstances = new List<object>();
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ServerEventDispatcher> _logger;

        public ServerEventDispatcher(IServiceProvider serviceProvider, IServerEventAggregator serverEventAggregator, ILogger<ServerEventDispatcher> logger)
        {
            _serviceProvider = serviceProvider;
            _serverEventAggregator = serverEventAggregator;
            _logger = logger;

            RegisterEventHandlers();
            SubscribeToEvents();

            _logger.LogInformation("Loaded ServerEventDispatcher");
        }

        private void SubscribeToEvents()
        {
            // Subscribe to specific events via the aggregator
            _serverEventAggregator.Subscribe<OnAccountLoggedInEventArgs>(HandleOnAccountLoggedIn);
            _serverEventAggregator.Subscribe<OnPlayerCashUpdateEventArgs>(HandleOnPlayerCashUpdate);
            _serverEventAggregator.Subscribe<OnCharacterSelectedEventArgs>(HandleOnCharacterSelected);
            _serverEventAggregator.Subscribe<OnPlayerInventoryItemUsedEventArgs>(HandleOnPlayerInventoryItemUsed);
        }

        private async Task HandleOnAccountLoggedIn(OnAccountLoggedInEventArgs eventArgs)
        {
            Console.WriteLine($"Handling OnAccountLoggedIn for username: {eventArgs.Account.GetAccountUsername()}");
            await DispatchEventAsync("OnAccountLoggedIn", eventArgs);
        }
        private async Task HandleOnCharacterSelected(OnCharacterSelectedEventArgs eventArgs)
        {
            Console.WriteLine($"Handling OnCharacterSelected for username: {eventArgs.Account.GetAccountUsername()}");
            await DispatchEventAsync("OnCharacterSelected", eventArgs);
        }

        private async Task HandleOnPlayerCashUpdate(OnPlayerCashUpdateEventArgs eventArgs)
        {
            Console.WriteLine($"Handling OnPlayerCashUpdate for player: {eventArgs.Player.Name}");
            await DispatchEventAsync("OnPlayerCashUpdate", eventArgs);
        }

        private async Task HandleOnPlayerInventoryItemUsed(OnPlayerInventoryItemUsedEventArgs eventArgs)
        {
            Console.WriteLine($"Handling OnPlayerInventoryItemUsed for player: {eventArgs.Player.Name}");
            await DispatchEventAsync("OnPlayerInventoryItemUsed", eventArgs);
        }

        private void RegisterEventHandlers()
        {
            foreach (var assembly in ServerSystemRegistry.RegisteredAssemblies)
            {
                // Get all IServerSystem types in the assembly
                var systemTypes = assembly.GetTypes()
                    .Where(t => typeof(IServerSystem).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

                foreach (var type in systemTypes)
                {
                    // Resolve the instance from DI
                    var instance = _serviceProvider.GetService(type);
                    if (instance == null) continue;

                    _systemInstances.Add(instance);

                    // Get methods with the ServerEvent attribute
                    var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                        .Where(m => m.GetCustomAttributes(typeof(ServerEventAttribute), inherit: false).Any());

                    foreach (var method in methods)
                    {
                        var attribute = method.GetCustomAttribute<ServerEventAttribute>();
                        string eventName = attribute?.EventName ?? method.Name;

                        if (!_eventHandlers.ContainsKey(eventName))
                        {
                            _eventHandlers[eventName] = new List<MethodInfo>();
                        }

                        _eventHandlers[eventName].Add(method);

                        _logger.LogDebug($"Registered method {method.Name} to EventHandler \"{eventName}\".");
                    }
                }
            }
        }

        /// <summary>
        /// Dispatches an event to all registered handlers.
        /// </summary>
        /// <param name="eventName">The name of the event to dispatch.</param>
        /// <param name="eventArgs">The event arguments.</param>
        public async Task DispatchEventAsync(string eventName, ServerEventArgs eventArgs)
        {
            if (_eventHandlers.TryGetValue(eventName, out var handlers))
            {
                foreach (var handler in handlers)
                {
                    // Find the instance that contains this method
                    var systemInstance = _systemInstances.FirstOrDefault(inst => handler.DeclaringType.IsInstanceOfType(inst));
                    if (systemInstance == null)
                        continue;

                    try
                    {
                        var parameters = handler.GetParameters();
                        object[] invokeParameters = new object[parameters.Length];

                        for (int i = 0; i < parameters.Length; i++)
                        {
                            var param = parameters[i];
                            if (param.ParameterType.IsAssignableFrom(eventArgs.GetType()))
                            {
                                invokeParameters[i] = eventArgs;
                            }
                            else
                            {
                                // Resolve other dependencies via DI
                                var service = _serviceProvider.GetService(param.ParameterType);
                                if (service == null)
                                {
                                    Console.WriteLine($"Unable to resolve service for type '{param.ParameterType}' required by method '{handler.Name}'.");
                                    invokeParameters[i] = null; // Assign null or handle as needed
                                }
                                else
                                {
                                    invokeParameters[i] = service;
                                }
                            }
                        }

                        // Invoke the handler method with resolved parameters
                        var result = handler.Invoke(systemInstance, invokeParameters);

                        // If the handler is asynchronous, await its completion
                        if (result is Task task)
                        {
                            await task;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error invoking handler for event '{eventName}': {ex.Message}");
                        Console.WriteLine(ex.ToString());
                    }
                }
            }
            else
            {
                Console.WriteLine($"No handlers registered for event '{eventName}'.");
            }
        }
    }
}
