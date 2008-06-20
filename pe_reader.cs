using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;

namespace Netcode.pe
{
    /* nsg.exe
    Offset      0  1  2  3  4  5  6  7   8  9  A  B  C  D  E  F
    00000000   4D 5A 90 00 03 00 00 00  04 00 00 00 FF FF 00 00   MZ�.........��..  -|
    00000010   B8 00 00 00 00 00 00 00  40 00 00 00 00 00 00 00   �.......@.......   |DOS
    00000020   00 00 00 00 00 00 00 00  00 00 00 00 00 00 00 00   ................   |���������
    00000030   00 00 00 00 00 00 00 00  00 00 00 00 B0 00 00 00   ............�...  -|
    00000040   0E 1F BA 0E 00 B4 09 CD  21 B8 01 4C CD 21 54 68   ..�..�.�!�.L�!Th  -|
    00000050   69 73 20 70 72 6F 67 72  61 6D 20 63 61 6E 6E 6F   is program canno   |
    00000060   74 20 62 65 20 72 75 6E  20 69 6E 20 44 4F 53 20   t be run in DOS    |
    00000070   6D 6F 64 65 2E 0D 0D 0A  24 00 00 00 00 00 00 00   mode....$.......   |����
    00000080   5D CF 9F 87 19 AE F1 D4  19 AE F1 D4 19 AE F1 D4   ]ϟ�.���.���.���   |
    00000090   97 B1 E2 D4 13 AE F1 D4  E5 8E E3 D4 18 AE F1 D4   ����.������.���   |
    000000A0   52 69 63 68 19 AE F1 D4  00 00 00 00 00 00 00 00   Rich.���........  -|
    000000B0   50 45 00 00 4C 01 02 00  1A A9 53 48 00 00 00 00   PE..L....�SH....  -- PE-���������
    */
    [StructLayout(LayoutKind.Sequential)]
    struct IMAGE_DOS_HEADER
    {
        public ushort e_magic;		// ��������� ��������� MZ Mark Zbinovski
        public ushort e_cblp;		// ���������� ���� �� ��������� �������� �����
        public ushort e_cp;		// ���������� ������� � �����
        public ushort e_crlc;		// Relocations
        public ushort e_cparhdr;		// ������ ��������� (IMAGE_DOS_HEADER) � ���������� (1 �������� == 16 ����) ��� �����
        public ushort e_minalloc;		// ����������� �������������� ���������
        public ushort e_maxalloc;		// ������������ �������������� ���������
        public ushort e_ss;		// ���������  ������������� �������� �������� SS
        public ushort e_sp;		// ��������� �������� �������� SP
        public ushort e_csum;		// ����������� �����. ���� � ������ ���� �� ��� ������� ���������
        public ushort e_ip;		// ��������� �������� �������� IP
        public ushort e_cs;		// ��������� ������������� �������� �������� CS
        public ushort e_lfarlc;		// ����� � ����� �� ������� ������������� (MS-DOS Stub Program, ������ �����)
        public ushort e_ovno;		// ���������� ��������
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public ushort[] e_res;		// ���������������[4]
        public ushort e_oemid;		// OEM ������������
        public ushort e_oeminfo;		// OEM ����������
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public ushort[] m;		// ���������������
        public UInt32 e_lfanew;		// ����� � ����� PE-���������. ��������� Windows �� ���� ��������� IMAGE_DOS_HEADER ������������� ������ ����� ������ - e_magic & e_lfanew
    }

    [StructLayout(LayoutKind.Sequential)]
    struct IMAGE_NT_HEADERS 
    {
        /// <summary>
        /// ��������� ����� (PE/NE/LE/LX � ����� ����-������� � ����� \0\0) ��. SignatureINTh
        /// </summary>
        public UInt32 Signature;
        public IMAGE_FILE_HEADER FileHeader;
        public IMAGE_OPTIONAL_HEADER32 OptionalHeader;
    }

    /// <summary>
    /// �������� IMAGE_OPTIONAL_HEADER32.DataDirectory[]
    /// </summary>
    enum DataDirectory
    {
        // �� �������� ���������� Base
        // ������� �������������� ��������
        IMAGE_DIRECTORY_ENTRY_EXPORT        = 0,
        // ������� ������������� ��������
        IMAGE_DIRECTORY_ENTRY_IMPORT        = 1,
        // ������� ��������
        IMAGE_DIRECTORY_ENTRY_RESOURCE      = 2,
        // ������� ����������
        IMAGE_DIRECTORY_ENTRY_EXCEPTION     = 3,
        // ������� ������������
        IMAGE_DIRECTORY_ENTRY_SECURITY      = 4,
        // ������� �������������
        IMAGE_DIRECTORY_ENTRY_BASERELOC     = 5,
        // ���������� �������
        IMAGE_DIRECTORY_ENTRY_DEBUG         = 6,
        // ������ ��������
        IMAGE_DIRECTORY_ENTRY_COPYRIGHT     = 7,
        // �������� �������� (MIPS GP)
        IMAGE_DIRECTORY_ENTRY_GLOBALPTR     = 8,
        // ������� TLS (Thread local storage - ��������� ������ �������)
        IMAGE_DIRECTORY_ENTRY_TLS           = 9,
        // ������� ������������ ��������
        IMAGE_DIRECTORY_ENTRY_LOAD_CONFIG   = 10,
        IMAGE_DIRECTORY_ENTRY_BOUND_IMPORT  = 11, 
        // ������� ������� �������
        IMAGE_DIRECTORY_ENTRY_IAT           = 12, 
        IMAGE_DIRECTORY_ENTRY_DELAY_IMPORT  = 13,
        // ���������� COM ��������
        IMAGE_DIRECTORY_ENTRY_COM_DESCRIPTOR= 14,
        // ���������, �� ���.   
        IMAGE_DIRECTORY_RESERVED            = 15
    }

