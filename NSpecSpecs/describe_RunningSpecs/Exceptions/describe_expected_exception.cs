﻿using System;
using System.Linq;
using NSpec;
using NSpec.Domain;
using NSpecSpecs.WhenRunningSpecs;
using NUnit.Framework;

namespace NSpecSpecs.describe_RunningSpecs.Exceptions
{
    [TestFixture]
    [Category("RunningSpecs")]
    public class describe_expected_exception : when_running_specs
    {
        private class SpecClass : nspec
        {
            void method_level_context()
            {
                before = () => { };

                it["should throw exception"] = expect<InvalidOperationException>(() => { throw new InvalidOperationException(); });

                it["should throw exception with error message Testing"] = expect<InvalidOperationException>("Testing", () => { throw new InvalidOperationException("Testing"); });

                it["should fail if no exception thrown"] = expect<InvalidOperationException>(() => { });

                it["should fail if wrong exception thrown"] = expect<InvalidOperationException>(() => { throw new ArgumentException(); });

                it["should fail if wrong error message is returned"] = expect<InvalidOperationException>("Testing", () => { throw new InvalidOperationException("Blah"); });
            }
        }

        [SetUp]
        public void setup()
        {
            Init(typeof(SpecClass)).Run();
        }

        [Test]
        public void should_be_three_failures()
        {
            classContext.Failures().Count().should_be(3);
        }

        [Test]
        public void given_exception_is_thrown_should_not_fail()
        {
            TheExample("should throw exception").should_not_have_failed();
        }

        [Test]
        public void given_exception_is_thrown_with_expected_message_should_not_fail()
        {
            TheExample("should throw exception with error message Testing").should_not_have_failed();
        }

        [Test]
        public void given_exception_not_thrown_should_fail()
        {
            TheExample("should fail if no exception thrown").Exception.GetType().should_be(typeof(ExceptionNotThrown));
        }

        [Test]
        public void given_wrong_exception_should_fail()
        {
            TheExample("should fail if wrong exception thrown").Exception.GetType().should_be(typeof(ExceptionNotThrown));
            TheExample("should fail if wrong exception thrown").Exception.Message.should_be("Exception of type InvalidOperationException was not thrown.");
        }

        [Test]
        public void given_wrong_error_message_should_fail()
        {
            TheExample("should fail if wrong error message is returned").Exception.GetType().should_be(typeof(ExceptionNotThrown));
            TheExample("should fail if wrong error message is returned").Exception.Message.should_be("Expected message: \"Testing\" But was: \"Blah\"");
        }
    }
}