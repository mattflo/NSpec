﻿using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using NSpec;
using NSpec.Assertions.nUnit;

namespace SampleSpecs.Bug
{
    internal class describe_failing_deserialization : nspec
    {
        MemoryStream stream;
        BinaryFormatter formatter;
        object _object;

        void when_serializing_objects()
        {
            before = () =>
            {
                stream = new MemoryStream();
                formatter = new BinaryFormatter();
            };

            act = () => formatter.Serialize(stream, _object);

            context["that are not in the search path"] = () =>
            {
                before = () => _object = new Action(() => { }).Method;

                it["should deserialize them again"] = () => // fails
                {
                    stream.Position = 0;
                    formatter.Deserialize(stream).should_not_be_null();
                };
            };

            context["that are in the search path"] = () =>
            {
                before = () => _object = new object();

                it["should deserialize them again"] = () =>
                {
                    stream.Position = 0;
                    formatter.Deserialize(stream).should_not_be_null();
                };
            };
        }
    }
}