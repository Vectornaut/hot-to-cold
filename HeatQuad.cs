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
