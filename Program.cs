using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LogicTest
{
    public abstract class LogicTestBase
    {
        public virtual bool doLog { get; } = true;
        /// <summary>
        /// LogicTestBaseを継承したクラスの実行を決めます。
        /// </summary>
        public virtual bool doInvoke { get; } = true;
        public abstract void Start();
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
                        if (instance.doInvoke)
                        {
                            if(instance.doLog) Debug.Log($"{type.Name} -> Running....");
                            instance.Start();
                            if(instance.doLog) Debug.Log($"{type.Name} -> Executed");
                        }
                    }
                }
            }

            Console.ReadLine();
        }
    }

    class TimePrint
    {
        public static string Current => DateTime.Now.ToString("HH:mm:ss");
    }

    public class Debug
    {
        public static void Log(string value)
        {
            Console.WriteLine($"[{TimePrint.Current}] {value}");
        }
    }
}
