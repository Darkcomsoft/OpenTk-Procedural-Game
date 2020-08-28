#version 330 core
layout (location = 0) in vec4 position;
layout (location = 1) in vec4 colors;
layout (location = 2) in vec2 aTexCoord;
layout (location = 3) in vec3 Normals;

out vec4 frag_colors;
out vec2 texCoord;

uniform mat4 world;
uniform mat4 view;
uniform mat4 projection;

void main()
{
	vec4 worldPosition = world * position;
	vec4 posRelativeCamera = view * worldPosition;

	frag_colors = colors;
	texCoord = aTexCoord;
	
	gl_Position = position * world * view * projection;
}