using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Altom.AltDriver.Commands;
using Altom.AltDriver.Logging;
using Altom.AltDriver.Notifications;

namespace Altom.AltDriver
{
    public enum By
    {
        TAG,
        LAYER,
        NAME,
        COMPONENT,
        PATH,
        ID,
        TEXT
    }

    public class AltDriver
    {
        private static readonly NLog.Logger logger = DriverLogManager.Instance.GetCurrentClassLogger();
        private readonly IDriverCommunication communicationHandler;
        public static readonly string VERSION = "1.8.1";

        public IDriverCommunication CommunicationHandler
        {
            get { return communicationHandler; }
        }

        /// <summary>
        /// Initiates AltDriver and begins connection with the instrumented Unity application through to AltProxy
        /// </summary>
        /// <param name="host">The ip or hostname  AltProxy is listening on.</param>
        /// <param name="port">The port AltProxy is listening on.</param>
        /// <param name="enableLogging">If true it enables driver commands logging to log file and Unity.</param>
        /// <param name="connectTimeout">The connect timeout in seconds.</param>
        public AltDriver(string host = "127.0.0.1", int port = 13000, bool enableLogging = false,
            int connectTimeout = 60)
        {
#if UNITY_EDITOR || ALTTESTER
            var defaultLevels =
 new Dictionary<AltLogger, AltLogLevel> { { AltLogger.File, AltLogLevel.Debug }, { AltLogger.Unity, AltLogLevel.Debug } };
#else
            var defaultLevels = new Dictionary<AltLogger, AltLogLevel>
                {{AltLogger.File, AltLogLevel.Debug}, {AltLogger.Console, AltLogLevel.Debug}};
#endif

            DriverLogManager.SetupAltDriverLogging(defaultLevels);

            if (!enableLogging)
                DriverLogManager.StopLogging();

            communicationHandler = new DriverCommunicationWebSocket(host, port, connectTimeout);
            communicationHandler.Connect();

            checkServerVersion().Wait();
        }

        private void splitVersion(string version, out string major, out string minor)
        {
            var parts = version.Split(new[] {"."}, StringSplitOptions.None);
            major = parts[0];
            minor = parts.Length > 1 ? parts[1] : string.Empty;
        }

        private async Task checkServerVersion()
        {
            string serverVersion = await GetServerVersion();

            string majorServer;
            string majorDriver;
            string minorDriver;
            string minorServer;

            splitVersion(serverVersion, out majorServer, out minorServer);
            splitVersion(VERSION, out majorDriver, out minorDriver);

            if (majorServer != majorDriver || minorServer != minorDriver)
            {
                string message = "Version mismatch. AltDriver version is " + VERSION + ". AltTester version is " +
                                 serverVersion + ".";
                logger.Warn(message);
            }
        }

        public void Stop()
        {
            communicationHandler.Close();
        }

        public Task ResetInput()
        {
            return new AltResetInput(communicationHandler).Execute();
        }

        public void SetCommandResponseTimeout(int commandTimeout)
        {
            communicationHandler.SetCommandTimeout(commandTimeout);
        }

        public void SetDelayAfterCommand(float delay)
        {
            communicationHandler.SetDelayAfterCommand(delay);
        }

        public float GetDelayAfterCommand()
        {
            return communicationHandler.GetDelayAfterCommand();
        }

        public async Task<string> GetServerVersion()
        {
            string serverVersion = await new AltGetServerVersion(communicationHandler).Execute();
            await communicationHandler.SleepFor(communicationHandler.GetDelayAfterCommand());
            return serverVersion;
        }

        public void SetLogging(bool enableLogging)
        {
            if (enableLogging)
                DriverLogManager.ResumeLogging();
            else
                DriverLogManager.StopLogging();
        }

        public async Task LoadScene(string scene, bool loadSingle = true)
        {
            await new AltLoadScene(communicationHandler, scene, loadSingle).Execute();
            await communicationHandler.SleepFor(communicationHandler.GetDelayAfterCommand());
        }

        public async Task UnloadScene(string scene)
        {
            await new AltUnloadScene(communicationHandler, scene).Execute();
            await communicationHandler.SleepFor(communicationHandler.GetDelayAfterCommand());
        }

        public async Task<List<string>> GetAllLoadedScenes()
        {
            var sceneList = await new AltGetAllLoadedScenes(communicationHandler).Execute();
            await communicationHandler.SleepFor(communicationHandler.GetDelayAfterCommand());
            return sceneList;
        }

