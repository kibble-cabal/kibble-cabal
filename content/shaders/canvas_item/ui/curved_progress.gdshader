shader_type canvas_item;

uniform float progress: hint_range(0.0, 1.0) = 0.5;
uniform int segments = 4;
uniform float gap: hint_range(0.0, 1.0) = 0.1;
uniform float start_angle: hint_range(0.0, 360.0) = 90.0;
uniform float end_angle: hint_range(0.0, 360.0) = 0.0;
uniform float inner_radius = 0.5;
uniform float outer_radius = 1.0;
uniform vec2 inner_center = vec2(0.0);
uniform vec2 outer_center = vec2(0.0);
uniform vec4 filled_color: source_color = vec4(1.0);
uniform vec4 empty_color: source_color = vec4(vec3(0.0), 0.5);

#include "res://assets/shaders/includes/utilities.gdshaderinc"
#define TOTAL_ANGLE deg_to_rad(end_angle - start_angle)
#define AA ((1.0 / resolution.y) * 2.0)
#define SEGMENT_ANGLE (TOTAL_ANGLE / float(segments))
#define IS_CLOCKWISE (sign(TOTAL_ANGLE) > 0.99)
#define START_ANGLE deg_to_rad(start_angle)
#define END_ANGLE deg_to_rad(end_angle)

varying vec2 resolution;

float theta(vec2 point) {
    float angle = atan(point.y, point.x);
    angle += TAU * float(angle < 0.0);
    return angle;
}

bool is_angle_outside(float angle) {
	return angle < deg_to_rad(min(start_angle, end_angle)) || angle > deg_to_rad(max(start_angle, end_angle));
}

bool hide_pixel(vec2 dir) {
    return fract(theta(dir) / SEGMENT_ANGLE) > gap;
}

void fragment() {
	resolution = 1.0 / SCREEN_PIXEL_SIZE;
	COLOR = empty_color;
	
	float outside = smoothstep(outer_radius + AA, outer_radius - AA, abs(distance(outer_center, UV)));
	float inside = smoothstep(inner_radius - AA, inner_radius + AA, abs(distance(inner_center, UV)));
	float alpha = outside * inside;
	
	float angle = get_angle_of_point(outer_center, UV);
	
	if (is_angle_outside(angle)) alpha = 0.0;
	
	if (alpha > 0.0) alpha = float(hide_pixel(UV - outer_center));
	
	float wrapped_angle = (angle < 0.0 ? angle + deg_to_rad(360.0) : angle);
	float angle_progress = abs(wrapped_angle / (START_ANGLE + END_ANGLE));
	if (IS_CLOCKWISE && progress > angle_progress) COLOR = filled_color;
	if (!IS_CLOCKWISE && 1.0 - progress < angle_progress) COLOR = filled_color;
	
	COLOR.a *= alpha;
}

