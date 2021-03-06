﻿using System;
using System.Drawing;
using System.Windows.Forms;

namespace Screenote
{
    public partial class Note : Form
    {
        bool MoveWindow = false;
        int MoveX;
        int MoveY;
        Size NoteSize;

        bool ResizeWindow = false;
        int WindowRight, WindowBottom;

        internal enum Direction
        {
            Left,
            Right,
            Top,
            Bottom,
            LeftTop,
            LeftBottom,
            RightTop,
            RightBottom
        }

        Direction direction;

        public Note(Bitmap bitmap, Point location, Size size)
        {
            InitializeComponent();
            NoteSize = size;
            this.MinimumSize = new Size(16, 16);
            this.MaximumSize = SystemInformation.VirtualScreen.Size;
            this.Location = new Point(location.X - 1, location.Y - 1);
            this.Width = NoteSize.Width + 2;
            this.Height = NoteSize.Height + 2;

            picture.BackgroundImage = bitmap;
            picture.MouseDown += Note_MouseDown;
            picture.MouseUp += Note_MouseUp;
            picture.MouseMove += Note_MouseMove;
            picture.DoubleClick += Note_DoubleClick;
        }

        private void Note_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if ((Control.MousePosition.X - this.Location.X < 7) || ((this.Location.X + this.Width) - Control.MousePosition.X < 7) || (Control.MousePosition.Y - this.Location.Y < 7) || ((this.Location.Y + this.Height) - Control.MousePosition.Y < 7))
                {
                    WindowRight = this.Right;
                    WindowBottom = this.Bottom;
                    ResizeWindow = true;
                    return;
                }

