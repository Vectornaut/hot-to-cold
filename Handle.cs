using Godot;
using System;

public partial class Handle : Node2D {
  [Signal] public delegate void DragEventHandler(Vector2 position);
  [Signal] public delegate void GrabEventHandler(bool grabbed);
  
  private float _Radius;
  private float _RadiusSq;
  private bool _Grabbed = false;
  private Vector2 _DragOffset;
  
  public Handle(Vector2 position, float radius = 6f) {
    Position = position;
    _Radius = radius;
    _RadiusSq = radius * radius;
    
    // always process
    ProcessMode = ProcessModeEnum.Always;
  }
  
  private void SetGrabbed(bool grabbed) {
    _Grabbed = grabbed;
    EmitSignal(SignalName.Grab, grabbed);
  }
  
  public override void _Draw() {
    DrawCircle(Vector2.Zero, _Radius, _Grabbed ? Colors.Red : Colors.Orange);
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
        QueueRedraw();
      } else if (_Grabbed && !evButton.Pressed) {
        SetGrabbed(false);
        QueueRedraw();
      }
    } else if (ev is InputEventMouseMotion evMotion && _Grabbed) {
      GlobalPosition = evMotion.Position - _DragOffset;
      EmitSignal(SignalName.Drag, GlobalPosition);
    }
  }
}
