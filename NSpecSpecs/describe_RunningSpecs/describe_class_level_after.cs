﻿using System.Linq;
using NSpecSpecs.WhenRunningSpecs;
using NSpec;
using NUnit.Framework;

namespace NSpecSpecs.describe_RunningSpecs
{
    [TestFixture]
    [Category("RunningSpecs")]
    public class describe_class_level_after : when_running_specs
    {
        class SpecClass : nspec
        {
            public string ExecutionSequence = string.Empty;

            void after_each()
            {
                ExecutionSequence += "B";
            }
        }

        class DerivedClass : SpecClass
        {
            void after_each()
            {
                ExecutionSequence += "A";
            }

            void running_example()
            {
                it["works"] = () => ExecutionSequence.should_be_empty();
            }
        }

        abstract class AbstractDerivedClass1 : DerivedClass
        {
            void after_each()
            {
                ExecutionSequence += "9";
            }
        }

        abstract class AbstractDerivedClass2 : AbstractDerivedClass1
        {
        }

        abstract class AbstractDerivedClass3 : AbstractDerivedClass2
        {
            void after_each()
            {
                ExecutionSequence += "8";
            }
        }

        class DerivedClass2 : AbstractDerivedClass3
        {
            void after_each()
            {
                ExecutionSequence += "7";
            }

            void running_example()
            {
                it["works"] = () => ExecutionSequence.should_be_empty();
            }
        }

        [Test]
        public void afters_are_run_in_the_correct_order()
        {
            Run(typeof(DerivedClass));

            var specInstance = classContext.GetInstance() as DerivedClass;

            var executionSequence = specInstance.ExecutionSequence;

            executionSequence.should_be("AB");

            classContext.Failures().Count().should_be(0);
        }

        [Test]
        public void afters_are_run_in_the_correct_order_when_abstract_middle_classes_are_present()
        {
            Run(typeof(DerivedClass2));

            var specInstance = classContext.GetInstance() as DerivedClass2;

            var executionSequence = specInstance.ExecutionSequence;

            executionSequence.should_be("789AB");

            classContext.Failures().Count().should_be(0);
        }
    }
}
