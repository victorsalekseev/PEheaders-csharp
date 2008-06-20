using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using Netcode.pe;

namespace WindowsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        IMAGE_DOS_HEADER s = new IMAGE_DOS_HEADER();
        IMAGE_NT_HEADERS nt_h = new IMAGE_NT_HEADERS();
        List<IMAGE_SECTION_HEADER> lsect = new List<IMAGE_SECTION_HEADER>();

        PE_Helper pe_hlp = new PE_Helper();
        byte[] al_hzch;
        byte[] al_sc;

        private void button1_Click(object sender, EventArgs e)
        {
            FileStream fs = new FileStream("1.exe", FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);

            byte[] buffer = new byte[Marshal.SizeOf(typeof(IMAGE_DOS_HEADER))];
            GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            Marshal.StructureToPtr(s, handle.AddrOfPinnedObject(), false);
            handle.Free();
            //В конце получаем buffer c байтами нашей структуры
            fs.Write(buffer, 0, buffer.Length);//write a IMAGE_DOS_HEADER
            
            uint size_dos_stub = s.e_lfanew - s.e_lfarlc;
            byte[] dos_stub = new byte[size_dos_stub];
            fs.Write(dos_stub, 0, (int)size_dos_stub);//write a nulled dos stub

            byte[] buffer2 = new byte[Marshal.SizeOf(typeof(IMAGE_NT_HEADERS))];
            GCHandle handle2 = GCHandle.Alloc(buffer2, GCHandleType.Pinned);
            Marshal.StructureToPtr(nt_h, handle2.AddrOfPinnedObject(), false);
            handle2.Free();
            //В конце получаем buffer2 c байтами нашей структуры
            fs.Write(buffer2, 0, buffer2.Length);//write a IMAGE_NT_HEADERS

            for (int i = 0; i < lsect.Count; i++)
            {
                byte[] buffer3 = new byte[Marshal.SizeOf(typeof(IMAGE_SECTION_HEADER))];
                GCHandle handle3 = GCHandle.Alloc(buffer3, GCHandleType.Pinned);
                Marshal.StructureToPtr(lsect[i], handle3.AddrOfPinnedObject(), false);
                handle3.Free();
                //В конце получаем buffer3 c байтами нашей структуры
                fs.Write(buffer3, 0, buffer3.Length);
                //считали из очередной структуры IMAGE_SECTION_HEADER секции и записали
            }

            fs.Write(al_hzch, 0, (int)al_hzch.Length);
            fs.Write(al_sc, 0, (int)al_sc.Length);

            fs.Close();

            //Еще можно воспользоваться BinaryReader
            //using (FileStream fs = new FileStream("1.txt", FileMode.Open, FileAccess.Read, FileShare.Read))
            //{
            //    using (BinaryReader br = new BinaryReader(fs))
            //    {
            //        myStruct mystruct = new myStruct();
            //        mystruct.a = br.ReadInt32();
            //        mystruct.b = br.ReadBoolean();
            //        //но это будет достаточно медленно, но тоже вариант и выглядит красивее
            //    }
            //}
        }

        private void button2_Click(object sender, EventArgs e)
        {
            lsect.Clear();
            FileStream fs = new FileStream("Sample_exe\\explorer.exe", FileMode.Open, FileAccess.Read, FileShare.Read);

            s = pe_hlp.ReadImDosHeader(fs);
            //считали из файла в структуру IMAGE_DOS_HEADER

            uint size_dos_stub = s.e_lfanew - s.e_lfarlc;//определили величину стаба
            byte[] stub = new byte[size_dos_stub];
            fs.Read(stub, 0, (int)size_dos_stub);//поместили в stub стаб

            nt_h = pe_hlp.ReadImNtHeaders(fs);
            //считали из файла в структуру IMAGE_NT_HEADERS
            //теперь указатель чтения на позиции начала секций
            DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0);
            dt = dt.AddSeconds(nt_h.FileHeader.TimeDateStamp);

            MachineType mt = (MachineType)nt_h.FileHeader.Machine;
            string str = mt.ToString();
            //0x00006000+0x00400000

            for (int i = 0; i < (int)nt_h.FileHeader.NumberOfSections; i++)
            {
                IMAGE_SECTION_HEADER sc_h = new IMAGE_SECTION_HEADER();
                sc_h = pe_hlp.ReadImSecHeaders(fs);
                string section_name = Encoding.ASCII.GetString(sc_h.Name);
                lsect.Add(sc_h);
                //считали из файла в структуру IMAGE_SECTION_HEADER очередную секцию
            }
            
            //Итак, мы прочитали заголовки...
            uint align = nt_h.OptionalHeader.SizeOfHeaders - (uint)fs.Position;
            al_hzch = new byte[align];
            fs.Read(al_hzch, 0, (int)align);//почитали байты до начала секций
            //А мона было воспользоваться FileAlignment

            //Читаем все секции сразу (чем руковод.: SizeOfHeaders или позицией читателя?)
            ushort cnt_sc = nt_h.FileHeader.NumberOfSections;
            uint point_last_sect = (lsect[cnt_sc - 1].PointerToRawData + lsect[cnt_sc - 1].SizeOfRawData) - nt_h.OptionalHeader.SizeOfHeaders;
            al_sc = new byte[point_last_sect];
            fs.Read(al_sc, 0, (int)point_last_sect);//почитали до конца файла

            fs.Close();

            dosHeader1.PrintIDH(s);
            ntHeaders1.PrintINTH(nt_h);
            sections1.PrintSect(lsect);
        }
    }
}