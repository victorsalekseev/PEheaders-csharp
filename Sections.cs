using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Netcode.pe;

namespace WindowsApplication1
{
    public partial class Sections : UserControl
    {
        public Sections()
        {
            InitializeComponent();
        }

        public void PrintSect(object _sect)
        {
            listViewSch.Items.Clear();
            List<IMAGE_SECTION_HEADER> sect = (List<IMAGE_SECTION_HEADER>)_sect;
            for (int i = 0; i < sect.Count; i++)
            {
                ListViewItem lvi = new ListViewItem(Encoding.ASCII.GetString(sect[i].Name));
                lvi.SubItems.Add("0x" + sect[i].VirtualSize.ToString("X8"));
                lvi.SubItems.Add("0x" + sect[i].VirtualAddress.ToString("X8"));
                lvi.SubItems.Add("0x" + sect[i].SizeOfRawData.ToString("X8"));
                lvi.SubItems.Add("0x" + sect[i].PointerToRawData.ToString("X8"));
                lvi.SubItems.Add("0x" + sect[i].PointerToRelocations.ToString("X8"));
                lvi.SubItems.Add("0x" + sect[i].PointerToLinenumbers.ToString("X8"));
                lvi.SubItems.Add("0x" + sect[i].NumberOfRelocations.ToString("X4"));
                lvi.SubItems.Add("0x" + sect[i].NumberOfLinenumbers.ToString("X4"));
                lvi.SubItems.Add("0x" + sect[i].Characteristics.ToString("X8"));
                listViewSch.Items.Add(lvi);
            }
        }
    }
}
