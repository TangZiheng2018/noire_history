float4x4 matWorldViewProj;
float4x4 matWorld;
float4 vecLightPos;
float4 vecEye;
float4 vDiffuseColor;
float4 vSpecularColor;
float4 vAmbient;

struct VToP
{
    float4 Pos : POSITION0;
    float4 L : TEXCOORD0;
    float4 N : TEXCOORD1;
    float4 V : TEXCOORD2;
    float4 Pos2 : TEXCOORD3;
};

VToP VS(float4 Pos : POSITION, float4 N : NORMAL)
{
    VToP vtop = (VToP) 0;
    vtop.Pos = mul(Pos, matWorldViewProj);
    vtop.N = mul(N, matWorld);
    float4 PosWorld = mul(Pos, matWorld);
    vtop.L = vecLightPos;
    vtop.V = vecEye - PosWorld;
    vtop.Pos2 = mul(Pos, matWorldViewProj);
    return vtop;
}

float4 PS(VToP vtop) : COLOR
{
    float4 Normal = normalize(vtop.N);
    float4 LightPos = vtop.L;
    float4 ViewDir = normalize(vtop.V);
    float4 LightDir = normalize(vtop.Pos2 - LightPos);
    LightDir.w = 1;
                                                                          
    float Diff = saturate(dot(Normal, LightDir));
    
    float4 Reflect = normalize(reflect(-LightDir, Normal));
    float Specular = pow(saturate(dot(Reflect, ViewDir)), 15);

    return vAmbient + vDiffuseColor * Diff + vSpecularColor * Specular;
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
        //ZFunc = NotEqual;
        //ZWriteEnable = true;
        //FillMode = Wireframe;
        CullMode = None;
        VertexShader = compile vs_1_1 VS();
        PixelShader = compile ps_2_0 PS();
    }
}
