shader_type canvas_item;

const int n_vertices = 4;
uniform vec2 vertices[n_vertices];

const float hit_dist = 1.;
const float max_steps = 40.;
const float n_paths = 400.;

const float gamma = 2.2;

// vertex in local coordinates
varying vec2 vertex_loc;

// === hash function ===

float hash14(vec4 p4) {
  p4 = fract(p4  * vec4(.1031, .1030, .0973, .1099));
  p4 += dot(p4, p4.wzxy+33.33);
  return fract((p4.x + p4.y) * (p4.z + p4.w));
}

// === walk on spheres ===

// the distance from the point `v` to the segment that runs from `start` to
// `end`. from Inigo Quilez's signed distance function stash:
//
//   https://iquilezles.org/articles/distfunctions2d/
//
float seg_dist(vec2 v, vec2 start, vec2 end) {
  vec2 v_rel = v - start;
  vec2 dis = end - start;
  float line_proj_param = dot(v_rel, dis) / dot(dis, dis);
  float seg_proj_param = clamp(line_proj_param, 0., 1.);
  return distance(v_rel, seg_proj_param * dis);
}

int side_hit(vec2 v, float path_cnt) {
  int nearest_side;
  for (float step_cnt = 0.; step_cnt < max_steps; ++step_cnt) {
    // get the distance to nearest side
    float min_side_dist = seg_dist(v, vertices[0], vertices[1]);
    nearest_side = 0;
    for (int k = 1; k < n_vertices; k++) {
      float side_dist = seg_dist(v, vertices[k], vertices[(k+1) % n_vertices]);
      if (side_dist < min_side_dist) {
        min_side_dist = side_dist;
        nearest_side = k;
      }
    }
    
    // if we're close enough to say we've hit a side, stop walking
    if (min_side_dist < hit_dist) return nearest_side;
    
    // take a step
    float angle = 2.*PI * hash14(vec4(v.xy, path_cnt, step_cnt));
    v += min_side_dist * vec2(cos(angle), sin(angle));
  }
  return -1 - nearest_side;
}

void vertex() {
  vertex_loc = VERTEX;
}

void fragment() {
  float hot_hits = 0.;
  float no_hits = 0.;
  for (float path_cnt = 0.; path_cnt < n_paths; ++path_cnt) {
    int side = side_hit(vertex_loc, path_cnt);
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
  /* distance test
  vec3 color;
  if (mod(seg_dist(vertex_loc, vertices[0], vertices[1]), 10.) < 5.) {
    color = 0.8 * vec3(1.0, 0.6, 0.8);
  } else {
    color = vec3(1.0, 0.8, 0.9);
  }
  COLOR = vec4(color, 1.);
  */
}
