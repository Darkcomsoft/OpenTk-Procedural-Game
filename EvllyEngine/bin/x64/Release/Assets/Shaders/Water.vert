#version 330 core
layout (location = 0) in vec4 position;
layout (location = 1) in vec4 colors;
layout (location = 2) in vec3 Normals;

out vec2 texCoord;
out vec4 colortest;
out vec3 N;
out float visiblity;
out float waterProfu;

uniform mat4 world;
uniform mat4 view;
uniform mat4 projection;
uniform float time;

uniform float FOG_Density; 
uniform float FOG_Gradiante;

const float Profudensity = 0.1; 
const float Profugradiante = -3.5; 

void main()
{
	vec4 worldPosition = position * world;
	vec4 posRelativeCamera = worldPosition * view;

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
	texCoord = vec2(xcoord, zcoord);
    //texCoord += vec2(time, -time) / 30;

    float distance = length(posRelativeCamera.xyz);
    visiblity = exp(-pow((distance * FOG_Density), FOG_Gradiante));
    visiblity = clamp(visiblity, 0.0, 1.0);

    waterProfu = exp(-pow((distance * Profudensity), Profugradiante));
    waterProfu = clamp(waterProfu, 0.0, 1.0);
	
	gl_Position = posRelativeCamera * projection;
}