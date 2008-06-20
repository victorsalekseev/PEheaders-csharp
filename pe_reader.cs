using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;

namespace Netcode.pe
{
    /* nsg.exe
    Offset      0  1  2  3  4  5  6  7   8  9  A  B  C  D  E  F
    00000000   4D 5A 90 00 03 00 00 00  04 00 00 00 FF FF 00 00   MZђ.........яя..  -|
    00000010   B8 00 00 00 00 00 00 00  40 00 00 00 00 00 00 00   ё.......@.......   |DOS
    00000020   00 00 00 00 00 00 00 00  00 00 00 00 00 00 00 00   ................   |Заголовок
    00000030   00 00 00 00 00 00 00 00  00 00 00 00 B0 00 00 00   ............°...  -|
    00000040   0E 1F BA 0E 00 B4 09 CD  21 B8 01 4C CD 21 54 68   ..є..ґ.Н!ё.LН!Th  -|
    00000050   69 73 20 70 72 6F 67 72  61 6D 20 63 61 6E 6E 6F   is program canno   |
    00000060   74 20 62 65 20 72 75 6E  20 69 6E 20 44 4F 53 20   t be run in DOS    |
    00000070   6D 6F 64 65 2E 0D 0D 0A  24 00 00 00 00 00 00 00   mode....$.......   |Стаб
    00000080   5D CF 9F 87 19 AE F1 D4  19 AE F1 D4 19 AE F1 D4   ]Пџ‡.®сФ.®сФ.®сФ   |
    00000090   97 B1 E2 D4 13 AE F1 D4  E5 8E E3 D4 18 AE F1 D4   —±вФ.®сФеЋгФ.®сФ   |
    000000A0   52 69 63 68 19 AE F1 D4  00 00 00 00 00 00 00 00   Rich.®сФ........  -|
    000000B0   50 45 00 00 4C 01 02 00  1A A9 53 48 00 00 00 00   PE..L....©SH....  -- PE-заголовок
    */
    [StructLayout(LayoutKind.Sequential)]
    struct IMAGE_DOS_HEADER
    {
        public ushort e_magic;		// Сигнатура заголовка MZ Mark Zbinovski
        public ushort e_cblp;		// количество байт на последней странице файла
        public ushort e_cp;		// количество страниц в файле
        public ushort e_crlc;		// Relocations
        public ushort e_cparhdr;		// Размер заголовка (IMAGE_DOS_HEADER) в параграфах (1 параграф == 16 байт) без стаба
        public ushort e_minalloc;		// Минимальные дополнительные параграфы
        public ushort e_maxalloc;		// Максимальные дополнительные параграфы
        public ushort e_ss;		// начальное  относительное значение регистра SS
        public ushort e_sp;		// начальное значение регистра SP
        public ushort e_csum;		// контрольная сумма. Всем и всегда было на нее глубоко наплевать
        public ushort e_ip;		// начальное значение регистра IP
        public ushort e_cs;		// начальное относительное значение регистра CS
        public ushort e_lfarlc;		// адрес в файле на таблицу переадресации (MS-DOS Stub Program, начало стаба)
        public ushort e_ovno;		// количество оверлеев
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public ushort[] e_res;		// Зарезервировано[4]
        public ushort e_oemid;		// OEM идентифкатор
        public ushort e_oeminfo;		// OEM информация
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public ushort[] m;		// Зарезервировано
        public UInt32 e_lfanew;		// адрес в файле PE-заголовка. Загрузчик Windows из всей структуры IMAGE_DOS_HEADER интересуеться только двумя полями - e_magic & e_lfanew
    }

    [StructLayout(LayoutKind.Sequential)]
    struct IMAGE_NT_HEADERS 
    {
        /// <summary>
        /// Сигнатура файла (PE/NE/LE/LX с двумя нуль-байтами в конце \0\0) см. SignatureINTh
        /// </summary>
        public UInt32 Signature;
        public IMAGE_FILE_HEADER FileHeader;
        public IMAGE_OPTIONAL_HEADER32 OptionalHeader;
    }

