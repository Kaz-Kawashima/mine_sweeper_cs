using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mine_sweeper_cs
{
    public enum OpenResult
    {
        Safe,
        Explosion
    }

    public abstract class Panel
    {
        internal bool isOpen = false;
        internal bool isFlagged = false;

        internal void Flag()
        {
            if (isFlagged)
            {
                isFlagged = false;
            } else
            {
                isFlagged |= true;
            }
        }
        public abstract OpenResult Open();
        public override abstract string ToString();
    }

    public class BombPanel : Panel
    {
        public override OpenResult Open()
        {
            isOpen = true;
            return OpenResult.Explosion;
        }

        public override string ToString()
        {
            if (isFlagged)
            {
                return "F";
            }
            else if (isOpen)
            {
                return "B";
            } else
            {
                return "#";
            }
        }
    }

    class BlankPanel : Panel
    {
        public int bombValue;

        public BlankPanel()
        {
            isOpen = false;
            isFlagged = false;
            bombValue = 0;
        }
        public override OpenResult Open()
        {
            isOpen = true;
            return OpenResult.Safe;
        }
        public override string ToString()
        {
            if (isFlagged)
            {
                return "F";
            }
            if (isOpen)
            {
                if (bombValue == 0)
                {
                    return " ";
                } else
                {
                    return bombValue.ToString();
                }
            } else
            {
                return "#";
            }
        }
    }
    class BorderPanel : Panel
    {
        public BorderPanel()
        {
            isOpen = true;
        }

        public override OpenResult Open()
        {
            return OpenResult.Safe;
        }

        public override string ToString()
        {
            return "=";
        }
    }
}
