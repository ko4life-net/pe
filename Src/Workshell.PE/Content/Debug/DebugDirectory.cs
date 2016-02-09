﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Annotations;
using Workshell.PE.Native;

namespace Workshell.PE
{

    public enum DebugDirectoryType
    {
        [EnumAnnotation("IMAGE_DEBUG_TYPE_UNKNOWN")]
        Unknown = 0,
        [EnumAnnotation("IMAGE_DEBUG_TYPE_COFF")]
        COFF = 1,
        [EnumAnnotation("IMAGE_DEBUG_TYPE_CODEVIEW")]
        CodeView = 2,
        [EnumAnnotation("IMAGE_DEBUG_TYPE_FPO")]
        FPO = 3,
        [EnumAnnotation("IMAGE_DEBUG_TYPE_MISC")]
        Misc = 4,
        [EnumAnnotation("IMAGE_DEBUG_TYPE_EXCEPTION")]
        Exception = 5,
        [EnumAnnotation("IMAGE_DEBUG_TYPE_FIXUP")]
        Fixup = 6,
        [EnumAnnotation("IMAGE_DEBUG_TYPE_OMAP_TO_SRC")]
        OMAPToSrc = 7,
        [EnumAnnotation("IMAGE_DEBUG_TYPE_OMAP_FROM_SRC")]
        OMAPFromSrc = 8,
        [EnumAnnotation("IMAGE_DEBUG_TYPE_BORLAND")]
        Bolrand = 9,
        [EnumAnnotation("IMAGE_DEBUG_TYPE_RESERVED10")]
        Reserved = 10,
        [EnumAnnotation("IMAGE_DEBUG_TYPE_CLSID")]
        CLSID = 11,
        [EnumAnnotation("IMAGE_DEBUG_TYPE_VC_FEATURE")]
        VCFeature = 12,
        [EnumAnnotation("IMAGE_DEBUG_TYPE_POGO")]
        POGO = 13,
        [EnumAnnotation("IMAGE_DEBUG_TYPE_ILTCG")]
        ILTCG = 14,
        [EnumAnnotation("IMAGE_DEBUG_TYPE_MPX")]
        MPX = 15
    }

    public class DebugDirectory : ISupportsLocation, ISupportsBytes
    {

        public static readonly int size = Utils.SizeOf<IMAGE_DEBUG_DIRECTORY>();

        private DebugDirectories directories;
        private Location location;
        private IMAGE_DEBUG_DIRECTORY directory;

        internal DebugDirectory(DebugDirectories debugDirs, Location dirLocation, IMAGE_DEBUG_DIRECTORY dir)
        {
            directories = debugDirs;
            location = dirLocation;
            directory = dir;
        }

        #region Methods

        public override string ToString()
        {
            return String.Format("Debug Type: {0}", GetDirectoryType());
        }

        public byte[] GetBytes()
        {
            Stream stream = directories.Content.DataDirectory.Directories.Reader.GetStream();
            byte[] buffer = Utils.ReadBytes(stream, location);

            return buffer;
        }

        public DateTime GetTimeDateStamp()
        {
            return Utils.ConvertTimeDateStamp(directory.TimeDateStamp);
        }

        public DebugDirectoryType GetDirectoryType()
        {
            return (DebugDirectoryType)directory.Type;
        }

        #endregion

        #region Properties

        public DebugDirectories Directory
        {
            get
            {
                return directories;
            }
        }

        public Location Location
        {
            get
            {
                return location;
            }
        }

        [FieldAnnotation("Characteristics")]
        public uint Characteristics
        {
            get
            {
                return directory.Characteristics;
            }
        }

        [FieldAnnotation("Date/Time Stamp")]
        public uint TimeDateStamp
        {
            get
            {
                return directory.TimeDateStamp;
            }
        }

        [FieldAnnotation("Major Version")]
        public ushort MajorVersion
        {
            get
            {
                return directory.MajorVersion;
            }
        }

        [FieldAnnotation("Minor Version")]
        public ushort MinorVersion
        {
            get
            {
                return directory.MinorVersion;
            }
        }

        [FieldAnnotation("Type")]
        public uint Type
        {
            get
            {
                return directory.Type;
            }
        }

        [FieldAnnotation("Size of Data")]
        public uint SizeOfData
        {
            get
            {
                return directory.SizeOfData;
            }
        }

        [FieldAnnotation("Address of Raw Data")]
        public uint AddressOfRawData
        {
            get
            {
                return directory.AddressOfRawData;
            }
        }

        [FieldAnnotation("Pointer to Raw Data")]
        public uint PointerToRawData
        {
            get
            {
                return directory.PointerToRawData;
            }
        }

        #endregion

    }

    public class DebugDirectories : IEnumerable<DebugDirectory>
    {

        private DebugContent content;
        private Location location;
        private Section section;
        private List<DebugDirectory> directories;

        internal DebugDirectories(DebugContent debugContent, Location dirLocation, Section dirSection, List<Tuple<ulong,IMAGE_DEBUG_DIRECTORY>> dirs)
        {
            content = debugContent;
            location = dirLocation;
            section = dirSection;
            directories = new List<DebugDirectory>();

            LoadDirectories(dirs);
        }

        public IEnumerator<DebugDirectory> GetEnumerator()
        {
            return directories.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            return String.Format("Debug Entry Count: {0}", directories.Count);
        }

        private void LoadDirectories(List<Tuple<ulong, IMAGE_DEBUG_DIRECTORY>> dirEntries)
        {
            LocationCalculator calc = content.DataDirectory.Directories.Reader.GetCalculator();
            ulong image_base = content.DataDirectory.Directories.Reader.NTHeaders.OptionalHeader.ImageBase;
            uint size = Convert.ToUInt32(Utils.SizeOf<IMAGE_DEBUG_DIRECTORY>());

            foreach(Tuple<ulong, IMAGE_DEBUG_DIRECTORY> tuple in dirEntries)
            {
                uint rva = calc.OffsetToRVA(section, tuple.Item1);
                ulong va = image_base + rva;
                Location dir_location = new Location(tuple.Item1, rva, va, size, size);
                DebugDirectory dir = new DebugDirectory(this, dir_location, tuple.Item2);

                directories.Add(dir);
            }
        }

        public DebugContent Content
        {
            get
            {
                return content;
            }
        }

        public Location Location
        {
            get
            {
                return location;
            }
        }

        public Section Section
        {
            get
            {
                return section;
            }
        }

        public int Count
        {
            get
            {
                return directories.Count;
            }
        }

        public DebugDirectory this[int index]
        {
            get
            {
                return this[index];
            }
        }

    }

}