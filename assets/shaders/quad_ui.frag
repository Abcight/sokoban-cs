#version 330

out vec4 FragColor;
in vec2 texCoord;

uniform vec4 color;
uniform vec2 offset;
uniform vec2 size;
uniform vec2 tileSize;

uniform sampler2D texture0;

float median(float r, float g, float b) 
{
    return max(min(r, g), min(max(r, g), b));
}

void main()
{
    vec2 uv = vec2(texCoord.x, 1 - texCoord.y) * tileSize;
    uv += vec2(offset.x, offset.y);

    vec3 smp = texture(texture0, uv).rgb;
    ivec2 sz = textureSize(texture0, 0).xy;
    float dx = dFdx(uv.x) * sz.x;
    float dy = dFdy(uv.y) * sz.y;
    float toPixels = 8.0 * inversesqrt(dx * dx + dy * dy);
    float sigDist = median(smp.r, smp.g, smp.b);
    float w = fwidth(sigDist);
    float opacity = smoothstep(0.5 - w, 0.5 + w, sigDist);
    
    FragColor = vec4(color.rgb, color.a * opacity);
}