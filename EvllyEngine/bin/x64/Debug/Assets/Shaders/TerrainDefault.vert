#version 330 core
layout (location = 0) in vec4 position;
layout (location = 1) in vec4 colors;
layout (location = 2) in vec2 aTexCoord;
layout (location = 3) in vec3 Normals;

out vec2 texCoord;
out vec4 TileColor;
out vec3 N;
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

	float xcoord = position.x - Normals.x;
    float zcoord = position.z - Normals.z;
    float ycoord = position.y - Normals.y;

    N = Normals;
	
    // projection1. y is largest normal component
    // so use x and z to sample texture
    //texCoord = vec2(xcoord,zcoord); //first projection
    // projection2. x is largest normal component
    // so use z and y to sample texture
    //texCoord= vec2(zcoord,ycoord); //second projection
    // projection3. z is largest normal component
    // so use x and y to sample texture
    //texCoord = vec2(xcoord,ycoord); //third projection

	TileColor = colors;
	texCoord = aTexCoord;
	
    float distance = length(posRelativeCamera.xyz);
    visiblity = exp(-pow((distance * FOG_Density), FOG_Gradiante));
    visiblity = clamp(visiblity, 0.0, 1.0);


	gl_Position = posRelativeCamera * projection;
}