using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using HtmlAgilityPack;
using VindictusCraftingCostCalculator.Items;

namespace VindictusCraftingCostCalculator
{
    public class CustomWebClient : UserControl, INotifyPropertyChanged
    {
        /// <summary>
        /// The URI to connect to
        /// </summary>
        private string baseURIDB = "http://vindictusdb.com/";
        private string URISet = "search?sw=";
        private string URIRecipe = "recipe?i=";

        /// <summary>
        /// Used to load the website as a html document
        /// </summary>
        private HtmlWeb htmlWeb = new HtmlWeb();

        public ObservableCollection<CraftingMaterial> Materials = new ObservableCollection<CraftingMaterial>();
        public ObservableCollection<ItemRecipe> ItemRecipes = new ObservableCollection<ItemRecipe>();


        /// <summary>
        /// Searches for a set on VindictusDB and returns all the materials needed to craft it
        /// </summary>
        public string FindSetURL(string setName, out bool set_found)
        {
            var set_url = "";

            // clear the list to ensure now old materials interfere
            ItemRecipes.Clear();

            // Well, we have to set it to something..
            set_found = false;

            HtmlDocument document = null;
            try
            {
                string setURL = baseURIDB + URISet + setName;
                document = htmlWeb.Load(setURL);
            }
            catch (WebException e)
            {
                // Prevents the "item not found" message - it's obvious that no item could be found it the request doesn't get through
                set_found = true;
                MessageBox.Show(e.Message, "Something went wrong");
            }

            // If the request got through and there is a page with this set name
            if (document != null && !document.DocumentNode.InnerText.Contains("No results"))
            {
                // We just said that we did find something
                set_found = true;

                string IdToFind = "set_table";
                var set_table = document.DocumentNode.SelectNodes(string.Format("//*[contains(@id,'{0}')]", IdToFind)).First();
                var tbody = set_table.ChildNodes[1];

                var sets = tbody.ChildNodes.Where(n => n.Name == "tr");



                foreach (var set in sets)
                {
                    var linkNode = set.ChildNodes[1];
                    var link = linkNode.ChildNodes[1];
                    var set_name = link.InnerHtml;
                    if (set_name.ToLowerInvariant().Equals(setName.Replace("+", " ").ToLowerInvariant()))
                    {
                        set_url = link.Attributes[0].Value;
                        break;
                    }
                }
            }
            return set_url;
        }