                MoveX = e.X;
                MoveY = e.Y;
                MoveWindow = true;
            }
            if (e.Button == MouseButtons.Right)
            {
                picture.BackgroundImage.Dispose();
                this.Close();
                GC.Collect();
            }
            if (e.Button == MouseButtons.Middle)
            {
                Clipboard.SetImage(picture.BackgroundImage);
            }
        }

        private void Note_MouseUp(object sender, MouseEventArgs e)
        {
            if (ResizeWindow == true)
            {
                ResizeWindow = false;
                return;
            }

            if (MoveWindow == true)
            {
                MoveWindow = false;
            }
        }

        private void Note_MouseMove(object sender, MouseEventArgs e)
        {
            if (MoveWindow == true)
            {
                this.Location = new Point(Control.MousePosition.X - MoveX, Control.MousePosition.Y - MoveY);
                return;
            }

            if (ResizeWindow == true)
            {
                int x = this.Location.X, y = this.Location.Y, width = this.Width, height = this.Height;

                switch (direction)
                {
                    case Direction.Left:
                        if (WindowRight - MousePosition.X > 15)
                        {
                            x = MousePosition.X;
                            width = WindowRight - MousePosition.X;
                        }
                        Cursor.Current = Cursors.SizeWE;
                        break;
                    case Direction.Right:
                        width = MousePosition.X - this.Left;
                        Cursor.Current = Cursors.SizeWE;
                        break;
                    case Direction.Top:
                        if (WindowBottom - MousePosition.Y > 15)
                        {
                            y = MousePosition.Y;
                            height = WindowBottom - MousePosition.Y;
                        }
                        Cursor.Current = Cursors.SizeNS;
                        break;
                    case Direction.Bottom:
                        height = MousePosition.Y - this.Top;
                        Cursor.Current = Cursors.SizeNS;
                        break;
                    case Direction.LeftTop:
                        if (WindowRight - MousePosition.X > 15)
                        {
                            x = MousePosition.X;
                            width = WindowRight - MousePosition.X;
                        }
                        if (WindowBottom - MousePosition.Y > 15)
                        {
                            y = MousePosition.Y;
                            height = WindowBottom - MousePosition.Y;
                        }
                        Cursor.Current = Cursors.SizeNWSE;
                        break;
                    case Direction.LeftBottom:
                        if (WindowRight - MousePosition.X > 15)
                        {
                            x = MousePosition.X;
                            width = WindowRight - MousePosition.X;
                        }
                        height = MousePosition.Y - this.Top;
                        Cursor.Current = Cursors.SizeNESW;
                        break;
                    case Direction.RightTop:
                        width = MousePosition.X - this.Left;
                        if (WindowBottom - MousePosition.Y > 15)
                        {
                            y = MousePosition.Y;
                            height = WindowBottom - MousePosition.Y;
                        }
                        Cursor.Current = Cursors.SizeNESW;
                        break;
                    case Direction.RightBottom:
                        width = MousePosition.X - this.Left;
                        height = MousePosition.Y - this.Top;
                        Cursor.Current = Cursors.SizeNWSE;
                        break;
                }

                this.Bounds = new Rectangle(new Point(x, y), new Size(width, height));
                this.Refresh();
                return;
            }
            else
            {
                if (Control.MousePosition.X - this.Location.X < 7)
                {
                    Cursor.Current = Cursors.SizeWE;
                    direction = Direction.Left;
                }
                if ((this.Location.X + this.Width) - Control.MousePosition.X < 7)
                {
                    Cursor.Current = Cursors.SizeWE;
                    direction = Direction.Right;
                }
                if (Control.MousePosition.Y - this.Location.Y < 7)
                {
                    Cursor.Current = Cursors.SizeNS;
                    direction = Direction.Top;
                }
                if ((this.Location.Y + this.Height) - Control.MousePosition.Y < 7)
                {
                    Cursor.Current = Cursors.SizeNS;
                    direction = Direction.Bottom;
                }
                if ((Control.MousePosition.X - this.Location.X < 7) && (Control.MousePosition.Y - this.Location.Y < 7))
                {
                    Cursor.Current = Cursors.SizeNWSE;
                    direction = Direction.LeftTop;
                }
                if ((Control.MousePosition.X - this.Location.X < 7) && ((this.Location.Y + this.Height) - Control.MousePosition.Y < 7))
                {
                    Cursor.Current = Cursors.SizeNESW;
                    direction = Direction.LeftBottom;
                }
                if (((this.Location.X + this.Width) - Control.MousePosition.X < 7) && (Control.MousePosition.Y - this.Location.Y < 7))
                {
                    Cursor.Current = Cursors.SizeNESW;
                    direction = Direction.RightTop;
                }
                if (((this.Location.X + this.Width) - Control.MousePosition.X < 7) && ((this.Location.Y + this.Height) - Control.MousePosition.Y < 7))
                {
                    Cursor.Current = Cursors.SizeNWSE;
                    direction = Direction.RightBottom;
                }
            }

        }

        private void Note_DoubleClick(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.DefaultExt = ".png|.jpg|.bmp|.tif";
            saveFileDialog.Filter = "PNG|*.png|JPEG|*.jpg|BMP|*.bmp|TIFF|*.tif";
            saveFileDialog.DefaultExt = ".png";
            saveFileDialog.FileName = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".png";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                switch (saveFileDialog.FileName.Substring(saveFileDialog.FileName.LastIndexOf("."), saveFileDialog.FileName.Length - saveFileDialog.FileName.LastIndexOf(".")).ToLower())
                {
                    case ".png":
                        picture.BackgroundImage.Save(saveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Png);
                        break;
                    case ".jpg":
                        picture.BackgroundImage.Save(saveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                        break;
                    case ".bmp":
                        picture.BackgroundImage.Save(saveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
                        break;
                    case ".tif":
                        picture.BackgroundImage.Save(saveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Tiff);
                        break;
                }

            }

        }

        private void Note_KeyDown(object sender, KeyEventArgs e)
        {
            int shift = 1;
            if (e.Shift)
            {
                shift = 10;
            }

            switch (e.KeyCode)
            {
                case Keys.Oemtilde:
                    {
                        int width = this.Width, height = this.Height;
                        if (e.Shift)
                        {
                            this.Width = NoteSize.Width + 2;
                            this.Height = NoteSize.Height + 2;
                        }
                        else
                        {
                            if ((this.Width - 2) / (this.Height - 2) > NoteSize.Width / NoteSize.Height)
                            {
                                this.Width = (this.Height - 2) * NoteSize.Width / NoteSize.Height + 2;
                            }
                            else
                            {
                                this.Height = (this.Width - 2) * NoteSize.Height / NoteSize.Width + 2;
                            }
                        }
                        this.Location = new Point(this.Location.X + (width - this.Width) / 2, this.Location.Y + (height - this.Height) / 2);
                        break;
                    }
                case Keys.Left:
                    this.Location = new Point(this.Location.X - shift, this.Location.Y);
                    break;
                case Keys.Right:
                    this.Location = new Point(this.Location.X + shift, this.Location.Y);
                    break;
                case Keys.Up:
                    this.Location = new Point(this.Location.X, this.Location.Y - shift);
                    break;
                case Keys.Down:
                    this.Location = new Point(this.Location.X, this.Location.Y + shift);
                    break;
                case Keys.Space:
                    Note_MouseDown(this, new MouseEventArgs(MouseButtons.Middle, 0, Cursor.Position.X, Cursor.Position.Y, 0));
                    break;
                case Keys.Escape:
                    Note_MouseDown(this, new MouseEventArgs(MouseButtons.Right, 0, Cursor.Position.X, Cursor.Position.Y, 0));
                    break;
                case Keys.Enter:
                    Note_DoubleClick(this, new EventArgs());
                    break;
            }
        }

    }
}
