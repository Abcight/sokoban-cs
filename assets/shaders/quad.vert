#version 330 core

out vec2 texCoord;
out vec3 FragPos;

uniform mat4 model;

void main()
{
    float x = float(((uint(gl_VertexID) + 2u) / 3u) % 2u);
    float y = float(((uint(gl_VertexID) + 1u) / 3u) % 2u);

    vec4 crd = vec4(-1.0f + x * 2.0f, -1.0f + y * 2.0f, 0.0f, 1.0f) * model;
    gl_Position = crd;
    FragPos = crd.xyz;
    
    texCoord = vec2(x, y);
}