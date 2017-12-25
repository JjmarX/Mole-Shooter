//#define My_Debug
using Mole_Shooter.Properties;
using System;
using System.Drawing;
using System.Media;
using System.Windows.Forms;
using System.Resources;

namespace Mole_Shooter
{  
    public partial class MoleShooter : Form
    {
        const int SplatNum = 3;
        const int FrameNum = 8;
        bool splat = false;

        int _hits = 0;
        int _missed = 0;
        int _totalScrore = 0;
        double _averageHits = 0;
        int _gameFrame = 0;
        int _splatTime = 0;
#if My_Debug
        int _cursX = 0;
        int _cursY = 0;
#endif
        CMole _mole;
        CSplat _splat;
        CScoreFrame _scoreFrame;
        CSign _sign;
        Random rnd = new Random();


        public MoleShooter()
        {
            InitializeComponent();
            DoubleBuffered = true;

            Bitmap b = new Bitmap(Resources.site);
            this.Cursor = CustomCursor.CreateCursor(b, b.Height / 2, b.Width / 2);

            _scoreFrame = new CScoreFrame() { Left = 10, Top = 10 };
            _sign = new CSign() { Left = 570, Top = 10 };
            _mole = new CMole() { Left = 10, Top = 200 };
            _splat = new CSplat();
        }
           

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics dc = e.Graphics;
            if (splat == true)
            {
                _splat.DrawImage(dc);
            }
            else
            {
                _mole.DrawImage(dc);
            }
            
            _sign.DrawImage(dc);
            _scoreFrame.DrawImage(dc);
#if My_Debug
            TextFormatFlags flags = TextFormatFlags.Left | TextFormatFlags.EndEllipsis;
            Font _font = new System.Drawing.Font("Stencil", 12, FontStyle.Regular);
            TextRenderer.DrawText(dc, "x=" + _cursX.ToString() + ":" + "Y=" + _cursY.ToString(), _font,
                new Rectangle(0, 0, 120, 20), SystemColors.ControlText, flags);
#endif 
            TextFormatFlags flags = TextFormatFlags.Left;
            Font _font = new Font("Stencil", 12, FontStyle.Regular);
            TextRenderer.DrawText(e.Graphics, "Shots:" + _totalScrore.ToString(), _font, new Rectangle(30, 32, 120, 20), SystemColors.ControlText, flags);
            TextRenderer.DrawText(e.Graphics, "Hits:" + _hits.ToString(), _font, new Rectangle(30, 52, 120, 20), SystemColors.ControlText, flags);
            TextRenderer.DrawText(e.Graphics, "Missed:" + _missed.ToString(), _font, new Rectangle(30, 72, 120, 20), SystemColors.ControlText, flags);
            TextRenderer.DrawText(e.Graphics, "Avg:" + _averageHits.ToString() + "%", _font, new Rectangle(30, 92, 120, 20), SystemColors.ControlText, flags);

            base.OnPaint(e);
        }
        private void MoleShooter_MouseMove (object sender, MouseEventArgs e)
        {
#if My_Debug
            _cursX = e.X;
            _cursY = e.Y;
#endif
            this.Refresh();
        }

        private void MoleShooter_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.X > 620 && e.X < 723 && e.Y > 38 && e.Y < 57)
            {
                timerGameLoop.Start();
            }
            else if (e.X > 620 && e.X < 698 && e.Y > 69 && e.Y < 90)
            {
                timerGameLoop.Stop();
            }
            else if (e.X > 620 && e.X < 723 && e.Y > 102 && e.Y < 120)
            {
                timerGameLoop.Stop();
                _totalScrore = 0;
                _hits = 0;
                _missed = 0;
                _averageHits = 0;
            }
            else if (e.X > 620 && e.X < 694 && e.Y > 134 && e.Y < 155)
            {
                timerGameLoop.Stop();
                Application.Exit();
            }
            else
            {
                if (_mole.Hit(e.X, e.Y))
                {
                    splat = true;
                    _splat.Left = _mole.Left - Resources.splat.Width / 3;
                    _splat.Top = _mole.Top - Resources.splat.Height / 3;
                    _hits++;
                }
                else
                    _missed++;
                _totalScrore = _missed + _hits;
                _averageHits = Math.Round((double)_hits / (double)_totalScrore * 100.0);
               
            }
            fireSound();
            
        }

        private void timerGameLoop_Tick(object sender, EventArgs e)
        {
            if (_gameFrame >= FrameNum)
            {
                UpdateMole();
                _gameFrame = 0;
            }
            _gameFrame++;
            this.Refresh(); 
            if (splat)
            {
                if (_splatTime >= SplatNum)
                    splat = false;
                _splatTime = 0;
                UpdateMole();
            }
            _splatTime++;
            
        }
        private void fireSound()
        {
            SoundPlayer simpleSound = new SoundPlayer(Resources.shotgun);
            simpleSound.Play();
        }

        private void UpdateMole()
        {
            _mole.Update(
                rnd.Next(9, this.Width - Resources.mole.Width),
                rnd.Next(this.Height / 2, this.Height - Resources.mole.Height * 2)
                );
        }
    }
}
