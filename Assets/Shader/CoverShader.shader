Shader "Unlit/CoverShader"
{
    Properties
    {
        _MainTex ("主纹理", 2D) = "white" {}
        _ScrollSpeed ("滚动速度", Float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            // _MainTex_ST 是Unity提供的纹理的缩放(Tiling)和偏移(Offset)值
            // _MainTex_ST.xy 是缩放, _MainTex_ST.zw 是偏移
            float4 _MainTex_ST;
            float _ScrollSpeed;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                // 使用TRANSFORM_TEX宏应用纹理的缩放和偏移，得到基础UV
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 计算水平滚动的UV偏移量
                // _Time.y 是自场景加载以来的时间（以秒为单位）
                // 乘以速度 _ScrollSpeed 并取负值，实现向左滚动（U值减小方向）
                float horizontalOffset = _Time.y * _ScrollSpeed;

                // 构建偏移向量。因为向左滚动只需改变U分量，所以V分量为0
                // 注意：取负号是实现向左滚动的核心
                float2 scrollVector = float2(-horizontalOffset, 0);

                // 将偏移量加到原始UV上
                // 由于纹理Wrap Mode设置为Repeat，当UV超出[0,1]范围时会自动重复贴图
                float2 scrolledUV = i.uv + scrollVector;

                // 对纹理进行采样
                fixed4 col = tex2D(_MainTex, scrolledUV);
                return col;
            }
            ENDCG
        }
    }
}