﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using NSpec.Domain.Extensions;

namespace NSpec.Domain
{
    public class ClassContext : Context
    {
        private readonly List<Type> classHierarchyToClass = new List<Type>();

        private IEnumerable<MethodInfo> GetMethodsFromHierarchy(Func<Type, MethodInfo> methodAccessor)
        {
            return classHierarchyToClass.Select(methodAccessor).Where(mi => mi != null);
        }

        private void BuildMethodLevelBefore()
        {
            var befores = GetMethodsFromHierarchy(conventions.GetMethodLevelBefore).ToList();;
            if(befores.Count > 0)
            {
                BeforeInstance = instance => befores.Do(b => b.Invoke(instance, null));
            }
        }

        private void BuildMethodLevelAct()
        {
            var acts = GetMethodsFromHierarchy(conventions.GetMethodLevelAct).ToList();;
            if(acts.Count > 0)
            {
                ActInstance = instance => acts.Do(a => a.Invoke(instance, null));
            }
        }

        private void BuildMethodLevelAfter()
        {
            var afters = GetMethodsFromHierarchy(conventions.GetMethodLevelAfter).Reverse().ToList();;
            if(afters.Count > 0)
            {
                AfterInstance = instance => afters.Do(a => a.Invoke(instance, null));
            }
        }

        public ClassContext(Type type, Conventions conventions = null, Tags tagsFilter = null, string tags = null)
            : base(type.GetPrettyName(), tags, 0)
        {
            this.type = type;

            this.conventions = conventions ?? new DefaultConventions().Initialize();

            this.tagsFilter = tagsFilter;

            if (type != typeof(nspec))
            {
                classHierarchyToClass.AddRange(type.GetAbstractBaseClassChainWithClass());
            }
        }

        public override void Build(nspec instance = null)
        {
            BuildMethodLevelBefore();

            BuildMethodLevelAct();

            BuildMethodLevelAfter();

            var nspec = type.Instance<nspec>();

            nspec.tagsFilter = tagsFilter ?? new Tags();

            base.Build(nspec);
        }

        public override bool IsSub(Type baseType)
        {
            while (baseType != null && baseType.IsAbstract)
            {
                baseType = baseType.BaseType;
            }

            return baseType == type;
        }

        Conventions conventions;

        public Type type;

        public Tags tagsFilter;
    }
}