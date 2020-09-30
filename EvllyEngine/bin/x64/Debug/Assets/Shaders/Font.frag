#version 330 core
layout(location = 0) out vec4 color;

in vec2 texCoord;

uniform sampler2D texture0;
uniform int HaveTexture;
uniform vec4 MainColor;

void main()
{
    color = vec4(MainColor.xyz, texture(texture0, texCoord).a);
	
	/*if (color.a < 0.5){
		discard;
	}*/
}