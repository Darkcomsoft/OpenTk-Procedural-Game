using EvllyEngine;
using Lidgren.Network;
using OpenTK;
using ProjectEvlly.src.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEvlly.src.Entitys
{
    /// <summary>
    /// Remote a client version of the player
    /// </summary>
    public class PlayerEntityRemote : Entity
    {
        public PlayerEntityRemote(NetViewSerializer entity) : base(entity)
        {
            
        }

        public override void OnUpdate()
        {
            Debug.Log("A Entity Tick!");
            base.OnUpdate();
        }
    }
}
