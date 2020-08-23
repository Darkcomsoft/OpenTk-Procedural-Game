using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BulletSharp;
using EvllyEngine;
using OpenTK;
using OpenTK.Input;
using ProjectEvlly;

namespace ProjectEvlly.src
{
    public class PlayerController
    {
        private PlayerEntity _playerEntity;

        public float MoveSpeed = 3f;
        private float sensitivity = 0.2f;

        private bool isOnGround;
        private float MAX_SLOPE_ANGLE = 45f;

        public Vector3 finalTarget;
        public Vector3 target;
        public Vector3 mouseRotationBuffer;
        public bool MouseLook = true;

        private Vector2 _lastPos;
        private bool _firstMove;

        private bool Jump = false;

        public float Yaw { get; private set; }
        public float Pitch { get; private set; }

        #region Camera
        public Camera _PlayerCamera;
        #endregion

        #region RigidBody
        public float _Mass;
        public bool _Static;
        private BulletSharp.RigidBody rigidBodyObject;
        public CollisionShape _Shape;
        #endregion

        public PlayerController(PlayerEntity playerEntity, float cameraHight)
        {
            _playerEntity = playerEntity;

            _PlayerCamera = new Camera(_playerEntity.transform);
            _PlayerCamera._cameraTrnasform.Position = new Vector3(0, cameraHight, 0);//Set the position of the camera

            _Mass = 1;
            _Static = false;
            _Shape = new CapsuleShape(0.5f, 1);
            rigidBodyObject = LocalCreateRigidBody(_Mass, Matrix4.CreateTranslation(_playerEntity.transform.Position), _Shape);
            rigidBodyObject.CollisionFlags = CollisionFlags.CharacterObject;
            rigidBodyObject.Friction = 1.0f;
            rigidBodyObject.SetDamping(0, 0);
        }

        public void UpdateController()
        {
            var moveVector = new Vector3(0, 0, 0);

            rigidBodyObject.Activate();

            if (Physics.RayCast(_playerEntity.transform.Position, _playerEntity.transform.Position + new Vector3(0, -1.05f, 0), out ClosestRayResultCallback hit))
            {
                //Debug.Log("RayCast Hited: " + hit.CollisionObject.UserObject);
                isOnGround = true;
                if (Jump)
                {
                    Jump = false;
                }
            }
            else
            {
                isOnGround = false;
            }

            if (EvllyEngine.MouseCursor.MouseLocked)
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

                if (Pitch < MathHelper.DegreesToRadians(-75.0f))
                {
                    Pitch = mouseRotationBuffer.Y - (mouseRotationBuffer.Y - MathHelper.DegreesToRadians(-75.0f));
                }

                if (Pitch > MathHelper.DegreesToRadians(75.0f))
                {
                    Pitch = mouseRotationBuffer.Y - (mouseRotationBuffer.Y - MathHelper.DegreesToRadians(75.0f));
                }

                _PlayerCamera._cameraTrnasform.Rotation = new Quaternion(-MathHelper.Clamp(mouseRotationBuffer.Y, MathHelper.DegreesToRadians(-75.0f), MathHelper.DegreesToRadians(75.0f)), 0, 0, 0);
                _playerEntity.transform.Rotation = new Quaternion(0, WrapAngle(mouseRotationBuffer.X), 0, 0);

                ///////////////////////////////////////////////////////////////////////////////////////

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

                if (isOnGround)
                {
                    if (Input.GetKeyDown(Key.Space))
                    {
                        Jump = true;
                    }
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
            }

            if (isOnGround)
            {
                if (Jump)
                {
                    moveVector.Y += 10;
                    MovePlayer(moveVector, false);
                }
                else
                {
                    MovePlayer(moveVector, false);
                }
            }
            else
            {
                MovePlayer(moveVector, true);
            }

            _playerEntity.transform.Position = rigidBodyObject.WorldTransform.ExtractTranslation();

            MidleWorld.instance.PlayerPos = _playerEntity.transform.Position;

            _PlayerCamera.UpdateCamera();//Update the camera
        }

        public void DisposeController()
        {
            _PlayerCamera.OnDestroy();
            _PlayerCamera = null;

            Physics.RemoveRigidBody(rigidBodyObject);
            _Shape.Dispose();
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

        private float GetGroundAngle()
        {
            if (Physics.RayCast(_playerEntity.transform.Position, _playerEntity.transform.Position + new Vector3(0, -2f, 0), out ClosestRayResultCallback hit))
            {
                return Vector3.CalculateAngle(_playerEntity.transform.Position + new Vector3(0, 1f, 0), hit.HitNormalWorld);
            }
            return 0;
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
            var camRotation = Matrix3.CreateRotationX(_playerEntity.transform.Rotation.X) * Matrix3.CreateRotationY(_playerEntity.transform.Rotation.Y) * Matrix3.CreateRotationZ(_playerEntity.transform.Rotation.Z);
            var rotatedVector = Vector3.Transform(moveVector, camRotation);
            //gameObject.GetRigidBody().Move(rotatedVector * moveSpeed);

            Move(rotatedVector * MoveSpeed, gravity);
        }
    }
}
