using System.Collections.Generic;

namespace QuartzMonolithDemo.Services.Interface
{
    public interface ILogManager
    {
        void WriteLog(string message);
        List<string> ReadLastLogs(int count = 10);
    }
}