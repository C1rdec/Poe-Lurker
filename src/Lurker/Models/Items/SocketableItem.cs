//-----------------------------------------------------------------------
// <copyright file="SocketableItem.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

using Lurker.Extensions;
using Lurker.Parsers;
using System.Collections.Generic;
using System.Linq;

namespace Lurker.Models.Items
{
    public abstract class SocketableItem: PoeItem
    {
        #region Fields

        private static readonly string SocketsMarker = "Sockets: ";
        private static readonly SocketParser SocketParser = new SocketParser();

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SocketableItem"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public SocketableItem(string value)
            : base(value)
        {
            this.Sockets = SocketParser.Parse(value.GetLine(SocketsMarker));
            this.SocketCount = this.Sockets.Count();

            var linkedCount = this.Sockets.Count(s => s.Linked);
            this.SocketLinks = linkedCount == 0 ? 0: linkedCount + 1;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the number of sockets.
        /// </summary>
        public IEnumerable<Socket> Sockets { get; private set; }

        /// <summary>
        /// Gets the number of sockets.
        /// </summary>
        public int SocketCount { get; private set; }

        /// <summary>
        /// Gets the socket links.
        /// </summary>
        public int SocketLinks { get; private set; }

        #endregion
    }
}
