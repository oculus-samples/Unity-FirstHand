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

namespace Oculus.Platform.Models
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using Oculus.Platform.Models;
    using UnityEngine;

    public class HttpTransferUpdate
    {
        public readonly UInt64 ID;
        public readonly byte[] Payload;
        public readonly bool IsCompleted;

        public HttpTransferUpdate(IntPtr o)
        {
            ID = CAPI.ovr_HttpTransferUpdate_GetID(o);
            IsCompleted = CAPI.ovr_HttpTransferUpdate_IsCompleted(o);

            long size = (long)CAPI.ovr_HttpTransferUpdate_GetSize(o);

            Payload = new byte[size];
            Marshal.Copy(CAPI.ovr_Packet_GetBytes(o), Payload, 0, (int)size);
        }
    }

}
