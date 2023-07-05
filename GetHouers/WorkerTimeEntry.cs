using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace GetHouers
{

    class WorkerTimeEntry
    {


        public int WorkerNumber { get; set; }
        public string WorkerName { get; set; }
        public DateTime At { get; set; }
        public string Type { get; set; }


        public static int GetWorkerNumberByPhone(string phone)
        {
            var text = HttpUtility.HtmlDecode(phone);
            var match = Regex.Match(text, @"<(\d+)>");
            if (!match.Success || match.Groups[1].Length > 6)
                return -1;

            return int.Parse(match.Groups[1].Value);
        }

        public static WorkerTimeEntry FromRawData(List<string> r)
        {
            var wT = new WorkerTimeEntry();
            wT.At = DateTime.Parse(r[0]);
            wT.Type = r[7] == "1" ? "IN" : "OT";
            var number =  GetWorkerNumberByPhone(r[3]);
            wT.WorkerNumber = number;
            //wT.WorkerName = name;

            return wT;
        }
    }
}
