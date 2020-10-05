using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BEPUphysics.Character;
using BEPUphysics.Entities.Prefabs;
using EvllyEngine;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Input;
using ProjectEvlly;
using ProjectEvlly.src.Engine.Render;
using ProjectEvlly.src.Utility;

namespace ProjectEvlly.src
{
    public class PlayerController : IDisposable
    {
        private PlayerEntity _playerEntity;

        private float MoveSpeed = 0.3f;
        private float sensitivity = 0.2f;

        const float characterHeight = 1;
        const float characterWidth = 0.5f;
        const float jumpSpeed = 10f;

        const float stepHeight = 5f;

        private Vector3 mouseRotationBuffer;

        private Vector2 _lastPos;
        private bool _firstMove;

        public float Yaw { get; private set; }
        public float Pitch { get; private set; }

        CharacterController _CharacterController;

        #region Camera
        public Camera _PlayerCamera;
        #endregion

        public PlayerController(PlayerEntity playerEntity, float cameraHight)
        {
            _playerEntity = playerEntity;

            _PlayerCamera = new Camera(_playerEntity.transform);
            _PlayerCamera._cameraTrnasform.Position = new Vector3(0, cameraHight, 0);//Set the position of the camera

            _CharacterController = new CharacterController();

            _CharacterController.Body.Position = new BEPUutilities.Vector3(playerEntity.transform.Position.X, playerEntity.transform.Position.Y, playerEntity.transform.Position.Z);
            //_CharacterController.Body.BecomeKinematic();
            Physics.Add(_CharacterController);
        }

        public void UpdateController()
        {
            var moveVector = new Vector2(0, 0);

            /*if (Physics.RayCast(_playerEntity.transform.Position, _playerEntity.transform.Position + new Vector3(0, -1.05f, 0), out ClosestRayResultCallback hit))
            {
                
            }*/

            /*if (_PlayerCamera._cameraTrnasform.Position.Y + _playerEntity.transform.Position.Y < -1)
            {
                Fog.Density = 0.4f;
                Fog.Distance = 0.5f;
                Fog.FogColor = new Vector4(0, 0, 0.5450981f, 1);
                gl.ClearColor(Color4.DarkBlue);
            }
            else
            {
                Fog.Density = 0.014f;
                Fog.Distance = 3.5f;
                Fog.FogColor = new Vector4(0, 0.7490196f, 1, 1);
                gl.ClearColor(new Color4(0, 0.7490196f, 1, 1));
            }*/

            #region CalculateMouseInput
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
                Yaw -= deltaX * sensitivity * Time._DeltaTime;
                Pitch -= deltaY * sensitivity * Time._DeltaTime; // reversed since y-coordinates range from bottom to top
            }
            #endregion

            if (EvllyEngine.MouseCursor.MouseLocked)
            {
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

                #region PlayerMove

                if (Input.GetKey(Key.W))
                {
                    moveVector += new Vector2(0, 1);
                }
                if (Input.GetKey(Key.S))
                {
                    moveVector += new Vector2(0, -1);
                }
                if (Input.GetKey(Key.A))
                {
                    moveVector += new Vector2(-1, 0);
                }
                if (Input.GetKey(Key.D))
                {
                    moveVector += new Vector2(1, 0);
                }

                if (Input.GetKey(Key.ShiftLeft))
                {
                    MoveSpeed = 5f;
                }
                else
                {
                    MoveSpeed = 3f;
                }

                _CharacterController.StandingSpeed = MoveSpeed * 2;

                if (Input.GetKey(Key.Z))
                    _CharacterController.StanceManager.DesiredStance = Stance.Prone;
                else if (Input.GetKey(Key.ControlLeft))
                    _CharacterController.StanceManager.DesiredStance = Stance.Crouching;
                else
                    _CharacterController.StanceManager.DesiredStance = Stance.Standing;

                if (Input.GetKeyDown(Key.Space))
                {
                    _CharacterController.Jump();
                }

                MovePlayer(moveVector);
                #endregion

            }
            
            //_playerEntity.transform.Position = kinematicCharacter.GhostObject.WorldTransform.ExtractTranslation();
            _playerEntity.transform.Position = new Vector3(_CharacterController.Body.Position.X, _CharacterController.Body.Position.Y, _CharacterController.Body.Position.Z);

            Game.GetWorld.PlayerPos = _playerEntity.transform.Position;

            _PlayerCamera.UpdateCamera();//Update the camera
        }

        public void Dispose()
        {
            _PlayerCamera.Dispose();
            _PlayerCamera = null;

            Physics.Remove(_CharacterController);
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

        private void MovePlayer(Vector2 moveVector)
        {
            /*var camRotation = Matrix3.CreateRotationX(_playerEntity.transform.Rotation.X) * Matrix3.CreateRotationY(_playerEntity.transform.Rotation.Y) * Matrix3.CreateRotationZ(_playerEntity.transform.Rotation.Z);
            var rotatedVector = Vector3.Transform(Vector3.Zero, camRotation);
            
            kinematicCharacter.SetWalkDirection(rotatedVector * MoveSpeed);*/
            
            _CharacterController.ViewDirection = Forward(_playerEntity.transform.RotationMatrix);
            _CharacterController.HorizontalMotionConstraint.MovementDirection = BEPUutilities.Vector2.Normalize(new BEPUutilities.Vector2(moveVector.X, moveVector.Y));
        }

        public BEPUutilities.Vector3 Forward(Matrix4 matrix)
        {
            BEPUutilities.Vector3 vector = new BEPUutilities.Vector3();

            vector.X = matrix.M31;
            vector.Y = matrix.M32;
            vector.Z = matrix.M33;
            return vector;
        }
    }
}