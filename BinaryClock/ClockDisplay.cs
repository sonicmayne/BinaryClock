using System.Collections;
using System.Runtime.InteropServices;

namespace BinaryClock
{
    public class ClockDisplay : Control
    {
        public BitArray Hours { get; set; } = new BitArray(4);

        public BitArray Minutes { get; set; } = new BitArray(6);

        public int RenderedWidth { get; set; }

        private int lastMinute  = -1;

        private System.Windows.Forms.Timer timer;

        private const int HT_CAPTION = 0x2;
        private const int WM_NCLBUTTONDOWN = 0xA1;

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        public ClockDisplay()
        {
            UpdateClock();
            timer = new()
            {
                Interval = 500
            };
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            UpdateClock();
        }

        private BitArray GetNumber(int value, int length)
        {
            string binary = Convert.ToString(value, 2);
            if (binary.Length < length)
            {
                binary = binary.PadLeft(length, '0');
            }

            BitArray ba = new(length);

            int index = 0;
            foreach (char bit in binary)
            {
                ba.Set(index, bit == '1');
                index++;
            }

            return ba;
        }

        public void UpdateClock()
        {
            DateTime time = DateTime.Now;
            if (time.Minute == lastMinute)
            {
                return;
            }

            Hours = GetNumber(time.Hour < 12 ? time.Hour : time.Hour - 12, 4);
            Minutes = GetNumber(time.Minute, 6);
            lastMinute = time.Minute;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            Pen pen = new(Color.Black);
            Brush brush = Brushes.Black;

            int x = 10;
            int y = 12;
            foreach (bool hour in Hours)
            {
                if (hour)
                {
                    g.FillRectangle(brush, new Rectangle(x, y, 25, 25));
                }
                else
                {
                    g.DrawRectangle(pen, new(x, y, 25, 25));
                }

                x += 35;
            }

            x += 10;

            g.DrawLine(pen, new(x, y), new(x, y + 25));

            x += 15;

            foreach (bool minute in Minutes)
            {
                if (minute)
                {
                    g.FillRectangle(brush, new Rectangle(x, y, 25, 25));
                }
                else
                {
                    g.DrawRectangle(pen, new(x, y, 25, 25));
                }

                x += 35;
            }

            RenderedWidth = x;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (e.Button == MouseButtons.Left)
            {
                IntPtr? handle = (Parent as Form)?.Handle;

                if (handle.HasValue)
                {
                    ReleaseCapture();
                    SendMessage(handle.Value, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
                }
            }
        }
    }
}
