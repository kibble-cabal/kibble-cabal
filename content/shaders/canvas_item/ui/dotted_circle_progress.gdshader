shader_type canvas_item;

// Copied with adjustments from https://www.shadertoy.com/view/msfSDj

uniform float progress: hint_range(0.0, 1.0) = 0.0;
uniform float filled_thickness = 0.04;
uniform float empty_thickness = 0.02;
uniform float num_segments = 30.0;
uniform vec3 filled_color: source_color = vec3(1.0);
uniform vec3 empty_color: source_color = vec3(0.2,0.2,0.2);

#include "res://assets/shaders/includes/rotate_uv.gdshaderinc"

mat2 rot (float a) {
    float c = cos(a);
    float s = sin(a);
    return mat2(vec2(c, -s), vec2(s, c));
}

float sd_dashed_circle (in vec2 p, float at, float inner, float outer, float nseg, float rat) {
    float le = length(p);
    float hw = (outer - inner) * 0.5;
    float ce = le - (outer - hw);
    ce = abs(ce) - hw;
    float gr = fract((at / TAU) * nseg); 
    gr = abs((gr - 0.5) * 2.0);
    gr = gr - rat;
    float an = gr * (PI / nseg);
    mat2 rm = rot(an);
    float c = rm[0][0];
    float ep = clamp(c * le, inner, outer);
    vec2  ne = p * rm;
    ne *= ep / le;  
    float ed = length(p - ne);
    return (an > 0.0) ? ed : ((ce > 0.0) ? ce : max(-ed, ce));
}

vec4 get_timer(in vec2 p, float fill) {
    float extra = 10.0;
    float thresh = num_segments + (extra * 0.5);
    fill *= (num_segments + 1.0 + extra);
    
    float at = atan(p.x, p.y);
    float count = floor(at / (TAU / num_segments)) + num_segments * 0.5;
    
    float fade = clamp((fill - count) * 0.1, 0.0, 1.0);
    if (fill > thresh) fade *= 1.0 - clamp((fill - thresh) * 0.05, 0.0, 1.0);

    // render circle
    float rad = 1.0 - filled_thickness;
    float height = mix(empty_thickness, filled_thickness, fade);    
    float inner = rad - height;
    float outer = rad + height;
    float rt = 0.65;
    float d =  sd_dashed_circle(p, at, inner, outer, num_segments, rt);
    float aa = 1.5 * rt * fwidth(at);
    float cc = smoothstep(aa, 0.0, d);
    return vec4(mix(empty_color * cc, filled_color * cc, fade), cc);
}


void fragment () {
	vec2 uv = flip_uv_y(UV);
	vec2 resolution = 1.0 / SCREEN_PIXEL_SIZE;
    vec2 p = (uv - vec2(0.5)) * 2.0;
    COLOR = get_timer(-p, progress);
	
	// gets rid of a line artefact bug that I don't want to troubleshoot atm
	if (uv.x > 0.49 && uv.x < 0.51) {
		COLOR.a = 0.0;
	}
}
