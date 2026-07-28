using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangaK_System.DAL.Entity.Enums
{
    public enum SeriesStatus
    {
        Processing = 1,
        Approved,
        Pending,
        Scheduled,
        Publishing,
        Rejected,
        Cancelled,
    }
}
