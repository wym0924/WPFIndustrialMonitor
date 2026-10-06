using Communication;
using Communication.Modbus;
using IndustrialMonitor.BLL;
using IndustrialMonitor.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhaoxi.Industrial.Base;

namespace IndustrialMonitor.common
{
    public class GlobalMonitor
    {
        public static List<StorageModel> StorageList = new List<StorageModel>();

        public static List<DevicesModel> DeviceList = new List<DevicesModel>();

        public static SerialInfo SerialInfo = new SerialInfo();

        static bool isRunning = true;
        static Task mainTask = null;
        static RTU rtu = null;
        public static void Start(Action successAction, Action<string> faultAction)
        {
            mainTask = Task.Run(async () =>
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

                // 初始化串口通信
                rtu = RTU.GetIinstance(SerialInfo);
                rtu.ResponseData = new Action<int, List<byte>>(ParsingData);
                if (rtu.Connection())
                {
                    successAction();
                    // 死循环不停的刷数据
                    while (isRunning)
                    {
                        // 遍历所有的存储区的数据
                        foreach(var item in StorageList)
                        {
                            // 判断长度，Modbus 协议限制：一次读寄存器不能读太多，很多设备单次最大支持 125 个寄存器，所以要拆分多次请求。
                            if (item.Length > 100)
                            {
                                int readCount = item.Length / 100;
                                for (int i = 0; i < readCount; i++)
                                {
                                    await rtu.Send(item.SlaveAddress, (byte)int.Parse(item.FuncCode), i * 100, 100);
                                }
                                await rtu.Send(item.SlaveAddress, (byte)int.Parse(item.FuncCode), readCount * 100, item.Length - readCount * 100);
                            }
                            else
                                await rtu.Send(item.SlaveAddress, (byte)int.Parse(item.FuncCode), 0, item.Length);
                        }
                    }
                }
                else
                {
                    faultAction("程序无法启动，串口连接初始化失败，请检查设备是否连接！");
                }

                
            });
        }

        /// <summary>
        /// 委托方法 解析数据
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="list"></param>
        private static void ParsingData(int start_addr, List<byte> byteList)
        {
            if(byteList != null && byteList.Count > 0)
            {
                // 利用Linq 查找当前返回的数据中的对应存储区中的监控点位信息
                var mvl = (from d in DeviceList
                           from mv in d.MonitorValuesList
                           where mv.StorageAreaId == byteList[0].ToString() + byteList[1].ToString("00") + start_addr.ToString()
                           select mv).ToList();

                int startByte;
                byte[] res = null;
                foreach(var item in mvl)
                {
                    // 需要检查每个 点位的数据类型
                    switch(item.DataType)
                    {
                        case "Float":
                            startByte = (item.StartAddress - start_addr) * 2 + 3;
                            res = new byte[4] { byteList[startByte], byteList[startByte + 1], byteList[startByte + 2], byteList[startByte + 3] };
                            item.CurrentValue = Convert.ToDouble(res.ByteArrysToFloat());
                            break;
                        case "Bool":
                            break;

                    }
                }

            }
        }

        public static void Dispose()
        {
            isRunning = false;
            if(rtu != null)
            {
                rtu.Dispose();
            }
            if (mainTask != null) 
                mainTask.Wait();
        }

    }
}
