using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LogicTest
{
    public abstract class LogicTestBase
    {
        public virtual bool IsRun { get; } = true;
        public abstract void Run();
    }

    class Program
    {
        static void Main(string[] args)
        {
            Type[] types = Assembly.GetExecutingAssembly().GetTypes();
            foreach (Type type in types)
            {
                if (type.IsSubclassOf(typeof(LogicTestBase)))
                {
                    if (Activator.CreateInstance(type) is LogicTestBase instance)
                    {
                        if (instance.IsRun)
                        {
                            Console.WriteLine($"Running {type}");
                            instance.Run();
                        }
                    }
                }
            }

            Console.ReadLine();
        }
    }
}
