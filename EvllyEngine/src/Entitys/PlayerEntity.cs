using BulletSharp;
using OpenTK;
using OpenTK.Input;
using ProjectEvlly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvllyEngine
{
    public class PlayerEntity : EntityLiving
    {
        public float MoveSpeed = 0.6f;
        private bool isOnGround;

        #region Camera
        public Camera _PlayerCamera;
        #endregion

        #region RigidBody
        public float _Mass;
        public bool _Static;
        private BulletSharp.RigidBody rigidBodyObject;
        public CollisionShape _Shape;
        #endregion

        public Vector3 finalTarget;
        public Vector3 target;
        public Vector3 mouseRotationBuffer;
        public bool MouseLook = true;

        private Vector2 _lastPos;
        private bool _firstMove;
        private float sensitivity = 0.2f;

        public float Yaw { get; private set; }
        public float Pitch { get; private set; }

        public override void OnStart()
        {
            _PlayerCamera = new Camera(transform);

            transform.Position = new Vector3(0,100,0);

            _Mass = 1;
            _Static = false;
            _Shape = new CapsuleShape(0.5f, 1);
            rigidBodyObject = LocalCreateRigidBody(_Mass, Matrix4.CreateTranslation(transform.Position), _Shape);
            rigidBodyObject.CollisionFlags = CollisionFlags.CharacterObject;
            rigidBodyObject.Friction = 0.1f;
            rigidBodyObject.SetDamping(0, 0);

            base.OnStart();
        }

        public override void OnUpdate(object obj, FrameEventArgs e)
        {
            var moveVector = new Vector3(0, 0, 0);

            rigidBodyObject.Activate();
            _PlayerCamera.UpdateCamera();

            /*Vector3 cameraTarget = Vector3.Zero;
            Vector3 cameraDirection = Vector3.Normalize(gameObject._transform._Position - cameraTarget);
            Vector3 up = Vector3.UnitY;
            Vector3 cameraRight = Vector3.Normalize(Vector3.Cross(up, cameraDirection));
            Vector3 cameraUp = Vector3.Cross(cameraDirection, cameraRight);*/

            if (Physics.RayCast(transform.Position, transform.Position + new Vector3(0, -1.5f, 0), out ClosestRayResultCallback hit))
            {
                //Debug.Log("RayCast Hited: " + hit.CollisionObject.UserObject);
                isOnGround = true;
            }
            else
            {
                isOnGround = false;
            }

            #region CameraLook
            if (Input.GetKeyDown(Key.P))
            {
                if (MouseCursor.MouseLocked)
                {
                    MouseCursor.UnLockCursor();
                }
                else
                {
                    MouseCursor.LockCursor();
                }
            }

            if (MouseCursor.MouseLocked)
            {
                var mouse = Mouse.GetState();

                if (_firstMove) // this bool variable is initially set to true
                {
                    _lastPos = new Vector2(mouse.X, mouse.Y);
                    _firstMove = false;
                }
                else
                {
                    // Calculate the offset of the mouse position
                    var deltaX = mouse.X - _lastPos.X;
                    var deltaY = mouse.Y - _lastPos.Y;
                    _lastPos = new Vector2(mouse.X, mouse.Y);

                    // Apply the camera pitch and yaw (we clamp the pitch in the camera class)
                    Yaw -= deltaX * sensitivity * Time._Time;
                    Pitch -= deltaY * sensitivity * Time._Time; // reversed since y-coordinates range from bottom to top
                }

                mouseRotationBuffer.X = Yaw;
                mouseRotationBuffer.Y = Pitch;

                _PlayerCamera._cameraTrnasform.Rotation = new Quaternion(-MathHelper.Clamp(mouseRotationBuffer.Y, MathHelper.DegreesToRadians(-75.0f), MathHelper.DegreesToRadians(75.0f)), 0, 0, 0);
                transform.Rotation = new Quaternion(0, WrapAngle(mouseRotationBuffer.X), 0, 0);
            }
            #endregion

            if (Input.GetKey(Key.W))
            {
                moveVector.Z += MoveSpeed;
            }
            if (Input.GetKey(Key.S))
            {
                moveVector.Z -= MoveSpeed;
            }
            if (Input.GetKey(Key.A))
            {
                moveVector.X += MoveSpeed;
            }
            if (Input.GetKey(Key.D))
            {
                moveVector.X -= MoveSpeed;
            }

            if (Input.GetKey(Key.Q))
            {
                moveVector.Y -= MoveSpeed;
            }
            if (Input.GetKey(Key.E))
            {
                moveVector.Y += MoveSpeed;
            }

            if (Input.GetKey(Key.ShiftLeft))
            {
                MoveSpeed = 5;
            }
            else
            {
                MoveSpeed = 3;
            }

            if (isOnGround)
            {
                MovePlayer(moveVector, false);
            }
            else
            {
                MovePlayer(moveVector, true);
            }
            transform.Position = rigidBodyObject.WorldTransform.ExtractTranslation();

            World.instance.PlayerPos = transform.Position;
        }

        public BulletSharp.RigidBody LocalCreateRigidBody(float mass, Matrix4 startTransform, CollisionShape shape)
        {
            bool isDynamic = (mass != 0.0f);

            Vector3 localInertia = Vector3.Zero;
            if (isDynamic || _Static == false)
            {
                shape.CalculateLocalInertia(mass, out localInertia);
            }

            DefaultMotionState myMotionState = new DefaultMotionState(startTransform);

            RigidBodyConstructionInfo rbInfo = new RigidBodyConstructionInfo(mass, myMotionState, shape, localInertia);
            BulletSharp.RigidBody body = new BulletSharp.RigidBody(rbInfo);

            Physics.AddRigidBody(body);

            return body;
        }

        /// <summary>
        /// this is for moving the rigidbody with Physics, the rigidbody whill interact with collision
        /// </summary>
        /// <param name="direction"></param>
        public void Move(Vector3 direction, bool gravity)
        {
            if (gravity)
            {
                rigidBodyObject.LinearVelocity = new Vector3(direction.X, rigidBodyObject.Gravity.Y, direction.Z);
            }
            else
            {
                rigidBodyObject.LinearVelocity = direction;
            }
            rigidBodyObject.AngularFactor = Vector3.Zero;
            rigidBodyObject.AngularVelocity = Vector3.Zero;

            //rigidBodyObject.WorldTransform = gameObject._transform.RotationMatrix * rigidBodyObject.WorldTransform * Matrix4.CreateScale(gameObject._transform._Size);
        }

        private void MovePlayer(Vector3 moveVector, bool gravity)
        {
            var camRotation = Matrix3.CreateRotationX(transform.Rotation.X) * Matrix3.CreateRotationY(transform.Rotation.Y) * Matrix3.CreateRotationZ(transform.Rotation.Z);
            var rotatedVector = Vector3.Transform(moveVector, camRotation);
            //gameObject.GetRigidBody().Move(rotatedVector * moveSpeed);

            Move(rotatedVector * MoveSpeed, gravity);
        }

        public static float WrapAngle(float angle)
        {
            if ((angle > -MathHelper.Pi) && (angle <= MathHelper.Pi))
                return angle;
            angle %= MathHelper.TwoPi;
            if (angle <= -MathHelper.Pi)
                return angle + MathHelper.TwoPi;
            if (angle > MathHelper.Pi)
                return angle - MathHelper.TwoPi;
            return angle;
        }

        public override void OnDestroy()
        {
            _PlayerCamera.OnDestroy();
            _PlayerCamera = null;

            Physics.RemoveRigidBody(rigidBodyObject);
            _Shape.Dispose();
            _Shape = null;

            base.OnDestroy();
        }
    }
}
