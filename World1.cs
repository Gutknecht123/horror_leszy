using Godot;
using System;

public partial class World1 : Node3D
{
    public override void _Ready()
    {
        Engine.MaxFps = 165;
    }
}
