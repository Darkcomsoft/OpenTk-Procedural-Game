#version 330 core
layout (location = 0) in vec3 position;
layout (location = 1) in vec4 colors;
layout (location = 2) in vec2 aTexCoord;

out vec4 frag_colors;
out vec2 texCoord;

uniform mat4 world;
uniform mat4 view;
uniform mat4 projection;

void main()
{
	gl_Position = vec4(position, 1.0) * world * view * projection;

	frag_colors = colors;
	texCoord = aTexCoord;
}