using System;
using System.Configuration;
using System.IO;

namespace Spider.DataServices
{
    public class BinaryLoggingService : IDisposable
    {
        private static BinaryWriter _visitedWebsiteBw, _dataBw;
        private static BinaryReader _visitedWebsiteBr, _dataBr;

        private BinaryLoggingService()
        {
           
        }

        public static BinaryLoggingService CreateService()
        {
            return new BinaryLoggingService();
        }

        public void LogUriAsVisited(string uri)
        {
             if(_visitedWebsiteBw != null) 
                _visitedWebsiteBw.Write(uri + ";");
             else
             {
                 SetupWebsiteLogging();
                 LogUriAsVisited(uri);
             }
        }

        public void WriteContent(string data)
        {
            if (_dataBw != null)
                _dataBw.Write(data + ";");
            else
            {
                SetupContentLogging();
                WriteContent(data);
            }
        }

        public string ReadContent()
        {
            if (_dataBr == null)
                SetupContentLogging();

                return _dataBr.ReadString();
        }

        public string ReadLoggedWebsites()
        {
            if (_visitedWebsiteBr == null)
                SetupWebsiteLogging();

           return _visitedWebsiteBr.PeekChar() == -1 ? String.Empty : 
             _visitedWebsiteBr.ReadString();

        }

        private static void SetupWebsiteLogging()
        {
            var path = ConfigurationManager.AppSettings["VisitedWebsitesStore"];
            var stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            _visitedWebsiteBw = new BinaryWriter(stream);
            _visitedWebsiteBr = new BinaryReader(stream);
        }
        
        private static void SetupContentLogging()
        {
            var path = ConfigurationManager.AppSettings["AmharicWordStore"];
            var stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            _dataBw = new BinaryWriter(stream);
            _dataBr = new BinaryReader(stream);
        }

        public void Dispose()
        {
            _visitedWebsiteBw.Flush();
            _visitedWebsiteBw.Close();
            var s = _visitedWebsiteBw.BaseStream;
            s.Flush();
            s.Close();
            s.Dispose();
            _visitedWebsiteBw.Dispose();
        }

        public bool IsWebsiteAlreadyScrapped(string uri)
        {
            return ReadLoggedWebsites().Contains(uri);
        }
    }
}
