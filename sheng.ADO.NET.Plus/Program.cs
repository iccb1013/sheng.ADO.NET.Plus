using Linkup.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace sheng.ADO.NET.Plus
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //确保第一时间调用  SetLogWriter 方法，否则如果 ExceptionHandlingService 先于它初始化
            //则会报错
            LogService.Instance.Write("Application_Start");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());

            
        }
    }
}
