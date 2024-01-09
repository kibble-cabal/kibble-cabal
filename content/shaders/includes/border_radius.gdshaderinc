#ifndef DEFAULT_RADIUS
#define DEFAULT_RADIUS 16.0
#endif

group_uniforms Radius;
uniform float radius_top_left = DEFAULT_RADIUS;
uniform float radius_top_right = DEFAULT_RADIUS;
uniform float radius_bottom_left = DEFAULT_RADIUS;
uniform float radius_bottom_right = DEFAULT_RADIUS;
group_uniforms;


float get_radius(vec2 _uv) {
	if (_uv.x < 0.5) return (_uv.y < 0.5) ? radius_top_left : radius_bottom_left;
	else return (_uv.y < 0.5) ? radius_top_right : radius_bottom_right;
}