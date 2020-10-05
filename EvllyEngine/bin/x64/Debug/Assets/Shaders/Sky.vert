#version 330 core
layout (location = 0) in vec4 position;
layout (location = 1) in vec2 aTexCoord;
layout (location = 2) in vec3 Normals;

out vec3 texCoord;

uniform mat4 world;
uniform mat4 view;
uniform mat4 projection;

void main()
{
	vec4 worldPosition = position * world;
	vec4 posRelativeCamera = worldPosition * view;

	texCoord = position.xyz;

	gl_Position = posRelativeCamera * projection;
}