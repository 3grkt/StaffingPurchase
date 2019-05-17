using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Core;

namespace StaffingPurchase.Core.Infrastructure
{
    public class ContainerManager
    {
        private static readonly object AutofacRequestLifetimeScopeTag = "AutofacWebRequest";

        private readonly IContainer _container;
        private readonly IContextManager _contextManager;

        public ContainerManager(IContainer container, IContextManager contextManager)
        {
            _container = container;
            _contextManager = contextManager;
        }

        public IContainer Container
        {
            get { return _container; }
        }

        public IContextManager ContextManager
        {
            get { return _contextManager; }
        }

        public T Resolve<T>(string key = "", ILifetimeScope scope = null, IEnumerable<Parameter> parameters = null)
            where T : class
        {
            if (scope == null)
            {
                //no scope specified
                scope = Scope();
            }
            if (string.IsNullOrEmpty(key))
            {
                if (parameters == null)
                    return scope.Resolve<T>();
                return scope.Resolve<T>(parameters);
            }

            if (parameters == null)
                return scope.ResolveKeyed<T>(key);
            return scope.ResolveKeyed<T>(key, parameters);
        }

        public object Resolve(Type type, ILifetimeScope scope = null)
        {
            if (scope == null)
            {
                //no scope specified
                scope = Scope();
            }
            return scope.Resolve(type);
        }

        public T[] ResolveAll<T>(string key = "", ILifetimeScope scope = null)
        {
            if (scope == null)
            {
                //no scope specified
                scope = Scope();
            }
            if (string.IsNullOrEmpty(key))
            {
                return scope.Resolve<IEnumerable<T>>().ToArray();
            }
            return scope.ResolveKeyed<IEnumerable<T>>(key).ToArray();
        }

        public T ResolveUnregistered<T>(ILifetimeScope scope = null) where T : class
        {
            return ResolveUnregistered(typeof (T), scope) as T;
        }

        public object ResolveUnregistered(Type type, ILifetimeScope scope = null)
        {
            if (scope == null)
            {
                //no scope specified
                scope = Scope();
            }
            var constructors = type.GetConstructors();
            foreach (var constructor in constructors)
            {
                try
                {
                    var parameters = constructor.GetParameters();
                    var parameterInstances = new List<object>();
                    foreach (var parameter in parameters)
                    {
                        var service = Resolve(parameter.ParameterType, scope);
                        if (service == null) throw new StaffingPurchaseException("Unkown dependency");
                        parameterInstances.Add(service);
                    }
                    return Activator.CreateInstance(type, parameterInstances.ToArray());
                }
                catch (StaffingPurchaseException)
                {
                }
            }
            throw new StaffingPurchaseException("No contructor was found that had all the dependencies satisfied.");
        }

        public bool TryResolve(Type serviceType, ILifetimeScope scope, out object instance)
        {
            if (scope == null)
            {
                //no scope specified
                scope = Scope();
            }
            return scope.TryResolve(serviceType, out instance);
        }

        public bool IsRegistered(Type serviceType, ILifetimeScope scope = null)
        {
            if (scope == null)
            {
                //no scope specified
                scope = Scope();
            }
            return scope.IsRegistered(serviceType);
        }

        public object ResolveOptional(Type serviceType, ILifetimeScope scope = null)
        {
            if (scope == null)
            {
                //no scope specified
                scope = Scope();
            }
            return scope.ResolveOptional(serviceType);
        }

        public ILifetimeScope Scope()
        {
            try
            {
                var contextScope = _contextManager.GetContextLifetimeScope();
                if (contextScope != null)
                    return contextScope;

                //when such lifetime scope is returned, you should be sure that it'll be disposed once used (e.g. in schedule tasks)
                return Container.BeginLifetimeScope(AutofacRequestLifetimeScopeTag);
            }
            catch (Exception exc)
            {
                //we can get an exception here if RequestLifetimeScope is already disposed
                //for example, requested in or after "Application_EndRequest" handler
                //but note that usually it should never happen

                //when such lifetime scope is returned, you should be sure that it'll be disposed once used (e.g. in schedule tasks)
                return Container.BeginLifetimeScope(AutofacRequestLifetimeScopeTag);
            }
        }
    }
}