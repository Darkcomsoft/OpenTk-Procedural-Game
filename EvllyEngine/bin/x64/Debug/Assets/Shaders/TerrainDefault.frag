#version 330 core

layout(location = 0) out vec4 color;

//in vec4 frag_colors;
in vec2 texCoord;
in vec4 colortest;
in vec3 N;

uniform sampler2D texture0;
uniform sampler2D texture1;
uniform sampler2D texture2;
uniform sampler2D texture3;
//uniform gl_FogParameters gl_Fog;

void main()
{
    if(N.y > 0){
        color = texture(texture1, texCoord);
    }

    color = texture(texture0, texCoord);
}

/*struct gl_FogParameters {
 vec4 color;
 float density;
 float start;
 float end;
 float scale; // Derived: 1.0 / (end - start)
};*/
