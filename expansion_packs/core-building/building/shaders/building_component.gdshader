shader_type canvas_item;

// Replaces a color in the tile map with the provided texture repeated.

uniform sampler2D screen_texture: hint_screen_texture;

uniform sampler2D floor_texture;
uniform vec4 floor_color: source_color;

uniform sampler2D interior_texture;
uniform vec4 interior_color: source_color;

uniform sampler2D exterior_texture;
uniform vec4 exterior_color: source_color;

uniform vec2 size = vec2(1.0, 1.0);

bool is_close(float a, float b) {
	return abs(b - a) < 0.00001;
}

bool same_color(vec4 a, vec4 b) {
	return (is_close(a.r, b.r) 
		&& is_close(a.g, b.g) 
		&& is_close(a.b, b.b) 
		&& is_close(a.a, b.a));
}

void fragment() {
	vec2 res = 1.0 / SCREEN_PIXEL_SIZE;
	COLOR = texture(screen_texture, SCREEN_UV);
	if (same_color(floor_color, COLOR))
		COLOR = texture(floor_texture, fract(UV * size));
	
	if (same_color(interior_color, COLOR))
		COLOR = texture(interior_texture, fract(UV * size));
	
	if (same_color(exterior_color, COLOR))
		COLOR = texture(exterior_texture, fract(UV * size));
	
}

//void light() {
	// Called for every pixel for every light affecting the CanvasItem.
	// Uncomment to replace the default light processing function with this one.
//}
