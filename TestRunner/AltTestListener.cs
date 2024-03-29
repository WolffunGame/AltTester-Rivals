//using Altom.AltTesterEditor.Logging;


namespace TestRunner
{
    public class AltTestRunListener : NUnit.Framework.Interfaces.ITestListener
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly TestRunDelegate callRunDelegate;

        public AltTestRunListener(TestRunDelegate callRunDelegate)
        {
            this.callRunDelegate = callRunDelegate;
        }

        public void TestStarted(NUnit.Framework.Interfaces.ITest test)
        {
            if (!test.IsSuite)
            {
                if (callRunDelegate != null)
                    callRunDelegate(test.Name);
            }
        }

        public void TestFinished(NUnit.Framework.Interfaces.ITestResult result)
        {
            if (!result.Test.IsSuite)
            {
                Logger.Info("==============> TEST " + result.Test.FullName + ": " +
                            result.ResultState.ToString().ToUpper());
                if (result.ResultState != NUnit.Framework.Interfaces.ResultState.Success)
                {
                    Logger.Error(result.Message + System.Environment.NewLine + result.StackTrace);
                }

                Logger.Info("======================================================");
            }
        }

        public void TestOutput(NUnit.Framework.Interfaces.TestOutput output)
        {
        }
    }
}