    /// <summary>
    /// IMAGE_FILE_HEADER.Machine
    /// </summary>
    enum MachineType
    {
        IMAGE_FILE_MACHINE_UNKNOWN          = 0,
        IMAGE_FILE_MACHINE_I386             = 0x014c,  // Intel 386.
        IMAGE_FILE_MACHINE_R3000            = 0x0162,  // MIPS little-endian, 0x160 big-endian
        IMAGE_FILE_MACHINE_R4000            = 0x0166,  // MIPS little-endian
        IMAGE_FILE_MACHINE_R10000           = 0x0168,  // MIPS little-endian
        IMAGE_FILE_MACHINE_WCEMIPSV2        = 0x0169,  // MIPS little-endian WCE v2
        IMAGE_FILE_MACHINE_ALPHA            = 0x0184,  // Alpha_AXP
        IMAGE_FILE_MACHINE_SH3              = 0x01a2,  // SH3 little-endian
        IMAGE_FILE_MACHINE_SH3DSP           = 0x01a3,
        IMAGE_FILE_MACHINE_SH3E             = 0x01a4,  // SH3E little-endian
        IMAGE_FILE_MACHINE_SH4              = 0x01a6,  // SH4 little-endian
        IMAGE_FILE_MACHINE_SH5              = 0x01a8,  // SH5
        IMAGE_FILE_MACHINE_ARM              = 0x01c0,  // ARM Little-Endian
        IMAGE_FILE_MACHINE_THUMB            = 0x01c2,
        IMAGE_FILE_MACHINE_AM33             = 0x01d3,
        IMAGE_FILE_MACHINE_POWERPC          = 0x01F0,  // IBM PowerPC Little-Endian
        IMAGE_FILE_MACHINE_POWERPCFP        = 0x01f1,
        IMAGE_FILE_MACHINE_IA64             = 0x0200,  // Intel 64
        IMAGE_FILE_MACHINE_MIPS16           = 0x0266,  // MIPS
        IMAGE_FILE_MACHINE_ALPHA64          = 0x0284,  // ALPHA64
        IMAGE_FILE_MACHINE_MIPSFPU          = 0x0366,  // MIPS
        IMAGE_FILE_MACHINE_MIPSFPU16        = 0x0466,  // MIPS
        IMAGE_FILE_MACHINE_AXP64            = IMAGE_FILE_MACHINE_ALPHA64,
        IMAGE_FILE_MACHINE_TRICORE          = 0x0520,  // Infineon
        IMAGE_FILE_MACHINE_CEF              = 0x0CEF,
        IMAGE_FILE_MACHINE_EBC              = 0x0EBC,  // EFI Byte Code
        IMAGE_FILE_MACHINE_AMD64            = 0x8664,  // AMD64 (K8)
        IMAGE_FILE_MACHINE_M32R             = 0x9041,  // M32R little-endian
        IMAGE_FILE_MACHINE_CEE              = 0xC0EE
    }

    /// <summary>
    /// IMAGE_FILE_HEADER.Characteristics
    /// </summary>
    enum FileCharacteristics
    {
        IMAGE_FILE_RELOCS_STRIPPED          = 0x0001,  // Relocation info stripped from file.
        IMAGE_FILE_EXECUTABLE_IMAGE         = 0x0002,  // File is executable  (i.e. no unresolved externel references).
        IMAGE_FILE_LINE_NUMS_STRIPPED       = 0x0004,  // Line nunbers stripped from file.
        IMAGE_FILE_LOCAL_SYMS_STRIPPED      = 0x0008,  // Local symbols stripped from file.
        IMAGE_FILE_AGGRESIVE_WS_TRIM        = 0x0010,  // Agressively trim working set
        IMAGE_FILE_LARGE_ADDRESS_AWARE      = 0x0020,  // App can handle >2gb addresses
        IMAGE_FILE_BYTES_REVERSED_LO        = 0x0080,  // Bytes of machine word are reversed.
        IMAGE_FILE_32BIT_MACHINE            = 0x0100,  // 32 bit word machine.
        IMAGE_FILE_DEBUG_STRIPPED           = 0x0200,  // Debugging info stripped from file in .DBG file
        IMAGE_FILE_REMOVABLE_RUN_FROM_SWAP  = 0x0400,  // If Image is on removable media, copy and run from the swap file.
        IMAGE_FILE_NET_RUN_FROM_SWAP        = 0x0800,  // If Image is on Net, copy and run from the swap file.
        IMAGE_FILE_SYSTEM                   = 0x1000,  // System File.
        IMAGE_FILE_DLL                      = 0x2000,  // File is a DLL.
        IMAGE_FILE_UP_SYSTEM_ONLY           = 0x4000,  // File should only be run on a UP machine
        IMAGE_FILE_BYTES_REVERSED_HI        = 0x8000  // Bytes of machine word are reversed.
    }

    /// <summary>
    /// �������� IMAGE_SECTION_HEADER.Characteristics
    /// </summary>
    enum SectionCharacteristics
    {
         /*
         Section characteristics.
         i.e.:
         characteristics:    C0000080
         UNINITIALIZED_DATA MEM_READ MEM_WRITE
         */

       //IMAGE_SCN_TYPE_REG                  = 0x00000000,  // Reserved.
       //IMAGE_SCN_TYPE_DSECT                = 0x00000001,  // Reserved.
       //IMAGE_SCN_TYPE_NOLOAD               = 0x00000002,  // Reserved.
       //IMAGE_SCN_TYPE_GROUP                = 0x00000004,  // Reserved.
         IMAGE_SCN_TYPE_NO_PAD               = 0x00000008,  // Reserved.
       //IMAGE_SCN_TYPE_COPY                 = 0x00000010,  // Reserved.

         IMAGE_SCN_CNT_CODE                  = 0x00000020,  // ��� ������ �������� ����������� ���. 
                                                            // ��� �������, ��������������� ������ � ������ (0�80000000)
         IMAGE_SCN_CNT_INITIALIZED_DATA      = 0x00000040,  // ������ ������ �������� ������������������ ������. 
                                                            // ����� ��� ���� ������, ����� �����������
                                                            // � .bss ������, ���� ���� ����������
         IMAGE_SCN_CNT_UNINITIALIZED_DATA    = 0x00000080,  // ������ ������ �������� ��������������������
                                                            // ������ (��������, .bss ������)

