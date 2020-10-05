#version 330 core

layout(location = 0) out vec4 color;

in vec4 frag_colors;
in vec2 texCoord;
in float visiblity;

uniform sampler2D texture0;
uniform vec4 FOG_Color;

uniform vec4 AmbienceColor;

void main()
{
    color = texture(texture0, texCoord) * AmbienceColor;

	if (color.a < 0.5) { discard; }//if the color alpha(Texture Alpha) is less of the 0.5, discard this fragment

	color = mix(FOG_Color, color, visiblity);
}