    /// <summary>
    /// Значения IMAGE_OPTIONAL_HEADER32.DataDirectory[]
    /// </summary>
    enum DataDirectory
    {
        // Не забываем прибавлять Base
        // Каталог экспортируемых объектов
        IMAGE_DIRECTORY_ENTRY_EXPORT        = 0,
        // Каталог импортируемых объектов
        IMAGE_DIRECTORY_ENTRY_IMPORT        = 1,
        // Каталог ресурсов
        IMAGE_DIRECTORY_ENTRY_RESOURCE      = 2,
        // Каталог исключений
        IMAGE_DIRECTORY_ENTRY_EXCEPTION     = 3,
        // Каталог безопасности
        IMAGE_DIRECTORY_ENTRY_SECURITY      = 4,
        // Таблица переадресации
        IMAGE_DIRECTORY_ENTRY_BASERELOC     = 5,
        // Отладочный каталог
        IMAGE_DIRECTORY_ENTRY_DEBUG         = 6,
        // Строки описания
        IMAGE_DIRECTORY_ENTRY_COPYRIGHT     = 7,
        // Машинный значения (MIPS GP)
        IMAGE_DIRECTORY_ENTRY_GLOBALPTR     = 8,
        // Каталог TLS (Thread local storage - локальная память потоков)
        IMAGE_DIRECTORY_ENTRY_TLS           = 9,
        // Каталог конфигурации загрузки
        IMAGE_DIRECTORY_ENTRY_LOAD_CONFIG   = 10,
        IMAGE_DIRECTORY_ENTRY_BOUND_IMPORT  = 11, 
        // таблица адресов импорта
        IMAGE_DIRECTORY_ENTRY_IAT           = 12, 
        IMAGE_DIRECTORY_ENTRY_DELAY_IMPORT  = 13,
        // информация COM объектов
        IMAGE_DIRECTORY_ENTRY_COM_DESCRIPTOR= 14,
        // Резервная, не исп.   
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
    /// Значения IMAGE_SECTION_HEADER.Characteristics
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

         IMAGE_SCN_CNT_CODE                  = 0x00000020,  // Эта секция содержит программный код. 
                                                            // Как правило, устанавливается вместе с флагом (0х80000000)
         IMAGE_SCN_CNT_INITIALIZED_DATA      = 0x00000040,  // Данная секция содержит инициализированные данные. 
                                                            // Почти для всех секций, кроме исполняемых
                                                            // и .bss секций, этот флаг установлен
         IMAGE_SCN_CNT_UNINITIALIZED_DATA    = 0x00000080,  // Данная секция содержит неинициализированные
                                                            // данные (например, .bss секции)

         IMAGE_SCN_LNK_OTHER                 = 0x00000100,  // Reserved.
         IMAGE_SCN_LNK_INFO                  = 0x00000200,  // Данная секция содержит комментарии или какой-нибудь
                                                            // другой вид информации. Типичное использование такой
                                                            // секции — это секция .drectve, создаваемая компилятором
                                                            // и содержащая команды для компоновщика
       //IMAGE_SCN_TYPE_OVER                 = 0x00000400,  // Reserved.
         IMAGE_SCN_LNK_REMOVE                = 0x00000800,  // Содержимое данной секции не должно быть помещено
                                                            // в конечный ЕХЕ-файл. Такая секция используется
                                                            // компилятором/ассемблером для передачи информации компоновщику
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
         IMAGE_SCN_MEM_DISCARDABLE           = 0x02000000,  // Данную секцию можно отбросить, так как она
                                                            // не используется программой, после того как последняя
                                                            // загружена. Чаще всего встречается отбрасываемая секция
                                                            // — это секция базовых поправок (.reloc)
         IMAGE_SCN_MEM_NOT_CACHED            = 0x04000000,  // Section is not cachable.
         IMAGE_SCN_MEM_NOT_PAGED             = 0x08000000,  // Section is not pageable.
         IMAGE_SCN_MEM_SHARED                = 0x10000000,  // Данная секция является совместно используемой.
                                                            // При использовании с DLL данные в такой секции используются
                                                            // совместно всеми процессами, использующими эту DLL.
                                                            // По умолчанию секции данных не являются совместно используемыми,
                                                            // т.е. каждый процесс, использующий DLL, имеет свою собственную
                                                            // отдельную копию такой секции данных. Говоря
                                                            // более техническим языком, совместно используемая
                                                            // секция дает указание менеджеру памяти устанавливать
                                                            // отображение страниц для этой секции так, что все
                                                            // процессы, использующие DLL, ссылаются на одну и
                                                            // ту же физическую страницу в памяти. Чтобы сделать
                                                            // секцию совместно используемой, установите атрибут
                                                            // SHARED во время компоновки.
                                                            // Например: LINK/SECTION:MYDATA,RWS...
                                                            // указывает компоновщику, что секция с названием MYDATA
                                                            // должна быть доступной для чтения, записи и совместно
                                                            // используемой. По умолчанию сегменты данных
                                                            // DLL Borland C++ имеют атрибуты совместного использования
         IMAGE_SCN_MEM_EXECUTE               = 0x20000000,  // Данная секция является исполняемой. Этот флаг обычно
                                                            // устанавливается каждый раз, когда устанавливается
                                                            // флаг "Программа" (Contains Code) (0х00000020)
         IMAGE_SCN_MEM_READ                  = 0x40000000   // Данная секция предназначена для чтения. Этот флаг
                                                            // почти всегда установлен для секций ЕХЕ-файлов
       //IMAGE_SCN_MEM_WRITE                 = 0x80000000   // Данная секция предназначена для записи. Если этот
                                                            // флаг не установлен в секции ЕХЕ-файла, загрузчик
                                                            // должен отметить отображенные в память страницы как
                                                            // предназначенные только для чтения или только
                                                            // для исполнения. Типичные секции с этим атрибутом
                                                            // — это .data и .bss
    }
    enum SizeA
    {
        IMAGE_SIZEOF_FILE_HEADER = 20,
        COUNT_DATA_DYRECTORY     = 16,
        IMAGE_SIZEOF_SECTION_HEADER =40,
        IMAGE_SIZEOF_SHORT_NAME = 8
    }

