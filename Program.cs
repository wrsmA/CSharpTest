using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System.Timers;
using Timer = System.Timers.Timer;

namespace UnityCycle
{
    public abstract class UniCycle
    {
        /// <summary>
        /// クラス作成時にログを表示します。
        /// </summary>
        public virtual bool doLog { get; } = true;
        /// <summary>
        /// LogicTestBaseを継承したクラスの実行を決めます。
        /// </summary>
        public virtual bool doInvoke { get; } = true;
        /// <summary>
        /// デバッグ開始時に一度だけ実行されます。
        /// </summary>
        public virtual void Awake() => noneExecutable();
        /// <summary>
        /// Updateが実行される最初の一度だけ実行されます。
        /// </summary>
        public virtual void Start() => noneExecutable();
        /// <summary>
        /// 毎フレーム実行されます。
        /// </summary>
        public virtual void Update() => noneExecutable();
        /// <summary>
        /// 毎秒実行されます。
        /// </summary>
        public virtual void FixedUpdate() => noneExecutable();

        /// <summary>
        /// 何も実行しない関数です。
        /// </summary>
        void noneExecutable() { }
    }

    class Program
    {
        static void Main(string[] args)
        {
            #region Private Fields

            var UniInstance = new Dictionary<Type, UniCycle>();
            var updateTimers = new List<Timer>();
            var fixedUpdateTimers = new List<Timer>();

            #endregion

            UniCycleFactory();
            #region UniCycleFactory

            void UniCycleFactory()
            {
                foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
                {
                    if (type.IsSubclassOf(typeof(UniCycle)))
                    {
                        if (Activator.CreateInstance(type) is UniCycle instance)
                        {
                            if (instance.doInvoke)
                            {
                                UniInstance.Add(type, instance);
                            }
                        }
                    }
                }
            }

            #endregion

            AwakeOrder();
            #region Awake

            void AwakeOrder()
            {
                var tasks = new List<Task>();
                foreach(var _uInstance in UniInstance)
                {
                    if(_uInstance.Value.doLog) Debug.Log($"{_uInstance.Key.Name} -> Awake Executed...");
                    tasks.Add(Task.Run(() => _uInstance.Value.Awake()));
                }
                Task.WhenAll(tasks);
            }

            #endregion
            
            StartOrder();
            #region Start

            /// <summary>
            /// timeLag = Awake終了後とStart開始時までのタイムラグ
            /// 1 timelag == 1 ミリ秒
            /// </summary>
            async void StartOrder(int timeLag = 1000)
            {
                await Task.Delay(timeLag);
                foreach(var _uInstance in UniInstance)
                {
                    if (_uInstance.Value.doLog) Debug.Log($"{_uInstance.Key.Name} -> Start Executed...");
                    _ = Task.Run(() => _uInstance.Value.Start());
                }
            }

            #endregion

            UpdateOrder();
            #region Update

            void UpdateOrder()
            {
                foreach (var _uInstance in UniInstance)
                {
                    var timer = new Timer()
                    {
                        Interval = 1,
                        AutoReset = true,
                        Enabled = true
                    };
                    timer.Elapsed += new ElapsedEventHandler((sender, e) => _uInstance.Value.Update());
                    updateTimers.Add(timer);
                }
            }

            #endregion

            FixedUpdateOrder();
            #region FixedUpdate

            void FixedUpdateOrder(){
                foreach (var _uInstance in UniInstance)
                {
                    var timer = new Timer() {
                        Interval = 1000,
                        AutoReset = true,
                        Enabled = true
                    };
                    timer.Elapsed += new ElapsedEventHandler((sender, e) => _uInstance.Value.FixedUpdate());
                    fixedUpdateTimers.Add(timer);
                }
            }

            #endregion

            Console.ReadLine();

            #region Time Dispose

            timeDisposer(updateTimers.ToArray());
            timeDisposer(fixedUpdateTimers.ToArray());
            void timeDisposer(Timer[] timers)
            {
                foreach (var tt in timers)
                {
                    tt.Enabled = false;
                    tt.Dispose();
                }
            }

            #endregion
        }
    }

    class TimePrint
    {
        public static string Current => DateTime.Now.ToString("HH:mm:ss");
    }

    public class Debug
    {
        public static void Log(string value) => Console.WriteLine($"[{TimePrint.Current}] {value}");
    }
}
