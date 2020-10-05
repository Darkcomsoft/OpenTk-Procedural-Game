#version 330 core
layout (location = 0) in vec4 position;
layout (location = 1) in vec2 aTexCoord;
layout (location = 2) in vec3 Normals;

out vec2 texCoord;
out float visiblity;

uniform mat4 world;
uniform mat4 view;
uniform mat4 projection;

uniform float time;

uniform float FOG_Density; 
uniform float FOG_Gradiante;

const float Profudensity = 0.010; 
const float Profugradiante = 1.5; 

void main()
{
	vec4 worldPosition = position * world;
	vec4 posRelativeCamera = worldPosition * view;


	float timeFinal = time / 200;

	texCoord = aTexCoord * vec2(0.1,0.1) + vec2(-timeFinal, timeFinal);
	
	float dis = length(posRelativeCamera.xyz);
    visiblity = exp(-pow((dis * Profudensity), Profugradiante));
    visiblity = clamp(visiblity, 0.0, 1.0);
	
	gl_Position = posRelativeCamera * projection;
}