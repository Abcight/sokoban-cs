#version 330

uniform sampler2D gPosition;
uniform sampler2D gLight;
layout (location = 3) out vec4 gl;

in vec2 TexCoord;

void main()
{
    float cut = 0;
    vec2 uv = TexCoord;
    for(int y = 0; y < 100; y++)
    {
        float depth = texture(gPosition, uv).g;
        if(depth == 1)
        {
            cut = 1;
            break;
        }
        uv.y += 1.0 / 100;
        uv.y = min(1.0, uv.y);
    }
    gl = vec4(cut);
}