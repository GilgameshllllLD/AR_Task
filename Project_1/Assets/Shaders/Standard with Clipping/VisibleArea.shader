Shader "VisibleArea"
{
	SubShader
	{
		Tags {"Queue" = "Geometry-5" "RenderType"="Transparent"}
		Cull Front
		Lighting Off
		ZWrite Off
		ColorMask 0 // Don't write RGBA
		Stencil {
			// Always write the value 1 to the stencil buffer
			Ref 1
			Comp Always
			Pass Replace
		}
		Pass {}
	}
}
