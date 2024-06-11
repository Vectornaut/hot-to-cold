shader_type canvas_item;

const int n_sides = 4;
uniform vec3 sides[n_sides];

const float hit_dist = 1.;
const float max_steps = 40.;
const float n_paths = 500.;

const float gamma = 2.2;

// === hash function ===

float hash14(vec4 p4) {
  p4 = fract(p4  * vec4(.1031, .1030, .0973, .1099));
  p4 += dot(p4, p4.wzxy+33.33);
  return fract((p4.x + p4.y) * (p4.z + p4.w));
}

// === walk on spheres ===

int side_hit(vec3 v, float path_cnt) {
  int nearest_side;
  for (float step_cnt = 0.; step_cnt < max_steps; ++step_cnt) {
    // get the distance to nearest side
    float min_side_dist = dot(sides[0], v);
    nearest_side = 0;
    for (int k = 1; k < n_sides; k++) {
      float side_dist = dot(sides[k], v);
      if (side_dist < min_side_dist) {
        min_side_dist = side_dist;
        nearest_side = k;
      }
    }
    
    // if we're close enough to say we've hit a side, stop walking
    if (min_side_dist < hit_dist) return nearest_side;
    
    // take a step
    float angle = 2.*PI * hash14(vec4(v.xy, path_cnt, step_cnt));
    v += min_side_dist * vec3(cos(angle), sin(angle), 0.);
  }
  return -1 - nearest_side;
}

void fragment() {
  vec3 v = vec3(VERTEX, 1.);
  float hot_hits = 0.;
  float no_hits = 0.;
  for (float path_cnt = 0.; path_cnt < n_paths; ++path_cnt) {
    int side = side_hit(v, path_cnt);
    if (side < 0) {
      ++no_hits;
      side = -1 - side;
    }
    if (side > 1) ++hot_hits;
  }
  float hot_hit_frac = hot_hits / n_paths;
  float no_hit_frac = no_hits / n_paths;
  vec3 color = mix(vec3(1., 0., 0.), vec3(hot_hit_frac), pow(1. - no_hit_frac, 4.));
  COLOR = vec4(pow(color, vec3(gamma)), 1.);
}
