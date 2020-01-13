//-----------------------------------------------------------------------
// <copyright file="Boots.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Models
{
    public class Boots : PoeItem
    {
        public Boots(string value) 
            : base(value)
        {
        }

        public override ItemClass ItemClass => ItemClass.Boots;
    }
}
