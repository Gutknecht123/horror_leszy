using Godot;
using System;

public partial class PlayerTest : CharacterBody3D
{
    
    const float speed = 5.0f;
    const float sprint_speed = 7.0f;
    const float crouching_speed = 2.5f; // Speed when crouching
    const float gravity = 30.0f;
    const float jumpForce = 10.0f;
    float acceleration = 0.5f;
    const float sensitivity = 0.005f;
    private float stand_Height = 0.0f;  // Standing height
    private float crouch_Height = -0.4f; // Crouching height
    private float crouch_speed = 10.0f; // Crouching speed
    private float defaultCameraY; // Starting position of camera
    private float targetHeight;
    private bool isCrouching = false;
    private bool isSprinting = false;

    public Node3D head;
    public Camera3D cam;
    public CollisionShape3D collisionShape;
    public MeshInstance3D mesh;
    public Node3D character_model;
    public AnimationPlayer animations;

    private Vector3 velocity = Vector3.Zero;

    public override void _Ready()
    {

        head = GetNode<Node3D>("Head");
        cam = GetNode<Camera3D>("Head/Camera3D");
        collisionShape = GetNode<CollisionShape3D>("CollisionShape3D");
        mesh = GetNode<MeshInstance3D>("MeshInstance3D");
        defaultCameraY = cam.Position.Y;
        targetHeight = defaultCameraY;
        Input.MouseMode = Input.MouseModeEnum.Captured;
        character_model = GetNode<Node3D>("Armature");
        animations = GetNode<AnimationPlayer>("AnimationPlayer");

    }

    public override void _Input(InputEvent @event)
    {
        if(@event is InputEventMouseMotion){
            InputEventMouseMotion mouseMotion = @event as InputEventMouseMotion;
            head.RotateY(-mouseMotion.Relative.X * sensitivity);
            cam.RotateX(-mouseMotion.Relative.Y * sensitivity);

            Vector3 cameraRotation = cam.Rotation;
            cameraRotation.X = Mathf.Clamp(cameraRotation.X, Mathf.DegToRad(-80f), Mathf.DegToRad(80f));
            cam.Rotation = cameraRotation;
        }
    }

    public override void _Process(double delta)
    {

        if(IsOnFloor() && Input.IsActionPressed("crouch") && !isSprinting){
            
            isCrouching = true;
            isSprinting = false;
            targetHeight = isCrouching ? defaultCameraY - (stand_Height - crouch_Height) : defaultCameraY;
            Vector3 camPos = cam.Position;
            camPos.Y = Mathf.Lerp(camPos.Y, targetHeight, (float)delta * crouch_speed); 
            cam.Position = camPos;
            collisionShape.Scale = new Vector3(1.0f, 0.6f, 1.0f);
            mesh.Position = new Vector3(0, -0.4f, 0);

        }else if(IsOnFloor() && !Input.IsActionPressed("crouch")){

            isCrouching = false;
            Vector3 camPos = cam.Position;
            camPos.Y = Mathf.Lerp(camPos.Y, 0, (float)delta * crouch_speed);  
            cam.Position = camPos;
            collisionShape.Scale = new Vector3(1.0f, 1.0f, 1.0f);
            mesh.Position = new Vector3(0, 0, 0);
        }
        Vector3 camDirection = -cam.GlobalTransform.Basis.Z;
        character_model.LookAt(character_model.GlobalTransform.Origin + new Vector3(-camDirection.X, 0, -camDirection.Z));

    }

    public override void _PhysicsProcess(double delta)
    {

        Vector2 inputDir = Input.GetVector("left", "right", "up", "down");
        Vector3 direction = (head.GlobalTransform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
        float newSpeed = speed;
        float newAcc = acceleration;

        if(direction != Vector3.Zero){

            if(Input.IsActionPressed("sprint") && !isCrouching){
                
                isSprinting = true;
                newAcc = 0.7f;
                newSpeed = sprint_speed;
                animations.Play("Run");
            }else{
                isSprinting = false;
            }
            if (Input.IsActionPressed("crouch") && !isSprinting){
                
                isCrouching = true;
                newAcc = 0.25f;
                newSpeed = crouching_speed;

            }else{
                isCrouching = false;
            }
            if(IsOnFloor() && !isSprinting && !isCrouching && (Input.IsActionPressed("up") || Input.IsActionPressed("down") || Input.IsActionPressed("left") || Input.IsActionPressed("right"))){
                animations.Play("Walk");
            }  
            acceleration = newAcc;
            velocity.X = direction.X * newSpeed;
            velocity.Z = direction.Z * newSpeed;

            

        }else{
            velocity.X = Mathf.MoveToward(Velocity.X, 0, newAcc);
            velocity.Z = Mathf.MoveToward(Velocity.Z, 0, newAcc);
            animations.Play("Iddle");
        }

        if(!IsOnFloor()){
            velocity.Y -= gravity * (float)delta;
        }

        if(IsOnFloor() && Input.IsActionJustPressed("jump")){
            velocity.Y = jumpForce;
            
        }


        
        Velocity = velocity;
        MoveAndSlide();
    }
}