         IMAGE_SCN_LNK_OTHER                 = 0x00000100,  // Reserved.
         IMAGE_SCN_LNK_INFO                  = 0x00000200,  // ������ ������ �������� ����������� ��� �����-������
                                                            // ������ ��� ����������. �������� ������������� �����
                                                            // ������ � ��� ������ .drectve, ����������� ������������
                                                            // � ���������� ������� ��� ������������
       //IMAGE_SCN_TYPE_OVER                 = 0x00000400,  // Reserved.
         IMAGE_SCN_LNK_REMOVE                = 0x00000800,  // ���������� ������ ������ �� ������ ���� ��������
                                                            // � �������� ���-����. ����� ������ ������������
                                                            // ������������/����������� ��� �������� ���������� ������������
         IMAGE_SCN_LNK_COMDAT                = 0x00001000,  // Section contents comdat.
       //                                    = 0x00002000,  // Reserved.
       //IMAGE_SCN_MEM_PROTECTED - Obsolete  = 0x00004000,
         IMAGE_SCN_NO_DEFER_SPEC_EXC         = 0x00004000,  // Reset speculative exceptions handling bits in the TLB entries for this section.
         IMAGE_SCN_GPREL                     = 0x00008000,  // Section content can be accessed relative to GP
         IMAGE_SCN_MEM_FARDATA               = 0x00008000,
       //IMAGE_SCN_MEM_SYSHEAP  - Obsolete   = 0x00010000,
         IMAGE_SCN_MEM_PURGEABLE             = 0x00020000,
         IMAGE_SCN_MEM_16BIT                 = 0x00020000,
         IMAGE_SCN_MEM_LOCKED                = 0x00040000,
         IMAGE_SCN_MEM_PRELOAD               = 0x00080000,

         IMAGE_SCN_ALIGN_1BYTES              = 0x00100000,  //
         IMAGE_SCN_ALIGN_2BYTES              = 0x00200000,  //
         IMAGE_SCN_ALIGN_4BYTES              = 0x00300000,  //
         IMAGE_SCN_ALIGN_8BYTES              = 0x00400000,  //
         IMAGE_SCN_ALIGN_16BYTES             = 0x00500000,  // Default alignment if no others are specified.
         IMAGE_SCN_ALIGN_32BYTES             = 0x00600000,  //
         IMAGE_SCN_ALIGN_64BYTES             = 0x00700000,  //
         IMAGE_SCN_ALIGN_128BYTES            = 0x00800000,  //
         IMAGE_SCN_ALIGN_256BYTES            = 0x00900000,  //
         IMAGE_SCN_ALIGN_512BYTES            = 0x00A00000,  //
         IMAGE_SCN_ALIGN_1024BYTES           = 0x00B00000,  //
         IMAGE_SCN_ALIGN_2048BYTES           = 0x00C00000,  //
         IMAGE_SCN_ALIGN_4096BYTES           = 0x00D00000,  //
         IMAGE_SCN_ALIGN_8192BYTES           = 0x00E00000,  //
       //Unused                              = 0x00F00000,
         IMAGE_SCN_ALIGN_MASK                = 0x00F00000,

         IMAGE_SCN_LNK_NRELOC_OVFL           = 0x01000000,  // Section contains extended relocations.
         IMAGE_SCN_MEM_DISCARDABLE           = 0x02000000,  // ������ ������ ����� ���������, ��� ��� ���
                                                            // �� ������������ ����������, ����� ���� ��� ���������
                                                            // ���������. ���� ����� ����������� ������������� ������
                                                            // � ��� ������ ������� �������� (.reloc)
         IMAGE_SCN_MEM_NOT_CACHED            = 0x04000000,  // Section is not cachable.
         IMAGE_SCN_MEM_NOT_PAGED             = 0x08000000,  // Section is not pageable.
         IMAGE_SCN_MEM_SHARED                = 0x10000000,  // ������ ������ �������� ��������� ������������.
                                                            // ��� ������������� � DLL ������ � ����� ������ ������������
                                                            // ��������� ����� ����������, ������������� ��� DLL.
                                                            // �� ��������� ������ ������ �� �������� ��������� �������������,
                                                            // �.�. ������ �������, ������������ DLL, ����� ���� �����������
                                                            // ��������� ����� ����� ������ ������. ������
                                                            // ����� ����������� ������, ��������� ������������
                                                            // ������ ���� �������� ��������� ������ �������������
                                                            // ����������� ������� ��� ���� ������ ���, ��� ���
                                                            // ��������, ������������ DLL, ��������� �� ���� �
                                                            // �� �� ���������� �������� � ������. ����� �������
                                                            // ������ ��������� ������������, ���������� �������
                                                            // SHARED �� ����� ����������.
                                                            // ��������: LINK/SECTION:MYDATA,RWS...
                                                            // ��������� ������������, ��� ������ � ��������� MYDATA
                                                            // ������ ���� ��������� ��� ������, ������ � ���������
                                                            // ������������. �� ��������� �������� ������
                                                            // DLL Borland C++ ����� �������� ����������� �������������
         IMAGE_SCN_MEM_EXECUTE               = 0x20000000,  // ������ ������ �������� �����������. ���� ���� ������
                                                            // ��������������� ������ ���, ����� ���������������
                                                            // ���� "���������" (Contains Code) (0�00000020)
         IMAGE_SCN_MEM_READ                  = 0x40000000   // ������ ������ ������������� ��� ������. ���� ����
                                                            // ����� ������ ���������� ��� ������ ���-������
       //IMAGE_SCN_MEM_WRITE                 = 0x80000000   // ������ ������ ������������� ��� ������. ���� ����
                                                            // ���� �� ���������� � ������ ���-�����, ���������
                                                            // ������ �������� ������������ � ������ �������� ���
                                                            // ��������������� ������ ��� ������ ��� ������
                                                            // ��� ����������. �������� ������ � ���� ���������
                                                            // � ��� .data � .bss
    }
    enum SizeA
    {
        IMAGE_SIZEOF_FILE_HEADER = 20,
        COUNT_DATA_DYRECTORY     = 16,
        IMAGE_SIZEOF_SECTION_HEADER =40,
        IMAGE_SIZEOF_SHORT_NAME = 8
    }

