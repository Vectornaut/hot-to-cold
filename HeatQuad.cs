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
    
    // create vertex handles
    Handle leftHandle = new Handle(new Vector2(-0.2f * res.X, 0.2f * res.Y));
    Handle rightHandle = new Handle(new Vector2(0.2f * res.X, -0.2f * res.Y));
    leftHandle.Drag += OnLeftHandleDrag;
    rightHandle.Drag += OnRightHandleDrag;
    AddChild(leftHandle);
    AddChild(rightHandle);
    
    // create vertices
    _Vertices = new Vector2[] {
      leftHandle.Position,
      new Vector2(0, -0.3f * res.Y),
      rightHandle.Position,
      new Vector2(0, 0.3f * res.Y)
    };
    Polygon = _Vertices;
    
    // load vertices into shader
    _ShaderMat = this.Material as ShaderMaterial;
    _ShaderMat.SetShaderParameter("vertices", _Vertices);
  }
  
  private void UpdateVertex(int k, Vector2 v) {
    _Vertices[k] = v;
    Polygon = _Vertices;
    _ShaderMat.SetShaderParameter("vertices", _Vertices);
  }
  
  private void OnLeftHandleDrag(Vector2 position) {
    UpdateVertex(0, ToLocal(position));
  }
  
  private void OnRightHandleDrag(Vector2 position) {
    UpdateVertex(2, ToLocal(position));
  }
}
