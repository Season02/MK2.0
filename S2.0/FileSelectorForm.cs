﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace S2_0
{
    public partial class FileSelectorForm : Form
    {
        private GlobalHookEvent globalhook;

        public FileSelectorForm(GlobalHookEvent globalhook)
        {
            InitializeComponent();
            this.globalhook = globalhook;
            initLV();
            initFormEvent();
        }
        
        private void initFormEvent()
        {
            globalhook.FileSelectorForm_Event += new GlobalHookEvent.FileSelectorForm_Event_Handler(updataList);
        }

        private void updataList(Object sender,String ip,int index)
        {
            this.host_lv.BeginUpdate();   //数据更新，UI暂时挂起，直到EndUpdate绘制控件，可以有效避免闪烁并大大提高加载速度

            ListViewItem lvi = new ListViewItem();
            lvi.Text = ip + " " + index;
            lvi.SubItems.Add("Online");
            lvi.SubItems.Add(index.ToString());
            this.host_lv.Items.Add(lvi);

            this.host_lv.EndUpdate();  //结束数据处理，UI界面一次性绘制。
        }

        void initLV()
        {
            ColumnHeader ch = new ColumnHeader();
            ch.Text = "HOST IP";   //设置列标题
            ch.Width = 180;    //设置列宽度
            ch.TextAlign = HorizontalAlignment.Center;   //设置列的对齐方式
            this.host_lv.Columns.Add(ch);    //将列头添加到ListView控件。

            this.host_lv.Columns.Add("STATUE", 120, HorizontalAlignment.Center); //一步添加
            this.host_lv.Columns.Add("INDEX", 120, HorizontalAlignment.Center); //一步添加

        }

    }
}