        public Task<List<AltObject>> FindObjects(By by, string value, By cameraBy = By.NAME, string cameraValue = "",
            bool enabled = true)
        {
            var listOfObjects = new AltFindObjects(communicationHandler, by, value, cameraBy, cameraValue, enabled)
                .Execute();
            communicationHandler.SleepFor(communicationHandler.GetDelayAfterCommand());
            return listOfObjects;
        }

        public Task<List<AltObject>> FindObjectsWhichContain(By by, string value, By cameraBy = By.NAME,
            string cameraValue = "", bool enabled = true)
        {
            var listOfObjects =
                new AltFindObjectsWhichContain(communicationHandler, by, value, cameraBy, cameraValue, enabled)
                    .Execute();
            communicationHandler.SleepFor(communicationHandler.GetDelayAfterCommand());
            return listOfObjects;
        }

        public Task<AltObject> FindObject(By by, string value, By cameraBy = By.NAME, string cameraValue = "",
            bool enabled = true)
        {
            var findObject =
                new AltFindObject(communicationHandler, by, value, cameraBy, cameraValue, enabled).Execute();
            communicationHandler.SleepFor(communicationHandler.GetDelayAfterCommand());
            return findObject;
        }

        public Task<AltObject> FindObjectWhichContains(By by, string value, By cameraBy = By.NAME,
            string cameraValue = "",
            bool enabled = true)
        {
            var findObject =
                new AltFindObjectWhichContains(communicationHandler, by, value, cameraBy, cameraValue, enabled)
                    .Execute();
            communicationHandler.SleepFor(communicationHandler.GetDelayAfterCommand());
            return findObject;
        }

        public async Task SetTimeScale(float timeScale)
        {
            await new AltSetTimeScale(communicationHandler, timeScale).Execute();
            await communicationHandler.SleepFor(communicationHandler.GetDelayAfterCommand());
        }

        public Task<float> GetTimeScale()
        {
            var timeScale = new AltGetTimeScale(communicationHandler).Execute();
            communicationHandler.SleepFor(communicationHandler.GetDelayAfterCommand());
            return timeScale;
        }

        public Task<T> CallStaticMethod<T>(string typeName, string methodName, string assemblyName,
            object[] parameters, string[] typeOfParameters = null)
        {
            var result = new AltCallStaticMethod<T>(communicationHandler, typeName, methodName, parameters,
                typeOfParameters, assemblyName).Execute();
            communicationHandler.SleepFor(communicationHandler.GetDelayAfterCommand());
            return result;
        }

        public Task<T> GetStaticProperty<T>(string componentName, string propertyName, string assemblyName,
            int maxDepth = 2)
        {
            var propertyValue =
                new AltGetStaticProperty<T>(communicationHandler, componentName, propertyName, assemblyName, maxDepth)
                    .Execute();
            communicationHandler.SleepFor(communicationHandler.GetDelayAfterCommand());
            return propertyValue;
        }

        public async Task SetStaticProperty(string componentName, string propertyName, string assemblyName,
            object updatedProperty)
        {
            await new AltSetStaticProperty(communicationHandler, componentName, propertyName, assemblyName,
                    updatedProperty)
                .Execute();
            await communicationHandler.SleepFor(communicationHandler.GetDelayAfterCommand());
        }

        public async Task DeletePlayerPref()
        {
            await new AltDeletePlayerPref(communicationHandler).Execute();
            await communicationHandler.SleepFor(communicationHandler.GetDelayAfterCommand());
        }

        public async Task DeleteKeyPlayerPref(string keyName)
        {
            await new AltDeleteKeyPlayerPref(communicationHandler, keyName).Execute();
            await communicationHandler.SleepFor(communicationHandler.GetDelayAfterCommand());
        }

        public async Task SetKeyPlayerPref(string keyName, int valueName)
        {
            await new AltSetKeyPLayerPref(communicationHandler, keyName, valueName).Execute();
            await communicationHandler.SleepFor(communicationHandler.GetDelayAfterCommand());
        }

        public async Task SetKeyPlayerPref(string keyName, float valueName)
        {
            await new AltSetKeyPLayerPref(communicationHandler, keyName, valueName).Execute();
            await communicationHandler.SleepFor(communicationHandler.GetDelayAfterCommand());
        }

