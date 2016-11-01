using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using VindictusCraftingCostCalculator.Items;
using VindictusCraftingCostCalculator.Utils;

namespace VindictusCraftingCostCalculator
{
	/// <summary>
	/// Interaktionslogik für MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{

		private CustomWebClient webClient = new CustomWebClient();
		public static bool IsSet;

		public MainWindow()
		{
			this.DataContext = this;
			InitializeComponent();
			webClient.Materials.CollectionChanged += delegate
			{
				Height = 70 * webClient.Materials.Count;
				UpdateLayout();
			};
			Loaded += delegate { itemNameTextBox.Focus(); };
		}

		public ObservableCollection<CraftingMaterial> Materials
		{
			get { return (ObservableCollection<CraftingMaterial>)GetValue(MaterialsProperty); }
			set { SetValue(MaterialsProperty, value); }
		}

		// Using a DependencyProperty as the backing store for Materials.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty MaterialsProperty =
			DependencyProperty.Register("Materials", typeof(ObservableCollection<CraftingMaterial>), typeof(MainWindow), new UIPropertyMetadata(new ObservableCollection<CraftingMaterial>()));



		public ObservableCollection<ItemRecipe> ItemRecipes
		{
			get { return (ObservableCollection<ItemRecipe>)GetValue(ItemRecipesProperty); }
			set { SetValue(ItemRecipesProperty, value); }
		}

		// Using a DependencyProperty as the backing store for ItemRecipes.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty ItemRecipesProperty =
			DependencyProperty.Register("ItemRecipes", typeof(ObservableCollection<ItemRecipe>), typeof(MainWindow), new UIPropertyMetadata(new ObservableCollection<ItemRecipe>()));



		/// <summary>
		/// This collection stores the crafting materials and item recipes that get displayed after calculating
		/// </summary>
		public ObservableCollection<Object> Results
		{
			get { return (ObservableCollection<Object>)GetValue(ResultsProperty); }
			set { SetValue(ResultsProperty, value); }
		}

		// Using a DependencyProperty as the backing store for Results.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty ResultsProperty =
			DependencyProperty.Register("Results", typeof(ObservableCollection<Object>), typeof(MainWindow), new UIPropertyMetadata(new ObservableCollection<Object>()));


		/// <summary>
		/// The total cost of a item
		/// </summary>
		public double TotalCost
		{
			get { return (double)GetValue(TotalCostProperty); }
			set { SetValue(TotalCostProperty, value); }
		}

		// Using a DependencyProperty as the backing store for TotalCost.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty TotalCostProperty =
			DependencyProperty.Register("TotalCost", typeof(double), typeof(MainWindow), new UIPropertyMetadata(0d));


		public int AmountOfStones { get; set; }
		public int CostOfStones { get; set; }

		/// <summary>
		/// Handler for key down event on the textbox with the item name
		/// </summary>
		private void itemNameTextBox_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				if (itemNameTextBox.Text.IsNotEmpty())
				{
					SearchForItem(itemNameTextBox.Text);
					e.Handled = true;
				}
				else
					MessageBox.Show("Please enter an item", "Empty item name");
			}
		}

		/// <summary>
		/// Formats the search strin and calls the webClient
		/// </summary>
		private void SearchForItem(string searchString)
		{
			bool item_found = false;
			searchString = searchString.Replace(" ", "+");

			if (searchString.EndsWith("Set", StringComparison.InvariantCultureIgnoreCase))
				IsSet = true;
			else
				IsSet = false;

			Materials.Clear();
			ItemRecipes.Clear();

			if (IsSet)
			{
				string url = webClient.FindSetURL(searchString, out item_found);
                Materials = webClient.ReadSetFromDB(url);
				ItemRecipes = webClient.ItemRecipes;
			}
			else
				Materials = webClient.ReadItemFromDB(searchString, out item_found);

			if (!item_found)
				MessageBox.Show("Could not find \"" + searchString + "\"", "Item not found");
		}


		/// <summary>
		/// Handler for the calculate button. Adds up the cost and adds the resulting crafting materials to the result list
		/// </summary>
		private void btCalculate_Click(object sender, RoutedEventArgs e)
		{
			Results.Clear();
			double totalCost = 0;

			if (IsSet)
			{
				foreach (ItemRecipe recipe in ItemRecipes)
				{
					foreach (CraftingMaterial material in recipe.RecipeMaterials)
					{
						CraftingMaterial tempMaterial = Materials.FirstOrDefault(mat => mat.MaterialName == material.MaterialName);
						material.Cost = tempMaterial.Cost * material.Amount;
						if (material.MaterialName == "Fee")
						{
							material.Cost = material.Amount;
							material.Amount = 1;
						}
						recipe.Cost += material.Cost;
					}
					TotalCost += recipe.Cost;
				}
				ItemRecipes.ToList<ItemRecipe>().ForEach(item => Results.Add(item));

				viewSingleItem.Visibility = Visibility.Collapsed;
				viewSet.Visibility = Visibility.Visible;
			}
			else
			{
				foreach (CraftingMaterial material in Materials)
				{
					if (material.MaterialName == "Fee")
					{
						material.Cost = material.Amount;
						material.Amount = 1;
					}
					totalCost += material.Cost * material.Amount;
					CraftingMaterial materialCost = new CraftingMaterial(material.ImageUrl, material.MaterialName, material.Amount, material.Amount * material.Cost);
					materialCost.ShowFeeTextBlock = true;
					Results.Add(materialCost);
				}
				CraftingMaterial materialTotalCost = new CraftingMaterial("Icons/Icon_Gold.png", "Total cost", 1, totalCost);
				materialTotalCost.ShowFeeTextBlock = true;
				Results.Add(materialTotalCost);

				viewSet.Visibility = Visibility.Collapsed;
				viewSingleItem.Visibility = Visibility.Visible;
			}
			e.Handled = true;
		}


		private void btSearch_Click(object sender, RoutedEventArgs e)
		{
			if (itemNameTextBox.Text.IsNotEmpty())
				SearchForItem(itemNameTextBox.Text);
			e.Handled = true;
		}
	}
}
