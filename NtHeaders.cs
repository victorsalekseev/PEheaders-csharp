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
    public partial class NtHeaders : UserControl
    {
        public NtHeaders()
        {
            InitializeComponent();
        }

        public void PrintINTH(object _inth)
        {
            listViewDd.Items.Clear();
            IMAGE_NT_HEADERS inth = (IMAGE_NT_HEADERS)_inth;

            //IMAGE_NT_HEADERS
            textBoxSignature.Text = "0x" + inth.Signature.ToString("X8");
            
            //IMAGE_FILE_HEADER
            if (System.Runtime.InteropServices.Marshal.SizeOf(inth.FileHeader) != (int)SizeA.IMAGE_SIZEOF_FILE_HEADER)
            {
                MessageBox.Show("Внимание! Размер структуры IMAGE_FILE_HEADER отличается от 20 байт: " + System.Runtime.InteropServices.Marshal.SizeOf(inth.FileHeader).ToString());
            }
            textBoxCharacteristics.Text = "0x" + inth.FileHeader.Characteristics.ToString("X4");
            textBoxMachine.Text = "0x" + inth.FileHeader.Machine.ToString("X4");
            textBoxNumberOfSections.Text = "0x" + inth.FileHeader.NumberOfSections.ToString("X4");
            textBoxNumberOfSymbols.Text = "0x" + inth.FileHeader.NumberOfSymbols.ToString("X8");
            textBoxPointerToSymbolTable.Text = "0x" + inth.FileHeader.PointerToSymbolTable.ToString("X8");
            textBoxSizeOfOptionalHeader.Text = "0x" + inth.FileHeader.SizeOfOptionalHeader.ToString("X4");
            textBoxTimeDateStamp.Text = "0x" + inth.FileHeader.TimeDateStamp.ToString("X8");

            //IMAGE_OPTIONAL_HEADER32
            //Стандартные поля
            textBoxMagic.Text = "0x" + inth.OptionalHeader.Magic.ToString("X4");
            textBoxMajorLinkerVersion.Text = inth.OptionalHeader.MajorLinkerVersion.ToString();
            textBoxMinorLinkerVersion.Text = inth.OptionalHeader.MinorLinkerVersion.ToString();
            textBoxSizeOfCode.Text = "0x" + inth.OptionalHeader.SizeOfCode.ToString("X8");
            textBoxSizeOfInitializedData.Text = "0x" + inth.OptionalHeader.SizeOfInitializedData.ToString("X8");
            textBoxSizeOfUninitializedData.Text = "0x" + inth.OptionalHeader.SizeOfUninitializedData.ToString("X8");
            textBoxAddressOfEntryPoint.Text = "0x" + inth.OptionalHeader.AddressOfEntryPoint.ToString("X8");
            textBoxBaseOfCode.Text = "0x" + inth.OptionalHeader.BaseOfCode.ToString("X8");
            textBoxBaseOfData.Text = "0x" + inth.OptionalHeader.BaseOfData.ToString("X8");
            //Дополнительные поля
            textBoxImageBase.Text = "0x" + inth.OptionalHeader.ImageBase.ToString("X8");
            textBoxSectionAlignment.Text = "0x" + inth.OptionalHeader.SectionAlignment.ToString("X8");
            textBoxFileAlignment.Text = "0x" + inth.OptionalHeader.FileAlignment.ToString("X8");
            textBoxMajorOperatingSystemVersion.Text = "0x" + inth.OptionalHeader.MajorOperatingSystemVersion.ToString("X4");
            textBoxMinorOperatingSystemVersion.Text = "0x" + inth.OptionalHeader.MinorOperatingSystemVersion.ToString("X4");
            textBoxMajorImageVersion.Text = "0x" + inth.OptionalHeader.MajorImageVersion.ToString("X4");
            textBoxMinorImageVersion.Text = "0x" + inth.OptionalHeader.MinorImageVersion.ToString("X4");
            textBoxMajorSubsystemVersion.Text = "0x" + inth.OptionalHeader.MajorSubsystemVersion.ToString("X4");
            textBoxMinorSubsystemVersion.Text = "0x" + inth.OptionalHeader.MinorSubsystemVersion.ToString("X4");
            textBoxWin32VersionValue.Text = "0x" + inth.OptionalHeader.Win32VersionValue.ToString("X8");
            textBoxSizeOfImage.Text = "0x" + inth.OptionalHeader.SizeOfImage.ToString("X8");
            textBoxSizeOfHeaders.Text = "0x" + inth.OptionalHeader.SizeOfHeaders.ToString("X8");
            textBoxCheckSum.Text = "0x" + inth.OptionalHeader.CheckSum.ToString("X8");
            textBoxSubsystem.Text = "0x" + inth.OptionalHeader.Subsystem.ToString("X4");
            textBoxDllCharacteristics.Text = "0x" + inth.OptionalHeader.DllCharacteristics.ToString("X4");
            textBoxSizeOfStackReserve.Text = "0x" + inth.OptionalHeader.SizeOfStackReserve.ToString("X8");
            textBoxSizeOfStackCommit.Text = "0x" + inth.OptionalHeader.SizeOfStackCommit.ToString("X8");
            textBoxSizeOfHeapReserve.Text = "0x" + inth.OptionalHeader.SizeOfHeapReserve.ToString("X8");
            textBoxSizeOfHeapCommit.Text = "0x" + inth.OptionalHeader.SizeOfHeapCommit.ToString("X8");
            textBoxLoaderFlags.Text = "0x" + inth.OptionalHeader.LoaderFlags.ToString("X8");
            textBoxNumberOfRvaAndSizes.Text = "0x" + inth.OptionalHeader.NumberOfRvaAndSizes.ToString("X8");

            if (inth.OptionalHeader.NumberOfRvaAndSizes != (int)SizeA.COUNT_DATA_DYRECTORY)
            {
                MessageBox.Show("Внимание! NumberOfRvaAndSizes отличается от стандартного в 0x0010: " + inth.OptionalHeader.NumberOfRvaAndSizes);
            }

            for (int i = 0; i < (int)SizeA.COUNT_DATA_DYRECTORY; i++)
            {
                ListViewItem lvi = new ListViewItem(string.Format("{0}",(DataDirectory)i));
                lvi.SubItems.Add("0x" + inth.OptionalHeader.DataDirectory[i].VirtualAddress.ToString("X8"));
                lvi.SubItems.Add("0x" + inth.OptionalHeader.DataDirectory[i].Size.ToString("X8"));
                listViewDd.Items.Add(lvi);
            }
        }
    }
}
