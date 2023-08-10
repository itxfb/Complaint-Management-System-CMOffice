using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;
using System.IO;

namespace PITB.CMS.Helper
{
    public static class TraceLogger
    {
        private static TraceSource source;
        private static void InitializeData()
        {
            source = new TraceSource("Logger",SourceLevels.All);
            FileStream stream = File.Open(@"C:\\Logs.txt",FileMode.OpenOrCreate,FileAccess.Write);
            stream.Seek(0,SeekOrigin.End);
            source.Listeners.Clear();
            source.Listeners.Add(new TextWriterTraceListener(stream));
        }
        private static void InitializeData(string filePath)
        {
            source = new TraceSource("Logger", SourceLevels.All);
            FileStream stream = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.Write);
            stream.Seek(0, SeekOrigin.End);
            source.Listeners.Clear();
            source.Listeners.Add(new TextWriterTraceListener(stream));
        }
        private static void Close()
        {
            source.Flush();
            source.Close();
        }
        public static void WriteMessage(TraceEventType messageType,ErrorType errorType,string messageText) 
        {
            try
            {
                InitializeData();
                source.TraceData(messageType, (int)errorType, new object[] { messageText });
                Close();
            }
            catch (Exception ex)
            {

            }   
        }
    }
    public enum ErrorType
    {
        Database = 0,
        General = 1
    }
}