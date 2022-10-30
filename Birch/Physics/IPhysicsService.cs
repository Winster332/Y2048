namespace Birch.Physics;

public interface IPhysicsService
{
    public void Step(float dt, int velocityIterations, int positionIterations);
    public void CreateRigidBody(RigidBody body);
    public void DestroyBody(RigidBody body);
}