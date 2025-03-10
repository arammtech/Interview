using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interview.Service.Interfaces
{
    public interface ILog
    {
        void Log(Exception ex, EventLogEntryType entryType = EventLogEntryType.Error);
    }
}