        public async Task SetKeyPlayerPref(string keyName, string valueName)
        {
            await new AltSetKeyPLayerPref(communicationHandler, keyName, valueName).Execute();
            await communicationHandler.SleepFor(communicationHandler.GetDelayAfterCommand());
        }

        public Task<int> GetIntKeyPlayerPref(string keyName)
        {
            var keyValue = new AltGetIntKeyPlayerPref(communicationHandler, keyName).Execute();
            communicationHandler.SleepFor(communicationHandler.GetDelayAfterCommand());
            return keyValue;
        }

        public Task<float> GetFloatKeyPlayerPref(string keyName)
        {
            var keyValue = new AltGetFloatKeyPlayerPref(communicationHandler, keyName).Execute();
            communicationHandler.SleepFor(communicationHandler.GetDelayAfterCommand());
            return keyValue;
        }

        public Task<string> GetStringKeyPlayerPref(string keyName)
        {
            var keyValue = new AltGetStringKeyPlayerPref(communicationHandler, keyName).Execute();
            communicationHandler.SleepFor(communicationHandler.GetDelayAfterCommand());
            return keyValue;
        }

        public Task<string> GetCurrentScene()
        {
            var sceneName = new AltGetCurrentScene(communicationHandler).Execute();
            communicationHandler.SleepFor(communicationHandler.GetDelayAfterCommand());
            return sceneName;
        }

        /// <summary>
        /// Simulates a swipe action between two points.
        /// </summary>
        /// <param name="start">Coordinates of the screen where the swipe begins</param>
        /// <param name="end">Coordinates of the screen where the swipe ends</param>
        /// <param name="duration">The time measured in seconds to move the mouse from start to end location. Defaults to <c>0.1</c>.</param>
        /// <param name="wait">If set wait for command to finish. Defaults to <c>True</c>.</param>
        public async Task Swipe(AltVector2 start, AltVector2 end, float duration = 0.1f, bool wait = true)
        {
            await new AltSwipe(communicationHandler, start, end, duration, wait).Execute();
            await communicationHandler.SleepFor(communicationHandler.GetDelayAfterCommand());
        }

        /// <summary>
        /// Simulates a multipoint swipe action.
        /// </summary>
        /// <param name="positions">A list of positions on the screen where the swipe be made.</param>
        /// <param name="duration">The time measured in seconds to swipe from first position to the last position. Defaults to <code>0.1</code>.</param>
        /// <param name="wait">If set wait for command to finish. Defaults to <c>True</c>.</param>
        public async Task MultipointSwipe(AltVector2[] positions, float duration = 0.1f, bool wait = true)
        {
            await new AltMultipointSwipe(communicationHandler, positions, duration, wait).Execute();
            await communicationHandler.SleepFor(communicationHandler.GetDelayAfterCommand());
        }

        /// <summary>
        /// Simulates holding left click button down for a specified amount of time at given coordinates.
        /// </summary>
        /// <param name="coordinates">The coordinates where the button is held down.</param>
        /// <param name="duration">The time measured in seconds to keep the button down.</param>
        /// <param name="wait">If set wait for command to finish. Defaults to <c>True</c>.</param>
        public Task HoldButton(AltVector2 coordinates, float duration, bool wait = true)
        {
            return Swipe(coordinates, coordinates, duration, wait);
        }

        /// <summary>
        /// Simulates key press action in your game.
        /// </summary>
        /// <param name="keyCode">The key code of the key simulated to be pressed.</param>
        /// <param name="power" >A value between [-1,1] used for joysticks to indicate how hard the button was pressed. Defaults to <c>1</c>.</param>
        /// <param name="duration">The time measured in seconds from the key press to the key release. Defaults to <c>0.1</c></param>
        /// <param name="wait">If set wait for command to finish. Defaults to <c>True</c>.</param>
        public Task PressKey(AltKeyCode keyCode, float power = 1, float duration = 0.1f, bool wait = true)
        {
            AltKeyCode[] keyCodes = {keyCode};
            return PressKeys(keyCodes, power, duration, wait);
        }

        /// <summary>
        /// Simulates multiple keys pressed action in your game.
        /// </summary>
        /// <param name="keyCodes">The list of key codes of the keys simulated to be pressed.</param>
        /// <param name="power" >A value between [-1,1] used for joysticks to indicate how hard the button was pressed. Defaults to <c>1</c>.</param>
        /// <param name="duration">The time measured in seconds from the key press to the key release. Defaults to <c>0.1</c></param>
        /// <param name="wait">If set wait for command to finish. Defaults to <c>True</c>.</param>
        public async Task PressKeys(AltKeyCode[] keyCodes, float power = 1, float duration = 0.1f, bool wait = true)
        {
            await new AltPressKeys(communicationHandler, keyCodes, power, duration, wait).Execute();
            await communicationHandler.SleepFor(communicationHandler.GetDelayAfterCommand());
        }

