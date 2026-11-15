namespace ShGame.Drawing;

using ShGame.Util;

public static class ShaderSources {

    public static string TEXTURE_VERTEX_SHADER_SOURCE = @"
#version 330 core

layout (location = 0) in vec3 aPosition;
uniform float u_WindowWidth;
uniform float u_WindowHeight;

out vec2 frag_texCoords;

void main()
{
    vec2 ndc = vec2(
		aPosition.x / ("+Constants.MAP_GRID_WIDTH+@".0/2) - 1.0,
		aPosition.y / ("+Constants.MAP_GRID_HEIGHT+@".0/2) - 1.0
	);
	gl_Position = vec4(ndc, aPosition.z, 1.0);

    frag_texCoords = ndc;
}
";

	public static string TEXTURE_FRAGMENT_SHADER_SOURCE = @"
#version 330 core

in vec2 frag_texCoords;
out vec4 out_color;

uniform float u_WindowWidth;
uniform float u_WindowHeight;

uniform sampler2D uTexture;

void main()
{
    out_color = texture(uTexture, frag_texCoords);
}
";

	public const string COLOR_VERTEX_SHADER_SOURCE =
@"
#version 330 core

layout(location = 0) in vec2 aPosition;
layout(location = 1) in vec4 aColor;
uniform float u_WindowWidth;
uniform float u_WindowHeight;

out vec4 vColor;

void main()
{
	vColor = aColor;

	vec2 ndc = vec2(
		-1.0 + aPosition.x / (u_WindowWidth / 2.0),
		1.0 - aPosition.y / (u_WindowHeight / 2.0));

	gl_Position = vec4(ndc, 0.0, 1.0);
}";

	public const string COLOR_FRAGMENT_SHADER_SOURCE =
@"
#version 330 core

out vec4 FragColor;

uniform vec2 u_mouse;
uniform float u_time;

in vec4 vColor;

void main()
{
	FragColor = vColor;
}";

}