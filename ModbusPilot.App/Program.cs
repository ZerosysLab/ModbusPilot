using ModbusPilot.Core.Services;
using ModbusPilot.UI.Common;
using System;
using System.Threading; // 必须引用
using System.Windows.Forms;

namespace ModbusPilot.App
{
    internal static class Program
    {
        // 定义一个静态互斥量，确保全系统唯一
        // 建议加上 "Global\" 前缀，确保多用户登录时也能防双开
        // 后面这串字符串最好改个独一无二的，比如加上你的项目 GUID
        private static Mutex _mutex = null;

        [STAThread]
        static void Main()
        {
            const string mutexName = "Global\\ModbusPilot_Single_Instance_Mutex_2026";
            bool createdNew;

            // 1. 尝试创建互斥锁
            _mutex = new Mutex(true, mutexName, out createdNew);

            // 2. 判断是否已经有实例在运行
            if (!createdNew)
            {
                // 如果 createdNew 为 false，说明锁已经被别人持有了
                MessageBox.Show("ModbusPilot 已经在运行中！\n请检查任务栏或托盘图标。",
                                "重复运行", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                // 退出当前尝试启动的进程
                return;
            }

            // --- 只有拿到锁（第一个实例）才会执行下面的代码 ---

            ApplicationConfiguration.Initialize();

            // 3. 全局异常捕获 (之前做好的)
            Application.ThreadException += (s, e) => HandleException(e.Exception);
            AppDomain.CurrentDomain.UnhandledException += (s, e) => HandleException(e.ExceptionObject as Exception);

            try
            {
                // 【核心调用】在软件还没露面时，就开始读授权文件和算机器码
                LicenseGuard.Initialize();

                // 4. 显示启动画面 (Splash Screen)
                // 这里检查一下 F_Splash 是否存在，如果还没做就注释掉
                using (var splash = new F_Splash())
                {
                    splash.Show();
                    splash.Refresh(); // 强制重绘，防止白屏

                    // 模拟加载步骤
                    splash.UpdateStatus("正在初始化驱动...");
                    Thread.Sleep(300); // 稍微停顿，让用户看清 Logo

                    splash.UpdateStatus("正在加载用户界面...");
                    Thread.Sleep(300);

                    splash.Close();
                }

                // 5. 启动主界面
                Application.Run(new MainForm());
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
            finally
            {
                // 6. 释放互斥锁 (虽然进程结束操作系统会自动释放，但显式释放是好习惯)
                if (_mutex != null)
                {
                    _mutex.ReleaseMutex();
                    _mutex.Close();
                }
            }
        }

        // 统一异常处理逻辑
        private static void HandleException(Exception ex)
        {
            if (ex == null) return;

            // 1. 写入本地日志
            ModbusPilot.Core.Utils.SystemLogger.WriteError(ex);

            string msg = $"发生未预料的错误：\n{ex.Message}\n\n建议保存数据并重启软件。";
            MessageBox.Show(msg, "系统错误", MessageBoxButtons.OK, MessageBoxIcon.Error);

        }
    }
}