﻿using System;
using System.IO;
using System.Reflection;
using NSpec.Domain;
using NSpec.Domain.Formatters;

namespace NSpecRunner
{
    [Serializable]
    public class NSpecDomain
    {
        //largely inspired from:
        //http://thevalerios.net/matt/2008/06/run-anonymous-methods-in-another-appdomain/

        public NSpecDomain(string config)
        {
            this.config = config;
        }

        public void Run(string dll, string filter, string tags, IFormatter outputFormatter, Action<string, string, string, IFormatter> action)
        {
            this.dll = dll;

            var setup = new AppDomainSetup();

            setup.ConfigurationFile = Path.GetFullPath(config);

            setup.ApplicationBase = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            domain = AppDomain.CreateDomain("NSpecDomain.Run", null, setup);

            var type = typeof(Wrapper);

            var assemblyName = type.Assembly.GetName().Name;

            var typeName = type.FullName;

            domain.AssemblyResolve += Resolve;

            var wrapper = (Wrapper)domain.CreateInstanceAndUnwrap(assemblyName, typeName);

            wrapper.Execute(dll, filter, tags, outputFormatter, action);

            AppDomain.Unload(domain);
        }

        Assembly Resolve(object sender, ResolveEventArgs args)
        {
            var name = args.Name;
            var argNameForResolve = args.Name.ToLower();

            if (!argNameForResolve.EndsWith(".dll") && !argNameForResolve.Contains(".resource"))
                name += ".dll";
            else if (argNameForResolve.Contains(".resource"))
                name = argNameForResolve.Substring(0, argNameForResolve.IndexOf(".resource")) + ".xml";

            var missing = Path.Combine(Path.GetDirectoryName(dll), name);

            var assembly = Assembly.LoadFrom(missing);

            return assembly;
        }

        string config;
        AppDomain domain;
        string dll;
    }
}