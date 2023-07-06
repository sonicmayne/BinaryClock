namespace BinaryClock
{
    public partial class Form1 : Form
    {
        private readonly ClockDisplay clock;

        public Form1()
        {
            InitializeComponent();
            clock = new ClockDisplay
            {
                Dock = DockStyle.Fill
            };
            Controls.Add(clock);

            this.Width = 425;
            this.Height = 35;

            this.TopMost = true;
            this.FormBorderStyle = FormBorderStyle.None;
        }
    }
}