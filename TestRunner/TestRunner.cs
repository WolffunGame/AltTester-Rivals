using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;
using Altom.AltDriver;
using Newtonsoft.Json;
using NUnit.Framework.Api;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal.Filters;

namespace TestRunner
{
    public delegate void TestRunDelegate(string name);

    public class TestRunner
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public enum TestRunMode
        {
            RunAllTest,
            RunSelectedTest,
            RunFailedTest
        }

        //This are for progressBar when are run
        private float progress;
        private float total;
        private string testName;
        const string NUNIT_ASSEMBLY_NAME = "nunit.framework";
        
        private NUnitTestAssemblyRunner _testAssemblyRunner;
        //result
        public Action<string> OnTestRunFinished;

        public void RunTests(List<AltMyTest> myTests, TestRunMode testMode)
        {
            Logger.Info("Started running test");

            System.Reflection.Assembly[] assemblies = System.AppDomain.CurrentDomain.GetAssemblies();

            List<string> assemblyList = new List<string>();
            var filters = AddTestToBeRun(myTests, testMode, out assemblyList);
            NUnit.Framework.Interfaces.ITestListener listener = new AltTestRunListener(ShowProgressBar);
            _testAssemblyRunner =
                new NUnit.Framework.Api.NUnitTestAssemblyRunner(new NUnit.Framework.Api.DefaultTestAssemblyBuilder());
            progress = 0;
            total = filters.Filters.Count;
            //log all tests
            Logger.Info($"Tests to be run: ${string.Join(",", myTests.Select(x => x.TestName))}");

            //create a thread from thread pool to run tests
            
            TNode xmlContent = new TNode("Test");
            foreach (var assembly in assemblies)
            {
                if (!assemblyList.Contains(assembly.GetName().Name))
                    continue;

                _testAssemblyRunner.Load(assembly, new Dictionary<string, object>());
                //run test thread
                var runTestThread = new System.Threading.Thread(() =>
                {
                    var result = _testAssemblyRunner.Run(listener, filters);
                    result.AddToXml(xmlContent, true);
                    setTestStatus(result);
                });

                runTestThread.Start();
                //log progress
                float previousProgress = progress - 1;
                while (runTestThread.IsAlive)
                {
                    if (previousProgress == progress) continue;
                    Logger.Info("Running test: " + testName + " " + progress + "/" + total);
                    previousProgress = progress;
                }

                Logger.Info("Test Thread Alive: " + runTestThread.IsAlive);
                runTestThread.Join();
            }

            CreateXmlReport($"test-report.xml", xmlContent);
            
            OnTestRunFinished?.Invoke($"{progress}/{total}");
        }

        public async Task RunTests(List<AltMyTest> myTests, TestRunMode testMode, int numberOfRun)
        {
            Task runTask = new Task(() =>
            {
                RunTests(myTests, testMode);
            });
            //run task sequencelly for number of run
            for (int i = 0; i < numberOfRun; i++)
            {
                runTask.Start();
                await runTask;
            }
        }

        public void StopTests()
        {
            Logger.Info("Stop running test");
            _testAssemblyRunner.StopRun(true);
            
            OnTestRunFinished?.Invoke($"Finished at {progress}/{total}");
        }


        private void ShowProgressBar(string name)
        {
            progress++;
            testName = name;
        }

