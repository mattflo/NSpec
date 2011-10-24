﻿using System;
using System.Linq;
using NSpec;
using NUnit.Framework;
using NSpec.Domain;

namespace NSpecSpecs.WhenRunningSpecs
{
    [TestFixture]
    [Category("RunningSpecs")]
    public class when_act_contains_exception : when_running_specs
    {
        private class SpecClass : nspec
        {
            void method_level_context()
            {
                act = () => { throw new InvalidOperationException(); };

                it["should fail this example because of act"] = () => "1".should_be("1");

                it["should also fail this example because of act"] = () => "1".should_be("1");
            }
        }

        [SetUp]
        public void setup()
        {
            Run(typeof(SpecClass));
        }

        [Test]
        public void the_example_level_failure_should_indicate_a_context_failure()
        {
            TheExample( "should fail this example because of act" )
                .ExampleLevelException.GetType().should_be( typeof( ContextFailureException ) );
            TheExample( "should also fail this example because of act" )
                .ExampleLevelException.GetType().should_be( typeof( ContextFailureException ) );
        }

        [Test]
        public void it_should_fail_all_examples_in_act()
        {
            TheExample( "should fail this example because of act" ).ExampleLevelException
                .InnerException.GetType().should_be( typeof( InvalidOperationException ) );
            TheExample( "should also fail this example because of act" ).ExampleLevelException
                .InnerException.GetType().should_be( typeof( InvalidOperationException ) );
        }
    }
}
