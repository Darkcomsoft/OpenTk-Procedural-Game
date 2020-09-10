#version 330 core

layout(location = 0) out vec4 color;

in vec2 texCoord;
in vec4 colortest;
in vec3 N;
in float visiblity;

uniform sampler2D texture0;
uniform vec4 FOG_Color;

void main()
{
    color = texture(texture0, texCoord);
    color = mix(FOG_Color, color, visiblity);
}