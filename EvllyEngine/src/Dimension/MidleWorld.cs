using EvllyEngine;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEvlly
{
    public class MidleWorld : Dimension
    {
        private World _World;

        public MidleWorld(string DimensionID)
        {
            _DimensionID = DimensionID;
            //Temporario
            AddEntity(new PlayerEntity());

            _World = new World();
        }

        public override void OnUpdate(object obj, FrameEventArgs e)
        {
            _World.UpdateWorld();
        }

        public override void Draw(FrameEventArgs e)
        {
            _World.Draw(e);
        }

        public override void OnUnloadDimension()
        {
            _World.OnDestroy();
            _World = null;
            base.OnUnloadDimension();
        }
    }
}
