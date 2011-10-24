﻿using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using System.Linq.Expressions;
using NSpec.Domain;

namespace NSpec
{
    public static class AssertionExtensions
    {
        public static void should<T>(this T o, Expression<Predicate<T>> predicate)
        {
            Assert.IsTrue(predicate.Compile()(o), Example.Parse(predicate.Body));
        }

        public static void should_not_be_null(this object o)
        {
            Assert.IsNotNull(o);
        }

        public static void should_not_be_default<T>(this T t)
        {
            Assert.AreNotEqual(default(T), t);
        }

        public static void is_not_null_or_empty(this string source)
        {
            Assert.IsNotNullOrEmpty(source);
        }

        public static void is_true(this bool actual) { actual.should_be_true(); }
        public static void should_be_true(this bool actual)
        {
            Assert.IsTrue(actual);
        }

        public static void is_false(this bool actual) { actual.should_be_false(); }
        public static void should_be_false(this bool actual)
        {
            Assert.IsFalse(actual);
        }

        public static void should_be_greater_than(this int greater, int lesser)
        {
            Assert.Greater(greater, lesser);
        }

        public static void should_be(this object actual, object expected)
        {
            actual.Is(expected);
        }

        public static void Is(this object actual, object expected)
        {
            Assert.AreEqual(expected, actual);
        }


        public static void should_not_be(this object actual, object expected)
        {
            Assert.AreNotEqual(expected, actual);
        }

        public static void should_be(this string actual, string expected)
        {
            Assert.AreEqual(expected, actual);
        }

        public static void should_end_with(this string actual, string end)
        {
            StringAssert.EndsWith(end, actual);
        }

        public static void should_start_with(this string actual, string start)
        {
            StringAssert.StartsWith(start, actual);
        }

        public static void should_contain(this string actual, string expected)
        {
            StringAssert.Contains(expected, actual);
        }

        public static IEnumerable<T> should_not_contain<T>(this IEnumerable<T> collection, Func<T, bool> predicate)
        {
            Assert.IsTrue(!collection.Any(predicate), "collection contains an item it should not.".With(collection, predicate));

            return collection;
        }

        public static IEnumerable<T> should_contain<T>(this IEnumerable<T> collection, Func<T, bool> predicate)
        {
            Assert.IsTrue(collection.Any(predicate), "collection does not contain an item it should.".With(collection, predicate));

            return collection;
        }

        public static IEnumerable<T> should_contain<T>(this IEnumerable<T> collection, T t)
        {
            CollectionAssert.Contains(collection, t);

            return collection;
        }

        public static IEnumerable<T> should_not_contain<T>(this IEnumerable<T> collection, T t)
        {
            CollectionAssert.DoesNotContain(collection, t);

            return collection;
        }

        public static IEnumerable<T> should_not_be_empty<T>( this IEnumerable<T> collection )
        {
            CollectionAssert.IsNotEmpty(collection);

            return collection;
        }

        public static IEnumerable<T> should_be_empty<T>(this IEnumerable<T> collection)
        {
            CollectionAssert.IsEmpty(collection);

            return collection;
        }

        public static IEnumerable<T> should_be<T>(this IEnumerable<T> actual, params T[] expected)
        {
            CollectionAssert.AreEqual(expected.ToArray(), actual.ToArray());

            return actual;
        }

        public static IEnumerable<T> should_be<T>(this IEnumerable<T> actual, IEnumerable<T> expected)
        {
            CollectionAssert.AreEqual(expected.ToArray(), actual.ToArray());

            return actual;
        }

        public static T should_cast_to<T>(this object value)
        {
            Assert.IsInstanceOf<T>(value);
            return (T)value;
        }

        public static void should_not_match(this string value, string pattern)
        {
            StringAssert.DoesNotMatch(pattern, value);
        }

        public static void should_be_same(this object actual, object expected)
        {
            Assert.AreSame(expected, actual);
        }

        public static void should_not_be_same( this object actual, object expected )
        {
            Assert.AreNotSame( expected, actual );
        }

        public static List<string> should_contain_tag( this List<string> collection, string tag )
        {
            CollectionAssert.Contains( collection, tag.TrimStart( new[] { '@' } ) );

            return collection;
        }

        public static Tags should_tag_as_included( this Tags tags, string tag )
        {
            CollectionAssert.Contains( tags.IncludeTags, tag.TrimStart( new[] { '@' } ) );

            return tags;
        }

        public static Tags should_tag_as_excluded( this Tags tags, string tag )
        {
            CollectionAssert.Contains( tags.ExcludeTags, tag.TrimStart( new[] { '@' } ) );

            return tags;
        }
    }
}