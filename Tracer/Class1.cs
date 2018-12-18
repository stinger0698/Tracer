using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using System.Threading;

namespace Tracer
{
    public interface ITracer
    {
        void StartTrace();

        void StopTrace();

        TraceResult GetResult();
    }

    public interface ISerialization
    {
        MemoryStream Serialize(TraceResult[] ElArr, MemoryStream stram);
    }

    public interface IWriter
    {
        void Write(MemoryStream memStram);        
    }

    public class Writer:IWriter
    {
        public void Write(MemoryStream memStream)
        {
            StreamReader sr = new StreamReader(memStream);

            Console.WriteLine(sr.ReadToEnd());                           
         
            using (FileStream fs = new FileStream("classes.xml", FileMode.Create))
            {
                memStream.WriteTo(fs);
            }
        }
    }

    public class XmlSerialization : ISerialization
    {
        XmlSerializer formatter = new XmlSerializer(typeof(TraceResult[]));
        
        public MemoryStream Serialize(TraceResult[] ElArr, MemoryStream memoryStream)
        {
            StreamWriter stream = new StreamWriter(memoryStream);
            formatter.Serialize(stream, ElArr);
            Console.WriteLine("Объект XML сериализован");                
            return memoryStream;
        }
    }

  /*  public class JsonSerialization : ISerialization
    {
        XmlSerializer formatter = new XmlSerializer(typeof(TraceResult[]));

        public void Serialize(TraceResult[] ElArr)
        {            
            //JSON
            string json = JsonConvert.SerializeObject(ElArr, Newtonsoft.Json.Formatting.Indented);
            using (FileStream fs = new FileStream("classes.json", FileMode.Create))
            {
                byte[] array = System.Text.Encoding.Default.GetBytes(json);
                fs.Write(array, 0, array.Length);
                Console.WriteLine("Объект JSON сериализован");
                Console.WriteLine(json);
            }
        }
    }*/

    [Serializable]
    [DataContract]
    public class TraceResult
    {
        internal String type;
        internal String classMethod;
        internal int methTime;
        internal Stopwatch Timer;
        internal String thread;


        public TraceResult() { }

        public TraceResult(string _type, string _classMethod, int _methTime, string _thread)
        {
            type = _type;
            classMethod = _classMethod;
            methTime = _methTime;
            thread = _thread;
        }

        [XmlElement(Order = 0)]
        [JsonProperty(Order = 0)]
        public String Thread
        {
            get { return thread; }            
            set { }
        }

        [XmlElement(Order = 1)]
        [JsonProperty(Order = 1)]
        public String Type
        {
            get { return type; }
            set { }
        }

        [XmlElement(Order = 2)]
        [JsonProperty(Order = 2)]
        public String ClassMethod
        {
            get { return classMethod; }
            set { }
        }

        [XmlElement(Order = 3)]
        [JsonProperty(Order = 3)]
        public int MethTime
        {
            get { return methTime; }
            set { }
        }

        [XmlElement(Order = 4)]
        [JsonProperty(Order = 4)]
        public List<TraceResult> NextCall = new List<TraceResult>();
        [XmlIgnore]
        [IgnoreDataMember]
        [NonSerialized]
        internal TraceResult PrevCall;                  
    }

    public class NewTracer : ITracer
    {
        TraceResult firstEl = new TraceResult();
        TraceResult previousEl;
        public void StartTrace()
        {
            TraceResult traceEl = new TraceResult();
            StackTrace stack = new StackTrace(true);

            StackFrame stackFrame = stack.GetFrame(1);
            //Метод
            traceEl.classMethod = stackFrame.GetMethod().ToString();
            //Класс
            traceEl.type = stackFrame.GetMethod().DeclaringType.ToString();
            //Поток
            traceEl.thread = Thread.CurrentThread.Name /*+ "; id:" + Thread.CurrentThread.ManagedThreadId*/;
            //Время
            traceEl.Timer = Stopwatch.StartNew();
            if (previousEl != null)
                traceEl.PrevCall = previousEl;
            else
                traceEl.PrevCall = firstEl;
            //Вложенность            
            if (firstEl.NextCall.Count == 0)
                firstEl.NextCall.Add(traceEl);
            else
                previousEl.NextCall.Add(traceEl);
            previousEl = traceEl;
        }

        public void StopTrace()
        {
            previousEl.Timer.Stop();
            previousEl.methTime = (int)previousEl.Timer.ElapsedMilliseconds;
            previousEl = previousEl.PrevCall;
        }

        public TraceResult GetResult()
        {
            return firstEl;          
        }        
    }
}
