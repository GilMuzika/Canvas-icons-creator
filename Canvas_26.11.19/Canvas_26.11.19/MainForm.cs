using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Canvas_26._11._19
{
    public partial class MainForm : Form
    {
        private buttonCanvasPen _pen;
        private bool _isPenInUse = false;
        private Canvas _currentBoard;
        private Panel pnlFieldsHolder = new Panel();
        private Panel pnlPreviewBox = new Panel();
        private Button btnPrewiew = new Button();
        private Button btnSave = new Button();
        private Button btnSaveLarge = new Button();
        private ComboBox cmbcanvasFieldSize = new ComboBox();



        public int FieldSize { get; set; } = 5;

        private LField[,] _localContainer;

        public MainForm()
        {
            InitializeComponent();
            initialiseToolBox();
            this.FieldSize = 25;
            initilizeMainCanvas();
            
            initialiseCreateNewCanvas();

            
        }

        private void initialiseCreateNewCanvas()
        {
            Panel pnlNewCanvas = new Panel();
            pnlNewCanvas.Width = pnlToolBox.Width;
            pnlNewCanvas.Height = buttonCanvasPen.Side * 3;
            pnlNewCanvas.Location = new Point(pnlToolBox.Location.X, pnlToolBox.Location.Y + pnlToolBox.Height + 5);
            pnlNewCanvas.drawBorder(1, Color.Black);

            Label lblNewCanvas = new Label();
            lblNewCanvas.Text = "New Canvas \nField Size:";
            lblNewCanvas.AutoSize = true;
            lblNewCanvas.Location = new Point(2, 2);
            pnlNewCanvas.Controls.Add(lblNewCanvas);

         
            cmbcanvasFieldSize.Location = new Point(2, 2 + lblNewCanvas.Height + 2);
            cmbcanvasFieldSize.Width = pnlNewCanvas.Width - 4;
            for (int i = 8; i <= 40; i++) cmbcanvasFieldSize.Items.Add(i);
            cmbcanvasFieldSize.SelectedItem = 25;
            pnlNewCanvas.Controls.Add(cmbcanvasFieldSize);

            Button btnCreateCanvas = new Button();
            btnCreateCanvas.Text = "Create new \ncanvas";
            btnCreateCanvas.AutoSize = true;
            btnCreateCanvas.Location = new Point(pnlNewCanvas.Width /2 - btnCreateCanvas.Width / 2 - 2, cmbcanvasFieldSize.Location.Y + cmbcanvasFieldSize.Height + 10);
            btnCreateCanvas.Click += (object sender, EventArgs e) => 
                {
                    pnlFieldsHolder.Controls.Clear();
                    pnlPreviewBox.Controls.Clear();
                    this.Controls.Remove(pnlFieldsHolder);
                    this.Controls.Remove(pnlPreviewBox);
                    this.Controls.Remove(btnPrewiew);
                    this.Controls.Remove(btnSave);

                    this.FieldSize = (int)cmbcanvasFieldSize.SelectedItem;
                    initilizeMainCanvas();



                };
            pnlNewCanvas.Controls.Add(btnCreateCanvas);


            this.Controls.Add(pnlNewCanvas);
        }

        private async void initilizeMainCanvas()
        {
            
            

            this.Width = Screen.PrimaryScreen.Bounds.Width;
            this.WindowState = FormWindowState.Maximized;

           


            pnlFieldsHolder.Width = (this.Width - this.Width / 5) - 20;
            pnlFieldsHolder.Height = Screen.PrimaryScreen.Bounds.Height - 100;

            if (pnlFieldsHolder.Height < pnlFieldsHolder.Width) pnlFieldsHolder.Width = pnlFieldsHolder.Height;

            pnlFieldsHolder.Location = new Point(this.Width - pnlFieldsHolder.Width - 10, 10);
            pnlFieldsHolder.drawBorder(1, Color.Black);

            this.Controls.Add(pnlFieldsHolder);

            
            int numberOfFields = (pnlFieldsHolder.Height - pnlFieldsHolder.Height / 25) / this.FieldSize;

            _currentBoard = new Canvas(numberOfFields * numberOfFields);
            _currentBoard.FieldSize = this.FieldSize;


            _currentBoard.exportFieldsNow +=  async (LField[,] fields) =>
            {

                Task tsk = Task.Run(() =>
                    {
                        _localContainer = fields;

                        LField[] plainContainer = new LField[fields.Length];
                        int count = 0;
                        foreach(var s in fields)
                        {
                            plainContainer[count] = s;
                            count++;
                        }

                        if(pnlFieldsHolder.InvokeRequired)
                        {
                            pnlFieldsHolder.Invoke((Action)delegate { pnlFieldsHolder.Controls.AddRange(plainContainer); });
                        }
                        else pnlFieldsHolder.Controls.AddRange(plainContainer);

                        initialisePreviewAndSave(pnlFieldsHolder.Location);
                        
                    }
                );

                await tsk;
            };


            initialiszeColorPicker();
            await _currentBoard.initialiseMatrix();
            



        }

        private void initialiseToolBox()
        {
            
            
            _pen = new buttonCanvasPen();
            this.pnlToolBox.Location = new Point(10, 10);
            this.pnlToolBox.Width = buttonCanvasPen.Side * 2 + 6;
            this.pnlToolBox.Height = buttonCanvasPen.Side * 5;
            this.pnlToolBox.drawBorder(1, Color.Black);
            _pen.Location = new Point(2, 2);

            

            _pen.Click += (object sender, EventArgs e) => {
                _ = _isPenInUse ? _isPenInUse = false : _isPenInUse = true;

                if (_isPenInUse)
                {
                    penInUse();
                }
                else
                {
                    penNotINUse();
                }
            };
            pnlToolBox.Controls.Add(_pen);
        }

        private void initialiszeColorPicker()
        {
            ColorDialog colorPicker = new ColorDialog();
            
            Button btnPickAColor = new Button();
            btnPickAColor.Width = buttonCanvasPen.Side; btnPickAColor.Height = buttonCanvasPen.Side;
            btnPickAColor.Image = Properties.Resources.pickAPenColor;
            btnPickAColor.Location = new Point(buttonCanvasPen.Side + 4, 2);
            btnPickAColor.Click += (object sender, EventArgs e) =>
                {
                    if(colorPicker.ShowDialog() == DialogResult.OK)
                    {
                        Statics.penColor = colorPicker.Color;
                        //_pen.Image = null;
                        _pen.drawBorder(15, colorPicker.Color);

                    }
                };
            pnlToolBox.Controls.Add(btnPickAColor);

        }
        private void initialisePreviewAndSave(Point pnlFieldsHoldwerLocation)
        {            
            PictureBox pcbPreviewBox = new PictureBox();
            pcbPreviewBox.Width = (_localContainer.GetLength(0)) * 10;
            pcbPreviewBox.Height = (_localContainer.GetLength(1)) * 10;
            pcbPreviewBox.Location = new Point(2, 2);

            if (pnlPreviewBox.InvokeRequired)
            {
                pnlPreviewBox.Invoke((Action)delegate
                {
                    pnlPreviewBox.Width = pcbPreviewBox.Width + 4;
                    pnlPreviewBox.Height = pcbPreviewBox.Height + 4;
                    pnlPreviewBox.Location = new Point(pnlFieldsHoldwerLocation.X - pcbPreviewBox.Width - 8, pnlFieldsHoldwerLocation.Y);
                    if(pnlPreviewBox.Bounds.IntersectsWith(pnlToolBox.Bounds))
                    {
                        //MessageBox.Show("Intersected");
                        pnlPreviewBox.Width = pnlPreviewBox.Height = this.Width - pnlFieldsHolder.Width - pnlToolBox.Width - 100;
                        pnlPreviewBox.Location = new Point(pnlFieldsHolder.Location.X - pnlPreviewBox.Width - 10, pnlFieldsHolder.Location.Y);
                        pcbPreviewBox.Width = pcbPreviewBox.Height = this.Width - pnlFieldsHolder.Width - pnlToolBox.Width - 104;
                    }
                    
                });
            }
            else
            {
                pnlPreviewBox.Width = pcbPreviewBox.Width + 4;
                pnlPreviewBox.Height = pcbPreviewBox.Height + 4;
                pnlPreviewBox.Location = new Point(pnlFieldsHoldwerLocation.X - pcbPreviewBox.Width - 8, pnlFieldsHoldwerLocation.Y);
                if (pnlPreviewBox.Bounds.IntersectsWith(pnlToolBox.Bounds))
                {
                    //MessageBox.Show("Intersected");
                    pnlPreviewBox.Width = pnlPreviewBox.Height = this.Width - pnlFieldsHolder.Width - pnlToolBox.Width - 100;
                    pnlPreviewBox.Location = new Point(pnlFieldsHolder.Location.X - pnlPreviewBox.Width - 10, pnlFieldsHolder.Location.Y);
                    pcbPreviewBox.Width = pcbPreviewBox.Height = this.Width - pnlFieldsHolder.Width - pnlToolBox.Width - 104;
                }
            }

            pnlPreviewBox.drawBorder(1, Color.Black);

            if(pnlPreviewBox.InvokeRequired)
            {
                pnlPreviewBox.Invoke((Action)delegate { pnlPreviewBox.Controls.Add(pcbPreviewBox); });
            }
            else pnlPreviewBox.Controls.Add(pcbPreviewBox);

            
            btnPrewiew.Text = "Preview";
            if(this.InvokeRequired)
            {
                this.Invoke((Action)delegate { this.Controls.Add(btnPrewiew); });
            }
            else this.Controls.Add(btnPrewiew);
            if (btnPrewiew.InvokeRequired)
            {
                btnPrewiew.Invoke((Action)delegate { btnPrewiew.Location = new Point(pnlPreviewBox.Location.X, pnlPreviewBox.Location.Y + pnlPreviewBox.Height + 5); });
            }
            else btnPrewiew.Location = new Point(pnlPreviewBox.Location.X, pnlPreviewBox.Location.Y + pnlPreviewBox.Height + 5);

            

            btnPrewiew.Click += (object sender, EventArgs e) =>             
                {
                    Bitmap btmp = _currentBoard.savePicture(_localContainer);
                    pcbPreviewBox.Image = btmp.BlockyResizeImage(pcbPreviewBox.Width, pcbPreviewBox.Height);
                };

            
            btnSave.Text = "Save";
            if(btnSave.InvokeRequired)
            {
                btnSave.Invoke((Action)delegate { btnSave.Location = new Point(btnPrewiew.Location.X + btnPrewiew.Width + 5, btnPrewiew.Location.Y); });
            }
            else btnSave.Location = new Point(btnPrewiew.Location.X + btnPrewiew.Width + 5, btnPrewiew.Location.Y);

            
            btnSave.Click += (object sender, EventArgs e) => 
                {
                    Bitmap btmp = _currentBoard.savePicture(_localContainer);
                    savingClick(btmp);
                };

            if (this.InvokeRequired)
            {
                this.Invoke((Action)delegate { this.Controls.Add(btnSave); });
                this.Invoke((Action)delegate { this.Controls.Add(pnlPreviewBox); });
            }
            else { this.Controls.Add(btnSave); this.Controls.Add(pnlPreviewBox); }


            if(btnSaveLarge.InvokeRequired)
            {
                btnSaveLarge.Invoke((Action)delegate { btnSaveLarge.Location = new Point(btnSave.Location.X, btnSave.Location.Y + btnSave.Height + 3); });
            }
            else btnSaveLarge.Location = new Point(btnSave.Location.X, btnSave.Location.Y + btnSave.Height + 3);
            btnSaveLarge.Text = "Save large as preview";
            btnSaveLarge.AutoSize = true;
            btnSaveLarge.Click += (object sender, EventArgs e) => 
                {
                    Bitmap btmp = _currentBoard.savePicture(_localContainer).BlockyResizeImage(pcbPreviewBox.Width, pcbPreviewBox.Height);
                    savingClick(btmp);
                };

            if(this.InvokeRequired)
            {
                this.Invoke((Action)delegate { this.Controls.Add(btnSaveLarge); });
            }
            else this.Controls.Add(btnSaveLarge);
            
        }

        private void savingClick(Bitmap btmp)
        {
            SaveFileDialog pickFileDialog = new SaveFileDialog();
            pickFileDialog.Filter = "Bitmap files(*.bmp)|*.bmp|Jpg files(*.jpg)|*.jpg|Gif files(*.gif)|*.gif|PNG files(*.png)|*.png";

            if (pickFileDialog.ShowDialog() == DialogResult.OK)
            {
                

                switch (pickFileDialog.FileName.Substring(pickFileDialog.FileName.LastIndexOf(".") + 1))
                {
                    case "bmp":
                        btmp.Save(pickFileDialog.FileName, ImageFormat.Bmp);
                        break;
                    case "jpg":
                        btmp.Save(pickFileDialog.FileName, ImageFormat.Jpeg);
                        break;
                    case "gif":
                        btmp.Save(pickFileDialog.FileName, ImageFormat.Gif);
                        break;
                    case "png":
                        btmp.Save(pickFileDialog.FileName, ImageFormat.Png);
                        break;
                }
            }
        }



        private void penInUse()
        {
            _pen.BackColor = Statics.penColor;
            foreach (var s in _localContainer)
            {
                s.clickOnSelf();
            }
        }

        private void penNotINUse()
        {
            _pen.BackColor = Statics.initiatePenColor;
            foreach (var s in _localContainer)
            {
                s.unClickOnSelf();
            }
        }

    }
}
