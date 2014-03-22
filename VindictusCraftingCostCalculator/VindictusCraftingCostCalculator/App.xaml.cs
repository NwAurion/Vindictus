using System.Reflection;
using System.Windows;

namespace VindictusCraftingCostCalculator
{
	/// <summary>
	/// Interaktionslogik für "App.xaml"
	/// </summary>
	public partial class App : Application
	{
		public App()
		{
			InitializeComponent();
		}

		public string FullVersionString
		{
			get { return "Vindictus Crafting Cost Calculatur v"+Assembly.GetExecutingAssembly().GetName().Version.ToString(); }
		}

	}
}
