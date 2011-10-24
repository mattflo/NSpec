﻿using System;
using System.Collections.Generic;
using System.Reflection;
using NSpec;
using NSpec.Domain;
using NSpec.Domain.Formatters;

namespace NSpecRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                ShowUsage();
                return;
            }
            try
            {
                // extract either a class filter or a tags filter (but not both)
                var classFilter = "";
                var argsTags = "";
                if( args.Length > 1 )
                {
                    // see rspec and cucumber for ideas on better ways to handle tags on the command line:
                    // https://github.com/cucumber/cucumber/wiki/tags
                    // https://www.relishapp.com/rspec/rspec-core/v/2-4/docs/command-line/tag-option
                    if( args[ 1 ] == "--tag" && args.Length > 2 )
                        argsTags = args[ 2 ];
                    else
                        classFilter = args[ 1 ];
                }

                var specDLL = args[0];

                var domain = new NSpecDomain(specDLL + ".config");

                var console = new ConsoleFormatter();

                domain.Run( specDLL, classFilter, argsTags, console, ( dll, filter, tags, formatter ) =>
                {
                    var finder = new SpecFinder(dll, new Reflector(), filter);

                    var tagsFilter = new Tags().ParseTagFilters( tags );

                    var builder = new ContextBuilder( finder, tagsFilter, new DefaultConventions() );

                    var runner = new ContextRunner(builder, formatter);

                    runner.Run();
                });
            }
            catch (Exception e)
            {
                //hopefully this is handled before here, but if not, this is better than crashing the runner
                Console.WriteLine(e);
            }
        }

        private static void ShowUsage()
        {
            Console.WriteLine("VERSION: {0}".With(Assembly.GetExecutingAssembly().GetName().Version));
            Console.WriteLine();
            Console.WriteLine("Example usage:");
            Console.WriteLine();
            Console.WriteLine("nspecrunner path_to_spec_dll [regex pattern]");
            Console.WriteLine();
            Console.WriteLine("The second parameter is optional. If supplied, only the classes that match the regex will be run.  The full class name including namespace is considered. Otherwise all spec classes in the dll will be run.");
        }
    }
}
