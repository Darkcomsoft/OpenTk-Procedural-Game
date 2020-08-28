#version 330 core
layout(location = 0) out vec4 color;

in vec4 frag_colors;
in vec2 texCoord;

uniform sampler2D texture0;

void main()
{
    color = texture(texture0, texCoord);
}