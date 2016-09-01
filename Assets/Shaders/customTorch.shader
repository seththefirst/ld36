Shader "Custom/Torch" {	
	SubShader {
		Tags { 
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
	    }
		
		ZTest Off 
		ZWrite Off 
		ColorMask 0

		//ZTest Off
		//Cull Off
		//Lighting Off
		//ZWrite Off
		//Blend One OneMinusSrcAlpha
		//ColorMask 0

		Pass {
			Stencil {				
				Ref 1
				Comp Always
				Pass Replace
			}
		}
	}
}