        private void SetTestStatus(List<NUnit.Framework.Interfaces.ITestResult> results, List<AltMyTest> tests)
        {
            bool passed = true;
            int numberOfTestPassed = 0;
            int numberOfTestFailed = 0;
            double totalTime = 0;
            foreach (var test in tests)
            {
                int counter = 0;
                // int testPassed = 0;
                int testPassedCounter = 0;
                int testFailedCounter = 0;
                foreach (var result in results)
                {
                    switch (test.Type.ToString())
                    {
                        case "NUnit.Framework.Internal.TestMethod":
                            var enumerator = result.Children.GetEnumerator();
                            enumerator.MoveNext();
                            if (enumerator.Current != null)
                            {
                                var enumerator2 = enumerator.Current.Children.GetEnumerator();
                                enumerator2.MoveNext();
                                if (enumerator2.Current != null && enumerator2.Current.FullName.Equals(test.TestName))
                                {
                                    if (enumerator2.Current.FailCount > 0)
                                    {
                                        test.Status = -1;
                                        test.TestResultMessage = enumerator2.Current.Message + " \n\n\n StackTrace:  " +
                                                                 enumerator2.Current.StackTrace;
                                        passed = false;
                                        numberOfTestFailed++;
                                    }
                                    else if (enumerator2.Current.PassCount > 0)
                                    {
                                        test.Status = 1;
                                        test.TestResultMessage = "Passed in " + enumerator2.Current.Duration;
                                        numberOfTestPassed++;
                                    }

                                    totalTime += (enumerator2.Current.EndTime - enumerator2.Current.StartTime)
                                        .TotalSeconds;
                                }

                                enumerator2.Dispose();
                            }

                            enumerator.Dispose();
                            break;
                        case "NUnit.Framework.Internal.TestFixture":
                            enumerator = result.Children.GetEnumerator();
                            enumerator.MoveNext();
                            if (enumerator.Current != null && enumerator.Current.FullName.Equals(test.TestName))
                            {
                                counter++;
                                var enumerator2 = enumerator.Current.Children.GetEnumerator();
                                enumerator2.MoveNext();
                                if (enumerator2.Current != null && enumerator2.Current.FailCount > 0)
                                {
                                    testFailedCounter++;
                                }
                                else if (enumerator2.Current != null && enumerator2.Current.PassCount > 0)
                                {
                                    testPassedCounter++;
                                }

                                enumerator2.Dispose();
                            }

                            enumerator.Dispose();
                            break;
                        case "NUnit.Framework.Internal.TestAssembly":
                            counter++;
                            enumerator = result.Children.GetEnumerator();
                            enumerator.MoveNext();
                            if (enumerator.Current != null)
                            {
                                var enumerator2 = enumerator.Current.Children.GetEnumerator();
                                enumerator2.MoveNext();
                                if (enumerator2.Current != null && enumerator2.Current.FailCount > 0)
                                {
                                    testFailedCounter++;
                                }
                                else if (enumerator2.Current != null && enumerator2.Current.PassCount > 0)
                                {
                                    testPassedCounter++;
                                }

                                enumerator2.Dispose();
                            }

                            enumerator.Dispose();
                            break;
                    }
                }

                if (test.Type.Equals("NUnit.Framework.Internal.TestMethod"))
                {
                    if (test.TestCaseCount == counter)
                    {
                        if (testFailedCounter == 0 && testPassedCounter == counter)
                        {
                            test.Status = 1;
                            test.TestResultMessage = "All method passed ";
                        }
                        else
                        {
                            test.Status = -1;
                            passed = false;
                            test.TestResultMessage = "There are methods that failed";
                        }
                    }
                }
            }

            var serializeTests = JsonConvert.SerializeObject(tests, Newtonsoft.Json.Formatting.Indented,
                new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });
            Logger.Debug("Serialized tests: " + serializeTests);
            // UnityEditor.EditorPrefs.SetString("tests", serializeTests);
            //
            // AltTesterEditorWindow.ReportTestPassed = numberOfTestPassed;
            // AltTesterEditorWindow.ReportTestFailed = numberOfTestFailed;
            // AltTesterEditorWindow.IsTestRunResultAvailable = true;
            // AltTesterEditorWindow.SelectedTest = -1;
            // AltTesterEditorWindow.TimeTestRan = totalTime;
            if (passed)
            {
                Logger.Debug("All test passed");
            }
            else
            {
                Logger.Debug("Test failed");
            }
        }

        private static NUnit.Framework.Internal.Filters.OrFilter AddTestToBeRun(List<AltMyTest> tests,
            TestRunMode testMode,
            out List<string> assemblyList)
        {
            var filter = new NUnit.Framework.Internal.Filters.OrFilter();
            assemblyList = new List<string>();
            switch (testMode)
            {
                case TestRunMode.RunAllTest:
                    foreach (var test in tests)
                        if (!test.IsSuite)
                        {
                            filter.Add(new NUnit.Framework.Internal.Filters.FullNameFilter(test.TestName));
                            if (!assemblyList.Contains(test.TestAssembly))
                                assemblyList.Add(test.TestAssembly);
                        }

                    break;
                case TestRunMode.RunSelectedTest:
                    foreach (var test in tests)
                        if (test.Selected && !test.IsSuite)
                        {
                            filter.Add(new NUnit.Framework.Internal.Filters.FullNameFilter(test.TestName));
                            if (!assemblyList.Contains(test.TestAssembly))
                                assemblyList.Add(test.TestAssembly);
                        }

                    break;
                case TestRunMode.RunFailedTest:
                    foreach (var test in tests)
                        if (test.Status == -1 && !test.IsSuite)
                        {
                            filter.Add(new NUnit.Framework.Internal.Filters.FullNameFilter(test.TestName));
                            if (!assemblyList.Contains(test.TestAssembly))
                                assemblyList.Add(test.TestAssembly);
                        }

                    break;
            }

            return filter;
        }

        private static int setTestStatus(NUnit.Framework.Interfaces.ITestResult test)
        {
            if (!test.Test.IsSuite)
            {
                var status = 0;
                if (test.PassCount == 1)
                {
                    status = 1;
                    //AltTesterEditorWindow.ReportTestPassed++;
                }
                else if (test.FailCount == 1)
                {
                    status = -1;
                    //AltTesterEditorWindow.ReportTestFailed++;
                }

                // AltTesterEditorWindow.TimeTestRan += test.Duration;
                // int index = AltTesterEditorWindow.EditorConfiguration.MyTests.FindIndex(a =>
                //     a.TestName.Equals(test.Test.FullName));
                // AltTesterEditorWindow.EditorConfiguration.MyTests[index].Status = status;
                // AltTesterEditorWindow.EditorConfiguration.MyTests[index].TestDuration = test.Duration;
                // AltTesterEditorWindow.EditorConfiguration.MyTests[index].TestStackTrace = test.StackTrace;
                // AltTesterEditorWindow.EditorConfiguration.MyTests[index].TestResultMessage = test.Message;
                return status;
            }

            var failCount = 0;
            var notExecutedCount = 0;
            var passCount = 0;
            foreach (var testChild in test.Children)
            {
                var status = setTestStatus(testChild);
                if (status == 0)
                    notExecutedCount++;
                else if (status == -1)
                {
                    failCount++;
                }
                else
                {
                    passCount++;
                }
            }

            if (test.Test.TestCaseCount != passCount + failCount + notExecutedCount)
            {
                // AltTesterEditorWindow.EditorConfiguration.MyTests[
                //         AltTesterEditorWindow.EditorConfiguration.MyTests.FindIndex(a =>
                //             a.TestName.Equals(test.Test.FullName))].Status = 0;
                return 0;
            }

            if (failCount > 0)
            {
                // AltTesterEditorWindow.EditorConfiguration
                //     .MyTests[
                //         AltTesterEditorWindow.EditorConfiguration.MyTests.FindIndex(a =>
                //             a.TestName.Equals(test.Test.FullName))].Status = -1;
                return -1;
            }

            // AltTesterEditorWindow.EditorConfiguration
            //     .MyTests[
            //         AltTesterEditorWindow.EditorConfiguration.MyTests.FindIndex(a =>
            //             a.TestName.Equals(test.Test.FullName))].Status = 1;
            return 1;
        }

        public static IEnumerator SetUpListTestCoroutine()
        {
            var myTests = new List<AltMyTest>();
            System.Reflection.Assembly[] assemblies = System.AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies)
            {
                /*
                 * Skips test runner assemblies and assemblies that do not contain references to test assemblies
                 */

                bool isNunitTestAssembly = assembly.GetReferencedAssemblies().FirstOrDefault(
                    reference => reference.Name.Contains(NUNIT_ASSEMBLY_NAME)) != null;
                if (!isNunitTestAssembly)
                    continue;

                var testSuite =
                    (NUnit.Framework.Internal.TestSuite) new NUnit.Framework.Api.DefaultTestAssemblyBuilder().Build(
                        assembly, new Dictionary<string, object>());
                var coroutine = AddTestSuiteToMyTestCoroutine(testSuite, myTests, assembly.GetName().Name);
                yield return coroutine;
            }

            SetCorrectCheck(myTests);
            // AltTesterEditorWindow.EditorConfiguration.MyTests = myTests;
            // AltTesterEditorWindow.loadTestCompleted = true;
            // AltTesterEditorWindow.Window.Repaint();
        }

        public static List<AltMyTest> SetUpListTest()
        {
            var myTests = new List<AltMyTest>();
            System.Reflection.Assembly[] assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                bool isNunitTestAssembly = assembly.GetReferencedAssemblies().FirstOrDefault(
                    reference => reference.Name.Contains(NUNIT_ASSEMBLY_NAME)) != null;
                if (!isNunitTestAssembly)
                    continue;

                var testSuite =
                    (NUnit.Framework.Internal.TestSuite) new NUnit.Framework.Api.DefaultTestAssemblyBuilder().Build(
                        assembly, new Dictionary<string, object>());
                AddTestSuiteToMyTest(testSuite, myTests, assembly.GetName().Name);
            }

            SetCorrectCheck(myTests);
            return myTests;
        }


        private static void SetCorrectCheck(List<AltMyTest> myTests)
        {
            bool classCheck = true;
            bool assemblyCheck = true;
            for (int i = myTests.Count - 1; i >= 0; i--)
            {
                AltMyTest test = myTests[i];
                switch (test.Type.ToString())
                {
                    case "NUnit.Framework.Internal.TestMethod":
                        if (!test.Selected) //test not selected then the class which the test belong must be not selected
                        {
                            classCheck = false;
                        }
                        else
                        {
                            // var parentTest =
                            //     AltTesterEditorWindow.EditorConfiguration.MyTests.FirstOrDefault(a =>
                            //         a.TestName.Equals(test.ParentName));
                            // parentTest.TestSelectedCount++;
                        }

                        break;
                    case "NUnit.Framework.Internal.TestFixture":
                        if (classCheck)
                        {
                            test.Selected = true;
                        }
                        else
                        {
                            test.Selected = false;
                            assemblyCheck =
                                false; //class not selected then the assembly which the test belong must be not selected
                        }

                        classCheck = true; //Reset value for new class
                        break;
                    case "NUnit.Framework.Internal.TestAssembly":
                        if (assemblyCheck)
                        {
                            test.Selected = true;
                        }
                        else
                        {
                            test.Selected = false;
                        }

                        assemblyCheck = true; //Reset value for new assembly
                        break;
                }
            }
        }

        private static IEnumerator AddTestSuiteToMyTestCoroutine(NUnit.Framework.Interfaces.ITest testSuite,
            List<AltMyTest> newMyTests, string assembly)
        {
            addCurrentSuiteToTestList(testSuite, newMyTests, assembly);
            foreach (var test in testSuite.Tests)
            {
                var coroutine = AddTestSuiteToMyTestCoroutine(test, newMyTests, assembly);
                yield return coroutine;
            }
        }

        private static void AddTestSuiteToMyTest(NUnit.Framework.Interfaces.ITest testSuite, List<AltMyTest> newMyTests,
            string assembly)
        {
            addCurrentSuiteToTestList(testSuite, newMyTests, assembly);
            foreach (var test in testSuite.Tests)
            {
                AddTestSuiteToMyTest(test, newMyTests, assembly);
            }
        }

        private static void addCurrentSuiteToTestList(ITest testSuite, List<AltMyTest> newMyTests, string assembly)
        {
            string path = null;

            if (testSuite.GetType() == typeof(NUnit.Framework.Internal.TestMethod))
            {
                string fullName = testSuite.FullName;
                int indexOfParenthesis = fullName.IndexOf("(");
                if (indexOfParenthesis > -1)
                    fullName = testSuite.FullName.Substring(0, indexOfParenthesis);
                var hierarchyNames = fullName.Split('.');
                var className = hierarchyNames[hierarchyNames.Length - 2];
                // var assets = UnityEditor.AssetDatabase.FindAssets(className);
                // if (assets.Length != 0)
                // {
                //     path = UnityEditor.AssetDatabase.GUIDToAssetPath(assets[0]);
                // }
            }

            var parentName = string.Empty;
            if (testSuite.Parent != null)
                parentName = testSuite.Parent.FullName;

            newMyTests.Add(new AltMyTest(false, testSuite.FullName, assembly, 0, testSuite.IsSuite,
                testSuite.GetType().ToString(),
                parentName, testSuite.TestCaseCount, false, null, null, 0, path, 0));
        }

        public static void RunTestFromCommandLine()
        {
            var arguments = System.Environment.GetCommandLineArgs();

            int failed = 0;
            bool runAllTests = true;
            bool createReport = false;
            string reportPath = "";
            List<string> assemblyList = new List<string>();
            var classToTest = new List<string>();
            var tests = new List<string>();
            var assemblyToTest = new List<string>();
            var filter = new NUnit.Framework.Internal.Filters.OrFilter();
            TNode xmlContent = new TNode("Test");
            var testAssemblyRunner =
                new NUnit.Framework.Api.NUnitTestAssemblyRunner(new NUnit.Framework.Api.DefaultTestAssemblyBuilder());
            NUnit.Framework.Interfaces.ITestListener listener = new AltTestRunListener(null);
            System.Reflection.Assembly[] assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
            var allTestsFoundInProject = SetUpTestsForCommandLineRun();

            CheckCommandLineArguments(arguments, ref runAllTests, ref createReport, ref reportPath, ref classToTest,
                ref tests, ref assemblyToTest);
            AddTestsToFilter(runAllTests, filter, assemblies, assemblyList, classToTest, tests, assemblyToTest,
                allTestsFoundInProject);


            foreach (var assembly in assemblies)
            {
                if (!assemblyList.Contains(assembly.GetName().Name))
                    continue;
                testAssemblyRunner.Load(assembly, new Dictionary<string, object>());
                var result = testAssemblyRunner.Run(listener, filter);
                failed += result.FailCount;
                result.AddToXml(xmlContent, true);
            }

            if (createReport)
                CreateXmlReport(reportPath, xmlContent);
        }

        private static void AddTestsToFilter(bool runAllTests, OrFilter filter, Assembly[] assemblies,
            List<string> assemblyList, List<string> classToTest, List<string> tests, List<string> assemblyToTest,
            List<AltMyTest> myTests)
        {
            if (runAllTests)
                AddAllTestsToFilter(filter, assemblyList, myTests);
            else
            {
                AddClassTestsToFilter(filter, assemblyList, classToTest, myTests);
                AddTestNamesToFilter(filter, assemblyList, tests, myTests);
                var allAssemblies = assemblies.ToList();
                AddAssemblyTestsToFilter(filter, assemblyList, assemblyToTest, myTests, allAssemblies);
            }
        }

        private static void AddTestNamesToFilter(OrFilter filter, List<string> assemblyList, List<string> Tests,
            List<AltMyTest> tests)
        {
            foreach (var testName in Tests)
            {
                var classIndex = tests.FindIndex(test => test.TestName.Equals(testName));
                if (classIndex == -1)
                    throw new System.Exception("Test name: " + testName + " not found");
                addTestToRun(filter, assemblyList, tests[classIndex]);
            }
        }


        private static void AddAssemblyTestsToFilter(OrFilter filter, List<string> assemblyList,
            List<string> assemblyToTest, List<AltMyTest> tests, List<Assembly> allAssemblies)
        {
            foreach (var assembly in assemblyToTest)
            {
                if (!allAssemblies.Exists(a => a.GetName().Name == assembly))
                    throw new System.Exception("Assembly: " + assembly + " not found");
            }

            if (assemblyToTest.Count != 0)
                foreach (var test in tests)
                    if (assemblyToTest.Contains(test.TestAssembly))
                        addTestToRun(filter, assemblyList, test);
        }

        private static void AddClassTestsToFilter(OrFilter filter, List<string> assemblyList, List<string> ClassToTest,
            List<AltMyTest> tests)
        {
            foreach (var className in ClassToTest)
            {
                var classIndex = tests.FindIndex(test => test.TestName.Equals(className));
                if (classIndex == -1)
                    throw new System.Exception("Class name: " + className + " not found");

                var classFoundInList = tests[classIndex];
                for (int i = 0; i < classFoundInList.TestCaseCount; i++)
                {
                    var index = i + classIndex + 1;
                    addTestToRun(filter, assemblyList, tests[index]);
                }
            }
        }

        private static void AddAllTestsToFilter(OrFilter filter, List<string> assemblyList, List<AltMyTest> tests)
        {
            foreach (var test in tests)
                if (!test.IsSuite)
                    addTestToRun(filter, assemblyList, test);
        }

        private static void addTestToRun(OrFilter filter, List<string> assemblyList, AltMyTest test)
        {
            filter.Add(new NUnit.Framework.Internal.Filters.FullNameFilter(test.TestName));
            if (!assemblyList.Contains(test.TestAssembly))
                assemblyList.Add(test.TestAssembly);
        }

        private static void CheckCommandLineArguments(string[] arguments, ref bool runAllTests, ref bool createReport,
            ref string reportPath, ref List<string> classToTest, ref List<string> tests,
            ref List<string> assemblyToTest)
        {
            for (int i = 0; i < arguments.Length; i++)
            {
                switch (arguments[i])
                {
                    case "-reportPath":
                        if (i == arguments.Length - 1 || arguments[i + 1].StartsWith("-") ||
                            !arguments[i + 1].EndsWith(".xml"))
                            throw new InvalidPathException(
                                "Invalid path for report, please add a valid path after -reportPath that ends with .xml");
                        createReport = true;
                        reportPath = arguments[i + 1];
                        break;
                    case "-testsClass":
                        runAllTests = false;
                        AddArgumentsToList(arguments, classToTest, i);
                        break;
                    case "-tests":
                        runAllTests = false;
                        AddArgumentsToList(arguments, tests, i);
                        break;
                    case "-testsAssembly":
                        runAllTests = false;
                        AddArgumentsToList(arguments, assemblyToTest, i);
                        break;
                }
            }
        }

        private static void AddArgumentsToList(string[] arguments, List<string> testsList, int i)
        {
            int j = i + 1;
            while (j < arguments.Length - 1 && !arguments[j].StartsWith("-"))
            {
                testsList.Add(arguments[j]);
                j++;
            }
        }

        private static void CreateXmlReport(string reportPath, TNode xmlContent)
        {
            XmlWriter xmlWriter = XmlWriter.Create(reportPath);
            xmlContent.WriteTo(xmlWriter);
            xmlWriter.Flush();
            xmlWriter.Close();
        }

        private static List<AltMyTest> SetUpTestsForCommandLineRun()
        {
            return SetUpListTest();
        }

        private static List<AltMyTest> GetTestsByParent(string parentName)
        {
            var tests = SetUpListTest();
            var parent = tests.Find(test => test.TestName == parentName);
            var parentIndex = tests.IndexOf(parent);
            var children = new List<AltMyTest>();
            for (int i = 0; i < parent.TestCaseCount; i++)
            {
                children.Add(tests[parentIndex + i + 1]);
            }

            return children;
        }

        public void RunTestByParent(string parentName)
        {
            var tests = GetTestsByParent(parentName);
            RunTests(tests, TestRunMode.RunAllTest);
        }
    }
}