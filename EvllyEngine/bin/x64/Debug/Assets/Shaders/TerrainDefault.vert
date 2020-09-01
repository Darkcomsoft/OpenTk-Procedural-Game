#version 330 core
layout (location = 0) in vec4 position;
layout (location = 1) in vec4 colors;
layout (location = 2) in vec2 aTexCoord;
layout (location = 3) in vec3 Normals;

//out vec4 frag_colors;
out vec2 texCoord;
out vec4 colortest;
out vec3 N;

uniform mat4 world;
uniform mat4 view;
uniform mat4 projection;

void main()
{
	vec4 worldPosition = world * position;
	vec4 posRelativeCamera = view * worldPosition;

	float xcoord = position.x - Normals.x;
    float zcoord = position.z - Normals.z;
    float ycoord = position.y - Normals.y;
     
	//colortest = vec4(Normals.x,Normals.y,Normals.z,1);

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

	//frag_colors = colors;
	texCoord = aTexCoord;
	
	gl_Position = position * world * view * projection;
}