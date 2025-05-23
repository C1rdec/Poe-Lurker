﻿//-----------------------------------------------------------------------
// <copyright file="PoeKeyboardHelper.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.Core.Helpers;

using System.Threading.Tasks;

/// <summary>
/// Represents the PoeKeyboardHelper.
/// </summary>
/// <seealso cref="PoeLurker.Core.Helpers.KeyboardHelper" />
public class PoeKeyboardHelper : KeyboardHelper
{
    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="PoeKeyboardHelper" /> class.
    /// </summary>
    /// <param name="processId">The process identifier.</param>
    public PoeKeyboardHelper(int processId)
        : base(processId)
    {
    }

    #endregion

    #region Methods

    /// <summary>
    /// Destroys this instance.
    /// </summary>
    /// <returns>The task.</returns>
    public Task Destroy()
        => SendCommand("/destroy");

    /// <summary>
    /// Remainings the monster.
    /// </summary>
    /// <returns>The task.</returns>
    public Task RemainingMonster()
        => SendCommand("/remaining");

    /// <summary>
    /// Whoes the is.
    /// </summary>
    /// <param name="playerName">Name of the player.</param>
    /// <returns>The task.</returns>
    public Task WhoIs(string playerName)
        => SendCommand($@"/whois {playerName}");

    /// <summary>
    /// Invites to party.
    /// </summary>
    /// <param name="playerName">Name of the player.</param>
    /// <returns>The task.</returns>
    public Task Invite(string playerName)
        => SendCommand($@"/invite {playerName}");

    /// <summary>
    /// Whisper to buyer.
    /// </summary>
    /// <param name="playerName">Name of the player.</param>
    /// <returns>The task.</returns>
    public Task Whisper(string playerName)
        => SendCommand($@"@{playerName} ", true);

    /// <summary>
    /// Kicks the specified player name.
    /// </summary>
    /// <param name="playerName">Name of the player.</param>
    /// <returns>The task.</returns>
    public Task Kick(string playerName)
        => SendCommand($@"/kick {playerName}");

    /// <summary>
    /// Leave the current party.
    /// </summary>
    /// <returns>The task.</returns>
    public Task Leave()
        => SendCommand("/leave");

    /// <summary>
    /// Trades the specified character name.
    /// </summary>
    /// <param name="playerName">Name of the player.</param>
    /// <returns>The task.</returns>
    public Task Trade(string playerName)
        => SendCommand($@"/tradewith {playerName}");

    /// <summary>
    /// Whispers the specified character name.
    /// </summary>
    /// <param name="playerName">Name of the character.</param>
    /// <param name="message">The message.</param>
    /// <returns>The task.</returns>
    public Task Whisper(string playerName, string message)
        => SendCommand($@"@{playerName} {message}");

    /// <summary>
    /// Sends the message.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <returns>The task.</returns>
    public Task SendMessage(string message)
        => SendCommand(message);

    /// <summary>
    /// Sends joni gild Hiedout.
    /// </summary>
    /// <returns>The task.</returns>
    public Task JoinGuildHideout()
        => SendCommand($@"/guild");

    /// <summary>
    /// Joins the hideout.
    /// </summary>
    /// <returns>The task.</returns>
    public Task JoinHideout()
        => JoinHideout(null);

    /// <summary>
    /// Joins the hideout.
    /// </summary>
    /// <param name="playerName">Name of the player.</param>
    /// <returns>The task.</returns>
    public Task JoinHideout(string playerName)
    {
        if (string.IsNullOrEmpty(playerName))
        {
            return SendCommand($@"/hideout");
        }

        return SendCommand($@"/hideout {playerName}");
    }

    #endregion
}