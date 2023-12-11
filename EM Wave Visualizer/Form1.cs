using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EM_Wave_Visualizer
{
    public partial class Form1 : Form
    {
        const double SPEED_OF_LIGHT = 300000000;
        const double PLANCKS_CONST = 6.62607015e-34;
        const double OSCILLATION_MASS = 7.36230017e-51;
        double MEASUREMENT_TIME = 1;
        
        double M_TIME_MOUSE = 1;
        bool IN_WINDOW = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.FillRectangle(new SolidBrush(Color.White), new Rectangle(0, 0, pictureBox1.Width, pictureBox1.Height));
            e.Graphics.DrawLine(new Pen(Color.Black), 15, pictureBox1.Height / 2, pictureBox1.Width - 5, pictureBox1.Height / 2);

            double.TryParse(periodInput.Text, out double period);
            double.TryParse(frequencyInput.Text, out double frequency);
            double.TryParse(wavelengthInput.Text, out double wavelength);
            double amplitude = 10;

            double scale = scaleSlider.Value * 0.05;

            // Draw sine
            for(int x = 0; x < pictureBox1.Width - 20; x++)
            {
                double angle = WrapAngle((x / (period * scale)) * 3.14 / 180);
                double y = amplitude * Math.Sin(angle);

                e.Graphics.FillRectangle(new SolidBrush(Color.DarkBlue), x + 15, (int)(pictureBox1.Height / 2 - y), 1, 1);
            }

            // Draw 1 second lines
            for (int i = 0; i < (int)((pictureBox1.Width - 20) / (360 * scale)) + 1; i++)
            {
                long x = (long)Math.Floor(i * (360 * scale));
                e.Graphics.DrawLine(new Pen(Color.Gray), x + 15, (pictureBox1.Height / 2 - 60), x + 15, (pictureBox1.Height / 2 + 60));
            }

            // Draw measurement time line
            long mX = (long)Math.Floor(MEASUREMENT_TIME * (360 * scale));
            e.Graphics.DrawLine(new Pen(Color.Red, 2), mX + 15, (pictureBox1.Height / 2 - 60), mX + 15, (pictureBox1.Height / 2 + 60));

            // Draw measurement time placer line
            if(IN_WINDOW)
            {
                mX = (long)Math.Floor(M_TIME_MOUSE * (360 * scale));
                e.Graphics.DrawLine(new Pen(Color.LightGray), mX + 15, (pictureBox1.Height / 2 - 60), mX + 15, (pictureBox1.Height / 2 + 60));
            }

            // Calculations
            double wave_energy = PLANCKS_CONST * frequency * MEASUREMENT_TIME;
            double wave_mass = wave_energy / (SPEED_OF_LIGHT * SPEED_OF_LIGHT);
            double wave_force = OSCILLATION_MASS * SPEED_OF_LIGHT * frequency;
            double wave_momentum = (PLANCKS_CONST * frequency * MEASUREMENT_TIME) / SPEED_OF_LIGHT;

            waveEnergy.Text = $"{wave_energy} J";
            waveMass.Text = $"{wave_mass} kg";
            waveForce.Text = $"{wave_force} N";
            waveMomentum.Text = $"{wave_momentum} N s";
        }

        private double WrapAngle(double angle)
        {
            return angle % 360;
        }

        private void periodInput_TextChanged(object sender, EventArgs e)
        {
            if (!periodInput.Focused) return;

            double.TryParse(periodInput.Text, out double period);
            if (period == 0) return;

            double freq = 1 / period;
            frequencyInput.Text = freq.ToString();
            wavelengthInput.Text = (SPEED_OF_LIGHT / freq).ToString();

            pictureBox1.Invalidate();
        }

        private void frequencyInput_TextChanged(object sender, EventArgs e)
        {
            if (!frequencyInput.Focused) return;

            double.TryParse(frequencyInput.Text, out double freq);
            if (freq == 0) return;

            double period = 1 / freq;
            periodInput.Text = period.ToString();
            wavelengthInput.Text = (SPEED_OF_LIGHT / freq).ToString();

            pictureBox1.Invalidate();
        }

        private void scaleSlider_Scroll(object sender, EventArgs e)
        {
            pictureBox1.Invalidate();
        }

        private void wavelengthInput_TextChanged(object sender, EventArgs e)
        {
            if (!wavelengthInput.Focused) return;

            double.TryParse(wavelengthInput.Text, out double wavelength);
            if (wavelength == 0) return;

            double freq = SPEED_OF_LIGHT / wavelength;
            double period = 1 / freq;
            frequencyInput.Text = freq.ToString();
            periodInput.Text = period.ToString();

            pictureBox1.Invalidate();
        }

        private void measurementTime_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(measurementTime.Text, out double mTime);
            if(mTime == 0) return;

            MEASUREMENT_TIME = mTime;
            pictureBox1.Invalidate();
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.X < 15 || e.X > pictureBox1.Width - 5)
                return;

            double scale = scaleSlider.Value * 0.05;
            M_TIME_MOUSE = (e.X - 15) / (360d * scale);

            pictureBox1.Invalidate();
        }

        private void pictureBox1_MouseEnter(object sender, EventArgs e)
        {
            IN_WINDOW = true;
        }

        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
            IN_WINDOW = false;
            pictureBox1.Invalidate();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            measurementTime.Text = M_TIME_MOUSE.ToString();
        }
    }
}
