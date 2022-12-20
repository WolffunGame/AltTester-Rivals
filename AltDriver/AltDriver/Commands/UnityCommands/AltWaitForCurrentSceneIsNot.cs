using System;
using System.Threading;
using Altom.AltDriver.Logging;

namespace Altom.AltDriver.Commands
{
    public class AltWaitForCurrentSceneIsNot :  AltBaseCommand
    {
        private readonly NLog.Logger _logger = DriverLogManager.Instance.GetCurrentClassLogger();
        private readonly string[] _sceneNames;
        private readonly double _timeout;
        private readonly double _interval;
        
        public AltWaitForCurrentSceneIsNot(IDriverCommunication commHandler, double timeout, double interval, params string[] sceneNames) : base(commHandler)
        {
            this._sceneNames = sceneNames;
            this._timeout = timeout;
            this._interval = interval;
        }

        public void Execute()
        {
            var startTime = DateTime.Now;
            var endTime = startTime.AddSeconds(_timeout);
            var currentScene = "";
            while (DateTime.Now < endTime)
            {
                currentScene = new AltGetCurrentScene(CommHandler).Execute();
                if (Array.IndexOf(_sceneNames, currentScene) == -1)
                {
                    _logger.Info($"Current scene is {currentScene}, which is not in the list of {string.Join(", ", _sceneNames)}");
                    return;
                }
                
                _logger.Debug($"Current scene is {currentScene}. Waiting for scene to change.");
                Thread.Sleep((int) _interval * 1000);
            }
            throw new Exception($"Current scene is still {currentScene} after {_timeout} seconds");
        }
    }
}