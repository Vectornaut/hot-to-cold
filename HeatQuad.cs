using Godot;
using System;

public partial class HeatQuad : Polygon2D {
  public override void _Ready() {
    Polygon = new Vector2[] {
      new Vector2(500, 100),
      new Vector2(700, 250),
      new Vector2(500, 500),
      new Vector2(300, 350)
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
  // left side. the line is specified by its start and end points in global
  // coordinates
  private Vector3 Side(Vector2 start, Vector2 end) {
    Vector2 dir = start.DirectionTo(end);
    Vector2 normal = new Vector2(-dir.Y, dir.X);
    return new Vector3(normal.X, normal.Y, -normal.Dot(ToLocal(start)));
  }
}
