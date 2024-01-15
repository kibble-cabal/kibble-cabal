shader_type canvas_item;

// This shader is kinda bad, should be rewritten

uniform vec2 top_left = vec2(0.0);
uniform vec2 top_right = vec2(0.0);
uniform vec2 bottom_left = vec2(0.0);
uniform vec2 bottom_right = vec2(0.0);

float cross2d( in vec2 a, in vec2 b ) { return a.x*b.y - a.y*b.x; }

vec2 invBilinear( in vec2 p, in vec2 a, in vec2 b, in vec2 c, in vec2 d )
{
    vec2 res = vec2(-1.0);

    vec2 e = b-a;
    vec2 f = d-a;
    vec2 g = a-b+c-d;
    vec2 h = p-a;
        
    float k2 = cross2d( g, f );
    float k1 = cross2d( e, f ) + cross2d( h, g );
    float k0 = cross2d( h, e );
    
    // if edges are parallel, this is a linear equation
    if( abs(k2)<0.001 )
    {
        res = vec2( (h.x*k1+f.x*k0)/(e.x*k1-g.x*k0), -k0/k1 );
    }
    // otherwise, it's a quadratic
    else
    {
        float w = k1*k1 - 4.0*k0*k2;
        if( w<0.0 ) return vec2(-1.0);
        w = sqrt( w );

        float ik2 = 0.5/k2;
        float v = (-k1 - w)*ik2;
        float u = (h.x - f.x*v)/(e.x + g.x*v);
        
        if( u<0.0 || u>1.0 || v<0.0 || v>1.0 )
        {
           v = (-k1 + w)*ik2;
           u = (h.x - f.x*v)/(e.x + g.x*v);
        }
        res = vec2( u, v );
    }
    
    return res;
}


void fragment() {
	vec2 p = UV;
    
    // background
    vec4 col = vec4(0.0);

    // move points
    
    vec2 a = cos(top_left * -1.0 + vec2(1.57));
    vec2 b = cos(top_right * vec2(1.0, -1.0) + vec2(0.0, 1.57));
    vec2 c = cos(bottom_right + vec2(0.0));
    vec2 d = cos(bottom_left * vec2(-1.0) + vec2(1.57, 0.0));

    // area of the quad
    vec2 uv = invBilinear( p, a, b, c, d );
    
    // inside of quad if uv in [0..1]^2
    if( max( abs(uv.x-0.5), abs(uv.y-0.5))<0.5 )
    {
        col = texture( TEXTURE, uv );
    }

	COLOR = col;
}