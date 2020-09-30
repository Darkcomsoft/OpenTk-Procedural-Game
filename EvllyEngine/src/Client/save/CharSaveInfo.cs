using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEvlly.src.save
{
    [System.Serializable]
    public struct CharSaveInfo
    {
        public string CharName;
        public WorldType WorldName;

        public CharSaveInfo(string charName, WorldType worldName)
        {
            CharName = charName;
            WorldName = worldName;
        }
    }
}
