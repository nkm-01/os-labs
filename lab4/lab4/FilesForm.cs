using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Mono.Unix;

namespace lab4
{
	public class FilesForm : Form
	{
		private ListView _fileList;
		private Label    _pathLabel;
		private Button   _submitButton;
		private Button   _backButton;
		private TextBox  _searchBox;
		private Button   _searchButton;
		private Button   _propertiesButton;
		private string   _path = "/";
		
		public FilesForm()
		{
			Width = 1024;
			
			_pathLabel        = new Label();
			_pathLabel.Top    = 10;
			_pathLabel.Width  = Width - 20;
			_pathLabel.Left   = 10;
			_pathLabel.Text   = "<No path>";
			_pathLabel.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
			Controls.Add(_pathLabel);

			_searchBox      = new TextBox();
			_searchBox.Top  = _pathLabel.Bottom + 10;
			//_searchBox.PlaceholderText = "Шаблон для поиска...";
			_searchBox.Left   = 10;
			_searchBox.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;

			_searchButton        =  new Button();
			_searchButton.Text   =  "Найти";
			_searchButton.Top    =  _searchBox.Top;
			_searchButton.Left   =  Width - 230;
			_searchButton.Width  =  200;
			_searchButton.Height =  _searchBox.Height;
			_searchButton.Anchor =  AnchorStyles.Right | AnchorStyles.Top;
			_searchButton.Click  += OnStartSearch;

			_searchBox.Width = _searchButton.Left - 20;

			Controls.Add(_searchBox);
			Controls.Add(_searchButton);
			
			_submitButton        =  new Button();
			_submitButton.Width  =  200;
			_submitButton.Height =  40;
			_submitButton.Top    =  _searchButton.Bottom + 10;
			_submitButton.Left   =  Width - 30 - _submitButton.Width;
			_submitButton.Text   =  "Перейти";
			_submitButton.Click  += OnSelectItem;
			_submitButton.Anchor =  AnchorStyles.Right | AnchorStyles.Top;
			Controls.Add(_submitButton);

			_backButton       = new Button();
			_backButton.Click += (object sender, EventArgs _) =>
			{
				_path = Directory.GetParent(
					_path.Substring(0, _path.Length - 1)
				)?.ToString();
				ListFiles();
			};
			_backButton.Width    = 200;
			_backButton.Height   = 40;
			_backButton.Top      = _submitButton.Bottom + 10;
			_backButton.Left     = Width - 30 - _submitButton.Width;
			_backButton.Text     = "Назад";
			_backButton.Anchor   = AnchorStyles.Right | AnchorStyles.Top;
			Controls.Add(_backButton);

			_propertiesButton        =  new Button();
			_propertiesButton.Text   =  "Свойства";
			_propertiesButton.Width  =  200;
			_propertiesButton.Height =  40;
			_propertiesButton.Top    =  _backButton.Bottom + 10;
			_propertiesButton.Left   =  _backButton.Left;
			_propertiesButton.Anchor =  AnchorStyles.Right | AnchorStyles.Top;
			_propertiesButton.Click  += ShowProperties;
			Controls.Add(_propertiesButton);

			_fileList              = new ListView();
			_fileList.Anchor       = AnchorStyles.Left | AnchorStyles.Right
							         | AnchorStyles.Top | AnchorStyles.Bottom;
			_fileList.Left         =  10;
			_fileList.Top          =  10 + _searchBox.Bottom;
			_fileList.Height       =  Height - 20 - _fileList.Top - _pathLabel.Height;
			_fileList.Width        =  Width - 50 - 200;
			_fileList.ItemActivate += OnSelectItem;
			_fileList.View         =  View.List;
			Controls.Add(_fileList);

			ListFiles();
		}

		public void ListFiles()
		{
			_fileList.Items.Clear();
			if (Directory.GetParent(_path) != null)
				_fileList.Items.Add("..");
			
			foreach (var d in Directory.GetDirectories(_path))
				_fileList.Items.Add(d.Replace(_path, ""));

			foreach (var f in Directory.GetFiles(_path))
				_fileList.Items.Add(f.Replace(_path, ""));

			_pathLabel.Text = _path;
		}

		private void OnSelectItem(object sender, EventArgs _)
		{
			string selected = _fileList.FocusedItem?.Text;
			if (selected != null)
			{
				if (selected.Equals(".."))
					_path = Directory.GetParent(
						_path.Substring(0, _path.Length - 1)
						)?.ToString();
				else
					_path += selected + "/";
				ListFiles();
			}
		}

