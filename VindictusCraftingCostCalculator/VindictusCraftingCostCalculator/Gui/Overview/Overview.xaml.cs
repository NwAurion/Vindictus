using System;
using System.Windows;
using System.Windows.Controls;
using VindictusCraftingCostCalculator.Items;

namespace VindictusCraftingCostCalculator.Gui.Overview
{
	/// <summary>
	/// Interaktionslogik für Overview.xaml
	/// </summary>
	public partial class Overview : Grid
	{
		public Overview()
		{
			InitializeComponent();
		}

		private void SpinnerControl_ValueChanged(object sender, RoutedPropertyChangedEventArgs<decimal> e)
		{
			SpinnerControl sc = sender as SpinnerControl;
			((CraftingMaterial)sc.Tag).Cost = Convert.ToDouble(e.NewValue);
		}


	}
}
