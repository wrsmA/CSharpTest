using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System.Threading;
using System.Timers;
using Timer = System.Timers.Timer;

namespace UnityCycle
{
    /// <summary>
    /// Esc = プログラム　一時停止 / 再開
    /// Tab = プログラム　正常終了
    /// </summary>
    public class UniCycle
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
        /// 
        public virtual void LateUpdate() => noneExecutable();
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
                    if (IsOverrideMethod(_uInstance.Key, "Awake"))
                    {
                        if (_uInstance.Value.doLog) Debug.Log($"{_uInstance.Key.Name} -> Awake Executed...");
                        tasks.Add(Task.Run(() => _uInstance.Value.Awake()));
                    }
                }
                Task.WhenAll(tasks);
            }

            #endregion
            
            // Awake終了後とStart開始時までのタイムラグ
            // Awake内に初期化処理が行われた時の為の措置
            Task.Delay(1000).Wait();

            StartOrder();
            #region Start

            void StartOrder()
            {
                foreach(var _uInstance in UniInstance)
                {
                    if(IsOverrideMethod(_uInstance.Key, "Start"))
                    {
                        if (_uInstance.Value.doLog) Debug.Log($"{_uInstance.Key.Name} -> Start Executed...");
                        _ = Task.Run(() => _uInstance.Value.Start());
                    }
                }
            }

            #endregion

            FixedUpdateOrder();
            #region FixedUpdate

            void FixedUpdateOrder(){
                foreach (var _uInstance in UniInstance)
                {
                    if (IsOverrideMethod(_uInstance.Key, "FixedUpdate"))
                    {
                        if (_uInstance.Value.doLog) Debug.Log($"{_uInstance.Key.Name} -> FixedUpdate Execute Started...");
                        var timer = new Timer() {
                            Interval = 1000 * 0.02,
                            AutoReset = true,
                            Enabled = true
                        };
                        timer.Elapsed += new ElapsedEventHandler((sender, e) => _uInstance.Value.FixedUpdate());
                        fixedUpdateTimers.Add(timer);
                    }
                }
            }

            #endregion

            UpdatesOrder();
            #region Updates


            // Update とLateUpdateのイベント登録
            void UpdatesOrder()
            {
                //
                List<Action> UpdateDelegates = new List<Action>();
                List<Action> LateUpdateDelegates = new List<Action>();
                foreach(var _uInstance in UniInstance)
                {
                    if (IsOverrideMethod(_uInstance.Key, "Update"))
                        UpdateDelegates.Add(
                        () => _uInstance.Value.Update()
                       );
                    if (IsOverrideMethod(_uInstance.Key, "LateUpdate"))
                        LateUpdateDelegates.Add(
                        () => _uInstance.Value.LateUpdate()
                       );
                }
                var timer = new Timer()
                {
                    Interval = 1000/60, // Unityに合わせて60fpsにしてます。
                    AutoReset = true,
                    Enabled = true
                };

                //if (_uInstance.Value.doLog) Debug.Log($"{_uInstance.Key.Name} -> Update Execute Started...");
                var combined = (Action)Delegate.Combine((UpdateDelegates.Concat(LateUpdateDelegates)).ToArray());
                timer.Elapsed += new ElapsedEventHandler(
                    (sender, e) => { combined(); });
                updateTimers.Add(timer);

            }

            #endregion


            #region Commands

            while (true)
            {
                switch (Console.ReadKey().Key)
                {
                    // 一時停止
                    case ConsoleKey.Escape:
                        foreach (var tt in updateTimers)
                        {
                            if (tt.Enabled)
                                tt.Stop();
                            else
                                tt.Start();
                        }
                        break;
                    // 正常終了
                    case ConsoleKey.Tab:
                        BeforeTermination();
                        return;
                }
            }

            #endregion

            #region Time Dispose
            void BeforeTermination()
            {
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
            }

            #endregion

            bool IsOverrideMethod(Type type, string methodName)
            {
                var method = type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);
                if (method == null) return false;
                return method.DeclaringType != method.GetBaseDefinition().DeclaringType;
            }
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
