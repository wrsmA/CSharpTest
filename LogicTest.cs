using System;
using System.Threading.Tasks;
using System.Threading;
using UnityCycle;

namespace LogicTest
{
    public class LogicA : UniCycle
    {
        public override void Update()
        {
            // 毎フレーム実行されます。
            Debug.Log("Update");
        }

        public override void LateUpdate()
        {
            Debug.Log("LateUpdate");
        }
    }
    public class LogicB : UniCycle
    {
        public override void Update()
        {
            // 毎フレーム実行されます。
            Debug.Log("Update");
        }

        public override void LateUpdate()
        {
            Debug.Log("LateUpdate");
        }
    }
}