    /// <summary>
    /// Значения IMAGE_OPTIONAL_HEADER32.Subsystem
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
    /// Значения IMAGE_OPTIONAL_HEADER32.DllCharacteristics
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
    /// Значения IMAGE_FILE_HEADER.Signature
    /// </summary>
    enum SignatureINTh
    {
        PE = 0x00004550,
        NE = 0x0000454E,
        LE = 0x0000454C,
        LX = 0x0000584C
    }

    /// <summary>
    /// Значения IMAGE_OPTIONAL_HEADER32.Magic
    /// </summary>
    enum MagicIOH32
    {
        IMAGE_NT_OPTIONAL_HDR_MAGIC  = 0x010B, //Нормальное исполняемое отображение (Значение для большей части файлов)
 	    IMAGE_ROM_OPTIONAL_HDR_MAGIC = 0x0107, //Отображение ПЗУ 
	    IMAGE_NT_OPTIONAL_HDR32_MAGIC = 0x010B, 
	    IMAGE_NT_OPTIONAL_HDR64_MAGIC = 0x020B 
    }

    [StructLayout(LayoutKind.Sequential)]
    struct IMAGE_FILE_HEADER 
    {
        /// <summary>
        /// Центральный процессор, для которого предназначен файл (см. MachineType)
        /// </summary>
        public ushort Machine;
        /// <summary>
        /// Количество секций в ЕХЕ- или OBJ-файле
        /// </summary>
        public ushort NumberOfSections;
        /// <summary>
        /// Время создания файла. Количество секунд, истекших с 16:00 31 декабря 1969 года
        /// </summary>
        public UInt32 TimeDateStamp;
        /// <summary>
        /// Файловое смещение COFF-таблицы символов. Это поле используется только в OBJ- и РЕ-файлах
        /// с информацией COFF-отладчика. РЕ-файлы поддерживают разнообразные отладочные форматы,
        /// так что отладчики должны ссылаться ко входу IMAGE_DIRECTORY_ENTRY_DEBUG в каталоге данных
        /// (определен в DataDirectoryNames)
        /// </summary>
        public UInt32 PointerToSymbolTable;
        /// <summary>
        /// Количество символов в COFF-таблице символов
        /// </summary>
        public UInt32 NumberOfSymbols;
        /// <summary>
        /// Размер необязательного заголовка, который может следовать за этой структурой.
        /// В исполняемых файлах — это размер структуры IMAGE_OPTIONAL_HEADER, которая следует
        /// за этой структурой. В объектных файлах, по утверждению Microsoft,
        /// это поле всегда содержит нуль. Однако при просмотре библиотеки вывода KERNEL32.LIB
        /// можно обнаружить объектный файл с ненулевым значением в этом поле, так что относитесь
        /// к высказыванию Microsoft с некоторым скептицизмом.
        /// </summary>
        public ushort SizeOfOptionalHeader;
        /// <summary>
        /// Characteristics, содержит специфические характеристики файла (см. FileCharacteristics),
        /// например, IMAGE_FILE_DEBUG_STRIPPED
        /// </summary>
        public ushort Characteristics;
    }

