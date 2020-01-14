//-----------------------------------------------------------------------
// <copyright file="QueryResult.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------


namespace Lurker.Models.TradeAPI
{
    using System.Collections.Generic;

    public class QueryResult<T>
    {
        public IEnumerable<T> Result { get; set; }
    }
}
