using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Threading;
using System.Windows.Forms;

namespace lab3
{
	public class RaceForm : Form
	{
		private Button _button1, _button2, _button3;
		private Button _runButton;
		private Mutex  _lockButtons;

		public RaceForm()
		{
			Width = 1400;
			
			_button1        = new Button();
			_button1.Width  = 200;
			_button1.Height = 50;
			_button1.Text   = "Кнопка 1";
			_button1.Left   = 0;
			_button1.Top    = 20;

			_button2        = new Button();
			_button2.Width  = 200;
			_button2.Height = 50;
			_button2.Text   = "Кнопка 2";
			_button2.Left   = 0;
			_button2.Top    = 40 + _button1.Height;

			_button3        = new Button();
			_button3.Width  = 200;
			_button3.Height = 50;
			_button3.Text   = "Кнопка 3";
			_button3.Left   = 0;
			_button3.Top    = 60 + _button1.Height + _button2.Height;

			_runButton       =  new Button();
			_runButton.Text  =  "Начать";
			_runButton.Left  =  10;
			_runButton.Top   =  80 + _button1.Height + _button2.Height + _button3.Height;
			_runButton.Click += (object sender, EventArgs _) =>
			{
				var t = new Thread(ManageRace);
				t.Start();
			};

			_lockButtons = new Mutex();
			
			Controls.Add(_button1);
			Controls.Add(_button2);
			Controls.Add(_button3);
			Controls.Add(_runButton);
		}

		protected void MoveButton(Button button)
		{
			var r = new Random();
			while (button.Right < this.Width)
			{
				_lockButtons.WaitOne();
				button.Left += 1;
				//button.Location = new Point(button.Left + 1, button.Top);
				Thread.Sleep(r.Next(2, 4));
				_lockButtons.ReleaseMutex();
			}
		}

		protected void ManageRace()
		{
			var t1 = new Thread(() => MoveButton(_button1));
			var t2 = new Thread(() => MoveButton(_button2));
			var t3 = new Thread(() => MoveButton(_button3));
			
			t1.Start();
			t2.Start();
			t3.Start();

			while (true)
			{
				_lockButtons.WaitOne();

				if (_button1.Right < Width && _button2.Right < Width && _button3.Right < Width)
				{
					_lockButtons.ReleaseMutex();
					Thread.Sleep(1);
					continue;
				}

				t1.Abort();
				t2.Abort();
				t3.Abort();

				if (_button1.Right >= Width)
					MessageBox.Show("Первая кнопка победила!", "Это победа!");
				else if (_button2.Right >= Width)
					MessageBox.Show("Вторая кнопка победила!", "Это победа!");
				else if (_button3.Right >= Width)
					MessageBox.Show("Третья кнопка победила!", "Это победа!");
				
				break;
			}
		}
	}
}
