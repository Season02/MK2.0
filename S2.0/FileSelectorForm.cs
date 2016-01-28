using System;
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
            var lvi = new ListViewItem();
            lvi.Text = ip + " " + index;
            lvi.SubItems.Add("Online");
            lvi.SubItems.Add(index.ToString());
            host_lv.BeginUpdate();   //数据更新，UI暂时挂起，直到EndUpdate绘制控件，可以有效避免闪烁并大大提高加载速度
            host_lv.Items.Add(lvi);
            //host_lv.Items[host_lv.Items.Count - 1].EnsureVisible();//滚动到最后  
            host_lv.EndUpdate();  //结束数据处理，UI界面一次性绘制。
        }

        void initLV()
        {
            host_lv.Columns.Add("HOST", 165,HorizontalAlignment.Center);//将列头添加到ListView控件。
            host_lv.Columns.Add("STATUE", 120, HorizontalAlignment.Center);
            host_lv.Columns.Add("INDEX", 120, HorizontalAlignment.Center);
        }

        private void host_lv_Click(object sender, EventArgs e)
        {
            int selectCount = host_lv.SelectedItems.Count; //SelectedItems.Count就是：取得值，表示SelectedItems集合的物件数目。 
            MessageBox.Show("select index:" + host_lv.SelectedIndices[0] +"\r\n" +
                host_lv.Items[host_lv.SelectedIndices[0]].SubItems[1].Text );
        }
    }
}
