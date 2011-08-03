using System.Linq.Expressions;
using NSpec.Domain;
using System;

namespace NSpec
{
    /// <summary>
    /// Inherit from this class to create your own specifications.  NSpecRunner will look through your project for 
    /// classes that derive from this class (inheritance chain is taken into consideration).
    /// </summary>
    public class nspec
    {
        public nspec()
        {
            context = new ActionRegister(AddContext);
            xcontext = new ActionRegister(AddIgnoredContext);
            describe = new ActionRegister(AddContext);
            xdescribe = new ActionRegister(AddIgnoredContext);

            it = new ActionRegister((name, action) => AddExample(new Example(name, action, pending: action == todo)));
            xit = new ActionRegister((name, action) => AddExample(new Example(name, action, pending: true)));
        }

        /// <summary>
        /// Create a specification/example using a single line lambda with an assertion(should). 
        /// The name of the specification will be parsed from the Expression
        /// <para>For Example:</para>
        /// <para>specify = () => _controller.should_be(false);</para>
        /// </summary>
        public virtual Expression<Action> specify
        {
            set { AddExample(new Example(value)); }
        }

        /// <summary>
        /// This Action get executed before each example is run.
        /// <para>For Example:</para>
        /// <para>before.each = () => someList = new List&lt;int&gt;();</para>
        /// <para>The before.each can be a multi-line lambda.  Setting the member multiple times through out sub-contexts will not override the action, but instead will append to your setup (this is a good thing).  For more information visit http://www.nspec.org</para>
        /// </summary>
        public virtual Action before
        {
            get { return Context.Before; }
            set { Context.Before = value; }
        }

        /// <summary>
        /// In development.
        /// </summary>
        public virtual Action after
        {
            get { return Context.After; }
            set { Context.After = value; }
        }

        /// <summary>
        /// Assign this member within your context.  The Action assigned will get executed
        /// with every example in scope.  Befores will run first, then acts, then your examples.  It's a way for you to define once a common Act in Arrange-Act-Assert for all subcontexts.  For more information visit http://www.nspec.org
        /// </summary>
        public virtual Action act
        {
            get { return Context.Act; }
            set { Context.Act = value; }
        }

        /// <summary>
        /// Create a subcontext.
        /// <para>For Examples see http://www.nspec.org</para>
        /// </summary>
        public ActionRegister context;

        /// <summary>
        /// Mark a subcontext as pending (add all child contexts as pending)
        /// </summary>
        public ActionRegister xcontext;

        /// <summary>
        /// This is an alias for creating a subcontext.  Use this to create sub contexts within your methods.
        /// <para>For Examples see http://www.nspec.org</para>
        /// </summary>
        public ActionRegister describe;

        /// <summary>
        /// This is an alias for creating a xcontext.
        /// <para>For Examples see http://www.nspec.org</para>
        /// </summary>
        public ActionRegister xdescribe;

        /// <summary>
        /// Create a specification/example using a name and a lambda with an assertion(should).
        /// <para>For Example:</para>
        /// <para>it["should return false"] = () => _controller.should_be(false);</para>
        /// </summary>
        public ActionRegister it;

        /// <summary>
        /// Mark a spec as pending 
        /// <para>For Example:</para>
        /// <para>xit["should return false"] = () => _controller.should_be(false);</para>
        /// <para>(the example will be marked as pending any lambda provided will not be executed)</para>
        /// </summary>
        public ActionRegister xit;

        /// <summary>
        /// Set up a pending spec.
        /// <para>For Example:</para>
        /// <para>it["a test i haven't flushed out yet, but need to"] = todo;</para>
        /// </summary>
        public readonly Action todo = () => { };

        /// <summary>
        /// Set up an expectation for a particular exception type to be thrown.
        /// <para>For Example:</para>
        /// <para>it["should throw exception"] = expect&lt;InvalidOperationException&gt;(() => SomeMethodThatThrowsException());</para>
        /// </summary>
        public virtual Action expect<T>(Action action) where T : Exception
        {
            return expect<T>( null, action );
        }

        /// <summary>
        /// Set up an expectation for a particular exception type to be thrown with an expected message.
        /// <para>For Example:</para>
        /// <para>it["should throw exception with message Error"] = expect&lt;InvalidOperationException&gt;("Error", () => SomeMethodThatThrowsException());</para>
        /// </summary>
        public virtual Action expect<T>(string expectedMessage, Action action) where T : Exception
        {
            return () =>
            {
                var closureType = typeof(T);

                try
                {
                    action();
                    throw new ExceptionNotThrown("Exception of type " + typeof(T).Name + " was not thrown.");
                }
                catch (Exception ex)
                {
                    if (ex.GetType() != closureType)
                    {
                        throw new ExceptionNotThrown("Exception of type " + typeof(T).Name + " was not thrown.");
                    }

                    if( expectedMessage != null && expectedMessage != ex.Message )
                    {
                        throw new ExceptionNotThrown( String.Format( "Expected message: \"{0}\" But was: \"{1}\"", expectedMessage, ex.Message ) );
                    }
                }
            };
        }

        void AddExample(Example example)
        {
            Context.AddExample(example);

            Context.Run(example, this);
        }

        void AddContext(string name, Action action)
        {
            var contextToRun = new Context(name, level);

            RunContext(contextToRun, action);
        }

        void AddIgnoredContext(string name, Action action)
        {
            var ignored = new Context(name, level, isPending: true);

            RunContext(ignored, action);
        }

        void RunContext(Context context, Action action)
        {
            level++;

            Context.AddContext(context);

            var beforeContext = Context;

            Context = context;

            action();

            level--;

            Context = beforeContext;
        }

        int level;

        //needs to be internal for one caller
        internal Context Context { get; set; }
    }
}