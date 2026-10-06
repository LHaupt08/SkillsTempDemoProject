using Godot;
using System;
using System.Linq;

public partial class GrapplingHook : Node3D
{
    [Export] public Node3D rope;
    [Export] public MeshInstance3D ropeMesh;

    private bool _isGrappled;
    private PhysicsBody3D _grappleTarget;
    private Vector3 _grapplePointLocalPosition;

    [Export] public CharacterBody3D player;
    [Export] public float acceleration = 5f;

    public override void _Ready()
    {
        base._Ready();
        rope.Visible = false;

        ProcessInput();
        ProcessGrapple();
    }

    private void ProcessInput()
    {
        if (Input.IsActionJustPressed("Grapple"))
        {
            if (_isGrappled)
            {
                _isGrappled = false;
                rope.Visible = false;
                _grapplePointLocalPosition = Vector3.Zero;
                _grappleTarget = null;
                this.GlobalRotationDegrees = Vector3.Zero;
            }
            else
            {
                var from = this.GlobalPosition;
                var forward = -this.GlobalBasis.Z;
                var to = this.GlobalPosition + forward * 500;
                var raycastResult = GetWorld3D().DirectSpaceState.IntersectRay(PhysicsRayQueryParameters3D.Create(from, to));
                if (raycastResult.Keys.Any())
                {
                    var nodeHit = (Node3D)raycastResult["collider"];
                    if (nodeHit is PhysicsBody3D hitphysicsBody)
                    {
                        _grappleTarget = hitphysicsBody;
                        _grapplePointLocalPosition = hitphysicsBody.ToLocal((Vector3)raycastResult["position"]);
                        _isGrappled = true;
                        rope.Visible = true;
                    }
                }
            }
        }
    }

    private void ProcessGrapple()
    {
        if (_isGrappled)
        {
            var grapplePointGlobalPosition = _grappleTarget.ToGlobal(_grapplePointLocalPosition);
            this.LookAt(grapplePointGlobalPosition);

            var displacement = GlobalPosition - grapplePointGlobalPosition;
            rope.Scale = new Vector3(1f, 1f, displacement.Length());

            player.Velocity = -displacement.Normalized() * acceleration;
        }
    }


}
