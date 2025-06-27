using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms; // For NotifyIcon
using System.Drawing; // For SystemIcons

namespace PROG6221_st10440651_Part3
{
    public class TaskItem
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Reminder { get; set; }
    }
}