Vindictus
=========

Tools I made for Vindictus. 

Well, actually only one tool, the Vindictus Crafting Cost Calculator. And with me having stopped playing Vindictus some months ago, it's unlikely i'll add more, but who knows.

Anyways, the Vindictus Crafting Cost Calculator allows users to calculate the cost to craft a single piece of equipment or a whole set at once. Prices have to be entered manually, as I can hardly pull them from the game itself even if I knew how.

The Vindictus Crafting Cost Calculator will look up the item on the Vindictus DB( vindictusdb.com ). As they do not supply an API for things like this I had to scrape the actual source of the page returned when doing a regular HTTP request. 

Unfortunately, not all items are formatted the same way. Fortunately, in most cases it was because they did drop the amount needed when only one piece was needed. The other big issue is single item versus full set. 

Unforunately, this is only most cases, not all. By now I don't remember any specifics anymore though.
