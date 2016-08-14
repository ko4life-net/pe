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
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.PE.Resources
{

    [Serializable]
    public class GraphicResourcesException : Exception
    {

        public GraphicResourcesException() : base()
        {
        }

        public GraphicResourcesException(string message) : base(message)
        {
        }

        public GraphicResourcesException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected GraphicResourcesException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

    }

    public static class GraphicResources
    {

        #region Methods

        public static void Register()
        {
            Register(false);
        }

        public static void Register(bool throwOnFail)
        {
            if (!BitmapResource.Register() && throwOnFail)
                throw new GraphicResourcesException("Could not register BitmapResource, already registered.");

            if (!CursorGroupResource.Register() && throwOnFail)
                throw new GraphicResourcesException("Could not register CursorGroupResource, already registered.");

            if (!CursorResource.Register() && throwOnFail)
                throw new GraphicResourcesException("Could not register CursorResource, already registered.");

            if (!IconGroupResource.Register() && throwOnFail)
                throw new GraphicResourcesException("Could not register IconGroupResource, already registered.");

            if (!IconResource.Register() && throwOnFail)
                throw new GraphicResourcesException("Could not register IconResource, already registered.");
        }

        public static bool IsPNG(byte[] data)
        {
            if (data.Length < 8)
                return false;

            ulong signature = BitConverter.ToUInt64(data, 0);

            return (signature == 727905341920923785L);
        }

        public static bool IsPNG(Stream stream)
        {
            byte[] buffer = new byte[sizeof(ulong)];
            int num_read = stream.Read(buffer, 0, buffer.Length);

            if (num_read < sizeof(ulong))
                return false;

            return IsPNG(buffer);
        }

        #endregion

    }

}