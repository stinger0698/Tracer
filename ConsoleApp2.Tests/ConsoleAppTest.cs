using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tracer;
using System.Threading;
using System.Collections.Generic;

namespace ConsoleApp2.Tests
{
    [TestClass]
    public class ConsoleAppTest
    {
        [TestMethod]
        public void Program_Test_Class()
        {
            //arrange
            NewTracer tracer = new NewTracer();
            InnerClass testObj = new InnerClass(tracer);

            String exceptedClass = "ConsoleApp2.InnerClass";
            //act
            testObj.InnerMethod();
            String actualClass = testObj.GetTracer().NextCall[0].Type;
            //assert
            Assert.AreEqual(exceptedClass, actualClass);
        }

        [TestMethod]
        public void Program_Test_Meth()
        {
            //arrange
            NewTracer tracer = new NewTracer();
            AbsoluteClass testObj = new AbsoluteClass(tracer);

            String exceptedMeth = "Void Absolute()";
            //act
            int x = 1;
            testObj.Absolute();
            String actualMeth = testObj.GetTracer().NextCall[0].ClassMethod;
            //assert
            Assert.AreEqual(exceptedMeth, actualMeth);
        }

        [TestMethod]
        public void Program_Test_Time()
        {
            //arrange
            NewTracer tracer = new NewTracer();
            AbsoluteClass testObj = new AbsoluteClass(tracer);

            int exceptedTime = 250;
            //act
            int x = 1;
            testObj.Absolute();
            int actualTime = testObj.GetTracer().NextCall[0].MethTime;
            //assert
            Assert.AreEqual(exceptedTime, actualTime);
        }

        [TestMethod]
        public void Program_Test_Thread()
        {
            //arrange
            NewTracer tracer = new NewTracer();            
            InnerClass testObj = new InnerClass(tracer);
            Thread testThread = new Thread(new ThreadStart(testObj.InnerMethod));
            testThread.Name = "TestThread #0";
            testThread.Start();
            testThread.Join();
            string exceptedThread = "TestThread #0";
            //act
            string actualThread = testObj.GetTracer().NextCall[0].Thread;
            //assert
            Assert.AreEqual(exceptedThread, actualThread);
        }

        [TestMethod]
        public void Program_Test_InnerCall()
        {
            //arrange
            NewTracer tracer = new NewTracer();
            InnerClass testObj = new InnerClass(tracer);
            testObj.InnerMethod();
            //act
            TraceResult actualNextCall = testObj.GetTracer().NextCall[0].NextCall[0];
            //assert
            Assert.IsNotNull(actualNextCall);
        }

        [TestMethod]
        public void Program_Test_Multi_Threads()
        {
            //arrange
            NewTracer tracer = new NewTracer();
            ZetClass testObj = new ZetClass(tracer);
            Thread testThread = new Thread(new ThreadStart(testObj.ZetMeth));
            testThread.Name = "TestThread #0";
            testThread.Start();
            testThread.Join();
            string exeptedSecondThread = "Thread_form_ZetMethod";
            //act
            string actualSecondThread = testObj.GetTracer().NextCall[0].NextCall[1].Thread;
            //assert
            Assert.AreEqual(exeptedSecondThread, actualSecondThread);
        }
    }
}
