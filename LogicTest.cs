using System;
using System.Threading.Tasks;
using System.Threading;
using UnityCycle;

namespace LogicTest
{
    public class Sample : UniCycle
    {
        public override void Start()
        {
            // 初回フレームに1度だけ実行されます。
        }
        public override void Update()
        {
            // 毎フレーム実行されます。
        }
    }
}