    /// <summary>
    /// �������� IMAGE_OPTIONAL_HEADER32.Subsystem
    /// </summary>
    enum Subsystem
    {
        IMAGE_SUBSYSTEM_UNKNOWN             = 0,   // Unknown subsystem.
        IMAGE_SUBSYSTEM_NATIVE              = 1,   // Image doesn't require a subsystem.
        IMAGE_SUBSYSTEM_WINDOWS_GUI         = 2,   // Image runs in the Windows GUI subsystem.
        IMAGE_SUBSYSTEM_WINDOWS_CUI         = 3,   // Image runs in the Windows character subsystem.
        IMAGE_SUBSYSTEM_OS2_CUI             = 5,   // image runs in the OS/2 character subsystem.
        IMAGE_SUBSYSTEM_POSIX_CUI           = 7,   // image runs in the Posix character subsystem.
        IMAGE_SUBSYSTEM_NATIVE_WINDOWS      = 8,   // image is a native Win9x driver.
        IMAGE_SUBSYSTEM_WINDOWS_CE_GUI      = 9,   // Image runs in the Windows CE subsystem.
        IMAGE_SUBSYSTEM_EFI_APPLICATION     = 10,  //
        IMAGE_SUBSYSTEM_EFI_BOOT_SERVICE_DRIVER = 11,   //
        IMAGE_SUBSYSTEM_EFI_RUNTIME_DRIVER  = 12,  //
        IMAGE_SUBSYSTEM_EFI_ROM             = 13,
        IMAGE_SUBSYSTEM_XBOX                = 14
    }

    /// <summary>
    /// �������� IMAGE_OPTIONAL_HEADER32.DllCharacteristics
    /// </summary>
    enum DllCharacteristics
    {
        //IMAGE_LIBRARY_PROCESS_INIT          = 0x0001,     // Reserved.
        //IMAGE_LIBRARY_PROCESS_TERM          = 0x0002,     // Reserved.
        //IMAGE_LIBRARY_THREAD_INIT           = 0x0004,     // Reserved.
        //IMAGE_LIBRARY_THREAD_TERM           = 0x0008,     // Reserved.
        IMAGE_DLLCHARACTERISTICS_NO_ISOLATION = 0x0200,     // Image understands isolation and doesn't want it
        IMAGE_DLLCHARACTERISTICS_NO_SEH       = 0x0400,     // Image does not use SEH.  No SE handler may reside in this image
        IMAGE_DLLCHARACTERISTICS_NO_BIND      = 0x0800,     // Do not bind this image.
        //                                    = 0x1000,     // Reserved.
        IMAGE_DLLCHARACTERISTICS_WDM_DRIVER   = 0x2000,     // Driver uses WDM model
        //                                    = 0x4000,     // Reserved.
        IMAGE_DLLCHARACTERISTICS_TERMINAL_SERVER_AWARE   = 0x8000
    }

    /// <summary>
    /// �������� IMAGE_FILE_HEADER.Signature
    /// </summary>
    enum SignatureINTh
    {
        PE = 0x00004550,
        NE = 0x0000454E,
        LE = 0x0000454C,
        LX = 0x0000584C
    }

    /// <summary>
    /// �������� IMAGE_OPTIONAL_HEADER32.Magic
    /// </summary>
    enum MagicIOH32
    {
        IMAGE_NT_OPTIONAL_HDR_MAGIC  = 0x010B, //���������� ����������� ����������� (�������� ��� ������� ����� ������)
 	    IMAGE_ROM_OPTIONAL_HDR_MAGIC = 0x0107, //����������� ��� 
	    IMAGE_NT_OPTIONAL_HDR32_MAGIC = 0x010B, 
	    IMAGE_NT_OPTIONAL_HDR64_MAGIC = 0x020B 
    }

    [StructLayout(LayoutKind.Sequential)]
    struct IMAGE_FILE_HEADER 
    {
        /// <summary>
        /// ����������� ���������, ��� �������� ������������ ���� (��. MachineType)
        /// </summary>
        public ushort Machine;
        /// <summary>
        /// ���������� ������ � ���- ��� OBJ-�����
        /// </summary>
        public ushort NumberOfSections;
        /// <summary>
        /// ����� �������� �����. ���������� ������, �������� � 16:00 31 ������� 1969 ����
        /// </summary>
        public UInt32 TimeDateStamp;
        /// <summary>
        /// �������� �������� COFF-������� ��������. ��� ���� ������������ ������ � OBJ- � ��-������
        /// � ����������� COFF-���������. ��-����� ������������ ������������� ���������� �������,
        /// ��� ��� ��������� ������ ��������� �� ����� IMAGE_DIRECTORY_ENTRY_DEBUG � �������� ������
        /// (��������� � DataDirectoryNames)
        /// </summary>
        public UInt32 PointerToSymbolTable;
        /// <summary>
        /// ���������� �������� � COFF-������� ��������
        /// </summary>
        public UInt32 NumberOfSymbols;
        /// <summary>
        /// ������ ��������������� ���������, ������� ����� ��������� �� ���� ����������.
        /// � ����������� ������ � ��� ������ ��������� IMAGE_OPTIONAL_HEADER, ������� �������
        /// �� ���� ����������. � ��������� ������, �� ����������� Microsoft,
        /// ��� ���� ������ �������� ����. ������ ��� ��������� ���������� ������ KERNEL32.LIB
        /// ����� ���������� ��������� ���� � ��������� ��������� � ���� ����, ��� ��� ����������
        /// � ������������ Microsoft � ��������� ������������.
        /// </summary>
        public ushort SizeOfOptionalHeader;
        /// <summary>
        /// Characteristics, �������� ������������� �������������� ����� (��. FileCharacteristics),
        /// ��������, IMAGE_FILE_DEBUG_STRIPPED
        /// </summary>
        public ushort Characteristics;
    }

