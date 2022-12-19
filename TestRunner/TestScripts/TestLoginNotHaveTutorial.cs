using System;
using System.Threading.Tasks;
using Altom.AltDriver;
using Altom.AltDriver.Logging;
using NUnit.Framework;
using TestRunner.Services;

namespace Wolffun.Automation.Tests
{
    [Timeout(1000)]
    public class TestLoginNotHaveTutorial
    {
        private AltDriver altDriver;
        private readonly int _port = -1;

        [OneTimeSetUp]
        public void SetUp()
        {
            var port = _port == -1 ? PoolPort.GetAvailablePort() : _port;
            altDriver = new AltDriver(host: TestsHelper.GetAltDriverHost(),
                port: port,
                enableLogging: true);
            Console.WriteLine("AltDriver started on port: " + port);
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

            Console.WriteLine("Loading level");
        }

        [Test]
        public async Task Test1CheckCurrentSceneIsLogin()
        {
            Console.WriteLine("Test1CheckCurrentSceneIsLogin");
            await altDriver.WaitForCurrentSceneToBe(Constant.LoginScene);
            Assert.AreEqual(Constant.LoginScene, altDriver.GetCurrentScene());
        }

        [Test]
        public async Task Test2ClickLoginAsGuessButton()
        {
            await altDriver.WaitFor(3);
            var loginAsGuess = await altDriver.FindObject(By.PATH, "/LoginCanvas/btnContainer/btnLoginAsGuest");
            await loginAsGuess.Click();
            var acceptWarning =
                await altDriver.WaitForObject(By.PATH, "/LoginCanvas/PUPlayAsGuest_popup/Content/BtnAccept");
            await acceptWarning.Click();
        }

        [Test]
        public async Task Test3ClickOnPlayButton()
        {
            await altDriver.WaitForCurrentSceneToBe(Constant.HomeWithoutLobbyScene);
            await altDriver.WaitFor(5);
            var playButton = await altDriver.FindObject(By.PATH, "//home_menu(Clone)/objPlayAndReady/btnPlay");
            await playButton.Click();

            var cancelButton = await altDriver.WaitForObject(By.PATH,
                "/LayerContainer/Main_Container/home_menu(Clone)/btnCancelFindMatch", enabled: true, timeout: 2);
            Assert.NotNull(cancelButton);
        }

        [Test]
        public async Task Test4CurrentSceneIsNotHome()
        {
            await altDriver.WaitForObject(By.PATH, "//Control PC", timeout: 50);
            Assert.That(altDriver.GetCurrentScene(), Is.Not.EqualTo(Constant.HomeWithoutLobbyScene));
            Assert.That(altDriver.GetCurrentScene(), Is.Not.EqualTo(Constant.LoadingScene));
        }

        [Test]
        public async Task Test5LoseAnyGame()
        {
            var leaveButton = await altDriver.WaitForObject(By.PATH, "//BaseCanvas/BtnLeave", timeout: 300);
            await altDriver.WaitFor(5);
            await leaveButton.Click();
            await altDriver.WaitForCurrentSceneToBe(Constant.BattleEndRewardScene);
            Assert.AreEqual(Constant.BattleEndRewardScene, altDriver.GetCurrentScene());
        }

        [Test]
        public async Task Test6ClickOnContinueButton()
        {
            await altDriver.WaitForCurrentSceneToBe(Constant.BattleEndRewardScene);
            await altDriver.WaitFor(4);
            var continueButton = await altDriver.FindObject(By.PATH, "/Canvas/battle_reward/continue_button");
            await continueButton.Click();
            await altDriver.WaitForCurrentSceneToBe(Constant.HomeWithoutLobbyScene);
            Assert.AreEqual(Constant.HomeWithoutLobbyScene, altDriver.GetCurrentScene());
        }
    }
}