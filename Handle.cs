using Godot;
using System;

public partial class Handle : Node2D {
  [Signal] public delegate void DragEventHandler(Vector2 position);
  [Signal] public delegate void GrabEventHandler(bool grabbed);
  
  // geometry
  private Control _Domain;
  private float _Radius;
  private float _RadiusSq;
  
  // interaction state
  private bool _Hovering = false;
  private bool _Grabbed = false;
  private Vector2 _DragOffset;
  
  public Handle(Vector2 position, Control domain, float radius = 6f) {
    Position = position;
    _Domain = domain;
    _Radius = radius;
    _RadiusSq = radius * radius;
  }
  
  private void SetHovering(bool hovering) {
    _Hovering = hovering;
    QueueRedraw();
  }
  
  private void SetGrabbed(bool grabbed) {
    _Grabbed = grabbed;
    EmitSignal(SignalName.Grab, grabbed);
    QueueRedraw();
  }
  
  public override void _Draw() {
    // set colors, mostly around Lab [59, 85], -27, -18
    Color fill;
    Color ink;
    if (_Grabbed) {
      fill = new Color(196f/255, 251f/255, 253f/255);
      ink = new Color(133f/255, 239f/255, 254f/255);
    } else if (_Hovering) {
      fill = new Color(124f/255, 234f/255, 251f/255);
      ink = new Color(23f/255, 156f/255, 173f/255);
    } else {
      fill = new Color(27f/255, 134f/255, 139f/255);
      ink = new Color(15f/255, 69f/255, 69f/255);
    }
    
    // draw handle
    DrawCircle(Vector2.Zero, _Radius, fill);
    DrawArc(Vector2.Zero, _Radius, 0, Mathf.Tau, 12, ink, 1f, true);
  }
  
  // drag handling, following the Godot mouse motion tutorial
  //
  //   https://docs.godotengine.org/en/stable/tutorials/inputs/input_examples.html#mouse-motion
  //
  public override void _Input(InputEvent ev) {
    if (ev is InputEventMouseButton evButton && evButton.ButtonIndex == MouseButton.Left) {
      Vector2 locEvPos = ToLocal(evButton.Position);
      if (!_Grabbed && evButton.Pressed && locEvPos.LengthSquared() < _RadiusSq) {
        _DragOffset = locEvPos;
        SetGrabbed(true);
      } else if (_Grabbed && !evButton.Pressed) {
        SetGrabbed(false);
      }
    } else if (ev is InputEventMouseMotion evMotion) {
      // respond to hovering
      Vector2 locEvPos = ToLocal(evMotion.Position);
      SetHovering(locEvPos.LengthSquared() < _RadiusSq);
      
      // respond to drag
      if (_Grabbed) {
        Vector2 inset = 0.5f * new Vector2(_Radius, _Radius);
        Vector2 minPos = _Domain.Position + inset;
        Vector2 maxPos = _Domain.Position + _Domain.Size - inset;
        GlobalPosition = (evMotion.Position - _DragOffset).Clamp(minPos, maxPos);
        EmitSignal(SignalName.Drag, GlobalPosition);
      }
    }
  }
}
