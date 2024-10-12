/************************************************************************************
Copyright : Copyright (c) Facebook Technologies, LLC and its affiliates. All rights reserved.

Your use of this SDK or tool is subject to the Oculus SDK License Agreement, available at
https://developer.oculus.com/licenses/oculussdk/

Unless required by applicable law or agreed to in writing, the Utilities SDK distributed
under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF
ANY KIND, either express or implied. See the License for the specific language governing
permissions and limitations under the License.
************************************************************************************/

//

struct VertexInput
{
    float4 vertex : POSITION;
    half3 normal : NORMAL;
    half4 vertexColor : COLOR;
    float4 texcoord : TEXCOORD0;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct VertexOutput
{
    float4 vertex : SV_POSITION;
    float3 worldPos : TEXCOORD1;
    float3 worldNormal : TEXCOORD2;
    half4 glowColor : COLOR;
    UNITY_VERTEX_INPUT_INSTANCE_ID
    UNITY_VERTEX_OUTPUT_STEREO
};

half4 OculusHand_CalcGlow(float4 texcoord)
{
    half4 glowColor = 0;
    half4 maskPixelColor = tex2Dlod(_FingerGlowMask, float4(texcoord.xy, 0, 0));

    int glowMaskR = maskPixelColor.r * 255;

    int thumbMask = (glowMaskR >> 3) & 0x1;
    int indexMask = (glowMaskR >> 4) & 0x1;
    int middleMask = (glowMaskR >> 5) & 0x1;
    int ringMask = (glowMaskR >> 6) & 0x1;
    int pinkyMask = (glowMaskR >> 7) & 0x1;

    half glowIntensity = saturate(
        maskPixelColor.g *
        (thumbMask * _ThumbGlowValue
            + indexMask * _IndexGlowValue
            + middleMask * _MiddleGlowValue
            + ringMask * _RingGlowValue
            + pinkyMask * _PinkyGlowValue));

    half4 glow = glowIntensity * _FingerGlowColor;
    glowColor.rgb = glow.rgb;
    glowColor.a = saturate(maskPixelColor.a + _WristFade) * _Opacity;
    return glowColor;
}

half4 OculusHand_SurfaceColor(float3 worldPos, float3 worldNormal, fixed4 glowColor)
{
    float3 worldViewDir = normalize(UnityWorldSpaceViewDir(worldPos));

    float fresnelNdot = dot(worldNormal, worldViewDir);
    float fresnel = 1.0 * pow(1.0 - fresnelNdot, _FresnelPower);
    float4 color = lerp(_ColorTop, _ColorBottom, fresnel);
    
    return half4(saturate(color + glowColor.rgb), glowColor.a);
}

VertexOutput vertInterior(VertexInput v)
{
    VertexOutput o;
    UNITY_SETUP_INSTANCE_ID(v);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
    UNITY_TRANSFER_INSTANCE_ID(v, o);

    o.worldPos = mul(unity_ObjectToWorld, v.vertex);
    o.worldNormal = UnityObjectToWorldNormal(v.normal);
    o.vertex = UnityObjectToClipPos(v.vertex);

    o.glowColor = OculusHand_CalcGlow(v.texcoord);
    return o;
}

half4 fragInterior(VertexOutput i) : SV_Target
{
    return OculusHand_SurfaceColor(i.worldPos, i.worldNormal, i.glowColor);
}

struct OutlineVertexInput
{
    float4 vertex : POSITION;
    float3 normal : NORMAL;
    float4 texcoord : TEXCOORD0;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct OutlineVertexOutput
{
    float4 vertex : SV_POSITION;
    half4 glowColor : TEXCOORD1;
    UNITY_VERTEX_INPUT_INSTANCE_ID
    UNITY_VERTEX_OUTPUT_STEREO
};

void OculusHand_Outline(float2 texcoord, float3 osNormal, inout float4 osPos, inout half4 glowColor)
{
    half4 maskPixelColor = tex2Dlod(_FingerGlowMask, float4(texcoord, 0, 0));

#if CONFIDENCE
    int glowMaskR = maskPixelColor.r * 255;
    int jointMaskB = maskPixelColor.b * 255;

    int thumbMask = (glowMaskR >> 3) & 0x1;
    int indexMask = (glowMaskR >> 4) & 0x1;
    int middleMask = (glowMaskR >> 5) & 0x1;
    int ringMask = (glowMaskR >> 6) & 0x1;
    int pinkyMask = (glowMaskR >> 7) & 0x1;

    int joint0 = (jointMaskB >> 4) & 0x1;
    int joint1 = (jointMaskB >> 5) & 0x1;
    int joint2 = (jointMaskB >> 6) & 0x1;
    int joint3 = (jointMaskB >> 7) & 0x1;

    half jointIntensity = saturate(
        ((1 - saturate(glowMaskR)) * _JointsGlow[0])
        + thumbMask * (joint0 * _JointsGlow[1]
            + joint1 * _JointsGlow[2]
            + joint2 * _JointsGlow[3]
            + joint3 * _JointsGlow[4])
        + indexMask * (joint1 * _JointsGlow[5]
            + joint2 * _JointsGlow[6]
            + joint3 * _JointsGlow[7])
        + middleMask * (joint1 * _JointsGlow[8]
            + joint2 * _JointsGlow[9]
            + joint3 * _JointsGlow[10])
        + ringMask * (joint1 * _JointsGlow[11]
            + joint2 * _JointsGlow[12]
            + joint3 * _JointsGlow[13])
        + pinkyMask * (joint0 * _JointsGlow[14]
            + joint1 * _JointsGlow[15]
            + joint2 * _JointsGlow[16]
            + joint3 * _JointsGlow[17]));

    half4 glow = lerp(_OutlineColor, _OutlineJointColor, jointIntensity);
    glowColor.rgb = glow.rgb;
    glowColor.a = saturate(maskPixelColor.a + _WristFade) * glow.a * _OutlineOpacity;
#else
    glowColor.rgb = _OutlineColor;
    glowColor.a = saturate(maskPixelColor.a + _WristFade) * _OutlineColor.a * _OutlineOpacity;
#endif

    osPos.xyz += osNormal * _OutlineWidth;
}

OutlineVertexOutput vertOutline(OutlineVertexInput v)
{
    OutlineVertexOutput o;
    UNITY_SETUP_INSTANCE_ID(v);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
    UNITY_TRANSFER_INSTANCE_ID(v, o);

    OculusHand_Outline(v.texcoord, v.normal, /*inout*/ v.vertex, /*inout*/ o.glowColor);

    o.vertex = UnityObjectToClipPos(v.vertex);

    return o;
}

half4 fragOutline(OutlineVertexOutput i) : SV_Target
{
    return i.glowColor;
}
