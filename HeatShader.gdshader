shader_type canvas_item;

uniform vec3 sides[4];

void fragment() {
  vec3 v = vec3(VERTEX, 1.);
  COLOR = exp(-0.006 * vec4(dot(v, sides[0]), dot(v, sides[1]), 0., 0.));
}