    /* Памятка
     * Relative Virtual Address - Многие поля в РЕ-файлах задаются именно с помощью их RVA.
     * RVA — это просто смещение данного элемента по отношению к адресу, с которого начинается
     * отображение файла в памяти.
     * 
     * (виртуальный адрес 0х401464) - (базовый адрес 0х400000) = RVA 0х1464
     * 
     * 
     * 
    */
    [StructLayout(LayoutKind.Sequential)]  
    struct IMAGE_OPTIONAL_HEADER32
    {
        // стандартные поля
        /// <summary>
        /// Слово-сигнатура, определяющее состояние отображенного файла (см. MagicIOH32)
        /// </summary>
        public ushort Magic;
        /// <summary>
        /// Старшая версия компоновщика, который создал данный файл.
        /// Числа должны быть представлены в десятичном виде, а не в шестнадцатеричном.
        /// </summary>
        public byte   MajorLinkerVersion;
        /// <summary>
        /// Младшая версия компоновщика, который создал данный файл.
        /// Числа должны быть представлены в десятичном виде, а не в шестнадцатеричном.
        /// </summary>
        public byte   MinorLinkerVersion;
        /// <summary>
        /// Суммарный размер программных секций, округленный к верхней границе.
        /// Обычно большинство файлов имеют только одну программную секцию, 
        /// так что это поле обычно соответствует размеру секции .text.
        /// </summary>
        public UInt32 SizeOfCode;
        /// <summary>
        /// Предполагается, что это общий размер всех секций, состоящих из инициализированных данных
        /// (не включая сегменты программного кода.) Однако не похоже, чтобы это совпадало с размером 
        /// секций инициализированных данных в файле.
        /// </summary>
        public UInt32 SizeOfInitializedData;
        /// <summary>
        /// Размер секций, под которые загрузчик выделяет место в виртуальном адресном пространстве,
        /// но которые не занимают никакого места в дисковом файле. В начале работы программы эти секции
        /// не обязаны иметь каких-либо определенных значений — отсюда название неинициализированные данные
        /// (Uninitialized Data). Неинициализированные данные обычно находятся в секции под названием .bss.
        /// </summary>
        public UInt32 SizeOfUninitializedData;
        /// <summary>
        /// Адрес, с которого отображение начинает выполнение.
        /// Это RVA, который можно найти в секции .text.
        /// Это поле применимо как для ЕХЕ-файла, так и для DLL.
        /// </summary>
        public UInt32 AddressOfEntryPoint;
        /// <summary>
        /// RVA, с которого начинаются программные секции файла. 
        /// Программные секции кода обычно идут в памяти перед секциями
        /// данных и после заголовка РЕ-файла. Этот RVA обычно равен 0х1000 для ЕХЕ-файлов, 
        /// созданных компоновщиками Microsoft. Для TLINK32 (Borland) значение этого поля равно 0х10000,
        /// так как по умолчанию этот компоновщик выравнивает объекты на границу в 64 Кбайт в отличие от 4 Кбайт
        /// в случае компоновщика Microsoft.
        /// </summary>
        public UInt32 BaseOfCode;
        /// <summary>
        /// RVA, с которого начинаются секции данных файла. 
        /// Секции данных обычно идут последними в памяти,
        /// после заголовка РЕ-файла и программных секций.
        /// </summary>
        public UInt32 BaseOfData;

