shader_type canvas_item;

// Borrowed from https://www.shadertoy.com/view/3styRH

uniform sampler2D sprite;
uniform float upscale_amount = 8.0;
// equality threshold of 2 colors before forming lines
uniform float threshold = 0.1;

// line thickness
#define LINE_THICKNESS 0.4

// anti aliasing scaling, smaller value make lines more blurry
#define AA_SCALE (upscale_amount * 1.0)


// draw diagonal line connecting 2 pixels if within threshold
vec4 diag(vec4 sum, vec2 uv, vec2 p1, vec2 p2) {
    vec4 v1 = texelFetch(sprite, ivec2(uv + vec2(p1.x, p1.y)), 0),
         v2 = texelFetch(sprite, ivec2(uv + vec2(p2.x, p2.y)), 0);
    if (length(v1 - v2) < threshold) {
    	vec2 dir = p2 - p1,
             lp = uv - (floor(uv + p1) + 0.5);
    	dir = normalize(vec2(dir.y, -dir.x));
        float l = clamp((LINE_THICKNESS - dot(lp, dir)) * AA_SCALE, 0.0 , 1.0);
        sum = mix(sum, v1, l);
    }
    return sum;
}

void fragment() {
    vec2 ip = UV / SCREEN_PIXEL_SIZE / upscale_amount;
    
    // start with nearest pixel as 'background'
    vec4 s = texelFetch(sprite, ivec2(ip), 0);

    // draw anti aliased diagonal lines of surrounding pixels as 'foreground'
    s = diag(s, ip, vec2(-1, 0), vec2(0, 1));
    s = diag(s, ip, vec2(0, 1), vec2(1, 0));
    s = diag(s, ip, vec2(1, 0), vec2(0, -1));
    s = diag(s, ip, vec2(0, -1), vec2(-1, 0));

    COLOR = s;
}