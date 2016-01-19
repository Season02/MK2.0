using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Windows.Forms;

namespace S2_0
{
    class forceTurnoff
    {
        private delegate uint ZwShutdownSystem(int ShutdownAction);//编译
        private delegate uint RtlAdjustPrivilege(int Privilege, bool Enable, bool CurrentThread, ref int Enabled);

        [DllImport("kernel32.dll")]
        private extern static IntPtr LoadLibrary(String path);
        [DllImport("kernel32.dll")]
        private extern static IntPtr GetProcAddress(IntPtr lib, String funcName);
        [DllImport("kernel32.dll")]
        private extern static bool FreeLibrary(IntPtr lib);

        [DllImport("User32.dll")]
        private extern static bool ExitWindowsEx(bool flg, bool rea);


        //将要执行的函数转换为委托
        private static Delegate Invoke(String APIName, Type t, IntPtr hLib)
        {
            IntPtr api = GetProcAddress(hLib, APIName);
            return (Delegate)Marshal.GetDelegateForFunctionPointer(api, t);
        }

        public forceTurnoff()
        {
            IntPtr hLib = LoadLibrary("ntdll.dll");
            RtlAdjustPrivilege rtla = (RtlAdjustPrivilege)Invoke("RtlAdjustPrivilege", typeof(RtlAdjustPrivilege), hLib);
            ZwShutdownSystem shutdown = (ZwShutdownSystem)Invoke("ZwShutdownSystem", typeof(ZwShutdownSystem), hLib);

            int en = 0;
            uint ret = rtla(0x13, true, false, ref en);//SE_SHUTDOWN_PRIVILEGE = 0x13;     //关机权限
            ret = shutdown(2); // POWEROFF = 0x2 // 关机 // REBOOT = 0x1 // 重启
        }

        public forceTurnoff(byte index)
        {
            IntPtr hLib = LoadLibrary("ntdll.dll");
            RtlAdjustPrivilege rtla = (RtlAdjustPrivilege)Invoke("RtlAdjustPrivilege", typeof(RtlAdjustPrivilege), hLib);
            ZwShutdownSystem shutdown = (ZwShutdownSystem)Invoke("ZwShutdownSystem", typeof(ZwShutdownSystem), hLib);

            int en = 0;
            uint ret = rtla(0x13, true, false, ref en);//SE_SHUTDOWN_PRIVILEGE = 0x13;     //关机权限
                ret = shutdown(index); // POWEROFF = 0x2 // 关机 // REBOOT = 0x1 // 重启
        }

        public static void hibernate()
        {
            ExitWindowsEx(false, false);
        }
 

    }
}
