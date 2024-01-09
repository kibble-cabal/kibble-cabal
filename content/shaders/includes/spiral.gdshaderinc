
float spiral(vec2 p, vec2 c, float num_spirals, float rotation) {
	float a = 1.0;
	float b = num_spirals;
    p = p - c;
    float t = atan(p.y, p.x) + rotation * 8.0;
    float r = length(p.xy);
    
    float n = (log(r / a) / b - t) / (2.0 * PI);

    float upper_r = a * exp(b * (t + 2.0 * PI * ceil(n)));
    float lower_r = a * exp(b * (t + 2.0 * PI * floor(n)));
    
    return min(abs(upper_r - r), abs(r - lower_r));
}