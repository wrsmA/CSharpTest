using C = LogicTest;
namespace LogicTest
{
    public class LogicA : LogicTestBase
    {
        public override void Run()
        {

        }
    }

    public class LogicB : LogicTestBase
    {
        public override bool IsRun => false;
        public override void Run()
        {

        }
    }
}