        //NT дополнительные поля.
        /// <summary>
        /// Когда компоновщик создает исполняемый файл, он предполагает, что файл будет отображен
        /// в определенное место в памяти. Вот именно этот адрес и хранится в этом поле. Знание адреса загрузки
        /// позволяет компоновщику провести оптимизацию. Если загрузчик действительно отобразил файл в память
        /// по этому адресу, то программа перед запуском не нуждается ни в какой настройке. В исполняемых файлах NT 3.1
        /// адрес отображения по умолчанию равен 0х10000. В случае DLL этот адрес по умолчанию равен 0х400000.
        /// В Windows 95 адрес 0х10000 нельзя использовать для загрузки 32-разрядных файлов ЕХЕ, так как он лежит
        /// в пределах линейной области адресного пространства, общего для всех процессов. 
        /// Поэтому для Windows NT 3.5 Microsoft изменила для исполняемых файлов Win32 базовый адрес по умолчанию,
        /// сделав его равным 0х400000. Более старые программы, которые были скомпонованы в предположении, что 
        /// базовый адрес равен 0х10000, загружаются Windows 95 дольше, потому что загрузчик должен применить
        /// базовые поправки. 
        /// </summary>
        public UInt32 ImageBase;
        /// <summary>
        /// Сегменты загружаются в адресное пространство процесса последовательно, начиная с ImageBase.
        /// SectionAlignment предписывает минимальный размер, который сегмент может занять при загрузке
        /// - так что сегменты оказываются выровненными по границе SectionAlignment.
        /// Выравнивание сегмента не может быть меньше размера страницы (в настоящий момент 4096 байт
        /// на платформе x86), и должно быть кратно размеру страницы, как предписывает поведение менеджера
        /// виртуальной памяти Windows NT. 4096 байт являются значением по умолчанию, но может быть установлено 
        /// также другое значение, используя опцию линковщика -ALIGN:
        /// 
        /// После отображения в память каждая секция будет обязательно начинаться с виртуального адреса,
        /// кратного данной величине. С учетом подкачки страниц минимальная величина этого поля 0х1000
        /// используется компоновщиком Microsoft по умолчанию. TLINK в Borland C++ использует 
        /// по умолчанию 0х10000 (64 Кбайт)
        /// </summary>
        public UInt32 SectionAlignment;
        /// <summary>
        /// В случае РЕ-файла исходные данные, которые входят в состав каждой секции,
        /// будут обязательно начинаться с адреса, кратного данной величине. 
        /// Значение, устанавливаемое по умолчанию, равно 0х200 байт и, вероятно, 
        /// выбрано так для того, чтобы начало секции всегда совпадало с началом дискового сектора
        /// (0х200 байт — это как раз размер дискового сектора). Это поле эквивалентно размеру 
        /// выравнивания сегмента/ресурса в NE-файлах. В отличие от NE-файлов, РЕ-файлы не состоят
        /// из сотен секций, так что память, теряемая при выравнивании секций файла, обычно очень незначительна.
        /// </summary>
        public UInt32 FileAlignment;
        /// <summary>
        /// Самая старая версия (Major) операционной системы, которая может использовать данный исполняемый файл.
        /// Назначение этого поля не совсем ясно, так как поля подсистемы (приведены ниже), похоже, имеют 
        /// такое же предназначение. В большей части файлов Win32 в этом поле содержится значение, 
        /// соответствующее версии 1.0
        /// </summary>
        public ushort MajorOperatingSystemVersion;
        /// <summary>
        /// Самая старая версия (Minor) операционной системы, которая может использовать данный исполняемый файл.
        /// Назначение этого поля не совсем ясно, так как поля подсистемы (приведены ниже), похоже, имеют 
        /// такое же предназначение. В большей части файлов Win32 в этом поле содержится значение, 
        /// соответствующее версии 1.0
        /// </summary>
        public ushort MinorOperatingSystemVersion;
        /// <summary>
        /// Определяемое пользователем поле. Это поле позволяет иметь различные версии ЕХЕ-файлов и DLL.
        /// Эти поля устанавливаются с помощью ключа компоновщика /VERSION (Major)
        /// </summary>
        public ushort MajorImageVersion;
        /// <summary>
        /// Определяемое пользователем поле. Это поле позволяет иметь различные версии ЕХЕ-файлов и DLL.
        /// Эти поля устанавливаются с помощью ключа компоновщика /VERSION (Minor)
        /// </summary>
        public ushort MinorImageVersion;
        /// <summary>
        /// Это Major-поле содержит самую старую версию подсистемы, позволяющую запускать данный исполняемый файл. 
        /// Типичное значение в этом поле 4.0 (обозначает Windows 4.0, что равносильно Windows 95)
        /// </summary>
        public ushort MajorSubsystemVersion;
        /// <summary>
        /// Это Minor-поле содержит самую старую версию подсистемы, позволяющую запускать данный исполняемый файл. 
        /// Типичное значение в этом поле 4.0 (обозначает Windows 4.0, что равносильно Windows 95)
        /// </summary>
        public ushort MinorSubsystemVersion;
        /// <summary>
        /// Это поле, по-видимому, всегда равно нулю. Зарезервировано
        /// </summary>
        public UInt32 Win32VersionValue;
        /// <summary>
        /// Представляет общий размер всех частей отображения, находящихся под контролем загрузчика.
        /// Эта величина равна размеру области памяти, начиная с базового адреса отображения и заканчивая
        /// адресом конца последней секции. Адрес конца секции выровнен на ближайшую верхнюю границу секции
        /// </summary>
        public UInt32 SizeOfImage;
        /// <summary>
        /// Размер заголовка РЕ-файла и таблицы секции (объекта). Исходные данные для секций начинаются 
        /// сразу после всех составляющих частей заголовка
        /// </summary>
        public UInt32 SizeOfHeaders;
        /// <summary>
        /// Предположительно отвечает контрольной сумме контроля циклическим избыточным кодом (CRC-контроль) 
        /// для данного файла. Как и для других исполняемых форматов Microsoft, это поле обычно игнорируется
        /// и устанавливается в нуль. Однако для всех DLL драйверов, DLL, загруженных во время загрузки ОС,
        /// и серверных DLL эта контрольная, сумма должна иметь правильное значение. Алгоритм для контрольной 
        /// суммы можно найти в IMAGEHLP.DLL. Исходники IMAGEHLP.DLL поставляются в WIN32 SDK
        /// </summary>
        public UInt32 CheckSum;
        /// <summary>
        /// Тип подсистемы, которую данный исполняемый файл использует для своего пользовательского интерфейса.
        /// (см. Subsystem)
        /// </summary>
        public ushort Subsystem;
        /// <summary>
        /// Набор флагов, показывающий, при каких обстоятельствах будет вызываться функция инициализации DLL
        /// (например, DllMain()). Эта величина, по-видимому, всегда должна устанавливаться в нуль, однако
        /// операционная система вызывает функцию инициализации DLL для всех четырех случаев (см. DllCharacteristics)
        /// </summary>
        public ushort DllCharacteristics;
        /// <summary>
        /// Объем виртуальной памяти, резервируемой под начальный стек цепочки. Однако не вся эта память
        /// выделяется (см. следующее поле). По умолчанию это поле устанавливается в 0х100000 (1 Мбайт).
        /// Если пользователь указывает 0 в качестве размера стека в CreateThread(), получившаяся цепочка 
        /// будет иметь стек того же размера
        /// </summary>
        public UInt32 SizeOfStackReserve;
        /// <summary>
        /// Количество памяти, изначально выделяемой под исходный стек цепочки. 
        /// Это поле по умолчанию равно 0х1000 байт (1 страница) для компоновщиков Microsoft,
        /// тогда как TLINK32 делает его равным 0х2000 (2 страницы)
        /// </summary>
        public UInt32 SizeOfStackCommit;
        /// <summary>
        /// Объем виртуальной памяти, резервируемой под изначальную кучу программы. 
        /// Этот дескриптор кучи можно получить, вызвав GetProcessHeap(). Однако не вся эта память выделяется 
        /// (см. следующее поле)
        /// </summary>
        public UInt32 SizeOfHeapReserve;
        /// <summary>
        /// Объем виртуальной памяти, изначально выделяемой под кучу процесса. 
        /// По умолчанию компоновщик делает это поле равным 0х1000 байт
        /// </summary>
        public UInt32 SizeOfHeapCommit;
        /// <summary>
        /// Указывает отладчику остановиться после загрузки, перейти к отладке после загрузки,
        /// или, значение по умолчанию, просто запустить файл.
        /// 1 = Запускать ли команду прерывания перед запуском процесса?
        /// 2 = Запускать ли отладчик программы после процесса?
        /// </summary>
        public UInt32 LoaderFlags;
        /// <summary>
        /// Количество входов в массиве DataDirectory (см. описание следующего поля).
        /// Современные программные средства всегда делают это значение равным 16
        /// </summary>
        public UInt32 NumberOfRvaAndSizes;
        /// <summary>
        /// Массив структур типа IMAGE_DATA_DIRECTORY. Начальные элементы массива содержат стартовый RVA и размеры
        /// важных частей исполняемого файла. В настоящее время некоторые элементы в конце массива не используются.
        /// Первый элемент массива — это всегда адрес и размер экспортированной таблицы функций (если она
        /// присутствует). Второй элемент массива — адрес и размер импортированной таблицы функций и т.д.
        /// (см. DataDirectory)
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
    struct IMAGE_EXPORT_DIRECTORY //Которая 0 в IMAGE_DATA_DIRECTORY[]
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
        public UInt32 AddressOfNames;//массив указателей на имена функций
        public UInt32 AddressOfNameOrdinals;  
    }

