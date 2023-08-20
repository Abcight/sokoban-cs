#version 330

layout (location = 0) out vec4 gPosition;
layout (location = 2) out vec4 gColor;

uniform vec2 offset;

in vec2 texCoord;

const int gridWidth = 16;
const int gridHeight = 4;

uniform int shadowCaster = 0;
uniform sampler2D texture0;

void main()
{
	vec2 tex = vec2(texCoord.x, 1 - texCoord.y);
	vec2 uv = vec2( tex.x / gridWidth, tex.y / gridHeight );

	float xCoord = uv.x + (offset.x / gridWidth);
	float yCoord = uv.y + (offset.y / gridHeight);

	vec2 coords = vec2(xCoord, yCoord);
	vec4 color = texture(texture0, coords);

	gPosition = vec4(0, shadowCaster * color.a, 0, 1.0);
	gColor = color;
}