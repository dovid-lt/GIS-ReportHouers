using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetHouers
{
    internal class ExportData
    {

        public async Task ExportOkets(IEnumerable<WorkerTimeEntry> workers, Settings settings)
        {
            using (var f = File.CreateText(settings.FileName))
                foreach (var wAct in workers)
                    await f.WriteLineAsync($"1 {wAct.WorkerNumber:d10} {wAct.At:dd/MM/yy HH:mm} {wAct.Type}");
        }
    
    }
}
