shader_type canvas_item;

//----------------------sRGB----------------------
// from nmz's 3d color space visualization
// https://www.shadertoy.com/view/XddGRN

// map colors from RGB space to sRGB space. in RGB space, color value is
// proportional to light intensity, so linear color-vector interpolation
// corresponds to physical light mixing. in sRGB space, the color encoding
// used by many monitors, we use more of the value interval to represent low
// intensities, and less of the interval to represent high intensities. this
// improves color quantization. see explore-lab/explore-lab-l.frag to learn more

float sRGB_ch(float t){ return mix(1.055*pow(t, 1./2.4) - 0.055, 12.92*t, step(t, 0.0031308)); }
vec3 sRGB(in vec3 c) { return vec3 (sRGB_ch(c.x), sRGB_ch(c.y), sRGB_ch(c.z)); }

//----------------------CIE Lab----------------------
// from nmz's 3d color space visualization
// https://www.shadertoy.com/view/XddGRN

// map colors from Lab space to RGB space. see explore-lab/explore-lab-l.frag
// to learn more

const vec3 wref =  vec3(.95047, 1.0, 1.08883);

float xyzR(float t){ return mix(t*t*t , 0.1284185*(t - 0.139731), step(t,0.20689655)); }

vec3 lab2rgb(in vec3 c)
{
    float lg = 1./116.*(c.x + 16.);
    vec3 xyz = vec3(wref.x*xyzR(lg + 0.002*c.y),
                    wref.y*xyzR(lg),
                    wref.z*xyzR(lg - 0.005*c.z));
    vec3 rgb = xyz*mat3(vec3( 3.2406, -1.5372,-0.4986),
                        vec3(-0.9689,  1.8758, 0.0415),
                        vec3( 0.0557, -0.2040, 1.0570));
    return rgb;
}

// === color scale ===

void fragment() {
  // set temperature scale endpoints
  const vec3 cold = vec3(24., 43., -59.);
  const vec3 hot = vec3(65., 62., -18.);
  
  // map temperature to color
  vec3 color = mix(cold, hot, COLOR.x);
  
  // convert color to screen RGB
  COLOR.rgb = sRGB(lab2rgb(color));
}