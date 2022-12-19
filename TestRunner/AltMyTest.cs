using System;

namespace TestRunner
{
    [System.Serializable]
    public class AltMyTest
    {
        private bool _selected;
        private string _testName;
        private string _testAssembly;
        private int _status;
        private bool _isSuite;
        private string _type;
        private string _parentName;
        private int _testCaseCount;
        private bool _foldOut;
        private string _testResultMessage;
        private string _testStackTrace;
        private Double _testDuration;
        private string path;
        private int _testSelectedCount;

        public AltMyTest(bool selected, string testName, string testAssembly, int status, bool isSuite, string type, string parentName, int testCaseCount, bool foldOut, string testResultMessage, string testStackTrace, Double testDuration, string path, int testSelectedCount)
        {
            _selected = selected;
            _testName = testName;
            _testAssembly = testAssembly;
            _status = status;
            _isSuite = isSuite;
            _type = type;
            _parentName = parentName;
            _testCaseCount = testCaseCount;
            _foldOut = foldOut;
            _testResultMessage = testResultMessage;
            _testStackTrace = testStackTrace;
            _testDuration = testDuration;
            _testSelectedCount = testSelectedCount;
            this.path = path;
        }

        public bool Selected
        {
            get
            {
                return _selected;
            }

            set
            {
                _selected = value;
            }
        }

        public string TestName
        {
            get
            {
                return _testName;
            }

            set
            {
                _testName = value;
            }
        }

        public string TestAssembly
        {
            get
            {
                return _testAssembly;
            }

            set
            {
                _testAssembly = value;
            }
        }

        public int Status
        {
            get
            {
                return _status;
            }

            set
            {
                _status = value;
            }
        }

        public bool IsSuite
        {
            get
            {
                return _isSuite;
            }

            set
            {
                _isSuite = value;
            }
        }

        public string Type
        {
            get
            {
                return _type;
            }

            set
            {
                _type = value;
            }
        }

        public string ParentName
        {
            get
            {
                return _parentName;
            }

            set
            {
                _parentName = value;
            }
        }

        public int TestCaseCount
        {
            get
            {
                return _testCaseCount;
            }

            set
            {
                _testCaseCount = value;
            }
        }
        public int TestSelectedCount
        {
            get
            {
                return _testSelectedCount;
            }
            set
            {
                _testSelectedCount = value;
            }
        }

        public bool FoldOut
        {
            get
            {
                return _foldOut;
            }

            set
            {
                _foldOut = value;
            }
        }

        public string TestResultMessage
        {
            get
            {
                return _testResultMessage;
            }

            set
            {
                _testResultMessage = value;
            }
        }

        public string TestStackTrace
        {
            get
            {
                return _testStackTrace;
            }

            set
            {
                _testStackTrace = value;
            }
        }

        public Double TestDuration
        {
            get
            {
                return _testDuration;
            }

            set
            {
                _testDuration = value;
            }
        }

        public string Path
        {
            get
            {
                return path;
            }

            set
            {
                path = value;
            }
        }

        public override string ToString()
        {
            return $"testName: {_testName}, testAssembly: {_testAssembly}, status: {_status}, isSuite: {_isSuite}, type: {_type}, parentName: {_parentName}, testCaseCount: {_testCaseCount}, foldOut: {_foldOut}, testResultMessage: {_testResultMessage}, testStackTrace: {_testStackTrace}, testDuration: {_testDuration}, path: {path}, testSelectedCount: {_testSelectedCount}";
        }
    }
}