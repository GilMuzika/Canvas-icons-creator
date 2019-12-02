using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Canvas_26._11._19
{
    class Canvas
    {
        public int FieldSize { get; set; } = 5;
        
        protected int _fieldsnumber;
        protected LField[,] _fieldsContainer;

        public delegate void exportFields(LField[,] fields);
        public event exportFields exportFieldsNow;


        public Canvas(int fieldsNumber)
        {
            this._fieldsnumber = fieldsNumber;

            _fieldsContainer = new LField[(int)Math.Sqrt(_fieldsnumber), (int)Math.Sqrt(_fieldsnumber)];
        }

        public async Task initialiseMatrix()
        {
            for(int i = 0; i < _fieldsContainer.GetLength(0); i++)
            {
                for (int j = 0; j < _fieldsContainer.GetLength(1); j++)
                {
                    _fieldsContainer[i, j] = new LField(FieldSize);
                    await sendFieldToMainFormAsync(_fieldsContainer[i, j], i, j);
                }
            }
            exportFieldsNow?.Invoke(_fieldsContainer);
        }

        private async Task sendFieldToMainFormAsync(LField field, int step1, int step2)
        {            
            Task tsk = Task.Run(() =>
                {

                    field.Location = new Point(step1 * (field.Width) + 10, step2 * (field.Height) + 10);
                    //_fieldsContainer[step1, step2] = field;

                }

            );
            await tsk;
        }

        public Bitmap savePicture(LField[,] container)
        {
            Bitmap picture = new Bitmap(container.GetLength(0), container.GetLength(1));

            for (int i = 0; i < container.GetLength(0); i++)
            {
                for (int j = 0; j < container.GetLength(1); j++)
                {
                    picture.SetPixel(i, j, _fieldsContainer[i, j].BackColor);
                }
            }
            using (Graphics graphicsPbj = Graphics.FromImage(picture))
            {
                graphicsPbj.DrawImage(picture, 0, 0);
            }
            return picture;
        }
        

    }
}