        public Task KeyDown(AltKeyCode keyCode, float power = 1)
        {
            AltKeyCode[] keyCodes = {keyCode};
            return KeysDown(keyCodes, power);
        }

        /// <summary>
        /// Simulates multiple keys down action in your game.
        /// </summary>
        /// <param name="keyCodes">The key codes of the keys simulated to be down.</param>
        /// <param name="power" >A value between [-1,1] used for joysticks to indicate how hard the button was pressed. Defaults to <c>1</c>.</param>
        public async Task KeysDown(AltKeyCode[] keyCodes, float power = 1)
        {
            await new AltKeysDown(communicationHandler, keyCodes, power).Execute();
            await communicationHandler.SleepFor(communicationHandler.GetDelayAfterCommand());
        }

        public Task KeyUp(AltKeyCode keyCode)
        {
            AltKeyCode[] keyCodes = {keyCode};
            return KeysUp(keyCodes);
        }

        /// <summary>
        /// Simulates multiple keys up action in your game.
        /// </summary>
        /// <param name="keyCodes">The key codes of the keys simulated to be up.</param>
        public async Task KeysUp(AltKeyCode[] keyCodes)
        {
            await new AltKeysUp(communicationHandler, keyCodes).Execute();
            await communicationHandler.SleepFor(communicationHandler.GetDelayAfterCommand());
        }

        /// <summary>
        /// Simulate mouse movement in your game.
        /// </summary>
        /// <param name="coordinates">The screen coordinates</param>
        /// <param name="duration">The time measured in seconds to move the mouse from the current mouse position to the set coordinates. Defaults to <c>0.1f</c></param>
        /// <param name="wait">If set wait for command to finish. Defaults to <c>True</c>.</param>
        public async Task MoveMouse(AltVector2 coordinates, float duration = 0.1f, bool wait = true)
        {
            await new AltMoveMouse(communicationHandler, coordinates, duration, wait).Execute();
            await communicationHandler.SleepFor(communicationHandler.GetDelayAfterCommand());
        }

        /// <summary>
        /// Simulate scroll action in your game.
        /// </summary>
        /// <param name="speed">Set how fast to scroll. Positive values will scroll up and negative values will scroll down. Defaults to <code> 1 </code></param>
        /// <param name="duration">The duration of the scroll in seconds. Defaults to <code> 0.1 </code></param>
        /// <param name="wait">If set wait for command to finish. Defaults to <c>True</c>.</param>
        public async Task Scroll(float speed = 1, float duration = 0.1f, bool wait = true)
        {
            await new AltScroll(communicationHandler, speed, 0, duration, wait).Execute();
            await communicationHandler.SleepFor(communicationHandler.GetDelayAfterCommand());
        }

        /// <summary>
        /// Simulate scroll action in your game.
        /// </summary>
        /// <param name="scrollValue">Set how fast to scroll. X is horizontal and Y is vertical. Defaults to <code> 1 </code></param>
        /// <param name="duration">The duration of the scroll in seconds. Defaults to <code> 0.1 </code></param>
        /// <param name="wait">If set wait for command to finish. Defaults to <c>True</c>.</param>
        public async Task Scroll(AltVector2 scrollValue, float duration = 0.1f, bool wait = true)
        {
            await new AltScroll(communicationHandler, scrollValue.y, scrollValue.x, duration, wait).Execute();
            await communicationHandler.SleepFor(communicationHandler.GetDelayAfterCommand());
        }

        /// <summary>
        /// Tap at screen coordinates
        /// </summary>
        /// <param name="coordinates">The screen coordinates</param>
        /// <param name="count">Number of taps</param>
        /// <param name="interval">Interval between taps in seconds</param>
        /// <param name="wait">If set wait for command to finish. Defaults to <c>True</c>.</param>
        public async Task Tap(AltVector2 coordinates, int count = 1, float interval = 0.1f, bool wait = true)
        {
            await new AltTapCoordinates(communicationHandler, coordinates, count, interval, wait).Execute();
            await communicationHandler.SleepFor(communicationHandler.GetDelayAfterCommand());
        }

