using IndustrialMonitor.common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace IndustrialMonitor
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            GlobalMonitor.Start(
                // 成功回调的委托执行
                () =>
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        // 需要在UI线程中进行
                        new Window().Show();
                    });
                    
                },
                // 失败回调的委托执行
                (msg) =>
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show(msg, "系统启动失败");
                        Application.Current.Shutdown();
                    });
                }
                );
        }

        protected override void OnExit(ExitEventArgs e)
        {
            GlobalMonitor.Dispose();

            base.OnExit(e);
        }
    }
}
