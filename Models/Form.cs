using System.Collections.Generic;

namespace Safecheck.Models
{
    public class Form
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public List<Section> sections {get;}

        public Form()
        {
            sections = new List<Section>();

        }


        public void addSection(Section s)
        {
            sections.Add(s);
        }

    }
}