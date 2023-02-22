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
namespace Oculus.Platform
{
  using UnityEngine;
  using System.Collections.Generic;

  public class VoipAudioSource : MonoBehaviour
  {
    public bool spatialize = true;

    BufferedAudioStream bufferedAudioStream;
    Decoder decoder;
    protected List<float> debugOutputData;

    void Start()
    {
      AudioSource audioSource = gameObject.AddComponent<AudioSource>();
      Debug.Log(audioSource);
      audioSource.spatialize = spatialize;
      bufferedAudioStream = new BufferedAudioStream(audioSource);
      decoder = new Decoder();
    }

    public void Stop()
    {
    }

    public void AddCompressedData(byte[] compressedData)
    {
      if(decoder == null || bufferedAudioStream == null)
      {
        throw new System.Exception("VoipAudioSource failed to init");
      }

      float[] decompressedData = decoder.Decode(compressedData);
      if (decompressedData != null && decompressedData.Length > 0)
      {
        bufferedAudioStream.AddData(decompressedData);
        if (debugOutputData != null)
        {
          debugOutputData.AddRange(decompressedData);
        }
      }
    }

    void Update()
    {
      if (bufferedAudioStream == null)
      {
        throw new System.Exception("VoipAudioSource failed to init");
      }

      bufferedAudioStream.Update();
    }
  }
}
#endif
