using System;
using System.Text;
using System.Windows.Forms;

namespace Lab1
{
	public partial class DefaultForm : Form
	{
		private Label    _infoLabel;
		private CheckBox _booleanMarker;
		private ComboBox _propertySelect;
		
		public DefaultForm()
		{
			this.Width = 1030;
			this.Text  = "Класс Environment";
			
			_propertySelect               = new ComboBox();
			_propertySelect.Left          = 10;
			_propertySelect.Top           = 10;
			_propertySelect.DropDownStyle = ComboBoxStyle.DropDownList;
			_propertySelect.Width         = 1000;
			_propertySelect.Items.Add("Командная строка");   // 0
			_propertySelect.Items.Add("Рабочий каталог");    // 1
			_propertySelect.Items.Add("ID текущего потока"); // 2
			_propertySelect.Items.Add("64-битная?");		 // 3
			_propertySelect.Items.Add("Имя NetBIOS машины"); // 4
			_propertySelect.Items.Add("Версия ОС");		     // 5
			_propertySelect.Items.Add("Кол-во процессоров"); // 6
			_propertySelect.Items.Add("Объём подкачки");	 // 7
			_propertySelect.Items.Add("Тип перевода строки");// 8
			_propertySelect.Items.Add("Версия CLR");		 // 9
			_propertySelect.Items.Add("Имя пользователя");	 // 10
			_propertySelect.Items.Add("Рабочий набор процесса");   // 11
			_propertySelect.Items.Add("Интерактивный процесс?");   // 12
			_propertySelect.Items.Add("Прошло с момента запуска"); // 13
			_propertySelect.SelectedIndexChanged += OnPropertySelect;
			Controls.Add(_propertySelect);

			_infoLabel        = new Label();
			_infoLabel.Left   = 10;
			_infoLabel.Top    = 20 + _propertySelect.Height;
			_infoLabel.Width  = _propertySelect.Width;
			_infoLabel.Height = 300;
			Controls.Add(_infoLabel);

			_booleanMarker           = new CheckBox();
			_booleanMarker.Visible   = false;
			_booleanMarker.AutoCheck = false;
			_booleanMarker.Left      = 10;
			_booleanMarker.Top       = _propertySelect.Height + 20;
			Controls.Add(_booleanMarker);
		}

		private void OnPropertySelect(object sender, System.EventArgs e)
		{
			var index = _propertySelect.SelectedIndex;
			_infoLabel.Visible     = true;
			_booleanMarker.Visible = false;
			switch (index)
			{
				case 0:
					_infoLabel.Text = Environment.CommandLine;
					break;
				case 1:
					_infoLabel.Text = Environment.CurrentDirectory;
					break;
				case 2:
					_infoLabel.Text = Environment.CurrentManagedThreadId.ToString();
					break;
				case 3:
					_infoLabel.Visible     = false;
					_booleanMarker.Visible = true;
					_booleanMarker.Checked = Environment.Is64BitOperatingSystem;
					_booleanMarker.Text    = "64-битная ОС";
					break;
				case 4:
					_infoLabel.Text = Environment.MachineName;
					break;
				case 5:
					_infoLabel.Text = Environment.OSVersion.VersionString;
					break;
				case 6:
					_infoLabel.Text = Environment.ProcessorCount.ToString();
					break;
				case 7:
					if (Environment.OSVersion.Platform != PlatformID.Win32Windows
					    && Environment.OSVersion.Platform != PlatformID.Win32NT)
						_infoLabel.Text = "Свойство работает только на Windows!";
					else
						_infoLabel.Text = Environment.SystemPageSize + " байт";
					break;
				case 8:
					var newLineBuilder = new StringBuilder();
					
					foreach (var character in Environment.NewLine)
					{
						if (character == '\n')
							newLineBuilder.Append("LF");
						else if (character == '\r')
							newLineBuilder.Append("CR");
					}

					_infoLabel.Text = newLineBuilder.ToString();
					break;
				case 9:
					_infoLabel.Text = Environment.Version.ToString();
					break;
				case 10:
					_infoLabel.Text = Environment.UserName;
					break;
				case 11:
					if (Environment.OSVersion.Platform != PlatformID.Win32Windows
					    && Environment.OSVersion.Platform != PlatformID.Win32NT)
						_infoLabel.Text = "Свойство работает только на Windows!";
					else
						_infoLabel.Text = (Environment.WorkingSet / 1024) + " киБ";
					break;
				case 12:
					_infoLabel.Visible     = false;
					_booleanMarker.Visible = true;
					_booleanMarker.Checked = Environment.UserInteractive;
					_booleanMarker.Text    = "Процесс запущен в интерактивной сессии";
					break;
				case 13:
					_infoLabel.Text = Environment.TickCount + " мс";
					break;
			}
		}
	}
}

