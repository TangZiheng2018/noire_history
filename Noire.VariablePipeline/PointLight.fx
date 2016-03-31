float4x4 matWorldViewProj;
float4x4 matWorld;
float4x4 matWorldInv;
float3 vecLightPos;
float3 vecEye;
float4 vDiffuseColor;
float4 vSpecularColor;
float4 vAmbient;
float fPower;
bool bShadowAmbient;
texture texSphere;
bool bUseTexture;

sampler MeshTextureSampler =
sampler_state
{
    Texture = <texSphere>;
    MipFilter = LINEAR;
    MinFilter = LINEAR;
    MagFilter = LINEAR;
    AddressU = WRAP;
    AddressV = WRAP;
    AddressW = WRAP;
};

struct VToP
{
    float4 Pos : POSITION0;
    float3 LightPos : TEXCOORD0;
    float4 Normal : TEXCOORD1;
    float3 LookAt : TEXCOORD2;
    float4 Pos2 : TEXCOORD3;
    float2 DepthZW : TEXCOORD4;
    float2 TexUV : TEXCOORD5;
};

VToP VS(float4 Pos : POSITION, float4 N : NORMAL, float2 TexUV : TEXCOORD)
{
    VToP vtop = (VToP) 0;
    vtop.Pos = mul(Pos, matWorldViewProj);
    //vtop.Normal = mul(N, matWorldInv);
    vtop.Normal = N;
    float4 PosWorld = mul(Pos, matWorld);
    vtop.LightPos = vecLightPos;
    vtop.LookAt = (float3) PosWorld - vecEye;
    //vtop.Pos.w = length(vecEye.xyz - vtop.Pos.xyz);
    vtop.Pos2 = vtop.Pos;
    vtop.DepthZW = vtop.Pos.zw;
    vtop.TexUV = TexUV;
    return vtop;
}

float4 PS(VToP vtop) : COLOR
{
    float4 Normal = normalize(vtop.Normal);
    float3 ViewDir = normalize(vtop.LookAt);
    float3 LightDir = normalize((float3) vtop.Pos2 - vtop.LightPos);
                                                                          
    float Diff = dot((float3) Normal, LightDir);
    
    if (!bShadowAmbient || Diff >= 0)
    {
        Diff = saturate(Diff);
        float3 Reflect = normalize(reflect(LightDir, (float3) Normal));
        //float Specular = pow(saturate(dot(Reflect, ViewDir)), fPower);
        float t = dot(Reflect, ViewDir);
        t = clamp(t, 0, 1);
        float Specular = pow(t, fPower);
        //float Specular = pow(dot(Reflect, ViewDir), fPower);
        float4 Earth = tex2D(MeshTextureSampler, vtop.TexUV);
        
        if (bUseTexture)
        {
            return Earth;
        }
        else
        {
            return vAmbient + vDiffuseColor * Diff + vSpecularColor * Specular;
        }
        //return vAmbient + Earth * Diff + vSpecularColor * Specular;
    }
    else
    {
        //return float4(0, 0, 0, 0);
        return vAmbient;
    }
    /*
    float fColor = vtop.DepthZW.x / 1000;
    float4 vColor = float4(fColor, fColor, fColor, 1.0);
    return vColor;
    */
}

technique SpecularLight
{
    pass P0
    {
        /*
        AlphaBlendEnable = true;
        BlendOp = Add;
        SrcBlend = SrcColor;
        DestBlend = InvSrcAlpha;
        AlphaTestEnable = true;
        */
        ZEnable = true;
        ZFunc = LessEqual;
        ZWriteEnable = true;
        //FillMode = Wireframe;
        CullMode = None;
        VertexShader = compile vs_1_1 VS();
        PixelShader = compile ps_2_0 PS();
    }
}
