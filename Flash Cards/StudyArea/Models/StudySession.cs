using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flash_Cards.StudyArea.Models
{
    internal class StudySession
    {
        public int Id { get; set; }
        public string LangName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Correct { get; set; }
    }
}
