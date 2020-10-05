#version 330 core

layout(location = 0) out vec4 color;

in vec2 texCoord;
in float visiblity;

uniform sampler2D texture0;
uniform vec4 FOG_Color;

uniform vec4 AmbienceColor;

void main()
{
    color = mix(vec4(1,1,1, 0.5), texture(texture0, texCoord), visiblity);
	

    if (color.a < 0.5){
        discard;
    }

	//color = mix(FOG_Color, color, visiblity);
}
