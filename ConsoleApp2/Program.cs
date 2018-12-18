using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using Tracer;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace ConsoleApp2
{    
    public class AbsoluteClass
    {
        public int ForR = 0;

        private NewTracer _tracer = new NewTracer();

        public AbsoluteClass() { }
        public AbsoluteClass( NewTracer tracer)
        {
            _tracer = tracer;            
        }

        public void Absolute()
        {
            _tracer.StartTrace();
            Thread.Sleep(250);            
            _tracer.StopTrace();
        }

        public TraceResult GetTracer()
        {
            return _tracer.GetResult();
        }
    }
    
    public class InnerClass
    {        
        private NewTracer _tracer = new NewTracer();
        private ZetClass wednsday;
        private AbsoluteClass lvl;

        public InnerClass() { }
        public InnerClass( NewTracer tracer)
        {
            _tracer = tracer;            
        }

        public void InnerMethod()
        {            
            _tracer.StartTrace();

            wednsday = new ZetClass(_tracer);
            lvl = new AbsoluteClass(_tracer);

            wednsday.ZetMeth();
            lvl.Absolute();
            Thread.Sleep(100);
            _tracer.StopTrace();
        }

        public TraceResult GetTracer()
        {
            return _tracer.GetResult();
        }
    }

    public class ZetClass
    {
        private NewTracer _tracer = new NewTracer();
        private AbsoluteClass _lvl;
        private AbsoluteClass foreThread;

        public ZetClass() { }
        public ZetClass( NewTracer tracer)
        {
            _tracer = tracer;
        }

        public void ZetMeth()
        {
            _tracer.StartTrace();
            foreThread = new AbsoluteClass(_tracer);
            _lvl = new AbsoluteClass( _tracer);
            int i = 0;
            _lvl.Absolute();
            Thread myThread = new Thread(new ThreadStart(foreThread.Absolute));
            myThread.Name = "Thread_form_ZetMethod";
            myThread.Start();
            myThread.Join();
            Thread.Sleep(50);
            _tracer.StopTrace();
        }

        public TraceResult GetTracer()
        {
            return _tracer.GetResult();
        }
    }    

    class Program
    {            
        static void Main(string[] args)
        {          
            TraceResult result;
            InnerClass tset = new InnerClass();
            ZetClass check = new ZetClass();            
            TraceResult[] resArr = new TraceResult[0];

            Thread myTHread = new Thread(new ThreadStart(tset.InnerMethod));
            Thread myTHread1 = new Thread(new ThreadStart(() => check.ZetMeth()));

            myTHread.Name = "Thread #1";
            myTHread1.Name = "Thread #2";

            myTHread.Start();
            myTHread1.Start();
            myTHread1.Join();
            myTHread.Join();

            Console.WriteLine(myTHread.IsAlive);

            result = tset.GetTracer();            
            foreach(var el in result.NextCall)
            {
                Array.Resize(ref resArr, resArr.Length + 1);
                resArr[resArr.Length - 1] = el;
            }
            
            
            result = check.GetTracer();
            foreach (var el in result.NextCall)
            {
                Array.Resize(ref resArr, resArr.Length + 1);
                resArr[resArr.Length - 1] = el;
            }
            MemoryStream stream = new MemoryStream();
            XmlSerialization newSer = new XmlSerialization();
            stream = newSer.Serialize(resArr, stream);
            Writer wr = new Writer();
            wr.Write(stream);

            Console.WriteLine("Press any key");
            Console.ReadKey();
        }
    }
}
