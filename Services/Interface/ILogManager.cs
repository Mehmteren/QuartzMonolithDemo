using QuartzMonolithDemo.Models;
using System.Collections.Generic;

namespace QuartzMonolithDemo.Services.Interface
{
    public interface ILogManager
    {
        void WriteLog(LogEntry entry);
        List<string> ReadLastLogs(int count = 10);
    }
}