﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace SDRSharp.Rtl_433
{
    public partial class FormDevicesListMessages : Form
    {
        private ClassInterfaceWithRtl433 classInterfaceWithRtl433;
        private string memoName = "";
        private int maxMessages = 0;
        private Dictionary<String, int> cacheListColumns;
        private ListViewItem[] cacheListMessages;
        private int nbMessage = 0;
        private Rtl_433_Panel classParent;
		private bool firstToTop = false;
        int nbColumn = 0;
        public FormDevicesListMessages(Rtl_433_Panel classParent, int maxDevices,int nbColumn,string name, ClassInterfaceWithRtl433 classInterfaceWithRtl433)
        {
            this.classInterfaceWithRtl433 = classInterfaceWithRtl433;
            this.nbColumn = nbColumn;
            InitializeComponent();
            this.classParent = classParent;
            this.maxMessages = maxDevices;
            typeof(Control).InvokeMember("DoubleBuffered", BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic, null, listViewListMessages, new object[] { true });

            ClassFunctionsListView.initListView(listViewListMessages, this.nbColumn);
            cacheListMessages = new ListViewItem[this.maxMessages];
            cacheListColumns = new Dictionary<String, int>();
            listViewListMessages.Columns[0].Text = "N° Mes.";
            cacheListColumns.Add("N° Mes.",  1);
            listViewListMessages.Columns.Add("");
            memoName = name;
            this.Text = name + " (Messages received : 0)";
            statusStripExport.ShowItemToolTips=true;
            toolStripStatusLabelExport.ToolTipText = "Record data  \n" +
                " to directory Recordings if exist else in SdrSharp.exe directory \n" +
                " You can reload file with Calc\n" +
                " WARNING the file is replaced if it exists\n" +
                " name file = title window";
         }
        //protected override void OnClosed(EventArgs e)
        //{
        //    classParent.closingFormListDevice();
        //    cacheListColumns = null;
        //    cacheListDevices = null;
        //    nbMessage = 0;
        //}
        #region private functions
        private void listViewListMessages_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            try
            {
                if (e.ItemIndex >= 0)
                {
                    ListViewItem lvi = cacheListMessages[e.ItemIndex];
                    if (lvi != null)
                        e.Item = lvi;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error fct(listViewListMessages_RetrieveVirtualItem)", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private int FindIndexIfDeviceExist(string device)
        {
            for (int row = 0; row < maxMessages; row++)
            {
                ListViewItem lvi = cacheListMessages[row];
                if (lvi != null && lvi.Text == device)
                    return row;
            }
            return -1;
        }
        private void EnsureVisible(int item)
        {
            listViewListMessages.Items[item].EnsureVisible();
        }
        #endregion
        #region publics functions
        public void refresh()
        {
            if (nbMessage > 0)
                listViewListMessages.Items[nbMessage - 1].EnsureVisible();
            this.Refresh();
        }
        public void setMessages(Dictionary<String, String> listData)
        {
            if (cacheListColumns == null)
                return;
            
            string deviceName = (nbMessage+1).ToString();
            if (nbMessage > maxMessages - 1)
                return;                    //message max row
 
            this.SuspendLayout();
            listViewListMessages.BeginUpdate();
            int indexColonne = 0;
            Boolean ret = false;
            //*********add name column if necessary****************
            foreach (KeyValuePair<string, string> _data in listData)
            {
                ret=cacheListColumns.TryGetValue(_data.Key, out indexColonne);
                if (!ret)
                {
                    if (cacheListColumns.Count < (nbColumn ))
                    {
                         listViewListMessages.Columns[cacheListColumns.Count].Text = _data.Key;
                         cacheListColumns.Add(_data.Key, cacheListColumns.Count + 1);
                         //listViewListMessages.Columns.Add("");
                    }
                 }
            }
            ListViewItem device = new ListViewItem(deviceName);
            for (int i = 0; i < nbColumn; i++)
            {
                device.SubItems.Add("-");
            }
             foreach (KeyValuePair<string, string> _data in listData)
            {
                ret=cacheListColumns.TryGetValue(_data.Key,out indexColonne);
                if(ret)
                    device.SubItems[indexColonne - 1].Text = _data.Value;
            }
            //************************************************
            //cacheListMessages[nbMessage] = device;       last message at the bottom list

			if (firstToTop)
			{
				cacheListMessages[nbMessage] = device;
			}
			else{
				//last message at the top list
	            for (int m=nbMessage;m>0;m--)
				{
					cacheListMessages[m] = cacheListMessages[m-1];
				}
				cacheListMessages[0] = device;			
			}

            //

            nbMessage += 1;
            this.Text = memoName + " (Messages received : " + nbMessage.ToString() + "/" + maxMessages.ToString() + ")";
            try
            {
            listViewListMessages.VirtualListSize = nbMessage;
            }
            catch {
                Console.WriteLine(this.Text);
            }
            ClassFunctionsListView.autoResizeColumns(listViewListMessages, cacheListColumns.Count);
            //refresh();  // display last message when it is displayed at the bottom list
            listViewListMessages.EndUpdate();
            this.ResumeLayout();
        }
        #endregion
        #region Events Form
        private void FormDevicesListMessages_FormClosing(object sender, FormClosingEventArgs e)
        {
            cacheListColumns = null;
            cacheListMessages = null;
            classParent.closingOneFormDeviceListMessages(memoName);
        }
        private void toolStripStatusLabelExport_Click(object sender, EventArgs e)
        {
            string directory = classInterfaceWithRtl433.getDirectoryRecording();
            string fileName = ClassFunctionsListView.valideNameFile(memoName,"_");
           if( ClassFunctionsListView.serializeText(directory + fileName + ".txt", cacheListColumns, cacheListMessages,true, nbMessage,false))
           {
                MessageBox.Show("Export OK", "Export messages", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        #endregion
    }
}