    [StructLayout(LayoutKind.Sequential)]
    struct IMAGE_SECTION_HEADER
    {
        /// <summary>
        /// Это 8-байтовое имя в стандарте ANSI (не Unicode), которое именует секцию. Большинство имен секций
        /// начинается с точки (например, .text), но это не обязательно, вопреки тому, в чем пытаются вас уверить
        /// отдельные документы по РЕ-файлам. Пользователь может давать имена своим собственным секциям с помощью
        /// либо сегментной директивы в ассемблере, либо с помощью директив #pragma data_seg и #pragma code_seg
        /// компилятора Microsoft C/C++. (Пользователи Borland C++ должны использовать #pragma codeseg.)
        /// Необходимо отметить, что, если имя секции занимает 8 полных байтов, отсутствует завершающий байт NULL.
        /// (Программа TDUMP в Borland C++ 4.Ox упускает из виду это обстоятельство и изрыгает последующий мусор
        /// из некоторых РЕ ЕХЕ-файлов.) Если вы приверженец printf(), то можете использовать "%.8s", чтобы не
        /// копировать строку-имя в другой буфер для завершения ее нулевым байтом
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] Name;
        /// <summary>
        /// Это поле имеет различные назначения в зависимости от того, встречается ли оно в ЕХЕ- или OBJ-файле. 
        /// В ЕХЕ-файле оно содержит виртуальный размер секции программного кода или данных.
        /// Это размер до округления на ближайшую верхнюю границу файла.
        /// Поле SizeOfRawData дальше в этой структуре содержит это округленное значение.
        /// Интересно, что Borland TLINK32 меняет местами значение этого поля и поля SizeOfRawData и,
        /// тем не менее, остается правильным компоновщиком. В случае OBJ-файлов это поле указывает 
        /// физический адрес секции. Первая секция начинается с адреса 0. Чтобы получить физический адрес
        /// следующей секции, надо прибавить значение в SizeOfRawData к физическому адресу данной секции
        /// </summary>
        public UInt32 VirtualSize;
        /// <summary>
        /// В случае ЕХЕ-файлов это поле содержит RVA, куда загрузчик должен отобразить секцию. 
        /// Чтобы вычислить реальный начальный адрес данной секции в памяти, необходимо к виртуальному адресу
        /// секции, содержащемуся в этом поле, прибавить базовый адрес отображения. 
        /// Средства Microsoft устанавливают по умолчанию RVA первой секции равным 0х1000.
        /// Для объектных файлов это поле не несет никакого смысла и устанавливается в 0
        /// </summary>
        public UInt32 VirtualAddress;
        /// <summary>
        /// В ЕХЕ-файлах это поле содержит размер секции, выровненный на ближайшую верхнюю границу размера файла.
        /// Например, допустим, что размер выравнивания файла 0х200. Если поле VirtualSize указывает,
        /// что длина секции 0х35A байт, то в данном поле будет указано, что размер секции 0х400 байт.
        /// Для OBJ-файлов это поле содержит точный размер секции, сгенерированной компилятором или ассемблером.
        /// Другими словами, для OBJ-файлов оно эквивалентно полю VirtualSize в ЕХЕ-файлах
        /// </summary>
        public UInt32 SizeOfRawData;
        /// <summary>
        /// Это файловое смещение участка, где находятся исходные данные для секции.
        /// Если пользователь сам отображает в память РЕ- или COFF-файл (вместо того,
        /// чтобы доверить загрузку операционной системе), это поле важнее, чем поле VirtualAddress.
        /// Причиной является то, что в этом случае получится абсолютно линейное отображение всего файла,
        /// так что данные для секций будут находиться по этому смещению, а не по RVA, указанному
        /// в поле VirtualAddress.
        /// </summary>
        public UInt32 PointerToRawData;
        /// <summary>
        /// В объектных файлах это файловое смещение информации о поправках для данной секции.
        /// Информация о поправках в любой секции объектного файла следует за исходными данными для этой секции.
        /// В ЕХЕ-файлах это (и следующее) поле не несет смысловой нагрузки и устанавливается в нуль.
        /// Когда компоновщик создает ЕХЕ-файл, он разрешает большинство привязок,
        /// а во время загрузки остается разрешить только базовые адресные поправки и импортированные функции.
        /// Информация о базовых поправках и импортированных функциях хранится в секциях базовых поправок
        /// и импортированных функций, так что нет необходимости в ЕХЕ-файле помещать данные поправок для 
        /// каждой секции после исходных данных секции
        /// </summary>
        public UInt32 PointerToRelocations;
        /// <summary>
        /// Файловое смещение таблицы номеров строк. Таблица номеров строк ставит в соответствие номера строк 
        /// исходного файла адресам, по которым можно найти код, сгенерированный для данной строки.
        /// В современных отладочных форматах, таких как формат CodeView, информация о номерах строк
        /// хранится как часть информации отладчика. В отладочном формате COFF, однако, информация 
        /// о номерах строк концептуально отлична от информации о символьных именах/типах. 
        /// Обычно только секции с программным кодом (например, .text или CODE) имеют номера строк.
        /// В ЕХЕ-файлах номера строк собраны в конце файла после исходных данных для секций.
        /// В объектных файлах таблица номеров строк для секции следует за исходными данными секции 
        /// и таблицей перемещений для этой секции
        /// </summary>
        public UInt32 PointerToLinenumbers;
        /// <summary>
        /// Количество перемещений в таблице поправок для данной секции (поле PointerToRelocations приведено выше).
        /// Это поле используется, по-видимому, только в объектных файлах
        /// </summary>
        public ushort NumberOfRelocations;
        /// <summary>
        /// Количество номеров строк в таблице номеров строк для данной секции 
        /// (поле PointerToLinenumbers приведено выше)
        /// </summary>
        public ushort NumberOfLinenumbers;
        /// <summary>
        /// То, что большая часть программистов называет флагами (flags),
        /// формат COFF/PE называет характеристиками (characteristics).
        /// Это поле представляет собой набор флагов, которые указывают на атрибуты секции 
        /// (программа/данные, предназначен для чтения, предназначен для записи и т.п.).
        /// За полным перечнем всех возможных атрибутов секции обращайтесь к IMAGE_SCN_XXX_XXX
        /// (см. SectionCharacteristics)
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
            fs.Read(buffer_dos, 0, buffer_dos.Length);// с начала

