﻿using System;
using System.Linq;
using System.Reflection;
using NSpec;
using NSpec.Domain;

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
                var argsTags = "";

                var failFast = IsFailFast(args);

                args = RemoveFailFastSwitch(args);

                if (args.Length > 1)
                {
                    // see rspec and cucumber for ideas on better ways to handle tags on the command line:
                    // https://github.com/cucumber/cucumber/wiki/tags
                    // https://www.relishapp.com/rspec/rspec-core/v/2-4/docs/command-line/tag-option
                    if (args[1] == "--tag" && args.Length > 2)
                        argsTags = args[2];
                    else
                        argsTags = args[1];
                }

                var specDLL = args[0];

                var invocation = new RunnerInvocation(specDLL, argsTags, failFast);

                var domain = new NSpecDomain(specDLL + ".config");

                var failures = domain.Run(invocation, i => i.Run().Failures().Count(), specDLL);

                if (failures > 0) Environment.Exit(1);
            }
            catch (Exception e)
            {
                //hopefully this is handled before here, but if not, this is better than crashing the runner
                Console.WriteLine(e);
                Environment.Exit(1);
            }
        }

        public static string[] RemoveFailFastSwitch(string[] args)
        {
            return args.Where(s => s != "--failfast").ToArray();
        }

        public static bool IsFailFast(string[] args)
        {
            return args.Any(s => s == "--failfast");
        }

        private static void ShowUsage()
        {
            Console.WriteLine("VERSION: {0}".With(Assembly.GetExecutingAssembly().GetName().Version));
            Console.WriteLine();
            Console.WriteLine("Example usage:");
            Console.WriteLine();
            Console.WriteLine("nspecrunner path_to_spec_dll [classname]");
            Console.WriteLine();
            Console.WriteLine("The second parameter is optional. If supplied, only that specific test class will run.  Otherwise all spec classes in the dll will be run.");
            Console.WriteLine();
            Console.WriteLine("nspecrunner path_to_spec_dll --tag classname");
            Console.WriteLine();
            Console.WriteLine("The command above is equivalent to specifing the second parameter in: nspecrunner path_to_spec_dll [classname]");
            Console.WriteLine();
            Console.WriteLine("Example usage (tagging):");
            Console.WriteLine();
            Console.WriteLine("nspecrunner path_to_spec_dll --tag tag1,tag2,tag3");
            Console.WriteLine();
            Console.WriteLine("This wil run all tests under tags specified.  A test class's name is automatically considered in tagging.");
            Console.WriteLine();
            Console.WriteLine("Example usage (failfast):");
            Console.WriteLine();
            Console.WriteLine("nspecrunner path_to_spec_dll [classname] --failfast");
            Console.WriteLine();
            Console.WriteLine("Adding --failfast to any of the commands above will stop execution immediately when a failure is encountered.");
        }
    }
}
