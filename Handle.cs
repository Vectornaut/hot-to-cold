using Godot;
using System;

public partial class Handle : Node2D {
  [Signal] public delegate void DragEventHandler(Vector2 position);
  [Signal] public delegate void GrabEventHandler(bool grabbed);
  
  private float _radius;
  private float _radiusSq;
  private bool _grabbed = false;
  private Vector2 _dragOffset;
  
  public Handle(Vector2 position, float radius = 6f) {
    Position = position;
    _radius = radius;
    _radiusSq = radius * radius;
    
    // always process
    ProcessMode = ProcessModeEnum.Always;
  }
  
  private void SetGrabbed(bool grabbed) {
    _grabbed = grabbed;
    EmitSignal(SignalName.Grab, grabbed);
  }
  
  public override void _Draw() {
    DrawCircle(Vector2.Zero, _radius, _grabbed ? Colors.Red : Colors.Orange);
  }
  
  // drag handling, following the Godot mouse motion tutorial
  //
  //   https://docs.godotengine.org/en/stable/tutorials/inputs/input_examples.html#mouse-motion
  //
  public override void _Input(InputEvent ev) {
    if (ev is InputEventMouseButton evButton && evButton.ButtonIndex == MouseButton.Left) {
      Vector2 locEvPos = ToLocal(evButton.Position);
      if (!_grabbed && evButton.Pressed && locEvPos.LengthSquared() < _radiusSq) {
        SetGrabbed(true);
        _dragOffset = locEvPos;
        QueueRedraw();
      } else if (_grabbed && !evButton.Pressed) {
        SetGrabbed(false);
        QueueRedraw();
      }
    } else if (ev is InputEventMouseMotion evMotion && _grabbed) {
      GlobalPosition = evMotion.Position - _dragOffset;
      EmitSignal(SignalName.Drag, GlobalPosition);
    }
  }
}
