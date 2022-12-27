using System;
using Altom.AltDriver;
using Altom.AltDriver.Logging;
using NUnit.Framework;
using WolffunTester.TestRunner;

namespace Wolffun.Automation.Tests
{
    [TestFixture]
    public class TestLoginNotHaveTutorial
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private Altom.AltDriver.AltDriver altDriver;
        private readonly int _port = -1;


        [OneTimeSetUp]
        public void SetUp()
        {
            var info = PoolConnectionInfo.GetAvailableConnectionInfo();
            var port = _port == -1 ? info.Port : _port;
            var host = info.Address;
            if (port == -1 || string.IsNullOrEmpty(host))
                throw new Exception("No available connection info");
            
            altDriver = new Altom.AltDriver.AltDriver(host: host, port: port, enableLogging: true);
            Logger.Info("AltDriver started on port: " + port);
            DriverLogManager.SetMinLogLevel(AltLogger.Console, AltLogLevel.Info);
            //DriverLogManager.SetMinLogLevel(AltLogger.Unity, AltLogLevel.Info);
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            altDriver.Stop();
        }

        [SetUp]
        public void LoadLevel()
        {


            altDriver.ResetInput();

            altDriver.SetCommandResponseTimeout(60);

            Logger.Info("Loading level");
        }

        [Test]
        public void Test1CheckCurrentSceneIsLogin()
        {
            Logger.Info("Test1CheckCurrentSceneIsLogin");
            altDriver.WaitForCurrentSceneToBe(Constant.LoginScene);
            Assert.AreEqual(Constant.LoginScene, altDriver.GetCurrentScene());
        }

        [Test]
        public void Test2ClickLoginAsGuessButton()
        {
            altDriver.WaitFor(3);
            var loginAsGuess = altDriver.FindObject(By.PATH, "/LoginCanvas/btnContainer/btnLoginAsGuest");
            loginAsGuess.Click();
            var acceptWarning = altDriver.WaitForObject(By.PATH, "/LoginCanvas/PUPlayAsGuest_popup/Content/BtnAccept");
            acceptWarning.Click();
        }

        [Test]
        public void Test3ClickOnPlayButton()
        {
            altDriver.WaitForCurrentSceneToBe(Constant.HomeWithoutLobbyScene);
            altDriver.WaitFor(5);
            var playButton = altDriver.FindObject(By.PATH, "//home_menu(Clone)/objPlayAndReady/btnPlay");
            playButton.Click();

            var cancelButton = altDriver.WaitForObject(By.PATH,
                "/LayerContainer/Main_Container/home_menu(Clone)/btnCancelFindMatch", enabled: true, timeout: 2);
            Assert.NotNull(cancelButton);
        }

        [Test]
        public void Test4CurrentSceneIsNotHome()
        {
            altDriver.WaiForCurrentSceneIsNot(new[] {Constant.HomeWithoutLobbyScene, Constant.LoadingScene}, 50, 5);
            Assert.AreNotEqual(Constant.HomeWithoutLobbyScene, altDriver.GetCurrentScene());
            Assert.AreNotEqual(Constant.LoadingScene, altDriver.GetCurrentScene());
        }

        [Test]
        public void Test5LoseAnyGame()
        {
            Logger.Info("Test5LoseAnyGame");
            altDriver.WaiForCurrentSceneIsNot(new[] {Constant.HomeWithoutLobbyScene, Constant.LoadingScene});
            var leaveButton = altDriver.WaitForObject(By.PATH, "//BaseCanvas/BtnLeave", timeout: 300, interval: 5);
            altDriver.WaitFor(5);
            leaveButton.Click();
        }

        [Test]
        public void Test6CurrentSceneIsBattleEndReward()
        {
            altDriver.WaitForCurrentSceneToBe(Constant.BattleEndRewardScene);
            Assert.AreEqual(Constant.BattleEndRewardScene, altDriver.GetCurrentScene());
        }

        [Test]
        public void Test7ClickOnContinueButton()
        {
            //altDriver.WaitForCurrentSceneToBe(Constant.BattleEndRewardScene);
            altDriver.WaitFor(6);
            var continueButton = altDriver.FindObject(By.PATH, "/Canvas/battle_reward/continue_button");
            continueButton.Click();
            altDriver.WaitForCurrentSceneToBe(Constant.HomeWithoutLobbyScene);
            Assert.AreEqual(Constant.HomeWithoutLobbyScene, altDriver.GetCurrentScene());
        }
    }
}