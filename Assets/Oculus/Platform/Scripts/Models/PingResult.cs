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
  using UnityEngine;
  using System;
  using System.ComponentModel;

  public class PingResult
  {
    public PingResult(UInt64 id, UInt64? pingTimeUsec) {
      this.ID = id;
      this.pingTimeUsec = pingTimeUsec;
    }

    public UInt64 ID { get; private set; }
    public UInt64 PingTimeUsec {
      get {
        return pingTimeUsec.HasValue ? pingTimeUsec.Value : 0;
      }
    }
    public bool IsTimeout {
      get {
        return !pingTimeUsec.HasValue;
      }
    }

    private UInt64? pingTimeUsec;
  }
}
