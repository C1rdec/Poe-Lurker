//-----------------------------------------------------------------------
// <copyright file="SocketParser.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Parser
{
    using Lurker.Models;
    using System;
    using System.Collections.Generic;

    public class SocketParser
    {
        #region Methods

        /// <summary>
        /// Parses the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The Sockets.</returns>
        public IEnumerable<Socket> Parse(string value)
        {
            var sockets = new List<Socket>();
            if (value == null)
            {
                return sockets;
            }

            var isColor = true;
            Socket currentSocket = default;
            foreach (var character in value)
            {
                if (isColor)
                {
                    currentSocket = new Socket()
                    {
                        Color = (SocketColor)Enum.ToObject(typeof(SocketColor), character)
                    };

                    sockets.Add(currentSocket);
                }
                else
                {
                    if (character == '-')
                    {
                        currentSocket.Linked = true;
                    }
                }

                isColor = !isColor;
            }


            return sockets;
        }

        #endregion
    }
}
