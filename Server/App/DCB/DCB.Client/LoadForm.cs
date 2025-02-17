using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DCB.Client
{
    public partial class LoadForm : Form
    {
        private int step = 1;

        private readonly string[] loadTexts =
        [
            "Подключаемся к серверу", "Загружаем мир", "Загружаем врагов", "Загружаем что-то ещё", "Ждём...", "Курим...",  "Ещё немного...", "Ещё пару секунд..."
        ];

        public LoadForm()
        {
            InitializeComponent();
            step = 0;           
            LoadProgressBar.Minimum = 0;
            LoadProgressBar.Maximum = 100;
            LoadProgressBar.Value = 0;
            LoadTimer.Interval = 1000;
            LoadTimer.Enabled = true;
            LoadTimer.Tick += LoadTimer_Tick;
            LoadTimer.Start();
        }

        private void LoadTimer_Tick(object? sender, EventArgs e)
        {
            if (LoadProgressBar.Value < step * 10 && LoadProgressBar.Value < LoadProgressBar.Maximum)
            {
                LoadProgressBar.Increment(1);
                if(loadTexts.Length > step)
                    LoadTextBox.Text = loadTexts[step];
            }            
        }

        public void SetStep(int num)
        {
            step = num;
        }

        public void Ready()
        {
            LoadTimer.Stop();
            LoadTimer.Dispose();
            Close();
        }
    }
}
