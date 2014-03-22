using System.Collections.ObjectModel;

namespace VindictusCraftingCostCalculator.Items
{
	public class ItemRecipe
	{
		public string ImageUrl { get; set; }
		public string ItemName { get; set; }
		public double Cost { get; set; }

		//private string _imageUrl;
		//public string ImageUrl
		//{
		//    get { return _imageUrl; }
		//    set { _imageUrl = value; }
		//}

		//private string _itemName;
		//public string ItemName
		//{
		//    get { return _itemName; }
		//    set { _itemName = value; }
		//}

		//private double _cost;
		//public double Cost
		//{
		//    get { return _cost; }
		//    set { _cost = value; }
		//}

		public int EnhancementLevel { get; set; }

		private ObservableCollection<CraftingMaterial> _recipeMaterials = new ObservableCollection<CraftingMaterial>();
		public ObservableCollection<CraftingMaterial> RecipeMaterials
		{
			get { return _recipeMaterials; }
			set { _recipeMaterials = value; }
		}




	}
}
