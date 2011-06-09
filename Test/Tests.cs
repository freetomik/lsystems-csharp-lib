using System;
using System.Collections.Generic;

namespace Test
{
    class A {}
    class B {}
    class C {}
    class D { }
    class OK { }

    class Tester : LSystems.SystemDefinition
    {
        [LSystems.Production("a > c")]
        public object RightReplacementSkipEntireBranch(A a, C c)
        {
            return new OK();
        }

        public void TestRightReplacementSkipEntireBranch()
        {
            var rule = LSystems.ProductionRule.GenerateRule(
                this.GetType().GetMethod("RightReplacementSkipEntireBranch"));

            object result = null;
            int numReplaced = 0;

            if (rule.TryLeftToRight(
                new List<object>() { new A(), StartBranch, new B(), EndBranch, new C() },
                0,
                new List<object>(),
                this,
                ref result,
                ref numReplaced)
                &&
                result.GetType() == typeof(OK)
                && 
                numReplaced == 1)
            {
                Console.WriteLine("OK");
            }
            else
            {
                Console.WriteLine("Failed");
            }
        }

        [LSystems.Production("a > open b close c")]
        public object RightReplacementSkipRestOfBranch(
            A a, LSystems.StartBranchModule open, B b, LSystems.EndBranchModule close, C c)
        {
            return new OK();
        }

        public void TestRightReplacementSkipRestOfBranch()
        {
            var rule = LSystems.ProductionRule.GenerateRule(
                this.GetType().GetMethod("RightReplacementSkipRestOfBranch"));

            object result = null;
            int numReplaced = 0;

            if (rule.TryLeftToRight(
                new List<object>() { new A(), StartBranch, new B(), new D(), EndBranch, new C() },
                0,
                new List<object>(),
                this,
                ref result,
                ref numReplaced)
                &&
                result.GetType() == typeof(OK)
                &&
                numReplaced == 1)
            {
                Console.WriteLine("OK");
            }
            else
            {
                Console.WriteLine("Failed");
            }
        }

        [LSystems.Production("a > open0 close0 open1 b close1 d")]
        public object RightReplacementMultiBranches(
            A a,
            LSystems.StartBranchModule open0, LSystems.EndBranchModule close0, 
            LSystems.StartBranchModule open1, B b, LSystems.EndBranchModule close1, 
            D d)
        {
            return new OK();
        }

        public void TestRightReplacementMultiBranches()
        {
            var rule = LSystems.ProductionRule.GenerateRule(
                this.GetType().GetMethod("RightReplacementMultiBranches"));

            object result = null;
            int numReplaced = 0;

            if (rule.TryLeftToRight(
                new List<object>() { new A(), StartBranch, new C(), EndBranch, 
                    StartBranch, new B(), EndBranch, new D() },
                0,
                new List<object>(),
                this,
                ref result,
                ref numReplaced)
                &&
                result.GetType() == typeof(OK)
                &&
                numReplaced == 1)
            {
                Console.WriteLine("OK");
            }
            else
            {
                Console.WriteLine("Failed");
            }
        }
    }
}