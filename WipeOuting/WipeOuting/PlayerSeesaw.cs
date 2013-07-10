using Box2DX.Dynamics;
using Box2DX.Collision;
using Box2DX.Common;

namespace WipeOuting
{
    class PlayerSeesaw
    {
        private World world;
        private Body seesaw;

        public PlayerSeesaw()
        {
            AABB worldAABB = new AABB();
            worldAABB.LowerBound.Set(-100, -100);
            worldAABB.UpperBound.Set(100, 100);
            this.world = new World(worldAABB, new Vec2(0.0f, -10.0f), false);
            
            Body dummy;
            {
                BodyDef bd = new BodyDef();
                bd.Position.Set(0f, 0.2f);
                PolygonDef pd = new PolygonDef();
                pd.SetAsBox(1.1f, 0.1f);
                pd.Density = 1.0f;
                pd.Friction = 0.1f;
                dummy = this.world.CreateBody(bd);
                this.seesaw = this.world.CreateBody(bd);
                this.seesaw.CreateShape(pd);
                this.seesaw.SetAngularVelocity(0);
                this.seesaw.SetLinearVelocity(new Vec2(0, 0));
                this.seesaw.SetMassFromShapes();
            }

            RevoluteJointDef jointDef = new RevoluteJointDef();
            jointDef.Initialize( this.seesaw, dummy, this.seesaw.GetPosition() );
            jointDef.EnableMotor = false;
            jointDef.MotorSpeed = 0.0f;
            jointDef.MaxMotorTorque = 100;
            jointDef.EnableLimit = true;
            jointDef.UpperAngle = (float)(System.Math.PI/14);
            jointDef.LowerAngle = -(float)(System.Math.PI/14);
            this.world.CreateJoint(jointDef);

        }

        public void Step(float x_meters, float dt, int velocityIterations, int positionIterations)
        {
            Vec2 p = seesaw.GetPosition();
            p.X += x_meters;
            this.seesaw.ApplyForce(new Vec2(0, 0.1f), p);
            this.world.Step(dt, velocityIterations, positionIterations);
        }

        public float Angle()
        {
            return this.seesaw.GetAngle();
        }

    }
}
