using Godot;
using System;

public partial class Ground : MeshInstance3D
{   
    public override void _Ready()
    {
        if(GetSurfaceOverrideMaterial(0) is StandardMaterial3D material){
            material.AlbedoColor = new Color(1,0,0);
        }else{
            StandardMaterial3D newMaterial = new();
            newMaterial.AlbedoColor = new Color(1,0,0);
            SetSurfaceOverrideMaterial(0, newMaterial);
        }
    }
}
