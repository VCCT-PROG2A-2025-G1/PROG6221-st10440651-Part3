using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms; // For NotifyIcon
using System.Drawing; // For SystemIcons

namespace PROG6221_st10440651_Part3
{
    public class QuizQuestion
    {
        public string Question { get; set; }
        public string[] Options { get; set; }
        public int CorrectAnswer { get; set; }
        public string Explanation { get; set; }
    }
}