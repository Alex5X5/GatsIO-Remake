namespace ShGame.Drawing;

using ShGame.Util;

public static class ShaderSources {

    public static string TEXTURE_VERTEX_SHADER_SOURCE =
@"
#version 330 core

layout (location = 0) in vec3 aPosition;

out vec2 frag_texCoords;

void main()
{
    vec2 ndc = vec2(
		aPosition.x / ("+Constants.MAP_GRID_WIDTH+@"/2) - 1.0,
		aPosition.y / ("+Constants.MAP_GRID_HEIGHT+@"/2) - 1.0
	);
	gl_Position = vec4(ndc, aPosition.z, 1.0);

    frag_texCoords = ndc;
}
";

    public static string TEXTURE_FRAGMENT_SHADER_SOURCE =
@"
#version 330 core

in vec2 frag_texCoords;
out vec4 out_color;

uniform sampler2D uTexture;

void main()
{
	vec2 ndc = vec2(
		frag_texCoords.x / ("+Constants.MAP_GRID_WIDTH+@"/2) - 1.0,
		frag_texCoords.y / ("+Constants.MAP_GRID_HEIGHT+@"/2) - 1.0
	);
    out_color = texture(uTexture, ndc);
}
";

    public static readonly string STATIC_VERTEXT_SHADER_SOURCE =
@"
#version 330 core

layout(location = 0) in vec3 aPosition;
uniform float u_WindowWidth;
uniform float u_WindowHeight;
void main()
{
	vec2 ndc = vec2(
		aPosition.x / ("+Constants.MAP_GRID_WIDTH+@"/2) - 1.0,
		aPosition.y / ("+Constants.MAP_GRID_HEIGHT+@"/2) - 1.0
	);
	gl_Position = vec4(ndc, aPosition.z, 1.0);
}";

    public static readonly string STATIC_FRAGMENT_SHADER_SOURCE =
@"
#version 330 core
#ifdef GL_ES
precision mediump float;
#endif

out vec4 FragColor;
uniform int colorMode;
uniform vec2 u_mouse;
uniform float u_time;
uniform float u_colorMode;
uniform float u_WindowWidth;
uniform float u_WindowHeight;

float function(vec2 point) {
	return ((sin(point.y*1.804-2.228)+sin(((point.x*7.168-1.476)/(0.948)))+((sin(8.920*point.x+8.160*point.y-
		9.224)+1.2*pow(sin(point.x*7.864+-1.512)+
		((sin(point.x*8.988+point.y*7.0-(sin(u_time/20.0)/2.0+0.5)*1.1))/(2.0)),2.0)+
		sin(point.x*3.104+5.124-(sin(u_time/20.0)/2.0+0.5)*14.220*point.y))/(2.400))+3.504)/(7.0));
}

void main()
{
	vec2 u_resolution = vec2(u_WindowWidth,u_WindowHeight);
	vec2 st = gl_FragCoord.xy/u_resolution.xy;
	st.x *= u_resolution.x/u_resolution.y;
	st.y *= u_resolution.y/u_resolution.x;

	vec3 color;

	if (colorMode == 0)
		color = vec3(
			function(st*2.0)*0.28+0.3,
			function(st*2.0)*0.36+0.3,
			function(st*2.0)*0.48+0.3
		);
	else if (colorMode == 1)
		color = vec3(1.0, 0.0, 0.0); // Red
	else if (colorMode == 2)
		color = vec3(0.0, 0.0, 1.0); // Blue
	else if (colorMode == 3)
		color = vec3(0.0, 0.0, 0.0); // BLACK
	else
		color = vec3(1.0); // Default white

	FragColor = vec4(color, 1.0);
}";

}