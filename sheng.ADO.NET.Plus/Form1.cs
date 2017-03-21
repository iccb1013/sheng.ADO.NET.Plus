using Linkup.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace sheng.ADO.NET.Plus
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://www.cnblogs.com/sheng_chao/p/6597672.html");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DatabaseWrapper database = ServiceUnity.Instance.Database;
            database.Insert(new UserEntity());
        }
    }
}
