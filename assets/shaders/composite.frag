#version 330

out vec4 FragColor;

in vec2 TexCoord;

const float PI = 3.14159265359;

uniform sampler2D gPosition;
uniform sampler2D gLight;
uniform sampler2D gColor;
uniform sampler2D gLightCut;

uniform int advancement;
uniform int reverse;
uniform float blurAmount;

float saturate(float f)
{
    return min(max(f, 0.0), 1.0);
}

float invlerp(float a, float b, float v)
{
    return (v - a) / (b - a);
}

float remap(float imin, float imax, float omin, float omax, float v)
{
    float t = invlerp(imin, imax, v);
    return mix(omin, omax, t);
}

vec4 gauss(sampler2D image, vec2 uv, vec2 resolution, vec2 direction) 
{
    vec4 color = vec4(0.0);
    
    vec2 off1 = vec2(1.411764705882353) * direction;
    vec2 off2 = vec2(3.2941176470588234) * direction;
    vec2 off3 = vec2(5.176470588235294) * direction;
    
    color += texture2D(image, uv) * 0.1964825501511404;
    color += texture2D(image, uv + (off1 / resolution)) * 0.2969069646728344;
    color += texture2D(image, uv - (off1 / resolution)) * 0.2969069646728344;
    color += texture2D(image, uv + (off2 / resolution)) * 0.09447039785044732;
    color += texture2D(image, uv - (off2 / resolution)) * 0.09447039785044732;
    color += texture2D(image, uv + (off3 / resolution)) * 0.010381362401148057;
    color += texture2D(image, uv - (off3 / resolution)) * 0.010381362401148057;
    
    return color;
}

void main()
{
    vec3 color = gauss(gColor, TexCoord, vec2(320, 180), vec2(blurAmount)).rgb;
    
    vec3 lightColor = vec3(255, 248, 227) / 255.0;

    float light = saturate(gauss(gLight, TexCoord, vec2(320, 180), vec2(0.5, 0)).r);
    float cut = texture(gLightCut, TexCoord).r;
    
    light = saturate(mix(light, 0, 1.2 - TexCoord.y));
    light = saturate(remap(0, 0.8, 0, 1, light));

    light -= cut * 0.3;
    light = saturate(light);
    
    vec3 ambient = 0.5 * color * lightColor;
    vec3 lighting = ambient * vec3(1 + light) * lightColor;

    lighting = lighting / (lighting + vec3(1.0));
    
    FragColor = vec4(lighting, 1.0);

    float vignette = length(vec2(0.5, 0.5) - TexCoord) * 0.5;
    FragColor = mix(FragColor, vec4(48 / 255.0, 25 / 255.0, 11 / 255.0, 1), vignette);
    
    float adv1 = round(TexCoord.x * 32) / 32 + (advancement - 1) / 32.0;
    float adv2 = round((1 - TexCoord.x) * 32) / 32 + (advancement - 1) / 32.0;
    
    float adv = adv1;
    if(reverse == 1)
        adv = adv2;
    
    float total = 1 - saturate(floor(adv));
    
    FragColor = mix(vec4(0,0,0,1), FragColor, total);
}