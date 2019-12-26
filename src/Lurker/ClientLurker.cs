//-----------------------------------------------------------------------
// <copyright file="ClientLurker.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker
{
    using Lurker.Events;
    using System;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines a file Watcher for the Client log file.
    /// </summary>
    public class ClientLurker : IDisposable
    {
        #region Fields

        private bool _lurking;
        private FileInfo _fileInformation;
        private DateTime _lastWriteTime;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientWatcher"/> class.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        public ClientLurker(string filePath)
        {
            this.FilePath = filePath;
            this._fileInformation = new FileInfo(filePath);
            this._lastWriteTime = this._fileInformation.LastWriteTimeUtc;
            this.Lurk();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the file path.
        /// </summary>
        private string FilePath { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the player changed the location.
        /// </summary>
        public event EventHandler<LocationChangedEvent> LocationChanged;

        /// <summary>
        /// Occurs when a trade is accepted.
        /// </summary>
        public event EventHandler<TradeAcceptedEvent> TradeAccepted;

        /// <summary>
        /// Occurs when the players ask the remaining monsters count[remaining monters].
        /// </summary>
        public event EventHandler<MonstersRemainEvent> RemainingMonsters;

        /// <summary>
        /// Occurs when a player join/leave an area.
        /// </summary>
        public event EventHandler<PlayerJoinedEvent> PlayerJoined;

        /// <summary>
        /// Occurs when [player left].
        /// </summary>
        public event EventHandler<PlayerLeftEvent> PlayerLeft;

        /// <summary>
        /// Occurs when [whispered].
        /// </summary>
        public event EventHandler<WhisperEvent> Whispered;


        /// <summary>
        /// Creates new offer.
        /// </summary>
        public event EventHandler<TradeEvent> NewOffer;

        #endregion

        #region Methods

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this._lurking = false;
            }
        }

        /// <summary>
        /// Lurks this instance.
        /// </summary>
        private async void Lurk()
        {
            this._lurking = true;
            while (this._lurking)
            {
                do
                {
                    await Task.Delay(100);
                    this._fileInformation.Refresh();
                }
                while (this._fileInformation.LastWriteTimeUtc == this._lastWriteTime);

                this._lastWriteTime = this._fileInformation.LastAccessTimeUtc;
                this.OnFileChanged();
            }
        }

        /// <summary>
        /// Called when [file changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="FileSystemEventArgs"/> instance containing the event data.</param>
        private void OnFileChanged()
        {
            var newline = ReadLastLineFromUTF8EncodedFile(this.FilePath);
            if (string.IsNullOrEmpty(newline))
            {
                return;
            }

            // TradeEvent need to be parse before whisper
            var tradeEvent = TradeEvent.TryParse(newline);
            if (tradeEvent != null)
            {
                this.NewOffer?.Invoke(this, tradeEvent);
                return;
            }

            var whisperEvent = WhisperEvent.TryParse(newline);
            if (whisperEvent != null)
            {
                this.Whispered?.Invoke(this, whisperEvent);
                return;
            }

            var locationEvent = LocationChangedEvent.TryParse(newline);
            if (locationEvent != null)
            {
                this.LocationChanged?.Invoke(this, locationEvent);
                return;
            }

            var tradeAcceptedEvent = TradeAcceptedEvent.TryParse(newline);
            if (tradeEvent != null)
            {
                this.TradeAccepted?.Invoke(this, tradeAcceptedEvent);
                return;
            }

            var monsterEvent = MonstersRemainEvent.TryParse(newline);
            if (monsterEvent != null)
            {
                this.RemainingMonsters?.Invoke(this, monsterEvent);
                return;
            }

            var playerJoinEvent = PlayerJoinedEvent.TryParse(newline);
            if (playerJoinEvent != null)
            {
                this.PlayerJoined?.Invoke(this, playerJoinEvent);
                return;
            }

            var playerLeftEvent = PlayerLeftEvent.TryParse(newline);
            if (playerLeftEvent != null)
            {
                this.PlayerLeft?.Invoke(this, playerLeftEvent);
                return;
            }
        }

        /// <summary>
        /// Reads the last line from ut f8 encoded file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        /// <exception cref="System.IO.IOException">Error reading from file at " + path</exception>
        public static string ReadLastLineFromUTF8EncodedFile(string path)
        {
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                if (stream.Length == 0)
                {
                    return null;
                }

                // start at end of file
                stream.Position = stream.Length - 1;

                // the file must end with a '\n' char, if not a partial line write is in progress
                int byteFromFile = stream.ReadByte();
                if (byteFromFile != '\n')
                {
                    // partial line write in progress, do not return the line yet
                    return null;
                }

                // move back to the new line byte - the loop will decrement position again to get to the byte before it
                stream.Position--;

                // while we have not yet reached start of file, read bytes backwards until '\n' byte is hit
                while (stream.Position > 0)
                {
                    stream.Position--;
                    byteFromFile = stream.ReadByte();
                    if (byteFromFile < 0)
                    {
                        // the only way this should happen is if someone truncates the file out from underneath us while we are reading backwards
                        throw new IOException("Error reading from file at " + path);
                    }
                    else if (byteFromFile == '\n')
                    {
                        // we found the new line, break out, fs.Position is one after the '\n' char
                        break;
                    }

                    stream.Position--;
                }

                // fs.Position will be right after the '\n' char or position 0 if no '\n' char
                var bytes = new BinaryReader(stream).ReadBytes((int)(stream.Length - stream.Position));
                return Encoding.UTF8.GetString(bytes).Replace(System.Environment.NewLine, string.Empty);
            }
        }

        #endregion
    }
}