        /// <summary>
        /// Click at screen coordinates
        /// </summary>
        /// <param name="coordinates">The screen coordinates</param>
        /// <param name="count" >Number of clicks.</param>
        /// <param name="interval">Interval between clicks in seconds</param>
        /// <param name="wait">If set wait for command to finish. Defaults to <c>True</c>.</param>
        public async Task Click(AltVector2 coordinates, int count = 1, float interval = 0.1f, bool wait = true)
        {
            await new AltClickCoordinates(communicationHandler, coordinates, count, interval, wait).Execute();
            await communicationHandler.SleepFor(communicationHandler.GetDelayAfterCommand());
        }

        /// <summary>
        /// Simulates device rotation action in your game.
        /// </summary>
        /// <param name="acceleration">The linear acceleration of a device.</param>
        /// <param name="duration">How long the rotation will take in seconds. Defaults to <code>0.1<code>.</param>
        /// <param name="wait">If set wait for command to finish. Defaults to <c>True</c>.</param>
        public async Task Tilt(AltVector3 acceleration, float duration = 0.1f, bool wait = true)
        {
            await new AltTilt(communicationHandler, acceleration, duration, wait).Execute();
            await communicationHandler.SleepFor(communicationHandler.GetDelayAfterCommand());
        }

        public Task<List<AltObject>> GetAllElements(By cameraBy = By.NAME, string cameraValue = "", bool enabled = true)
        {
            var listOfObjects = new AltGetAllElements(communicationHandler, cameraBy, cameraValue, enabled).Execute();
            communicationHandler.SleepFor(communicationHandler.GetDelayAfterCommand());
            return listOfObjects;
        }

        public Task<List<AltObjectLight>> GetAllElementsLight(By cameraBy = By.NAME, string cameraValue = "",
            bool enabled = true)
        {
            var listOfObjects =
                new AltGetAllElementsLight(communicationHandler, cameraBy, cameraValue, enabled).Execute();
            communicationHandler.SleepFor(communicationHandler.GetDelayAfterCommand());
            return listOfObjects;
        }

        public async Task WaitForCurrentSceneToBe(string sceneName, double timeout = 10, double interval = 1)
        {
            await new AltWaitForCurrentSceneToBe(communicationHandler, sceneName, timeout, interval).Execute();
            await communicationHandler.SleepFor(communicationHandler.GetDelayAfterCommand());
        }

        public Task<AltObject> WaitForObject(By by, string value, By cameraBy = By.NAME, string cameraValue = "",
            bool enabled = true, double timeout = 20, double interval = 0.5)
        {
            var objectFound = new AltWaitForObject(communicationHandler, by, value, cameraBy, cameraValue, enabled,
                timeout, interval).Execute();
            communicationHandler.SleepFor(communicationHandler.GetDelayAfterCommand());
            return objectFound;
        }

        public async Task WaitForObjectNotBePresent(By by, string value, By cameraBy = By.NAME, string cameraValue = "",
            bool enabled = true, double timeout = 20, double interval = 0.5)
        {
            await new AltWaitForObjectNotBePresent(communicationHandler, by, value, cameraBy, cameraValue, enabled,
                timeout,
                interval).Execute();
            await communicationHandler.SleepFor(communicationHandler.GetDelayAfterCommand());
        }

        public Task<AltObject> WaitForObjectWhichContains(By by, string value, By cameraBy = By.NAME,
            string cameraValue = "",
            bool enabled = true, double timeout = 20, double interval = 0.5)
        {
            var objectFound = new AltWaitForObjectWhichContains(communicationHandler, by, value, cameraBy, cameraValue,
                enabled, timeout, interval).Execute();
            communicationHandler.SleepFor(communicationHandler.GetDelayAfterCommand());
            return objectFound;
        }

        public Task<List<string>> GetAllScenes()
        {
            var listOfScenes = new AltGetAllScenes(communicationHandler).Execute();
            communicationHandler.SleepFor(communicationHandler.GetDelayAfterCommand());
            return listOfScenes;
        }

        public Task<List<AltObject>> GetAllCameras()
        {
            var listOfCameras = new AltGetAllCameras(communicationHandler).Execute();
            communicationHandler.SleepFor(communicationHandler.GetDelayAfterCommand());
            return listOfCameras;
        }

