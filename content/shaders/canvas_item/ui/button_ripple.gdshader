shader_type canvas_item;

// Copied from https://godotshaders.com/shader/ripple-effect-2/

uniform vec2 circle_center = vec2(0.0);
uniform float time: hint_range(0.0, 1.0) = 1.0;
uniform vec2 size = vec2(64.0);
uniform vec4 color: source_color = vec4(1.0);
uniform vec2 margin = vec2(0.0);

void fragment() {
	vec2 center = vec2(0.5);
	vec2 coords = (UV - center) / vec2((size.x - margin.x * 2.0) / size.x, (size.y - margin.y * 2.0) / size.y) + center;
	vec4 edges = vec4(0.0);
	if (abs(coords.x - center.x) <= 0.5 && abs(coords.y - center.y) <= 0.5) {
		edges.a = 1.0;
	}
	vec2 uv = (UV - circle_center);
	float ratio = size.x / size.y;
    uv.x *= ratio;
	float d = length(uv);
	float circle = smoothstep(d - 0.01, d, 0.25 + (ratio - 0.25) * time - 0.01);
	COLOR = vec4(color.rgb, max(circle - max(time, 0.25), 0.0)) * edges.a;
}