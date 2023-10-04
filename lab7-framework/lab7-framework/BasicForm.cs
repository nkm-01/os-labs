using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace lab7_framework
{
	public class BasicForm : Form
	{
		public BasicForm()
		{
			Width  = 1024;
			Height = 600;

			_openFileDialog             = new OpenFileDialog();
			_openFileDialog.Multiselect = false;

			_chunkViewer = new ListBox();
			_chunkViewer.Anchor = AnchorStyles.Bottom
								  | AnchorStyles.Top
								  | AnchorStyles.Left
								  | AnchorStyles.Right;
			_chunkViewer.Left   = 10;
			_chunkViewer.Top    = 10;
			_chunkViewer.Width  = Width - 20 - 300 - 20;
			_chunkViewer.Height = Height - 20;
			Controls.Add(_chunkViewer);

			_openButton        = new Button();
			_openButton.Width  = 150;
			_openButton.Top    = 10;
			_openButton.Left   = _chunkViewer.Right + 10;
			_openButton.Height = 40;
			_openButton.Text   = "Открыть...";
			_openButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			_openButton.Click += (object sender, EventArgs e) =>
			{
				var result = _openFileDialog.ShowDialog();
				if (result == DialogResult.OK)
				{
					var filename = _openFileDialog.FileName;
					DivideToChunks(filename);
				}
			};
			Controls.Add(_openButton);
			
			_blockSizeLabel        = new Label();
			_blockSizeLabel.Width  = 150;
			_blockSizeLabel.Top    = _openButton.Bottom + 10;
			_blockSizeLabel.Left   = _chunkViewer.Right + 10;
			_blockSizeLabel.Height = 40;
			_blockSizeLabel.Text   = "Размер блока";
			_blockSizeLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			Controls.Add(_blockSizeLabel);
			
			_blockSizeUpDown       = new NumericUpDown();
			_blockSizeUpDown.Width = 150;
			_blockSizeUpDown.Left  = _blockSizeLabel.Right;
			_blockSizeUpDown.Top   = _blockSizeLabel.Top;
			_blockSizeUpDown.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			Controls.Add(_blockSizeUpDown);

			_searchBox        = new TextBox();
			_searchBox.Width  = 300;
			_searchBox.Top    = _blockSizeUpDown.Bottom + 10;
			_searchBox.Left   = _chunkViewer.Right + 10;
			_searchBox.Height = 40;
			_searchBox.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			Controls.Add(_searchBox);
			
			_searchButton        =  new Button();
			_searchButton.Width  =  150;
			_searchButton.Text   =  "Найти";
			_searchButton.Top    =  _searchBox.Bottom + 10;
			_searchButton.Left   =  _chunkViewer.Right + 10;
			_searchButton.Height =  40;
			_searchButton.Anchor =  AnchorStyles.Top | AnchorStyles.Right;
			_searchButton.Click  += (_1, _2) => Search(_searchBox.Text);
			Controls.Add(_searchButton);
		}

		private void DivideToChunks(string filename)
		{
			_chunks = new List<string>();
			var text = File.ReadAllText(filename);
			var chunkSize = Convert.ToInt32(_blockSizeUpDown.Value);
			for (int i = 0; i < text.Length; i += chunkSize)
			{
				var chunk = new StringBuilder();
				for (int j = i; j < Math.Min(i + chunkSize, text.Length); j++)
				{
					chunk.Append(text[j] == '\n' ? ' ' : text[j]);
				}
				_chunks.Add(chunk.ToString());
			}
			Search("");
		}

		private void Search(string pattern)
		{
			_chunkViewer.Items.Clear();
			foreach (var chunk in _chunks)
			{
				if (string.IsNullOrWhiteSpace(pattern)
					|| chunk.Contains(pattern)
					)
					_chunkViewer.Items.Add(chunk);
			}
		}
		
		private OpenFileDialog _openFileDialog;
		private Button         _openButton;
		private Label          _blockSizeLabel;
		private NumericUpDown  _blockSizeUpDown;
		private ListBox        _chunkViewer;
		private TextBox        _searchBox;
		private Button         _searchButton;

		private List<string> _chunks;
	}
}

