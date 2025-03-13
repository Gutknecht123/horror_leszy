using Godot;
using System;

public partial class DialogueBoxControl : Control
{

    private Label label;
    private Panel panel;
    private Control control;


    public override void _Ready()
    {
        control = GetNode<Control>("CanvasLayer/Control");
        panel = GetNode<Panel>("CanvasLayer/Control/Panel");
        label = GetNode<Label>("CanvasLayer/Control/Panel/Label");
        control.Hide();
    }

    public void ShowDialogue(string text){
        
        label.Text = text;
        control.Show();
        
        GetTree().CreateTimer(3.0f).Timeout += () => control.Hide();
    }
}
