#version 330 core
layout (location = 0) in vec4 position;
layout (location = 1) in vec4 colors;
layout (location = 2) in vec2 aTexCoord;
layout (location = 3) in vec3 Normals;

out vec4 frag_colors;
out vec2 texCoord;
out float visiblity;

uniform mat4 world;
uniform mat4 view;
uniform mat4 projection;

uniform float FOG_Density; 
uniform float FOG_Gradiante;

void main()
{
	vec4 worldPosition = position * world;
	vec4 posRelativeCamera = worldPosition * view;

	frag_colors = colors;
	texCoord = aTexCoord;
	
	float distance = length(posRelativeCamera.xyz);
    visiblity = exp(-pow((distance * FOG_Density), FOG_Gradiante));
    visiblity = clamp(visiblity, 0.0, 1.0);
	
	gl_Position = posRelativeCamera * projection;
}