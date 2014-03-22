
namespace VindictusCraftingCostCalculator.Items
{
	/// <summary>
	/// Interaktionslogik für CraftingMaterial.xaml
	/// </summary>
	public class CraftingMaterial
	{
		public string ImageUrl { get; set; }
		public string MaterialName { get; set; }
		public double Amount { get; set; }
		public double Cost { get; set; }

		private bool _allowAmountInput = true;
		public bool AllowAmountInput
		{
			get { return _allowAmountInput; }
			set { _allowAmountInput = value; }
		}

		private bool _showFeeTextBlock = false;
		public bool ShowFeeTextBlock
		{
			get { return _showFeeTextBlock; }
			set { _showFeeTextBlock = value; }
		}

		private bool _showAmount = true;
		public bool ShowAmount
		{
			get { return _showAmount; }
			set { _showAmount = value; }
		}


		public CraftingMaterial(string imageUrl, string name, double amount)
		{
			ImageUrl = imageUrl;
			MaterialName = name;
			Amount = amount;
		}

		public CraftingMaterial(string imageUrl, string name, double amount, double cost)
		{
			ImageUrl = imageUrl;
			MaterialName = name;
			Amount = amount;
			Cost = cost;
		}
	}
}
