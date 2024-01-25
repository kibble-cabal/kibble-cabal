shader_type canvas_item;
render_mode unshaded;

uniform float warp_frequency = 1.0;
uniform float warp_speed = 3.0;
uniform float warp_effect = 1.0;
uniform int seed = 1;

void vertex() {
	// Get the UV Coordinate of your texture or Screen Texture, yo!
	vec2 uv = UV;
	
	// Flip that shit, cause shadertool be all "yolo opengl"
	//uv.y = -1.0 - uv.y;
	uv.x += sin(uv.y * warp_frequency + (float(seed) / 3.0 + TIME) * warp_speed) / warp_frequency;
	uv.y += sin(uv.x * warp_frequency + (float(seed) / 5.0 + TIME) * warp_speed) / warp_frequency;
	
	vec2 vert_effect = uv * warp_effect * 10.0;
	VERTEX += vert_effect;	
}