shader_type canvas_item;
//render_mode blend_add;

uniform float num_bubbles = 9.0;
uniform float seed = 10.0;
uniform vec3 color: source_color = vec3(0.0);
#define TWO_PI 6.28318530718

// via Dave Hoskins
float hash11(float p)
{
    p = fract(p * 0.1031);
    p *= p + 33.33;
    p *= p + p;
    p = fract(p);
    return p * 2. - 1.;
}

varying vec2 vert;

void vertex() {
	vert = VERTEX;
}

void fragment() {
	//vec2 res = 1.0 / SCREEN_PIXEL_SIZE;
	vec2 uv = (UV * 2.0 - vec2(1.0));

	float c = 0.;
    float r = 1.;
    
    // center circle
    c = 1. - (length(uv) - (0.5 * r));

    // edge circles	
	for (float i = 0.; i < num_bubbles; i++) {
        float rnd = hash11((i / num_bubbles) + seed);
        
        float th = (i / num_bubbles) + rnd * 0.02;
        float x = cos(th * TWO_PI) * r;
        float y = sin(th * TWO_PI) * r;
      	
		float size = 1.25 + sin(abs(rnd)) + sin(300.0 * rnd + TIME * 0.75) * 0.3;
		float rad = (0.2 + 0.5 * size * 0.25) * r;
		vec2  pos = vec2(x, y) * 0.5 * r;
		float dis = length(uv - pos);
		
		c = max(c, 1. - (dis - rad));
	}
    
    c = smoothstep(0.99, 1., c);
	COLOR = vec4(color, c);
}