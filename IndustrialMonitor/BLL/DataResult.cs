using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialMonitor.BLL
{

    /// <summary>
    /// 用于返回异常信息的类
    /// </summary>
    public class DataResult<T>
    {
        public bool State { get; set; } = false;// 状态
        public string Message { get; set; } // 消息
        public T Data { get; set; } // 数据
    }

    public class DataResult : DataResult<string> { }
}
