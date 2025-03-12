using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Muqabalati.Service.Interfaces;

namespace Muqabalati.Service.Implementations
{
    public class LogService : ILog
    {
        public void Log(Exception ex, EventLogEntryType entryType = EventLogEntryType.Error)
        {
            EventLog.WriteEntry("Muqabalati", this.FormatMessage(ex), entryType);
        }

        private string FormatMessage(Exception ex)
        {
                                return $@"
                    -------------------- Exception Log --------------------
                    Timestamp      : {DateTime.Now:yyyy-MM-dd HH:mm:ss}
                    Message        : {ex.Message}
                    Inner Exception: {(ex.InnerException != null ? ex.InnerException.Message : "N/A")}
                    Source         : {ex.Source}
                    Stack Trace    : 
                    {ex.StackTrace}
                    -------------------------------------------------------
                    ";
        }

    }
}
