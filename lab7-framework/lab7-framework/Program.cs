using System.Net.Mime;
using System.Windows.Forms;

namespace lab7_framework
{
	internal class Program
	{
		public static void Main(string[] args)
		{
			var f = new BasicForm();
			f.Show();
			Application.Run(f);
		}
	}
}
