//-----------------------------------------------------------------------
// <copyright file="ExactEnumParserBase.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Parsers
{
    public abstract class ExactEnumParserBase<T>: EnumParserBase<T>
    {
        protected override bool Compare(string value, string dictionnaryValue) => value.ToLower() == dictionnaryValue;
    }
}