    /* �������
     * Relative Virtual Address - ������ ���� � ��-������ �������� ������ � ������� �� RVA.
     * RVA � ��� ������ �������� ������� �������� �� ��������� � ������, � �������� ����������
     * ����������� ����� � ������.
     * 
     * (����������� ����� 0�401464) - (������� ����� 0�400000) = RVA 0�1464
     * 
     * 
     * 
    */
    [StructLayout(LayoutKind.Sequential)]  
    struct IMAGE_OPTIONAL_HEADER32
    {
        // ����������� ����
        /// <summary>
        /// �����-���������, ������������ ��������� ������������� ����� (��. MagicIOH32)
        /// </summary>
        public ushort Magic;
        /// <summary>
        /// ������� ������ ������������, ������� ������ ������ ����.
        /// ����� ������ ���� ������������ � ���������� ����, � �� � �����������������.
        /// </summary>
        public byte   MajorLinkerVersion;
        /// <summary>
        /// ������� ������ ������������, ������� ������ ������ ����.
        /// ����� ������ ���� ������������ � ���������� ����, � �� � �����������������.
        /// </summary>
        public byte   MinorLinkerVersion;
        /// <summary>
        /// ��������� ������ ����������� ������, ����������� � ������� �������.
        /// ������ ����������� ������ ����� ������ ���� ����������� ������, 
        /// ��� ��� ��� ���� ������ ������������� ������� ������ .text.
        /// </summary>
        public UInt32 SizeOfCode;
        /// <summary>
        /// ��������������, ��� ��� ����� ������ ���� ������, ��������� �� ������������������ ������
        /// (�� ������� �������� ������������ ����.) ������ �� ������, ����� ��� ��������� � �������� 
        /// ������ ������������������ ������ � �����.
        /// </summary>
        public UInt32 SizeOfInitializedData;
        /// <summary>
        /// ������ ������, ��� ������� ��������� �������� ����� � ����������� �������� ������������,
        /// �� ������� �� �������� �������� ����� � �������� �����. � ������ ������ ��������� ��� ������
        /// �� ������� ����� �����-���� ������������ �������� � ������ �������� �������������������� ������
        /// (Uninitialized Data). �������������������� ������ ������ ��������� � ������ ��� ��������� .bss.
        /// </summary>
        public UInt32 SizeOfUninitializedData;
        /// <summary>
        /// �����, � �������� ����������� �������� ����������.
        /// ��� RVA, ������� ����� ����� � ������ .text.
        /// ��� ���� ��������� ��� ��� ���-�����, ��� � ��� DLL.
        /// </summary>
        public UInt32 AddressOfEntryPoint;
        /// <summary>
        /// RVA, � �������� ���������� ����������� ������ �����. 
        /// ����������� ������ ���� ������ ���� � ������ ����� ��������
        /// ������ � ����� ��������� ��-�����. ���� RVA ������ ����� 0�1000 ��� ���-������, 
        /// ��������� �������������� Microsoft. ��� TLINK32 (Borland) �������� ����� ���� ����� 0�10000,
        /// ��� ��� �� ��������� ���� ����������� ����������� ������� �� ������� � 64 ����� � ������� �� 4 �����
        /// � ������ ������������ Microsoft.
        /// </summary>
        public UInt32 BaseOfCode;
        /// <summary>
        /// RVA, � �������� ���������� ������ ������ �����. 
        /// ������ ������ ������ ���� ���������� � ������,
        /// ����� ��������� ��-����� � ����������� ������.
        /// </summary>
        public UInt32 BaseOfData;

