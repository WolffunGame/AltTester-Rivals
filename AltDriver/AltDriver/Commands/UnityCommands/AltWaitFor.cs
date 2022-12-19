using System;
using System.Threading;
using System.Threading.Tasks;
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
        
        public async Task Execute()
        {
            logger.Info("Waiting for " + _timeout + " seconds");
            await Task.Delay(TimeSpan.FromSeconds(_timeout));
        }
    }
}