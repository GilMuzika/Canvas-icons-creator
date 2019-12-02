using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Canvas_26._11._19
{
    class LField: Label
    {
        private EventHandler selfClickingEvent;

        public LField(int side)
        {
            this.Width = side; this.Height = side;
            this.drawBorder(1, Color.Black);
            Statics.fieldsBackColor = this.BackColor;
            selfClickingEvent = new EventHandler((object sender, EventArgs e) => { this.BackColor = Statics.penColor; });
        }

        public void clickOnSelf()
        {
            this.Click += selfClickingEvent;
            this.MouseHover += selfClickingEvent;
        }

        public void unClickOnSelf()
        {
            this.Click -= selfClickingEvent;
            this.MouseHover -= selfClickingEvent;
        }

    }
}
