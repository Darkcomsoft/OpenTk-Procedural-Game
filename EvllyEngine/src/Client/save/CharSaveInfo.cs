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
        public string WorldName;

        public CharSaveInfo(string charName, string worldName)
        {
            CharName = charName;
            WorldName = worldName;
        }
    }
}
