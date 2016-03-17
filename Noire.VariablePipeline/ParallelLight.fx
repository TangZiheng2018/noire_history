float4x4 matWorldViewProj;
float4x4 matWorld;
float4 vecLightDir;
float4 vecEye;
float4 vDiffuseColor;
float4 vSpecularColor;
float4 vAmbient;
float fPower;

struct VToP
{
    float4 Pos : POSITION0;
    float4 L : TEXCOORD0;
    float4 N : TEXCOORD1;
    float4 V : TEXCOORD2;
};

VToP VS(float4 Pos : POSITION, float4 N : NORMAL)
{
    VToP vtop = (VToP) 0;
    vtop.Pos = mul(Pos, matWorldViewProj);
    vtop.N = mul(N, matWorld);
    float4 PosWorld = mul(Pos, matWorld);
    vtop.L = vecLightDir;
    vtop.V = vecEye - PosWorld;
    return vtop;
}

float4 PS(VToP vtop) : COLOR
{
    float4 Normal = normalize(vtop.N);
    float4 LightDir = normalize(vtop.L);
    float4 ViewDir = normalize(vtop.V);
                                                                          
    float Diff = saturate(dot(Normal, LightDir));
    
    float4 Reflect = normalize(reflect(-LightDir, Normal));
    float Specular = pow(saturate(dot(Reflect, ViewDir)), fPower);

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
