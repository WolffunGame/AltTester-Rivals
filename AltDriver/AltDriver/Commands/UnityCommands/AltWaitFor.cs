using System.Threading;
using Altom.AltDriver.Logging;

namespace Altom.AltDriver.Commands
{
    public class AltWaitFor : AltBaseCommand
    {
        readonly NLog.Logger logger = DriverLogManager.Instance.GetCurrentClassLogger();
        
        private double _timeout;
        
        public AltWaitFor(IDriverCommunication commandHandler, double timeout) : base(commandHandler)
        {
            _timeout = timeout;
        }
        
        public void Execute()
        {
            logger.Info("Waiting for " + _timeout + " seconds");
            double time = 0;
            while (time < _timeout)
            {
                Thread.Sleep(1000);
                time++;
            }
        }
    }
}