		private void OnStartSearch(object sender, EventArgs _)
		{
			string filter = _searchBox.Text;
			
			if (String.IsNullOrEmpty(filter))
				return;
			
			_fileList.Items.Clear();
			_fileList.Invalidate();

			var t = new Thread(() =>
			{
				Search(filter, _path, _path);
				_pathLabel.Text = _path;
			});
			t.Start();
			_pathLabel.Text = "Поиск в [" + _path + "]...";
		}

		private void Search(string filter, string origin, string path)
		{
			if (String.IsNullOrEmpty(path))
				return;
			try
			{
				foreach (var f in Directory.GetFiles(path, filter))
				{
					try
					{
						_fileList.Items.Add(f.Replace(origin, ""));
					}
					catch (NullReferenceException)
					{}
				}

				foreach (var d in Directory.GetDirectories(path))
				{
					Search(filter, origin, d + "/");
				}
			}
			catch (UnauthorizedAccessException)
			{}
		}

		private void ShowProperties(object sender, EventArgs _)
		{
			var file = _path + _fileList.FocusedItem?.Text;
			if (!string.IsNullOrEmpty(file))
			{
				var attr = File.GetAttributes(file);
				
				var propertiesMessage = new StringBuilder();
				propertiesMessage.AppendLine($"Свойства файла {file}:");
				if (attr.HasFlag(FileAttributes.ReadOnly))
					propertiesMessage.AppendLine("Только чтение");
				if (attr.HasFlag(FileAttributes.Directory))
					propertiesMessage.AppendLine("Каталог");
				if (attr.HasFlag(FileAttributes.System))
					propertiesMessage.AppendLine("Системный");
				if (attr.HasFlag(FileAttributes.Archive))
					propertiesMessage.AppendLine("Архивируемый");
				if (attr.HasFlag(FileAttributes.Compressed))
					propertiesMessage.AppendLine("Сжатый");
				if (attr.HasFlag(FileAttributes.Hidden))
					propertiesMessage.AppendLine("Скрытый");
				if (attr.HasFlag(FileAttributes.Device))
					propertiesMessage.AppendLine("Устройство");
				if (attr.HasFlag(FileAttributes.Encrypted))
					propertiesMessage.AppendLine("Зашифрованный");

				var created = File.GetCreationTime(file);
				var changed = File.GetLastWriteTime(file);
				propertiesMessage.AppendLine($"Создан: {created}");
				propertiesMessage.AppendLine($"Изменён: {changed}");

				var permitions = new Mono.Unix.UnixFileInfo(file);

				propertiesMessage.AppendLine($"Владелец: {permitions.OwnerUser.UserName}");
				propertiesMessage.AppendLine($"Группа: {permitions.OwnerGroup.GroupName}");
				propertiesMessage.Append("Разрешения: ");
				// User
				propertiesMessage.Append(permitions.FileAccessPermissions.HasFlag(FileAccessPermissions.UserRead)
					? "r"
					: "-");
				propertiesMessage.Append(permitions.FileAccessPermissions.HasFlag(FileAccessPermissions.UserWrite)
					? "w"
					: "-");
				propertiesMessage.Append(permitions.FileAccessPermissions.HasFlag(FileAccessPermissions.UserExecute)
					? "x "
					: "- ");
				
				// Group
				propertiesMessage.Append(permitions.FileAccessPermissions.HasFlag(FileAccessPermissions.GroupRead)
					? "r"
					: "-");
				propertiesMessage.Append(permitions.FileAccessPermissions.HasFlag(FileAccessPermissions.GroupWrite)
					? "w"
					: "-");
				propertiesMessage.Append(permitions.FileAccessPermissions.HasFlag(FileAccessPermissions.GroupExecute)
					? "x "
					: "- ");
				
				// World
				propertiesMessage.Append(permitions.FileAccessPermissions.HasFlag(FileAccessPermissions.OtherRead)
					? "r"
					: "-");
				propertiesMessage.Append(permitions.FileAccessPermissions.HasFlag(FileAccessPermissions.OtherWrite)
					? "w"
					: "-");
				propertiesMessage.Append(permitions.FileAccessPermissions.HasFlag(FileAccessPermissions.OtherExecute)
					? "x "
					: "- ");
				MessageBox.Show(propertiesMessage.ToString(), "Свойства");
			}
		}
	}
}
