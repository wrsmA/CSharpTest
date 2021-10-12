using System;
using System.Threading.Tasks;
using System.Threading;
using UnityCycle;

namespace LogicTest
{
    public class LogicA : UniCycle
    {
        int count = 0;
        public override void Awake()
        {
            // デバッグ開始時に一度だけ実行されます。
        }
        public override void Start()
        {
            // Updateが実行される最初の一回に実行されます。
        }

        public override void Update()
        {
            // 毎フレーム実行されます。
        }
        public override void FixedUpdate()
        {
            // 毎秒実行されます。
        }
    }
}