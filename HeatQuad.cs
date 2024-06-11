using Godot;
using System;

public partial class HeatQuad : Polygon2D {
  private Vector2[] _Vertices;
  private Vector3[] _Sides;
  private ShaderMaterial _ShaderMat;
  
  public override void _Ready() {
    // move to center of screen
    Vector2 res = GetViewportRect().Size;
    Position = 0.5f * res;
    
    // create vertices
    _Vertices = new Vector2[] {
      new Vector2(-0.3f * res.X, 0.1f * res.Y),
      new Vector2(0, -0.2f * res.Y),
      new Vector2(0.3f * res.X, -0.1f * res.Y),
      new Vector2(0, 0.2f * res.Y)
    };
    Polygon = _Vertices;
    
    // load vertices into shader
    _ShaderMat = this.Material as ShaderMaterial;
    _ShaderMat.SetShaderParameter("vertices", _Vertices);
  }
  
  public Vector2 GetVertex(int k) {
    return _Vertices[k];
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
  
  private void UpdateVertex(int k, Vector2 v) {
    _Vertices[k] = v;
    Polygon = _Vertices;
    _ShaderMat.SetShaderParameter("vertices", _Vertices);
  }
  
  public void OnLeftHandleDrag(Vector2 position) {
    UpdateVertex(0, ToLocal(position));
  }
  
  public void OnRightHandleDrag(Vector2 position) {
    UpdateVertex(2, ToLocal(position));
  }
}
