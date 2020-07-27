using OpenTK;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvllyEngine
{
    public class ScriptTest : ScriptBase
    {
        public override void Update()
        {
            Vector3 moveVector = Vector3.Zero;
            if (Input.GetKey(Key.Up))
            {
                moveVector = new Vector3(0.1f, 0,0);
            }
            else if(Input.GetKey(Key.Down))
            {
                moveVector = new Vector3(-0.1f, 0, 0);
            }
            else if (Input.GetKey(Key.Left))
            {
                moveVector = new Vector3(0, 0, -0.1f);
            }
            else if (Input.GetKey(Key.Right))
            {
                moveVector = new Vector3(0, 0, 0.1f);
            }
            //gameObject.GetRigidBody().Move(moveVector * 1000);
            //gameObject._transform._Position += moveVector;
            //gameObject._transform._Rotation = new Quaternion(MathHelper.DegreesToRadians(Time._Tick * 200), MathHelper.DegreesToRadians(Time._Tick * 200), MathHelper.DegreesToRadians(Time._Tick * 200), 0);
            base.Update();
        }
    }
}
