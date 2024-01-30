shader_type canvas_item;

#define AMOUNT_OF_COLORS 8
#define EPSILON 0.01

group_uniforms Color1;
uniform bool should_replace_color_1;
uniform vec4 replace_color_1: source_color;
uniform vec4 use_color_1: source_color;
group_uniforms;

group_uniforms Color2;
uniform bool should_replace_color_2;
uniform vec4 replace_color_2: source_color;
uniform vec4 use_color_2: source_color;
group_uniforms;


group_uniforms Color3;
uniform bool should_replace_color_3;
uniform vec4 replace_color_3: source_color;
uniform vec4 use_color_3: source_color;
group_uniforms;

group_uniforms Color4;
uniform bool should_replace_color_4;
uniform vec4 replace_color_4: source_color;
uniform vec4 use_color_4: source_color;
group_uniforms;



bool same_color(vec4 a, vec4 b) {
	vec4 diff = abs(b - a);
	return diff.r <= EPSILON && diff.g <= EPSILON && diff.b <= EPSILON && diff.a <= 0.65;
}


void fragment() {
	if (should_replace_color_1 && same_color(COLOR, replace_color_1)) {
		COLOR = use_color_1;
	} else if (should_replace_color_2 && same_color(COLOR, replace_color_2)) {
		COLOR = use_color_2;
	} else if (should_replace_color_3 && same_color(COLOR, replace_color_3)) {
		COLOR = use_color_3;
	} else if (should_replace_color_4 && same_color(COLOR, replace_color_4)) {
		COLOR = use_color_4;
	}
}