using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Canvas_26._11._19
{
    class buttonCanvasPen : Button
    {
        static public int Side { get; set; }  = 50;
        public buttonCanvasPen()
        {
            this.Width = Side; this.Height = Side;
            this.Image = new Bitmap(Properties.Resources.penImage, new Size(Side-5, Side-5));
            Statics.initiatePenColor = this.BackColor;            
        }

    }
}