            handle_dos = GCHandle.Alloc(buffer_dos, GCHandleType.Pinned);
            //handle_dos.Free();
            return  (IMAGE_DOS_HEADER)Marshal.PtrToStructure(handle_dos.AddrOfPinnedObject(), typeof(IMAGE_DOS_HEADER));
        }

        public IMAGE_NT_HEADERS ReadImNtHeaders(FileStream fs)
        {
            byte[] buffer_nt = new byte[Marshal.SizeOf(typeof(IMAGE_NT_HEADERS))];
            GCHandle handle_nt = GCHandle.Alloc(buffer_nt, GCHandleType.Pinned);
            fs.Read(buffer_nt, 0, buffer_nt.Length);// со значения IMAGE_DOS_HEADER->e_lfanew

            handle_nt = GCHandle.Alloc(buffer_nt, GCHandleType.Pinned);
            return (IMAGE_NT_HEADERS)Marshal.PtrToStructure(handle_nt.AddrOfPinnedObject(), typeof(IMAGE_NT_HEADERS));
        }

        public IMAGE_SECTION_HEADER ReadImSecHeaders(FileStream fs)
        {
            byte[] buffer_sh = new byte[Marshal.SizeOf(typeof(IMAGE_SECTION_HEADER))];
            GCHandle handle_sh = GCHandle.Alloc(buffer_sh, GCHandleType.Pinned);
            fs.Read(buffer_sh, 0, buffer_sh.Length);// со значения IMAGE_DOS_HEADER->e_lfanew

            handle_sh = GCHandle.Alloc(buffer_sh, GCHandleType.Pinned);
            return (IMAGE_SECTION_HEADER)Marshal.PtrToStructure(handle_sh.AddrOfPinnedObject(), typeof(IMAGE_SECTION_HEADER));
        }

        /// <summary>
        /// Проверяет размер заголовка
        /// </summary>
        /// <param name="size">Размер</param>
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
