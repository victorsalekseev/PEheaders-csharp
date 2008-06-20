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
    public partial class DosHeader : UserControl
    {
        public DosHeader()
        {
            InitializeComponent();
        }

        public void PrintIDH(object _idh)
        {
            IMAGE_DOS_HEADER idh = (IMAGE_DOS_HEADER)_idh;
            textBoxe_cblp.Text = "0x" + idh.e_cblp.ToString("X4");
            textBoxe_cp.Text = "0x" + idh.e_cp.ToString("X4");
            textBoxe_cparhdr.Text = "0x" + idh.e_cparhdr.ToString("X4");
            textBoxe_crlc.Text = "0x" + idh.e_crlc.ToString("X4");
            textBoxe_cs.Text = "0x" + idh.e_cs.ToString("X4");
            textBoxe_csum.Text = "0x" + idh.e_csum.ToString("X4");
            textBoxe_ip.Text = "0x" + idh.e_ip.ToString("X4");
            textBoxe_lfanew.Text = "0x" + idh.e_lfanew.ToString("X8");
            textBoxe_lfarlc.Text = "0x" + idh.e_lfarlc.ToString("X4");
            textBoxe_magic.Text = "0x" + idh.e_magic.ToString("X4");
            textBoxe_maxalloc.Text = "0x" + idh.e_maxalloc.ToString("X4");
            textBoxe_minalloc.Text = "0x" + idh.e_minalloc.ToString("X4");
            textBoxe_oemid.Text = "0x" + idh.e_oemid .ToString("X4");
            textBoxe_oeminfo.Text = "0x" + idh.e_oeminfo.ToString("X4");
            textBoxe_ovno.Text = "0x" + idh.e_ovno.ToString("X4");

            textBoxe_res0.Text = "0x" + idh.e_res[0].ToString("X4");
            textBoxe_res1.Text = "0x" + idh.e_res[1].ToString("X4");
            textBoxe_res2.Text = "0x" + idh.e_res[2].ToString("X4");
            textBoxe_res3.Text = "0x" + idh.e_res[3].ToString("X4");

            textBoxe_sp.Text = "0x" + idh.e_sp.ToString("X4");
            textBoxe_ss.Text = "0x" + idh.e_ss.ToString("X4");
            
            textBoxm0.Text = "0x" + idh.m[0].ToString("X4");
            textBoxm1.Text = "0x" + idh.m[1].ToString("X4");
            textBoxm2.Text = "0x" + idh.m[2].ToString("X4");
            textBoxm3.Text = "0x" + idh.m[3].ToString("X4");
            textBoxm4.Text = "0x" + idh.m[4].ToString("X4");
            textBoxm5.Text = "0x" + idh.m[5].ToString("X4");
            textBoxm6.Text = "0x" + idh.m[6].ToString("X4");
            textBoxm7.Text = "0x" + idh.m[7].ToString("X4");
            textBoxm8.Text = "0x" + idh.m[8].ToString("X4");
            textBoxm9.Text = "0x" + idh.m[9].ToString("X4");
        }
    }
}
