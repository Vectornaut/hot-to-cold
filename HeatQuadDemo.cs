using Godot;
using System;

public partial class HeatQuadDemo : Node2D {
  private TextureRect _Display;
  private SubViewport _View;
  private HeatQuad _Quad;
  private Vector2 _Oversampling;
  
  public override void _Ready() {
    // get display and viewport
    _Display = GetNode("Display") as TextureRect;
    _View = GetNode("QuadViewport") as SubViewport;
    _Oversampling = _View.Size / _Display.Size;
    
    // get quadrilateral
    this._Quad = _View.GetNode("HeatQuad") as HeatQuad;
    
    // create vertex handles
    Vector2 leftVertex = ViewToDisplay(_Quad.ToGlobal(_Quad.GetVertex(0)));
    Vector2 rightVertex = ViewToDisplay(_Quad.ToGlobal(_Quad.GetVertex(2)));
    Handle leftHandle = new Handle(leftVertex, _Display);
    Handle rightHandle = new Handle(rightVertex, _Display);
    leftHandle.Drag += OnLeftHandleDrag;
    rightHandle.Drag += OnRightHandleDrag;
    leftHandle.Grab += OnGrab;
    rightHandle.Grab += OnGrab;
    AddChild(leftHandle);
    AddChild(rightHandle);
  }
  
  private Vector2 DisplayToView(Vector2 displayPoint) {
    Vector2 viewPoint = displayPoint;
    if (_Display.FlipV) {
      viewPoint.Y = _Display.Size.Y -  viewPoint.Y;
    }
    viewPoint *= _Oversampling;
    return viewPoint;
  }
  
  private Vector2 ViewToDisplay(Vector2 viewPoint) {
    Vector2 displayPoint = viewPoint / _Oversampling;
    if (_Display.FlipV) {
      displayPoint.Y = _Display.Size.Y -  displayPoint.Y;
    }
    return displayPoint;
  }
  
  private void OnLeftHandleDrag(Vector2 displayPosition) {
    _Quad.OnLeftHandleDrag(DisplayToView(displayPosition));
    _View.RenderTargetUpdateMode = SubViewport.UpdateMode.Once;
  }
  
  private void OnRightHandleDrag(Vector2 displayPosition) {
    _Quad.OnRightHandleDrag(DisplayToView(displayPosition));
    _View.RenderTargetUpdateMode = SubViewport.UpdateMode.Once;
  }
  
  private void OnGrab(bool grabbed) {
    _Quad.OnGrab(grabbed);
    _View.RenderTargetUpdateMode = SubViewport.UpdateMode.Once;
  }
}
