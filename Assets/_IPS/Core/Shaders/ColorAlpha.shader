// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "IronPirate/Color Alpha" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_Alpha ("Alpha", Range(0, 1)) = 1
}

SubShader {
	Tags { "Queue"="Transparent" "RenderType"="Transparent"}
	LOD 100
	
	Pass {  
		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite Off
		
		CGPROGRAM
		
		#pragma vertex vert
		#pragma fragment frag
		
		struct VertexInput {
			half4 vertex : POSITION;
		};
		
		struct VertexOutput {
			half4 pos : SV_POSITION;
		};

		half4 _Color;
		half _Alpha;
		
		VertexOutput vert (VertexInput i)
		{
			VertexOutput o;
			o.pos = UnityObjectToClipPos(i.vertex);
			return o;
		}
		
		half4 frag (VertexOutput i) : COLOR
		{
		    _Color.a = _Alpha;
			return _Color;
		}
		ENDCG
	}
}

}

