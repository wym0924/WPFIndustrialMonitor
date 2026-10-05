using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialMonitor.Model
{
    public class DevicesModel
    {
        public string Id { get; set; }
        public string DeviceName { get; set; } // 设备名 

        public bool isWarnning { get; set; } = false; // 判断是否正在报警
        // 监控数据的集合 用带通知属性的集合装
        public ObservableCollection<MonitorValuesModel> MonitorValuesList { get; set; } = new ObservableCollection<MonitorValuesModel>();

        // 异常值通知集合
        public ObservableCollection<WarningMessageModel> WarngingMessageList { get; set; } = new ObservableCollection<string>();
    }
}
