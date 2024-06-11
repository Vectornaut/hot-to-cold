using Godot;
using System;

public partial class HeatQuad : Polygon2D {
  public override void _Ready() {
    // move to center of screen
    Vector2 res = GetViewportRect().Size;
    Position = 0.5f * res;
    
    Polygon = new Vector2[] {
      new Vector2(-0.2f * res.X, 0.2f * res.Y),
      new Vector2(0, -0.4f * res.Y),
      new Vector2(0.2f * res.X, -0.2f * res.Y),
      new Vector2(0, 0.4f * res.Y)
    };
    
    // find side distance functions
    Vector3[] sides = new Vector3[4];
    for (int k = 0; k < sides.Length; k++) {
      sides[k] = Side(Polygon[k], Polygon[(k+1) % Polygon.Length]);
    }
    
    // pass side equations to shader
    ShaderMaterial shaderMat = this.Material as ShaderMaterial;
    shaderMat.SetShaderParameter("sides", sides);
  }
  
  // in homogeneous coordinates, the inner product with this vector gives the
  // signed distance to the given oriented line, with positive distance on the
  // left side. the line is specified by its start and end points in local
  // coordinates
  private Vector3 Side(Vector2 start, Vector2 end) {
    Vector2 dir = start.DirectionTo(end);
    Vector2 normal = new Vector2(-dir.Y, dir.X);
    return new Vector3(normal.X, normal.Y, -normal.Dot(start));
  }
}
