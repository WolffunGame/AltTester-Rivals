using Altom.AltDriver;
using Altom.AltDriver.Logging;
using NUnit.Framework;

namespace Wolffun.Automation.Tests
{
    [Timeout(30000)]
    public class TestLoginNotHaveTutorial
    {
        private AltDriver altDriver;
        
        [OneTimeSetUp]
        public void SetUp()
        {
            altDriver = new AltDriver(host: TestsHelper.GetAltDriverHost(), port: TestsHelper.GetAltDriverPort(),
                enableLogging: true);
            DriverLogManager.SetMinLogLevel(AltLogger.Console, AltLogLevel.Info);
            DriverLogManager.SetMinLogLevel(AltLogger.Unity, AltLogLevel.Info);
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
           
        }
        
        [Test]
        public void Test1CheckCurrentSceneIsLogin()
        {
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

            var cancelButton = altDriver.WaitForObject(By.PATH, "/LayerContainer/Main_Container/home_menu(Clone)/btnCancelFindMatch", enabled: true, timeout: 2);
            Assert.NotNull(cancelButton);
        }
        
        [Test]
        public void Test4CurrentSceneIsNotHome()
        {
            altDriver.WaitForObject(By.PATH, "//Control PC", timeout: 50);
            Assert.That(altDriver.GetCurrentScene(), Is.Not.EqualTo(Constant.HomeWithoutLobbyScene));
            Assert.That(altDriver.GetCurrentScene(), Is.Not.EqualTo(Constant.LoadingScene));
        }

        [Test]
        public void Test5LoseAnyGame()
        {
            var leaveButton = altDriver.WaitForObject(By.PATH, "//BaseCanvas/BtnLeave", timeout: 300);
            altDriver.WaitFor(5);
            leaveButton.Click();
            altDriver.WaitForCurrentSceneToBe(Constant.BattleEndRewardScene);
            Assert.AreEqual(Constant.BattleEndRewardScene, altDriver.GetCurrentScene());
        }
        
        [Test]
        public void Test6ClickOnContinueButton()
        {
            altDriver.WaitForCurrentSceneToBe(Constant.BattleEndRewardScene);
            altDriver.WaitFor(4);
            var continueButton = altDriver.FindObject(By.PATH, "/Canvas/battle_reward/continue_button");
            continueButton.Click();
            altDriver.WaitForCurrentSceneToBe(Constant.HomeWithoutLobbyScene);
            Assert.AreEqual(Constant.HomeWithoutLobbyScene, altDriver.GetCurrentScene());
        }

    }
}