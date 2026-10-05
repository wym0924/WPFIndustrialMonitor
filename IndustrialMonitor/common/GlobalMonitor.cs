using Communication;
using IndustrialMonitor.BLL;
using IndustrialMonitor.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialMonitor.common
{
    public class GlobalMonitor
    {
        public static List<StorageModel> StorageList = new List<StorageModel>();

        public static List<DevicesModel> DeviceList = new List<DevicesModel>();

        public static SerialInfo SerialInfo = new SerialInfo();

        static bool isRunning = true;
        static Task mainTask = null;
        public static void Start(Action successAction, Action<string> faultAction)
        {
            mainTask = Task.Run(() =>
            {
                IndustrialBLL bll = new IndustrialBLL();
                var si = bll.InitSerialInfo();  // 获取串口配置信息
                if(si.State)  
                {
                    SerialInfo = si.Data;
                }
                else
                {
                    faultAction(si.Message);
                    return;
                }

                var sa = bll.InitStorageArea();  // 获取存储区信息
                if (sa.State)
                {
                    StorageList = sa.Data;
                }
                else
                {
                    faultAction(sa.Message);
                    return;
                }

                var dL = bll.InitDevices(); // 获取设备集合及
                if (dL.State)
                {
                    DeviceList = dL.Data;
                }
                else
                {
                    faultAction(dL.Message);
                    return;
                }


                successAction();

                while (isRunning)
                {

                }
            });
        }

        public static void Dispose()
        {
            isRunning = false;
            if (mainTask != null) mainTask.Wait();
        }

    }
}
