using System;
using System.Linq;
using System.Reflection;
using NSpec.Domain.Extensions;
using System.Text.RegularExpressions;

namespace NSpec.Domain
{
    public class ConventionSpecification
    {
        public void SetBefore(string startsWith)
        {
            SetBefore(new Regex("^" + startsWith));
        }

        public void SetBefore(Regex regex)
        {
            Before = regex;
        }

        public void SetAct(string startsWith)
        {
            SetAct(new Regex("^" + startsWith));
        }

        public void SetAct(Regex regex)
        {
            Act = regex;
        }

        public void SetAfter(string startsWith)
        {
            SetAfter(new Regex("^" + startsWith));
        }

        public void SetAfter(Regex regex)
        {
            After = regex;
        }

        public void SetExample(string startsWith)
        {
            SetExample(new Regex("^" + startsWith));
        }

        public void SetExample(Regex regex)
        {
            Example = regex;
        }

        public void SetContext(string startsWith)
        {
            SetContext(new Regex("^" + startsWith));
        }

        public void SetContext(Regex regex)
        {
            Context = regex;
        }

        public Regex Before { get; private set; }

        public Regex Act { get; private set; }

        public Regex After { get; private set; }

        public Regex Example { get; private set; }

        public Regex Context { get; private set; }
    }

    public abstract class Conventions
    {
        public Conventions Initialize()
        {
            specification = new ConventionSpecification();
            SpecifyConventions(specification);
            return this;
        }

        public abstract void SpecifyConventions(ConventionSpecification specification);

        public MethodInfo GetMethodLevelBefore(Type type)
        {
            return GetMethodMatchingRegex(type, specification.Before);
        }

        public MethodInfo GetMethodLevelAct(Type type)
        {
            return GetMethodMatchingRegex(type, specification.Act);
        }

        public MethodInfo GetMethodLevelAfter(Type type)
        {
            return GetMethodMatchingRegex(type, specification.After);
        }

        private MethodInfo GetMethodMatchingRegex(Type type, Regex regex)
        {
            return type.Methods().Where(mi => mi.DeclaringType == type).FirstOrDefault(mi => regex.IsMatch(mi.Name));
        }

        public bool IsMethodLevelExample(string name)
        {
            return specification.Example.IsMatch(name);
        }

        public bool IsMethodLevelBefore(string name)
        {
            return specification.Before.IsMatch(name);
        }

        public bool IsMethodLevelAct(string name)
        {
            return specification.Act.IsMatch(name);
        }

        public bool IsMethodLevelAfter(string name)
        {
            return specification.After.IsMatch(name);
        }

        public bool IsMethodLevelContext(string name)
        {
            if (IsMethodLevelBefore(name)) return false;

            if (IsMethodLevelAct(name)) return false;

            if (IsMethodLevelExample(name)) return false;

            if (IsMethodLevelAfter(name)) return false;

            return specification.Context.IsMatch(name);
        }

        private ConventionSpecification specification;
    }
}
