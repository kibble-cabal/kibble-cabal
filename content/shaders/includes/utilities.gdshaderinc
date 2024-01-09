#ifndef UTILITIES
#define UTILITIES

float map_float(float value, float min1, float max1, float min2, float max2) {
	return min2 + (value - min1) * (max2 - min2) / (max1 - min1);
}

vec2 map_vec2(vec2 value, vec2 min1, vec2 max1, vec2 min2, vec2 max2) {
	return vec2(
		map_float(value.x, min1.x, max1.x, min2.x, max2.x),
		map_float(value.y, min1.y, max1.y, min2.y, max2.y)
	);
}


vec2 get_point_on_circle(vec2 _center, float _radius, float _angle) {
	return vec2(
		_center.x + (_radius * cos(_angle)),
		_center.y + (_radius * sin(_angle))
	);
}

float get_angle_of_point(vec2 _center, vec2 _point) {
	return atan(_point.y - _center.y, _point.x - _center.x);
}

float rad_to_deg(float _rad) {
	return _rad * 180.0 / PI;
}

float deg_to_rad(float _deg) {
	return _deg * PI / 180.0;
}

#endif