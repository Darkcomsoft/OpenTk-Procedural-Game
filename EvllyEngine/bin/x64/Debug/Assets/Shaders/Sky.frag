#version 330 core

layout(location = 0) out vec4 color;

in vec3 texCoord;

uniform samplerCube texture0;

uniform vec4 SKY_Color;
uniform vec4 SKY_HoriColor;

void main()
{
    //color = texture(texture0, texCoord);
	float dis = length(texCoord.y);
	
	if (texCoord.y < 0){
		dis = 0;
	}
	
	color += mix(SKY_HoriColor, SKY_Color, abs(dis));
}
