shader_type canvas_item;


void fragment() {
	vec2 res = 1.0 / SCREEN_PIXEL_SIZE;
	float dist = distance(UV, vec2(0.5));
	COLOR.a *= smoothstep(0.5, 0.5 + 5.0 / res.x, 1.0 - dist);
}
