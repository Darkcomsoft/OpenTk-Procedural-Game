#version 330 core
layout (location = 0) in vec2 position;

out vec2 texCoord;

uniform mat4 world;
//uniform mat4 view;
uniform mat4 projection;

void main()
{
	texCoord = vec2((position.x + 1.0) / 2, (position.y + 1.0) / 2);

	gl_Position = projection  * vec4(position, 0.0, 1.0) * world;
}