        //NT �������������� ����.
        /// <summary>
        /// ����� ����������� ������� ����������� ����, �� ������������, ��� ���� ����� ���������
        /// � ������������ ����� � ������. ��� ������ ���� ����� � �������� � ���� ����. ������ ������ ��������
        /// ��������� ������������ �������� �����������. ���� ��������� ������������� ��������� ���� � ������
        /// �� ����� ������, �� ��������� ����� �������� �� ��������� �� � ����� ���������. � ����������� ������ NT 3.1
        /// ����� ����������� �� ��������� ����� 0�10000. � ������ DLL ���� ����� �� ��������� ����� 0�400000.
        /// � Windows 95 ����� 0�10000 ������ ������������ ��� �������� 32-��������� ������ ���, ��� ��� �� �����
        /// � �������� �������� ������� ��������� ������������, ������ ��� ���� ���������. 
        /// ������� ��� Windows NT 3.5 Microsoft �������� ��� ����������� ������ Win32 ������� ����� �� ���������,
        /// ������ ��� ������ 0�400000. ����� ������ ���������, ������� ���� ������������ � �������������, ��� 
        /// ������� ����� ����� 0�10000, ����������� Windows 95 ������, ������ ��� ��������� ������ ���������
        /// ������� ��������. 
        /// </summary>
        public UInt32 ImageBase;
        /// <summary>
        /// �������� ����������� � �������� ������������ �������� ���������������, ������� � ImageBase.
        /// SectionAlignment ������������ ����������� ������, ������� ������� ����� ������ ��� ��������
        /// - ��� ��� �������� ����������� ������������ �� ������� SectionAlignment.
        /// ������������ �������� �� ����� ���� ������ ������� �������� (� ��������� ������ 4096 ����
        /// �� ��������� x86), � ������ ���� ������ ������� ��������, ��� ������������ ��������� ���������
        /// ����������� ������ Windows NT. 4096 ���� �������� ��������� �� ���������, �� ����� ���� ����������� 
        /// ����� ������ ��������, ��������� ����� ���������� -ALIGN:
        /// 
        /// ����� ����������� � ������ ������ ������ ����� ����������� ���������� � ������������ ������,
        /// �������� ������ ��������. � ������ �������� ������� ����������� �������� ����� ���� 0�1000
        /// ������������ ������������� Microsoft �� ���������. TLINK � Borland C++ ���������� 
        /// �� ��������� 0�10000 (64 �����)
        /// </summary>
        public UInt32 SectionAlignment;
        /// <summary>
        /// � ������ ��-����� �������� ������, ������� ������ � ������ ������ ������,
        /// ����� ����������� ���������� � ������, �������� ������ ��������. 
        /// ��������, ��������������� �� ���������, ����� 0�200 ���� �, ��������, 
        /// ������� ��� ��� ����, ����� ������ ������ ������ ��������� � ������� ��������� �������
        /// (0�200 ���� � ��� ��� ��� ������ ��������� �������). ��� ���� ������������ ������� 
        /// ������������ ��������/������� � NE-������. � ������� �� NE-������, ��-����� �� �������
        /// �� ����� ������, ��� ��� ������, �������� ��� ������������ ������ �����, ������ ����� �������������.
        /// </summary>
        public UInt32 FileAlignment;
        /// <summary>
        /// ����� ������ ������ (Major) ������������ �������, ������� ����� ������������ ������ ����������� ����.
        /// ���������� ����� ���� �� ������ ����, ��� ��� ���� ���������� (��������� ����), ������, ����� 
        /// ����� �� ��������������. � ������� ����� ������ Win32 � ���� ���� ���������� ��������, 
        /// ��������������� ������ 1.0
        /// </summary>
        public ushort MajorOperatingSystemVersion;
        /// <summary>
        /// ����� ������ ������ (Minor) ������������ �������, ������� ����� ������������ ������ ����������� ����.
        /// ���������� ����� ���� �� ������ ����, ��� ��� ���� ���������� (��������� ����), ������, ����� 
        /// ����� �� ��������������. � ������� ����� ������ Win32 � ���� ���� ���������� ��������, 
        /// ��������������� ������ 1.0
        /// </summary>
        public ushort MinorOperatingSystemVersion;
        /// <summary>
        /// ������������ ������������� ����. ��� ���� ��������� ����� ��������� ������ ���-������ � DLL.
        /// ��� ���� ��������������� � ������� ����� ������������ /VERSION (Major)
        /// </summary>
        public ushort MajorImageVersion;
        /// <summary>
        /// ������������ ������������� ����. ��� ���� ��������� ����� ��������� ������ ���-������ � DLL.
        /// ��� ���� ��������������� � ������� ����� ������������ /VERSION (Minor)
        /// </summary>
        public ushort MinorImageVersion;
        /// <summary>
        /// ��� Major-���� �������� ����� ������ ������ ����������, ����������� ��������� ������ ����������� ����. 
        /// �������� �������� � ���� ���� 4.0 (���������� Windows 4.0, ��� ����������� Windows 95)
        /// </summary>
        public ushort MajorSubsystemVersion;
        /// <summary>
        /// ��� Minor-���� �������� ����� ������ ������ ����������, ����������� ��������� ������ ����������� ����. 
        /// �������� �������� � ���� ���� 4.0 (���������� Windows 4.0, ��� ����������� Windows 95)
        /// </summary>
        public ushort MinorSubsystemVersion;
        /// <summary>
        /// ��� ����, ��-��������, ������ ����� ����. ���������������
        /// </summary>
        public UInt32 Win32VersionValue;
        /// <summary>
        /// ������������ ����� ������ ���� ������ �����������, ����������� ��� ��������� ����������.
        /// ��� �������� ����� ������� ������� ������, ������� � �������� ������ ����������� � ����������
        /// ������� ����� ��������� ������. ����� ����� ������ �������� �� ��������� ������� ������� ������
        /// </summary>
        public UInt32 SizeOfImage;
        /// <summary>
        /// ������ ��������� ��-����� � ������� ������ (�������). �������� ������ ��� ������ ���������� 
        /// ����� ����� ���� ������������ ������ ���������
        /// </summary>
        public UInt32 SizeOfHeaders;
        /// <summary>
        /// ���������������� �������� ����������� ����� �������� ����������� ���������� ����� (CRC-��������) 
        /// ��� ������� �����. ��� � ��� ������ ����������� �������� Microsoft, ��� ���� ������ ������������
        /// � ��������������� � ����. ������ ��� ���� DLL ���������, DLL, ����������� �� ����� �������� ��,
        /// � ��������� DLL ��� �����������, ����� ������ ����� ���������� ��������. �������� ��� ����������� 
        /// ����� ����� ����� � IMAGEHLP.DLL. ��������� IMAGEHLP.DLL ������������ � WIN32 SDK
        /// </summary>
        public UInt32 CheckSum;
        /// <summary>
        /// ��� ����������, ������� ������ ����������� ���� ���������� ��� ������ ����������������� ����������.
        /// (��. Subsystem)
        /// </summary>
        public ushort Subsystem;
        /// <summary>
        /// ����� ������, ������������, ��� ����� ��������������� ����� ���������� ������� ������������� DLL
        /// (��������, DllMain()). ��� ��������, ��-��������, ������ ������ ��������������� � ����, ������
        /// ������������ ������� �������� ������� ������������� DLL ��� ���� ������� ������� (��. DllCharacteristics)
        /// </summary>
        public ushort DllCharacteristics;
        /// <summary>
        /// ����� ����������� ������, ������������� ��� ��������� ���� �������. ������ �� ��� ��� ������
        /// ���������� (��. ��������� ����). �� ��������� ��� ���� ��������������� � 0�100000 (1 �����).
        /// ���� ������������ ��������� 0 � �������� ������� ����� � CreateThread(), ������������ ������� 
        /// ����� ����� ���� ���� �� �������
        /// </summary>
        public UInt32 SizeOfStackReserve;
        /// <summary>
        /// ���������� ������, ���������� ���������� ��� �������� ���� �������. 
        /// ��� ���� �� ��������� ����� 0�1000 ���� (1 ��������) ��� ������������� Microsoft,
        /// ����� ��� TLINK32 ������ ��� ������ 0�2000 (2 ��������)
        /// </summary>
        public UInt32 SizeOfStackCommit;
        /// <summary>
        /// ����� ����������� ������, ������������� ��� ����������� ���� ���������. 
        /// ���� ���������� ���� ����� ��������, ������ GetProcessHeap(). ������ �� ��� ��� ������ ���������� 
        /// (��. ��������� ����)
        /// </summary>
        public UInt32 SizeOfHeapReserve;
        /// <summary>
        /// ����� ����������� ������, ���������� ���������� ��� ���� ��������. 
        /// �� ��������� ����������� ������ ��� ���� ������ 0�1000 ����
        /// </summary>
        public UInt32 SizeOfHeapCommit;
        /// <summary>
        /// ��������� ��������� ������������ ����� ��������, ������� � ������� ����� ��������,
        /// ���, �������� �� ���������, ������ ��������� ����.
        /// 1 = ��������� �� ������� ���������� ����� �������� ��������?
        /// 2 = ��������� �� �������� ��������� ����� ��������?
        /// </summary>
        public UInt32 LoaderFlags;
        /// <summary>
        /// ���������� ������ � ������� DataDirectory (��. �������� ���������� ����).
        /// ����������� ����������� �������� ������ ������ ��� �������� ������ 16
        /// </summary>
        public UInt32 NumberOfRvaAndSizes;
        /// <summary>
        /// ������ �������� ���� IMAGE_DATA_DIRECTORY. ��������� �������� ������� �������� ��������� RVA � �������
        /// ������ ������ ������������ �����. � ��������� ����� ��������� �������� � ����� ������� �� ������������.
        /// ������ ������� ������� � ��� ������ ����� � ������ ���������������� ������� ������� (���� ���
        /// ������������). ������ ������� ������� � ����� � ������ ��������������� ������� ������� � �.�.
        /// (��. DataDirectory)
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public IMAGE_DATA_DIRECTORY[] DataDirectory;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct IMAGE_DATA_DIRECTORY
    {
        public UInt32 VirtualAddress;
        public UInt32 Size;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct IMAGE_EXPORT_DIRECTORY //������� 0 � IMAGE_DATA_DIRECTORY[]
    {//http://www.firststeps.ru/mfc/winapi/r.php?28
        public UInt32 Characteristics;
        public UInt32 TimeDateStamp;
        public ushort MajorVersion;
        public ushort MinorVersion;
        public UInt32 Name;
        public UInt32 Base;
        public UInt32 NumberOfFunctions;
        public UInt32 NumberOfNames;
        public UInt32 AddressOfFunctions;
        public UInt32 AddressOfNames;//������ ���������� �� ����� �������
        public UInt32 AddressOfNameOrdinals;  
    }

    [StructLayout(LayoutKind.Sequential)]
    struct IMAGE_SECTION_HEADER
    {
        /// <summary>
        /// ��� 8-�������� ��� � ��������� ANSI (�� Unicode), ������� ������� ������. ����������� ���� ������
        /// ���������� � ����� (��������, .text), �� ��� �� �����������, ������� ����, � ��� �������� ��� �������
        /// ��������� ��������� �� ��-������. ������������ ����� ������ ����� ����� ����������� ������� � �������
        /// ���� ���������� ��������� � ����������, ���� � ������� �������� #pragma data_seg � #pragma code_seg
        /// ����������� Microsoft C/C++. (������������ Borland C++ ������ ������������ #pragma codeseg.)
        /// ���������� ��������, ���, ���� ��� ������ �������� 8 ������ ������, ����������� ����������� ���� NULL.
        /// (��������� TDUMP � Borland C++ 4.Ox �������� �� ���� ��� �������������� � �������� ����������� �����
        /// �� ��������� �� ���-������.) ���� �� ����������� printf(), �� ������ ������������ "%.8s", ����� ��
        /// ���������� ������-��� � ������ ����� ��� ���������� �� ������� ������
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] Name;
        /// <summary>
        /// ��� ���� ����� ��������� ���������� � ����������� �� ����, ����������� �� ��� � ���- ��� OBJ-�����. 
        /// � ���-����� ��� �������� ����������� ������ ������ ������������ ���� ��� ������.
        /// ��� ������ �� ���������� �� ��������� ������� ������� �����.
        /// ���� SizeOfRawData ������ � ���� ��������� �������� ��� ����������� ��������.
        /// ���������, ��� Borland TLINK32 ������ ������� �������� ����� ���� � ���� SizeOfRawData �,
        /// ��� �� �����, �������� ���������� �������������. � ������ OBJ-������ ��� ���� ��������� 
        /// ���������� ����� ������. ������ ������ ���������� � ������ 0. ����� �������� ���������� �����
        /// ��������� ������, ���� ��������� �������� � SizeOfRawData � ����������� ������ ������ ������
        /// </summary>
        public UInt32 VirtualSize;
        /// <summary>
        /// � ������ ���-������ ��� ���� �������� RVA, ���� ��������� ������ ���������� ������. 
        /// ����� ��������� �������� ��������� ����� ������ ������ � ������, ���������� � ������������ ������
        /// ������, ������������� � ���� ����, ��������� ������� ����� �����������. 
        /// �������� Microsoft ������������� �� ��������� RVA ������ ������ ������ 0�1000.
        /// ��� ��������� ������ ��� ���� �� ����� �������� ������ � ��������������� � 0
        /// </summary>
        public UInt32 VirtualAddress;
        /// <summary>
        /// � ���-������ ��� ���� �������� ������ ������, ����������� �� ��������� ������� ������� ������� �����.
        /// ��������, ��������, ��� ������ ������������ ����� 0�200. ���� ���� VirtualSize ���������,
        /// ��� ����� ������ 0�35A ����, �� � ������ ���� ����� �������, ��� ������ ������ 0�400 ����.
        /// ��� OBJ-������ ��� ���� �������� ������ ������ ������, ��������������� ������������ ��� �����������.
        /// ������� �������, ��� OBJ-������ ��� ������������ ���� VirtualSize � ���-������
        /// </summary>
        public UInt32 SizeOfRawData;
        /// <summary>
        /// ��� �������� �������� �������, ��� ��������� �������� ������ ��� ������.
        /// ���� ������������ ��� ���������� � ������ ��- ��� COFF-���� (������ ����,
        /// ����� �������� �������� ������������ �������), ��� ���� ������, ��� ���� VirtualAddress.
        /// �������� �������� ��, ��� � ���� ������ ��������� ��������� �������� ����������� ����� �����,
        /// ��� ��� ������ ��� ������ ����� ���������� �� ����� ��������, � �� �� RVA, ����������
        /// � ���� VirtualAddress.
        /// </summary>
        public UInt32 PointerToRawData;
        /// <summary>
        /// � ��������� ������ ��� �������� �������� ���������� � ��������� ��� ������ ������.
        /// ���������� � ��������� � ����� ������ ���������� ����� ������� �� ��������� ������� ��� ���� ������.
        /// � ���-������ ��� (� ���������) ���� �� ����� ��������� �������� � ��������������� � ����.
        /// ����� ����������� ������� ���-����, �� ��������� ����������� ��������,
        /// � �� ����� �������� �������� ��������� ������ ������� �������� �������� � ��������������� �������.
        /// ���������� � ������� ��������� � ��������������� �������� �������� � ������� ������� ��������
        /// � ��������������� �������, ��� ��� ��� ������������� � ���-����� �������� ������ �������� ��� 
        /// ������ ������ ����� �������� ������ ������
        /// </summary>
        public UInt32 PointerToRelocations;
        /// <summary>
        /// �������� �������� ������� ������� �����. ������� ������� ����� ������ � ������������ ������ ����� 
        /// ��������� ����� �������, �� ������� ����� ����� ���, ��������������� ��� ������ ������.
        /// � ����������� ���������� ��������, ����� ��� ������ CodeView, ���������� � ������� �����
        /// �������� ��� ����� ���������� ���������. � ���������� ������� COFF, ������, ���������� 
        /// � ������� ����� ������������� ������� �� ���������� � ���������� ������/�����. 
        /// ������ ������ ������ � ����������� ����� (��������, .text ��� CODE) ����� ������ �����.
        /// � ���-������ ������ ����� ������� � ����� ����� ����� �������� ������ ��� ������.
        /// � ��������� ������ ������� ������� ����� ��� ������ ������� �� ��������� ������� ������ 
        /// � �������� ����������� ��� ���� ������
        /// </summary>
        public UInt32 PointerToLinenumbers;
        /// <summary>
        /// ���������� ����������� � ������� �������� ��� ������ ������ (���� PointerToRelocations ��������� ����).
        /// ��� ���� ������������, ��-��������, ������ � ��������� ������
        /// </summary>
        public ushort NumberOfRelocations;
        /// <summary>
        /// ���������� ������� ����� � ������� ������� ����� ��� ������ ������ 
        /// (���� PointerToLinenumbers ��������� ����)
        /// </summary>
        public ushort NumberOfLinenumbers;
        /// <summary>
        /// ��, ��� ������� ����� ������������� �������� ������� (flags),
        /// ������ COFF/PE �������� ���������������� (characteristics).
        /// ��� ���� ������������ ����� ����� ������, ������� ��������� �� �������� ������ 
        /// (���������/������, ������������ ��� ������, ������������ ��� ������ � �.�.).
        /// �� ������ �������� ���� ��������� ��������� ������ ����������� � IMAGE_SCN_XXX_XXX
        /// (��. SectionCharacteristics)
        /// </summary>
        public UInt32 Characteristics;
    }

