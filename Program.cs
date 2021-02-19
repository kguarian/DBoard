using System;
namespace DBoard
{
    class Program
    {

        delegate bool Test(); //for runAllTests method

        public static void Main()
        {
            DBoard DecBoard = new DBoard();
            string retVal = "";
            while(retVal != "quit"){
                retVal = DecBoard.Run();
                Console.WriteLine($"{retVal}\n");
            }
        }

        public static bool runAllTests()
        {
            Test[] tests = { StorageTester };
            foreach (Test test in tests)
            {
                if (test.Invoke() == false)
                    return false;
            }
            return true;
        }
        public static bool StorageTester()
        {
            Event newEvent;
            Storage testStorage1;
            Storage testStorage2;
            string testPath = "testStorage";
            newEvent = new Event("testEvent", DecTime.Convert(DateTime.Now), "This is a test Event.\n Please work with the newline.");
            testStorage1 = new Storage();
            testStorage1.AddEvent("test", newEvent);
            testStorage1.Export(testPath);
            testStorage2 = Storage.Import(testPath);

            if (testStorage2.GetEvent("test").ToString() == "testEvent1/1/2021 1:12:32 PMThis is a test Event. Please work with the newline.")
                return true;
            else
                return false;
        }
    }
}