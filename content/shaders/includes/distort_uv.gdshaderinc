#ifdef USE_UNIFORMS

group_uniforms Distortion;
uniform float speed = 1.0;
uniform float power = 0.1;
uniform float frequency = 5.0;
uniform float angle = 0.0;
uniform int seed = 1;
group_uniforms;

#define distort(_uv) _run_distort(_uv, angle, power, speed, frequency, seed)

#else

#define distort(_uv, _angle, _power, _speed, _frequency, _seed) _run_distort(_uv, _angle, _power, _speed, _frequency, _seed)

#endif

vec2 _run_distort(vec2 _uv, float _angle, float _power, float _speed, float _frequency, int _seed) {
	float _theta  = atan(_uv.y, _uv.x);
    float _radius = length(_uv);
    _radius = pow(_radius, _power * sin(_radius * _frequency - (TIME + (float(_seed) * 3.0)) * _speed) + 1.0);
	_uv.x = _radius * cos(_theta);
    _uv.y = _radius * sin(_theta);
	_uv = _uv - 0.5; // correct positioning
    return 0.5 * (_uv + 1.0);
}