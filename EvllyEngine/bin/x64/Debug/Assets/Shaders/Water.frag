#version 330 core

layout(location = 0) out vec4 color;

in vec2 texCoord;
in vec4 colortest;
in vec3 N;
in float visiblity;
in float waterProfu;

uniform sampler2D texture0;
uniform vec4 FOG_Color;

uniform vec4 AmbienceColor;

void main()
{
    color = texture(texture0, texCoord);
	color = mix(AmbienceColor, color, AmbienceColor.a);
    color = vec4(color.x, color.y, color.z, 0.95);
    color = mix(vec4(color.x, color.y, color.z, 0.5), color, waterProfu);
    color = mix(FOG_Color, color, visiblity);
}