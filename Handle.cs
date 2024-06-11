using Godot;
using System;

public partial class Handle : Node2D {
  [Signal] public delegate void DragEventHandler(Vector2 position);
  
  private float _radius;
  private float _radiusSq;
  private bool _dragging = false;
  private Vector2 _dragOffset;
  
  public Handle(Vector2 position, float radius = 6f) {
    Position = position;
    _radius = radius;
    _radiusSq = radius * radius;
  }
  
  public override void _Draw() {
    DrawCircle(Vector2.Zero, _radius, _dragging ? Colors.Red : Colors.Orange);
  }
  
  // drag handling, following the Godot mouse motion tutorial
  //
  //   https://docs.godotengine.org/en/stable/tutorials/inputs/input_examples.html#mouse-motion
  //
  public override void _Input(InputEvent ev) {
    if (ev is InputEventMouseButton evButton && evButton.ButtonIndex == MouseButton.Left) {
      Vector2 locEvPos = ToLocal(evButton.Position);
      if (!_dragging && evButton.Pressed && locEvPos.LengthSquared() < _radiusSq) {
        _dragging = true;
        _dragOffset = locEvPos;
        QueueRedraw();
      } else if (_dragging && !evButton.Pressed) {
        _dragging = false;
        QueueRedraw();
      }
      _dragging &= evButton.Pressed;
    } else if (ev is InputEventMouseMotion evMotion && _dragging) {
      GlobalPosition = evMotion.Position - _dragOffset;
      EmitSignal(SignalName.Drag, GlobalPosition);
    }
  }
}
