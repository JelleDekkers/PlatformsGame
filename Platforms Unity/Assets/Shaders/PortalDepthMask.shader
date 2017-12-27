Shader "Depth Mask" {
    SubShader {
        Tags {"Queue" = "Geometry+1" }       
        Lighting Off
        ZTest LEqual
        ZWrite On
        ColorMask 0
        Pass {}
    }
}