        public ObservableCollection<CraftingMaterial> ReadSetFromDB(string url)
        {
            HtmlDocument document = null;
            try
            {
                document = htmlWeb.Load(url);
            }
            catch (WebException e)
            {
                MessageBox.Show(e.Message, "Something went wrong");
            }


            string classToFind = "browsetb";
            var set_table = document.DocumentNode.SelectNodes(string.Format("//*[contains(@class,'{0}')]", classToFind)).First();
            var tbody = set_table.ChildNodes[3];

            HtmlNode nodeWithSetItems = tbody;
            List<CraftingMaterial> tempMaterialList = new List<CraftingMaterial>();
            // Here we go through each item to extract it's materials. Each row of the table is one item.
            foreach (HtmlNode nodeWithSingleItem in nodeWithSetItems.ChildNodes.Where(n => n.Name == "tr"))
            {
                classToFind = "td-rend";
                var item_info = document.DocumentNode.SelectNodes(string.Format("//*[contains(@class,'{0}')]", classToFind)).First();

                classToFind = "vitem_info_gold";
                var info_gold = document.DocumentNode.SelectNodes(string.Format("//*[contains(@class,'{0}')]", classToFind)).First();

                ItemRecipe setItem = new ItemRecipe();
                // Info about the item itself, like name and the image
                HtmlNode nodeWithItemInfo = item_info.ChildNodes[0].ChildNodes[0];
                string itemNameTemp = nodeWithItemInfo.Attributes[0].Value;
                string itemName = itemNameTemp.Substring(itemNameTemp.IndexOf("=") + 1).Replace("+", " ").Replace("%27", "'");

                HtmlNode nodeWithItemImage = nodeWithItemInfo.ChildNodes[0].ChildNodes[0].ChildNodes[2];
                string itemImageUrl = nodeWithItemImage.Attributes[0].Value;

                // Extract the gold info
                HtmlNode nodeWithGoldCost = info_gold.ChildNodes[0];
                double gold = double.Parse(nodeWithGoldCost.InnerText.Replace(",", "."));

                CraftingMaterial fee = new CraftingMaterial(FindResource("Icon_Gold").ToString(), "Fee", gold);
                fee.ShowAmount = false;
                fee.AllowAmountInput = false;
                fee.ShowFeeTextBlock = true;

                tempMaterialList.Add(fee);
                setItem.RecipeMaterials.Add(fee);

                // This node has all materials for a single item, so we can go through all materials in it
                HtmlNode nodeWithAllMaterialsForItem = nodeWithSingleItem.ChildNodes[7];
                foreach (HtmlNode nodeWithOnlyOneMaterial in nodeWithAllMaterialsForItem.ChildNodes)
                {
                    // The name of the material, e.g. "Superior Iron Ore"
                    string materialNameTemp = nodeWithOnlyOneMaterial.ChildNodes[0].Attributes[0].Value;
                    string materialName = materialNameTemp.Substring(materialNameTemp.IndexOf("=") + 1).Replace("+", " ").Replace("%27", "'"); ;

                    // This node contains both how much of this material need aswell as the image belonging to it
                    HtmlNode nodeWithAmountAndImage = nodeWithOnlyOneMaterial.ChildNodes[0].ChildNodes[0].ChildNodes[0];
                    double materialAmount;

                    HtmlNode materialImageNode;

                    /* If the amount is 1, it's not specified on the website, so we have to handle this case seperately
                     * This also means that there are less nodes, so the node with the image is in a different position */
                    if (nodeWithAmountAndImage.ChildNodes.Count == 1)
                    {
                        materialAmount = 1;
                        materialImageNode = nodeWithAmountAndImage.ChildNodes[0];
                    }
                    else // If the amount is not 1, we can simply parse it
                    {
                        HtmlNode nodeWithMaterialAmount = nodeWithAmountAndImage.ChildNodes[1].ChildNodes[1]; ;
                        materialAmount = double.Parse(nodeWithMaterialAmount.InnerText.Substring(1));
                        materialImageNode = nodeWithAmountAndImage.ChildNodes[2];
                    }

                    // Finally we can set the image url
                    string materialImageUrl = materialImageNode.Attributes[0].Value;

                    // Create a new crafting material with all the info and add it to the list
                    CraftingMaterial craftingMaterial = new CraftingMaterial(materialImageUrl, materialName, materialAmount);
                    tempMaterialList.Add(craftingMaterial);
                    setItem.RecipeMaterials.Add(craftingMaterial);
                }
                setItem.ImageUrl = itemImageUrl;
                setItem.ItemName = itemName;
                ItemRecipes.Add(setItem);
            }

            //Now we clean up a bit. Currently, if several items use the same materials, they are all listed individually. Here we add them up, so we get a nice little list instead
            for (int i = 0; i < tempMaterialList.Count; i++)
            {
                // We need to save how much of each material was needed
                double amount = 0;

                // This is the "reference" material, the others get compared to it
                CraftingMaterial outerMaterial = new CraftingMaterial(tempMaterialList[i].ImageUrl, tempMaterialList[i].MaterialName, tempMaterialList[i].Amount)
                {
                    AllowAmountInput = tempMaterialList[i].AllowAmountInput,
                    ShowAmount = tempMaterialList[i].ShowAmount,
                    ShowFeeTextBlock = tempMaterialList[i].ShowFeeTextBlock
                };
                for (int j = 0; j < tempMaterialList.Count; j++)
                {
                    if (j == i) // same position means same material, we can ignore it
                        continue;

                    // The current material we want to compare to our reference. Same name means same material, so we can add it up
                    CraftingMaterial innerMaterial = tempMaterialList[j];
                    if (innerMaterial.MaterialName == outerMaterial.MaterialName)
                    {
                        amount += innerMaterial.Amount;
                        tempMaterialList.Remove(innerMaterial);
                    }
                }

                // Add up the amount and add the material to the list
                outerMaterial.Amount += amount;
                Materials.Add(outerMaterial);
            }

            // Finally done. Return the list. Note that it's empty if the item was not found
            return Materials;

        }

