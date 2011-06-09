using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.ComponentModel;

namespace Test
{
    class Program
    {        
        static void Main(string[] args)        
        {
            /*
            var tester = new Tester();
            tester.TestRightReplacementSkipEntireBranch();
            tester.TestRightReplacementSkipRestOfBranch();
            tester.TestRightReplacementMultiBranches();
            */

            AnabaenaDefinition.DoTest();

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }        
    }
}
