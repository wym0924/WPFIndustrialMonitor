using IndustrialMonitor.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialMonitor.Model
{
    public class MonitorValuesModel
    {
        // 需要新建一个委托，当需要报警时触发
        public Action<MonitorValueState, string, string> ValueStateChanged; // 报警状态，消息, 当前报警值
        public string ValueId { get; set; }
        public string ValueName { get; set; }
        public string StorageAreaId { get; set; }
        public int StartAddress { get; set; }
        public string DataType { get; set; }
        public bool IsAlarm { get; set; }
        public double LoLoAlarm { get; set; }
        public double LowAlarm { get; set; }
        public double HighAlarm { get; set; }
        public double HiHiAlarm { get; set; }
        public string Description { get; set; }

        public string Unit { get; set; }

        public double _currentValue;
        public double CurrentValue  // 用于判断是否在正常范围
        {
            get { return _currentValue; }
            set 
            { 
                _currentValue = value; 
                if(IsAlarm)
                {
                    string Message = Description;
                    MonitorValueState state = MonitorValueState.OK;  // 默认
                    if (value <= LoLoAlarm)
                    {
                        Message += "极低";
                        state = MonitorValueState.LoLo;
                    }
                    else if (value <= LowAlarm)
                    {
                        Message += "过低";
                        state = MonitorValueState.Low;
                    }
                    else if (value >= HighAlarm && value < HiHiAlarm)
                    {
                        Message += "过高";
                        state = MonitorValueState.High;
                    }
                    else if (value >= HiHiAlarm)
                    {
                        Message += "极高";
                        state = MonitorValueState.HiHi;
                    }
                    ValueStateChanged(state, Message + "，当前值：" + value.ToString(), ValueId);
                }
            }
        }


    }
}
