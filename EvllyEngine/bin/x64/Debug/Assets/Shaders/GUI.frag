#version 330 core
layout(location = 0) out vec4 color;

in vec2 texCoord;

uniform sampler2D texture0;
uniform int HaveTexture;
uniform vec4 MainColor;

void main()
{
    if (HaveTexture == 1){
        color = texture(texture0, texCoord);
        color *= MainColor;
    }else{
        color = MainColor;
    }
    //color = vec4(1,1,1,1);
}