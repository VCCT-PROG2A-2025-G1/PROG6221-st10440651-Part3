﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms; // For NotifyIcon
using System.Drawing; // For SystemIcons

namespace PROG6221_st10440651_Part3
{
    public class ActivityLog
    {
        public string Action { get; set; }
        public DateTime Timestamp { get; set; }
    }
}