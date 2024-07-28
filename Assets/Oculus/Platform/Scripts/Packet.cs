/*
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * Licensed under the Oculus SDK License Agreement (the "License");
 * you may not use the Oculus SDK except in compliance with the License,
 * which is provided at the time of installation or download, or which
 * otherwise accompanies this software in either electronic or hard copy form.
 *
 * You may obtain a copy of the License at
 *
 * https://developer.oculus.com/licenses/oculussdk/
 *
 * Unless required by applicable law or agreed to in writing, the Oculus SDK
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

namespace Oculus.Platform
{
    using System;
    using System.Runtime.InteropServices;

    public sealed class Packet : IDisposable
    {
        private readonly ulong size;
        private readonly IntPtr packetHandle;

        public Packet(IntPtr packetHandle)
        {
            this.packetHandle = packetHandle;
            size = (ulong)CAPI.ovr_Packet_GetSize(packetHandle);
        }

        /**
         * Copies all the bytes in the payload into byte[] destination.  ex:
         *   Package package ...
         *   byte[] destination = new byte[package.Size];
         *   package.ReadBytes(destination);
         */
        public ulong ReadBytes(byte[] destination)
        {
            if ((ulong)destination.LongLength < size)
            {
                throw new System.ArgumentException(String.Format("Destination array was not big enough to hold {0} bytes", size));
            }
            Marshal.Copy(CAPI.ovr_Packet_GetBytes(packetHandle), destination, 0, (int)size);
            return size;
        }

        public UInt64 SenderID
        {
            get { return CAPI.ovr_Packet_GetSenderID(packetHandle); }
        }

        public ulong Size
        {
            get { return size; }
        }

        public SendPolicy Policy
        {
            get { return (SendPolicy)CAPI.ovr_Packet_GetSendPolicy(packetHandle); }
        }

        #region IDisposable

        ~Packet()
        {
            Dispose();
        }

        public void Dispose()
        {
            CAPI.ovr_Packet_Free(packetHandle);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