        /// <summary>
        /// Searches for an item on VindictusDB and returns all the materials needed to craft it
        /// </summary>
        public ObservableCollection<CraftingMaterial> ReadItemFromDB(string itemName, out bool item_found)
        {
            // clear the list to ensure now old materials interfere
            Materials.Clear();

            // Well, we have to set it to something..
            item_found = false;

            HtmlDocument document = null;

            try
            {
                document = htmlWeb.Load(baseURIDB + URIRecipe + itemName);
            }
            catch (WebException e)
            {
                // Actually, we don't know, but this surpressed the "item not found" message.
                item_found = true;
                MessageBox.Show(e.Message, "Something went wrong");
            }

            if (document != null && !document.DocumentNode.InnerText.Contains("Invalid Entry"))
            {
                item_found = true;

                HtmlNode nodeWithGoldCost = document.DocumentNode.SelectSingleNode("/html[1]/body[1]/div[1]/div[4]/div[1]/div[4]/div[2]/table[1]/tbody[1]/tr[1]/td[2]");

                double gold = double.Parse(nodeWithGoldCost.InnerText.Replace(",", "."));
                CraftingMaterial fee = new CraftingMaterial(FindResource("Icon_Gold").ToString(), "Fee", gold);
                fee.ShowAmount = false;
                fee.AllowAmountInput = false;
                fee.ShowFeeTextBlock = true;
                Materials.Add(fee);

                HtmlNode nodeWithAllMaterials = document.DocumentNode.SelectSingleNode("/html[1]/body[1]/div[1]/div[4]/div[1]/div[4]/div[2]/table[1]/tbody[1]/tr[1]/td[3]");
                foreach (HtmlNode childNode in nodeWithAllMaterials.ChildNodes)
                {
                    HtmlNode nodeWithSingleMaterial = childNode.ChildNodes[0];
                    HtmlAttribute materialNameAttribute = nodeWithSingleMaterial.Attributes[0];
                    string materialName = materialNameAttribute.Value.Substring(materialNameAttribute.Value.IndexOf("=") + 1).Replace("+", " ").Replace("%27", "'");


                    HtmlNode subNode = nodeWithSingleMaterial.ChildNodes[0].ChildNodes[0];
                    double materialAmount;

                    HtmlNode materialImageNode;
                    if (subNode.ChildNodes.Count == 1)
                    {
                        materialAmount = 1;
                        materialImageNode = subNode.ChildNodes[0];
                    }
                    else
                    {
                        HtmlNode nodeWithMaterialAmount = subNode.ChildNodes[1].ChildNodes[1];
                        materialAmount = double.Parse(nodeWithMaterialAmount.InnerText.Substring(1));
                        materialImageNode = subNode.ChildNodes[2];
                    }

                    string materialImageUrl = materialImageNode.Attributes[0].Value;

                    CraftingMaterial craftingMaterial = new CraftingMaterial(materialImageUrl, materialName, materialAmount);
                    Materials.Add(craftingMaterial);
                }
            }
            return Materials;

        }


        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string prop)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        /*
        /// <summary>
        /// Tries to get the info from the wiki
        /// </summary>
        public ObservableCollection<CraftingMaterial> ReadItemFromWiki(string itemName, out bool item_found)
        {
            item_found = false;

            Materials.Clear();
            HtmlWeb htmlWeb = new HtmlWeb();

            try
            {
                // Creates an HtmlDocument object from an URL
                HtmlDocument document = htmlWeb.Load(baseURIWiki + itemName);

                // We have to start somewhere, and this is the node that contains the actual content
                HtmlNode npccraft = document.DocumentNode.ChildNodes.FirstOrDefault(n => n.InnerText.Contains("Obtain"));
                if (npccraft != null)
                {
                    item_found = true;
                    // like above, we have to start somewhere. Ugly, but it works.
                    HtmlNode body = npccraft.ChildNodes.FirstOrDefault(n => n.Name == "body");
                    if (body != null)
                    {
                        // we simply continue this way for a bit
                        HtmlNode content = body.ChildNodes.FirstOrDefault(n => n.Id == "content");
                        if (content != null)
                        {
                            // you should know how it works by now. Always trying to get the node with the actual content
                            HtmlNode bodyContent = content.ChildNodes.FirstOrDefault(n => n.Id == "bodyContent");
                            if (bodyContent != null)
                            {
                                // The node we want doesn't have a unique name or id (div is used multiple times), but as the others have an id, it does make it unique again..
                                HtmlNode div = bodyContent.ChildNodes.FirstOrDefault(n => n.Name == "div" && n.Id == "");
                                if (div != null)
                                {
                                    // Here we again want the one with "Obtain", as this contains the table etc. with the content.
                                    HtmlNode obtain = div.ChildNodes.FirstOrDefault(n => n.InnerText.Contains("Obtain"));
                                    if (obtain != null)
                                    {
                                        // Again, multiple div nodes, but we actually want the one that does not contain "Obtain", as this is actually only the header.
                                        HtmlNode obtainSubNode = obtain.ChildNodes.FirstOrDefault(n => n.Name == "div" && !n.InnerText.Contains("Obtain"));
                                        if (obtainSubNode != null)
                                        {
                                            // For single items, the content is in a single table. Item sets work differently, see below, were we ignore the table node.
                                            HtmlNode table = obtainSubNode.ChildNodes.FirstOrDefault(n => n.Name == "table");
                                            if (table != null)
                                            {
                                                // The image urls have to be extracted from the long string with *all* urls
                                                List<string> imageUrlsSplitted;
                                                if (itemName.EndsWith("Set"))
                                                    imageUrlsSplitted = obtainSubNode.InnerHtml.Split(new string[] { "src=\"/w/images/" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                                                else
                                                    imageUrlsSplitted = table.InnerHtml.Split(new string[] { "src=\"/w/images/" }, StringSplitOptions.RemoveEmptyEntries).ToList();

                                                // Sometimes the wiki is not properly organized and has both tables (npc and expertise) in one


                                                // removes unneeded entries like the materials icon
                                                List<string> imageUrls = imageUrlsSplitted.FindAll(str => !str.Contains("Crafted by") && !str.Contains("Material") && str.IsNotEmpty());
                                                int breakPos = imageUrls.Count;
                                                int count = imageUrlsSplitted.Count(str => str.Contains("Material"));
                                                //if (count > 1)
                                                //    breakPos = imageUrlsSplitted.IndexOf(imageUrlsSplitted.Last(str => str.Contains("Material"))) - count;
                                                // Extract the actual url part, removing the html and other stuff
                                                for (int i = 0; i < breakPos; i++)
                                                    imageUrls[i] = imageUrls[i].Substring(0, imageUrls[i].IndexOf(".png") + 4);

                                                imageUrls.RemoveAll(str => imageUrls.IndexOf(str) > breakPos);
                                                HtmlNode tr;
                                                if (itemName.EndsWith("Set"))
                                                    tr = obtainSubNode;
                                                else
                                                    tr = table.ChildNodes.LastOrDefault(n => n.Name == "tr");
                                                if (tr != null)
                                                {
                                                    string innerText = tr.InnerText;
                                                    string innerTextAdjusted = innerText.Replace("\n", "").Replace("&#160", "");
                                                    List<string> materialList = innerTextAdjusted.Split(';').ToList();
                                                    int pos = 0;
                                                    materialList = materialList.FindAll(str => !str.Contains("Crafted by") && str.IsNotEmpty());
                                                    foreach (string materialAsString in materialList)
                                                    {

                                                        string imageurl = imageBaseUrl;
                                                        string[] materialSplitted;
                                                        if (materialAsString.Contains("Fee"))
                                                        {
                                                            pos++;
                                                            materialSplitted = materialAsString.Split(':');
                                                            materialSplitted[1] = materialSplitted[1].Replace(",", "");
                                                            imageurl += imageUrls[0];
                                                        }
                                                        else
                                                        {
                                                            materialSplitted = materialAsString.Split('×');
                                                            imageurl += imageUrls[pos];
                                                        }
                                                        pos++;

                                                        string materialName = materialSplitted[0];
                                                        string materialAmountString = materialSplitted[1];

                                                        double materialAmount;
                                                        double.TryParse(materialAmountString, out materialAmount);
                                                        CraftingMaterial material = new CraftingMaterial(imageurl, materialName, materialAmount);
                                                        if (material.MaterialName == "Fee")
                                                        {
                                                            material.AllowAmountInput = false;
                                                            material.ShowFeeTextBlock = true;
                                                            material.ShowAmount = false;
                                                        }
                                                        Materials.Add(material);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (WebException e)
            {
                MessageBox.Show(e.Message, "Problem connecting to the wiki");
            }

            return Materials;
        }*/
    }
}
