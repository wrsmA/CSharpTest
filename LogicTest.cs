using System;
using System.Threading.Tasks;
using System.Threading;
using UnityCycle;

namespace LogicTest
{
    public class LogicA : UniCycle
    {
        int count = 0;
        public override void Update()
        {
            Debug.Log(count.ToString());
            count++;
        }
    }
}