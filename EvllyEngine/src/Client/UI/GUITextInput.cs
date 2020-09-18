using ProjectEvlly.src.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEvlly.src.UI
{
    public class GUITextInput : GUIBase
    {
        public string Value = "";
        public string PlacerHolder = "A Input Text(TextBox)";

        public bool ShowValue = false;


        public GUITextInput(Rectangle rec) : base(rec)
        {

        }

        public GUITextInput(Rectangle rec, UIDock uIDock) : base(rec, uIDock)
        {

        }
    }
}
