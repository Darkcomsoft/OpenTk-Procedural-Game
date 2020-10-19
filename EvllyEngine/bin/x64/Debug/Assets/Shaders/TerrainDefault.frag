#version 330 core

layout(location = 0) out vec4 color;

in vec2 texCoord;
in vec4 TileColor;
in vec3 N;
in float visiblity;

uniform sampler2D texture0;
uniform vec4 FOG_Color;

uniform vec4 AmbienceColor;

//Blend Fucntion By https://github.com/Jam3/glsl-blend-overlay
vec3 blendOverlay(vec3 base, vec3 blend) {
    return mix(1.0 - 2.0 * (1.0 - base) * (1.0 - blend), 2.0 * base * blend, step(base, vec3(0.5)));
}

void main()
{
	color = mix(texture(texture0, texCoord),vec4(blendOverlay(texture(texture0, texCoord).rgb, TileColor.rgb), 1), TileColor.a) * AmbienceColor;	
    color = mix(FOG_Color, color, visiblity);
}