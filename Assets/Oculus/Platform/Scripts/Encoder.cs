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

//This file is deprecated.  Use the high level voip system instead:
// https://developer3.oculus.com/documentation/platform/latest/concepts/dg-core-content/#dg-cc-voip
#if false
using UnityEngine;
using System.Collections;
using System;


namespace Oculus.Platform {

public class Encoder : IDisposable {
    IntPtr enc;

    public Encoder() {
      enc = CAPI.ovr_Voip_CreateEncoder();
    }

    public void Dispose()
    {
      if (enc != IntPtr.Zero)
      {
        CAPI.ovr_Voip_DestroyEncoder(enc);
        enc = IntPtr.Zero;
      }
    }

    public byte[] Encode(float[] samples) {
      CAPI.ovr_VoipEncoder_AddPCM(enc, samples, (uint)samples.Length);

      ulong size = (ulong)CAPI.ovr_VoipEncoder_GetCompressedDataSize(enc);
      if(size > 0) {
        byte[] compressedData = new byte[size]; //TODO 10376403 - pool this
        ulong sizeRead = (ulong)CAPI.ovr_VoipEncoder_GetCompressedData(enc, compressedData, (UIntPtr)size);

        if (sizeRead != size)
        {
          throw new Exception("Read size differed from reported size");
        }
        return compressedData;
      }
      return null;
    }
  }
}
#endif
