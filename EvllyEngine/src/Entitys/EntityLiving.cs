using ProjectEvlly.src.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEvlly
{
    public class EntityLiving : Entity
    {
        private int HP;
        private int MaxHP;

        private bool _isDead;

        public virtual void OnDead() { }
    }
}
