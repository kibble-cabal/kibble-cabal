#ifndef ROTATE_UV
#define ROTATE_UV
vec2 rotate_uv(vec2 _uv, float _rotation) {
    float _midpoint = 0.5;
    return vec2(
        cos(_rotation) * (_uv.x - _midpoint) + sin(_rotation) * (_uv.y - _midpoint) + _midpoint,
        cos(_rotation) * (_uv.y - _midpoint) - sin(_rotation) * (_uv.x - _midpoint) + _midpoint
    );
}

vec2 rotate_uv_with_midpoint(vec2 _uv, float _rotation, vec2 _midpoint) {
    return vec2(
      cos(_rotation) * (_uv.x - _midpoint.x) + sin(_rotation) * (_uv.y - _midpoint.y) + _midpoint.x,
      cos(_rotation) * (_uv.y - _midpoint.y) - sin(_rotation) * (_uv.x - _midpoint.x) + _midpoint.y
    );
}

vec2 flip_uv_x (vec2 _uv) {
    _uv.x = 1.0 - _uv.x;
    return _uv;
}

vec2 flip_uv_y (vec2 _uv) {
    _uv.y = 1.0 - _uv.y;
	return _uv;
}

vec2 flip_uv (vec2 _uv) {
	_uv = flip_uv_x(_uv);
	return flip_uv_y(_uv);
}
#endif