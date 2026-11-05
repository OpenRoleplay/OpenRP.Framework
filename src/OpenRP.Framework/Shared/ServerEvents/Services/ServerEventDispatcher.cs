using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenRP.Framework.Shared.ServerEvents.Attributes;
using OpenRP.Framework.Shared.ServerEvents.Entities;
using OpenRP.Framework.Shared.ServerEvents.Entities.EventArgs;
using OpenRP.Framework.Shared.ServerEvents.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace OpenRP.Framework.Shared.ServerEvents.Services
{
    /// <summary>
    /// Finds every [ServerEvent] method in the registered IServerSystem classes and runs them when their event is
    /// published. A handler's event is the type of its ServerEventArgs parameter; its name doesn't matter.
    /// </summary>
    public class ServerEventDispatcher : IServerEventDispatcher
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ServerEventDispatcher> _logger;
        private readonly List<ServerEventHandler> _handlers = new List<ServerEventHandler>();

        public ServerEventDispatcher(IServiceProvider serviceProvider, ILogger<ServerEventDispatcher> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            RegisterHandlers();
        }

        public async Task DispatchAsync(ServerEventArgs eventArgs)
        {
            List<ServerEventHandler> handlers = GetHandlersFor(eventArgs);
            _logger.LogDebug("Dispatching {EventName} to {HandlerCount} handlers.", eventArgs.GetType().Name, handlers.Count);

            foreach (ServerEventHandler handler in handlers)
            {
                await InvokeHandlerAsync(handler, eventArgs);
            }
        }

        private List<ServerEventHandler> GetHandlersFor(ServerEventArgs eventArgs)
        {
            Type eventArgsType = eventArgs.GetType();
            return _handlers.Where(handler => handler.EventType.IsAssignableFrom(eventArgsType)).ToList();
        }

        private void RegisterHandlers()
        {
            foreach (Type systemType in GetServerSystemTypes())
            {
                object? system = _serviceProvider.GetService(systemType);
                if (system == null)
                {
                    _logger.LogWarning("Server system {SystemName} is not registered as a service, so its handlers are ignored.", systemType.Name);
                    continue;
                }

                foreach (MethodInfo method in GetServerEventMethods(systemType))
                {
                    RegisterHandler(system, method);
                }
            }

            int eventCount = _handlers.Select(handler => handler.EventType).Distinct().Count();
            _logger.LogInformation("Registered {HandlerCount} server event handlers for {EventCount} events.", _handlers.Count, eventCount);
        }

        private static IEnumerable<Type> GetServerSystemTypes()
        {
            return ServerSystemRegistry.RegisteredAssemblies
                .Distinct()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => typeof(IServerSystem).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract);
        }

        private static IEnumerable<MethodInfo> GetServerEventMethods(Type systemType)
        {
            return systemType
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(method => method.IsDefined(typeof(ServerEventAttribute), inherit: false));
        }

        private void RegisterHandler(object system, MethodInfo method)
        {
            string handlerName = $"{method.DeclaringType?.Name}.{method.Name}";
            string? error = GetHandlerError(method);
            if (error != null)
            {
                _logger.LogError("Server event handler {HandlerName} is ignored: {Reason}", handlerName, error);
                return;
            }

            if (IsAsyncVoid(method))
            {
                _logger.LogWarning("Server event handler {HandlerName} is async void, so it isn't awaited and its errors are lost. Make it return Task.", handlerName);
            }

            ParameterInfo[] parameters = method.GetParameters();
            Type eventType = parameters.Single(IsEventParameter).ParameterType;
            _handlers.Add(new ServerEventHandler(system, method, parameters, eventType));
        }

        /// <summary>
        /// Why this method can't be a handler, or null when it can.
        /// </summary>
        private string? GetHandlerError(MethodInfo method)
        {
            ParameterInfo[] parameters = method.GetParameters();
            int eventParameterCount = parameters.Count(IsEventParameter);
            if (eventParameterCount != 1)
            {
                return $"it needs exactly one parameter deriving from {nameof(ServerEventArgs)}, but has {eventParameterCount}.";
            }

            ParameterInfo? unresolvableParameter = parameters
                .Where(parameter => !IsEventParameter(parameter))
                .FirstOrDefault(parameter => !IsService(parameter.ParameterType));
            if (unresolvableParameter != null)
            {
                return $"parameter '{unresolvableParameter.Name}' of type {unresolvableParameter.ParameterType.Name} is not a registered service.";
            }
            return null;
        }

        private static bool IsEventParameter(ParameterInfo parameter)
        {
            return typeof(ServerEventArgs).IsAssignableFrom(parameter.ParameterType);
        }

        private bool IsService(Type type)
        {
            IServiceProviderIsService? serviceChecker = _serviceProvider.GetService<IServiceProviderIsService>();
            return serviceChecker == null || serviceChecker.IsService(type);
        }

        private static bool IsAsyncVoid(MethodInfo method)
        {
            return method.ReturnType == typeof(void) && method.IsDefined(typeof(AsyncStateMachineAttribute), inherit: false);
        }

        private async Task InvokeHandlerAsync(ServerEventHandler handler, ServerEventArgs eventArgs)
        {
            try
            {
                object?[] arguments = ResolveArguments(handler, eventArgs);
                if (handler.Method.Invoke(handler.System, arguments) is Task task)
                {
                    await task;
                }
            }
            catch (Exception ex)
            {
                Exception error = ex is TargetInvocationException { InnerException: not null } invocationException ? invocationException.InnerException : ex;
                _logger.LogError(error, "Server event handler {HandlerName} failed while handling {EventName}.", handler.GetName(), eventArgs.GetType().Name);
            }
        }

        /// <summary>
        /// Fills the handler's parameters: the event args, and services from the root provider. Not from a scope,
        /// because a handler may pass its services on to dialogs whose callbacks run after it returns (for example
        /// OnAccountLoggedIn opening the character selection), so they must outlive the handler. SampSharp resolves
        /// the services of [Event] handlers the same way.
        /// </summary>
        private object?[] ResolveArguments(ServerEventHandler handler, ServerEventArgs eventArgs)
        {
            return handler.Parameters
                .Select(parameter => IsEventParameter(parameter) ? eventArgs : _serviceProvider.GetRequiredService(parameter.ParameterType))
                .ToArray();
        }
    }
}
