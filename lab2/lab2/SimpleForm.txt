using System;
using System.Dynamic;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace lab2
{
	public class SimpleForm : Form
	{
		private Button  _runLabThreadButton;
		private TextBox _inputOutputTextBox;
		
		public SimpleForm()
		{
			Width = 520;
			
			_inputOutputTextBox          = new TextBox();
			_inputOutputTextBox.Top      = 10;
			_inputOutputTextBox.Left     = 10;
			_inputOutputTextBox.Width    = 500;
			_inputOutputTextBox.Text     = "Имя файла...";
			Controls.Add(_inputOutputTextBox);
			
			_runLabThreadButton        = new Button();
			_runLabThreadButton.Left   = 10;
			_runLabThreadButton.Width  = 300;
			_runLabThreadButton.Height = 60;
			_runLabThreadButton.Top    = 40;
			_runLabThreadButton.Text   = "Суммировать все числа в файле";
			_runLabThreadButton.Click +=
				(_, __) =>
				{
					var t = new Thread(FindAndSumAllNumbers);
					t.Start();
				};
			Controls.Add(_runLabThreadButton);
		}

		private void FindAndSumAllNumbers()
		{
			var filepath = _inputOutputTextBox.Text;
			_runLabThreadButton.Text    = "Ждите...";
			_runLabThreadButton.Enabled = false;
			
			if (!File.Exists(filepath))
			{
				MessageBox.Show("Такого файла нет!");
				_runLabThreadButton.Text    = "Суммировать все числа в файле";
				_runLabThreadButton.Enabled = true;
				return;
			}

			_inputOutputTextBox.Text = "Считаем...";
			var sum = 0;
			using (var f = File.OpenText(filepath))
			{
				var content = f.ReadToEnd();
				
				var num = 0;
				foreach (var c in content)
				{
					if (!Char.IsDigit(c))
					{
						sum += num;
						num =  0;
					}
					else
					{
						num *= 10;
						num += c - '0';
					}
				}
				
			}

			_inputOutputTextBox.Text    = sum.ToString();
			_runLabThreadButton.Text    = "Суммировать все числа в файле";
			_runLabThreadButton.Enabled = true;
			MessageBox.Show("Готово!");
		}
	}
}
