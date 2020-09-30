#version 330 core
layout (location = 0) in vec2 position;
layout (location = 1) in vec2 uv;

out vec2 texCoord;

uniform mat4 projection;


void main()
{
	//texCoord = vec2((position.x + 1.0) / 2, (position.y + 1.0) / 2);
	texCoord = uv;

	gl_Position = vec4(position, 0.0, 1.0) * projection;
}