        public Task<List<AltObject>> GetAllActiveCameras()
        {
            var listOfCameras = new AltGetAllActiveCameras(communicationHandler).Execute();
            communicationHandler.SleepFor(communicationHandler.GetDelayAfterCommand());
            return listOfCameras;
        }

        public Task<AltTextureInformation> GetScreenshot(AltVector2 size = default(AltVector2),
            int screenShotQuality = 100)
        {
            var textureInformation = new AltGetScreenshot(communicationHandler, size, screenShotQuality).Execute();
            communicationHandler.SleepFor(communicationHandler.GetDelayAfterCommand());
            return textureInformation;
        }

        public Task<AltTextureInformation> GetScreenshot(int id, AltColor color, float width,
            AltVector2 size = default(AltVector2), int screenShotQuality = 100)
        {
            var textureInformation =
                new AltGetHighlightObjectScreenshot(communicationHandler, id, color, width, size, screenShotQuality)
                    .Execute();
            communicationHandler.SleepFor(communicationHandler.GetDelayAfterCommand());
            return textureInformation;
        }

        // public AltTextureInformation GetScreenshot(AltVector2 coordinates, AltColor color, float width, out AltObject selectedObject, AltVector2 size = default(AltVector2), int screenShotQuality = 100)
        // {
        //     var textureInformation = new AltGetHighlightObjectFromCoordinatesScreenshot(communicationHandler, coordinates, color, width, size, screenShotQuality).Execute(out selectedObject);
        //     communicationHandler.SleepFor(communicationHandler.GetDelayAfterCommand());
        //     return textureInformation;
        // }

        public async Task GetPNGScreenshot(string path)
        {
            await new AltGetPNGScreenshot(communicationHandler, path).Execute();
            await communicationHandler.SleepFor(communicationHandler.GetDelayAfterCommand());
        }

        public Task<List<AltObjectLight>> GetAllLoadedScenesAndObjects(bool enabled = true)
        {
            var listOfObjects = new AltGetAllLoadedScenesAndObjects(communicationHandler, enabled).Execute();
            communicationHandler.SleepFor(communicationHandler.GetDelayAfterCommand());
            return listOfObjects;
        }

        public async Task SetServerLogging(AltLogger logger, AltLogLevel logLevel)
        {
            await new AltSetServerLogging(communicationHandler, logger, logLevel).Execute();
            await communicationHandler.SleepFor(communicationHandler.GetDelayAfterCommand());
        }

        public Task<int> BeginTouch(AltVector2 screenPosition)
        {
            var touchId = new AltBeginTouch(communicationHandler, screenPosition).Execute();
            communicationHandler.SleepFor(communicationHandler.GetDelayAfterCommand());
            return touchId;
        }

        public async Task MoveTouch(int fingerId, AltVector2 screenPosition)
        {
            await new AltMoveTouch(communicationHandler, fingerId, screenPosition).Execute();
            await communicationHandler.SleepFor(communicationHandler.GetDelayAfterCommand());
        }

        public async Task EndTouch(int fingerId)
        {
            await new AltEndTouch(communicationHandler, fingerId).Execute();
            await communicationHandler.SleepFor(communicationHandler.GetDelayAfterCommand());
        }

        /// <summary>
        /// Retrieves the Unity object at given coordinates.
        /// Uses EventSystem.RaycastAll to find object. If no object is found then it uses UnityEngine.Physics.Raycast and UnityEngine.Physics2D.Raycast and returns the one closer to the camera.
        /// </summary>
        /// <param name="coordinates">The screen coordinates</param>
        /// <returns>The UI object hit by event system Raycast, null otherwise</returns>
        public Task<AltObject> FindObjectAtCoordinates(AltVector2 coordinates)
        {
            var objectFound = new AltFindObjectAtCoordinates(communicationHandler, coordinates).Execute();
            communicationHandler.SleepFor(communicationHandler.GetDelayAfterCommand());
            return objectFound;
        }

        public Task AddNotificationListener<T>(NotificationType notificationType, Action<T> callback, bool overwrite)
        {
            return new AddNotificationListener<T>(communicationHandler, notificationType, callback, overwrite)
                .Execute();
        }

        public Task RemoveNotificationListener(NotificationType notificationType)
        {
            return new RemoveNotificationListener(communicationHandler, notificationType).Execute();
        }

        public async Task WaitFor(int seconds)
        {
            await new AltWaitFor(communicationHandler, seconds).Execute();
            await communicationHandler.SleepFor(communicationHandler.GetDelayAfterCommand());
        }
    }
}