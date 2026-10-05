using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialMonitor.Model
{
    public class StorageModel
    {
        public string Id { get; set; }
        public int SlaveAddress { get; set; }
        public string FuncCode { get; set; }
        public int StartAddress { get; set; }
        public int Length { get; set; }


    }
}
