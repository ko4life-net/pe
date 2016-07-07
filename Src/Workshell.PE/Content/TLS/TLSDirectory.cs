﻿#region License
//  Copyright(c) 2016, Workshell Ltd
//  All rights reserved.
//  
//  Redistribution and use in source and binary forms, with or without
//  modification, are permitted provided that the following conditions are met:
//     * Redistributions of source code must retain the above copyright
//       notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright
//       notice, this list of conditions and the following disclaimer in the
//       documentation and/or other materials provided with the distribution.
//     * Neither the name of Workshell Ltd nor the names of its contributors
//  may be used to endorse or promote products
//  derived from this software without specific prior written permission.
//  
//  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
//  ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//  WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//  DISCLAIMED.IN NO EVENT SHALL WORKSHELL BE LIABLE FOR ANY
//  DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
//  (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//  LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
//  ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
//  (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//  SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Annotations;
using Workshell.PE.Extensions;
using Workshell.PE.Native;

namespace Workshell.PE
{

    public abstract class TLSDirectory : ExecutableImageContent, ISupportsBytes
    {

        internal TLSDirectory(DataDirectory dataDirectory, Location tlsLocation) : base(dataDirectory,tlsLocation)
        {
        }

        #region Static Methods

        public static TLSDirectory Get(ExecutableImage image)
        {
            if (!image.NTHeaders.DataDirectories.Exists(DataDirectoryType.TLSTable))
                return null;

            DataDirectory directory = image.NTHeaders.DataDirectories[DataDirectoryType.TLSTable];

            if (DataDirectory.IsNullOrEmpty(directory))
                return null;

            LocationCalculator calc = directory.Directories.Image.GetCalculator();
            Section section = calc.RVAToSection(directory.VirtualAddress);
            ulong file_offset = calc.RVAToOffset(section, directory.VirtualAddress);
            ulong image_base = directory.Directories.Image.NTHeaders.OptionalHeader.ImageBase;
            Location location = new Location(file_offset, directory.VirtualAddress, image_base + directory.VirtualAddress, directory.Size, directory.Size, section);
            Stream stream = directory.Directories.Image.GetStream();

            stream.Seek(file_offset.ToInt64(), SeekOrigin.Begin);

            bool is_64bit = directory.Directories.Image.Is64Bit;
            TLSDirectory tls_directory;

            if (!is_64bit)
            {
                IMAGE_TLS_DIRECTORY32 tls_dir = Utils.Read<IMAGE_TLS_DIRECTORY32>(stream);

                tls_directory = new TLSDirectory32(directory, location, tls_dir);
            }
            else
            {
                IMAGE_TLS_DIRECTORY64 tls_dir = Utils.Read<IMAGE_TLS_DIRECTORY64>(stream);

                tls_directory = new TLSDirectory64(directory, location, tls_dir);
            }

            return tls_directory;
        }

        #endregion

        #region Methods

        public byte[] GetBytes()
        {
            Stream stream = DataDirectory.Directories.Image.GetStream();
            byte[] buffer = Utils.ReadBytes(stream,Location);

            return buffer;
        }

        #endregion

        #region Properties

        [FieldAnnotation("Start Address of Raw Data")]
        public abstract ulong StartAddress
        {
            get;
        }

        [FieldAnnotation("End Address of Raw Data")]
        public abstract ulong EndAddress
        {
            get;
        }

        [FieldAnnotation("Address of Index")]
        public abstract ulong AddressOfIndex
        {
            get;
        }

        [FieldAnnotation("Address of Callbacks")]
        public abstract ulong AddressOfCallbacks
        {
            get;
        }

        [FieldAnnotation("Size of Zero Fill")]
        public abstract uint SizeOfZeroFill
        {
            get;
        }

        [FieldAnnotation("Characteristics")]
        public abstract uint Characteristics
        {
            get;
        }

        #endregion

    }

    public sealed class TLSDirectory32 : TLSDirectory
    {

        private IMAGE_TLS_DIRECTORY32 directory;

        internal TLSDirectory32(DataDirectory dataDirectory, Location tlsLocation, IMAGE_TLS_DIRECTORY32 tlsDirectory) : base(dataDirectory,tlsLocation)
        {
            directory = tlsDirectory;
        }

        #region Properties

        public override ulong StartAddress
        {
            get
            {
                return directory.StartAddress;
            }
        }

        public override ulong EndAddress
        {
            get
            {
                return directory.EndAddress;
            }
        }

        public override ulong AddressOfIndex
        {
            get
            {
                return directory.AddressOfIndex;
            }
        }

        public override ulong AddressOfCallbacks
        {
            get
            {
                return directory.AddressOfCallbacks;
            }
        }

        public override uint SizeOfZeroFill
        {
            get
            {
                return directory.SizeOfZeroFill;
            }
        }

        public override uint Characteristics
        {
            get
            {
                return directory.Characteristics;
            }
        }

        #endregion

    }

    public sealed class TLSDirectory64 : TLSDirectory
    {

        private IMAGE_TLS_DIRECTORY64 directory;

        internal TLSDirectory64(DataDirectory dataDirectory, Location tlsLocation, IMAGE_TLS_DIRECTORY64 tlsDirectory) : base(dataDirectory,tlsLocation)
        {
            directory = tlsDirectory;
        }

        #region Properties

        public override ulong StartAddress
        {
            get
            {
                return directory.StartAddress;
            }
        }

        public override ulong EndAddress
        {
            get
            {
                return directory.EndAddress;
            }
        }

        public override ulong AddressOfIndex
        {
            get
            {
                return directory.AddressOfIndex;
            }
        }

        public override ulong AddressOfCallbacks
        {
            get
            {
                return directory.AddressOfCallbacks;
            }
        }

        public override uint SizeOfZeroFill
        {
            get
            {
                return directory.SizeOfZeroFill;
            }
        }

        public override uint Characteristics
        {
            get
            {
                return directory.Characteristics;
            }
        }

        #endregion

    }

}
