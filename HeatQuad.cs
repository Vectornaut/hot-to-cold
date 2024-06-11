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
    
    // find side distance functions
    _Sides = new Vector3[4];
    for (int k = 0; k < _Sides.Length; k++) {
      _Sides[k] = Side(_Vertices[k], _Vertices[(k+1) % _Vertices.Length]);
    }
    
    // save a reference to the shader material
    _ShaderMat = this.Material as ShaderMaterial;
    
    LoadVerticesAndSides();
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
  
  // set the vertex array and pass the side equations to shader
  private void LoadVerticesAndSides() {
    Polygon = _Vertices;
    _ShaderMat.SetShaderParameter("sides", _Sides);
  }
  
  private void UpdateVertex(int k, Vector2 v) {
    // update vertex
    _Vertices[k] = v;
    
    // update sides
    int prev = (k + _Vertices.Length - 1) % _Vertices.Length;
    int next = (k + 1) % _Vertices.Length;
    _Sides[prev] = Side(_Vertices[prev], _Vertices[k]);
    _Sides[k] = Side(_Vertices[k], _Vertices[next]);
    
    LoadVerticesAndSides();
  }
  
  private void OnLeftHandleDrag(Vector2 position) {
    UpdateVertex(0, ToLocal(position));
  }
  
  private void OnRightHandleDrag(Vector2 position) {
    UpdateVertex(2, ToLocal(position));
  }
}
