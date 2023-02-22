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

  public class Decoder : IDisposable {

    IntPtr dec;
    float[] decodedScratchBuffer;

    public Decoder() {
      dec = CAPI.ovr_Voip_CreateDecoder();
      decodedScratchBuffer = new float[480 * 10];
    }

    public void Dispose()
    {
      if (dec != IntPtr.Zero)
      {
        CAPI.ovr_Voip_DestroyEncoder(dec);
        dec = IntPtr.Zero;
      }
    }

    public float[] Decode(byte[] data) {
      CAPI.ovr_VoipDecoder_Decode(dec, data, (uint)data.Length);

      ulong gotSize = (ulong)CAPI.ovr_VoipDecoder_GetDecodedPCM(dec, decodedScratchBuffer, (UIntPtr)decodedScratchBuffer.Length);

      if (gotSize > 0)
      {
        float[] pcm = new float[gotSize];
        Array.Copy(decodedScratchBuffer, pcm, (int)gotSize);
        return pcm;
      }

      return null;
    }
  }
}
#endif
