using System.Collections.Generic;

namespace Safecheck.Models
{
    public class Section
    {
        public string name {get; set;}
        public bool repeatable {get; set;}

        public List<Item> items {get;}


        public Section()
        {
            items = new List<Item>();
        }


        public void addItem(Item i)
        {
                items.Add(i);
        }


    }
}