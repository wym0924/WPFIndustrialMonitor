using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication
{
    public class SerialInfo
    {
        // 串口信息类
        public string PortName { get; set; } // 串口名称
        public int BaudRate { get; set; } // 波特率
        public int DataBits { get; set; } // 数据位
        public StopBits StopBits { get; set; } // 停止位
        public Parity Parity { get; set; } // 校验位



    }
}