    class PE_Helper
    {
        public PE_Helper()
        {
        }

        public IMAGE_DOS_HEADER ReadImDosHeader(FileStream fs)
        {
            byte[] buffer_dos = new byte[Marshal.SizeOf(typeof(IMAGE_DOS_HEADER))];
            GCHandle handle_dos = GCHandle.Alloc(buffer_dos, GCHandleType.Pinned);
            fs.Read(buffer_dos, 0, buffer_dos.Length);// � ������

            handle_dos = GCHandle.Alloc(buffer_dos, GCHandleType.Pinned);
            //handle_dos.Free();
            return  (IMAGE_DOS_HEADER)Marshal.PtrToStructure(handle_dos.AddrOfPinnedObject(), typeof(IMAGE_DOS_HEADER));
        }

        public IMAGE_NT_HEADERS ReadImNtHeaders(FileStream fs)
        {
            byte[] buffer_nt = new byte[Marshal.SizeOf(typeof(IMAGE_NT_HEADERS))];
            GCHandle handle_nt = GCHandle.Alloc(buffer_nt, GCHandleType.Pinned);
            fs.Read(buffer_nt, 0, buffer_nt.Length);// �� �������� IMAGE_DOS_HEADER->e_lfanew

            handle_nt = GCHandle.Alloc(buffer_nt, GCHandleType.Pinned);
            return (IMAGE_NT_HEADERS)Marshal.PtrToStructure(handle_nt.AddrOfPinnedObject(), typeof(IMAGE_NT_HEADERS));
        }

        public IMAGE_SECTION_HEADER ReadImSecHeaders(FileStream fs)
        {
            byte[] buffer_sh = new byte[Marshal.SizeOf(typeof(IMAGE_SECTION_HEADER))];
            GCHandle handle_sh = GCHandle.Alloc(buffer_sh, GCHandleType.Pinned);
            fs.Read(buffer_sh, 0, buffer_sh.Length);// �� �������� IMAGE_DOS_HEADER->e_lfanew

            handle_sh = GCHandle.Alloc(buffer_sh, GCHandleType.Pinned);
            return (IMAGE_SECTION_HEADER)Marshal.PtrToStructure(handle_sh.AddrOfPinnedObject(), typeof(IMAGE_SECTION_HEADER));
        }

        /// <summary>
        /// ��������� ������ ���������
        /// </summary>
        /// <param name="size">������</param>
        /// <returns></returns>
        public bool CheckFileHeaderBySize(int size)
        {
            if (size != (int)SizeA.IMAGE_SIZEOF_FILE_HEADER)
            {
                return false;
            }
            else
            {
                return true;
            }
        }


    }


}
