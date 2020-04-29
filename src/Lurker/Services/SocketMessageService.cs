//-----------------------------------------------------------------------
// <copyright file="SocketMessageService.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Services
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;
    using System.Threading.Tasks;

    public class SocketMessageService : IDisposable
    {
        #region Fields

        private int _messageByteSize;
        private TcpListener _tcpListener;
        private CancellationTokenSource _cancellationTokenSource;
        private CancellationToken _cancellationToken;
        private Task _listeningTask;
        private ManualResetEvent _portSetEvent;

        private bool _disposed = false;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SocketMessageService"/> class.
        /// </summary>
        /// <param name="messageByteSize">Size of the message byte.</param>
        public SocketMessageService(int messageByteSize)
        {
            this._messageByteSize = messageByteSize;
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when [socket message received].
        /// </summary>
        public event EventHandler<SocketMessageReceivedEventArgs> SocketMessageReceived;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the port.
        /// </summary>
        public int Port { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is listening.
        /// </summary>
        public bool IsListening => this._listeningTask != null && !this._listeningTask.IsCompleted;

        #endregion

        #region Methods

        /// <summary>
        /// Starts the listening.
        /// </summary>
        public void StartListening()
        {
            this._cancellationTokenSource = new CancellationTokenSource();
            this._cancellationToken = _cancellationTokenSource.Token;
            this._portSetEvent = new ManualResetEvent(false);
            this._listeningTask = Task.Run(() =>
            {
                this._tcpListener = new TcpListener(IPAddress.Loopback, 0);
                this._tcpListener.Start(1);
                Port = ((IPEndPoint)this._tcpListener.LocalEndpoint).Port;
                this._portSetEvent.Set();

                try
                {
                    var bytes = new byte[this._messageByteSize];
                    using var client = this._tcpListener.AcceptTcpClient();
                    using var stream = client.GetStream();
                    int bytecount, offset = 0;
                    while ((bytecount = stream.Read(bytes, offset, bytes.Length - offset)) != 0)
                    {
                        if (bytecount + offset == this._messageByteSize)
                        {
                            offset = 0;
                            SocketMessageReceived(this, new SocketMessageReceivedEventArgs
                            {
                                MessageBytes = bytes
                            });
                        }
                        else
                        {
                            offset = bytecount;
                        }

                        if (this._cancellationToken.IsCancellationRequested)
                        {
                            break;
                        }
                    }
                }
                catch (System.IO.IOException exception)
                {
                    if (exception.InnerException is SocketException socketException)
                    {
                        // Connection gets reset when the hooked client terminates
                        if (socketException.SocketErrorCode != SocketError.ConnectionReset)
                        {
                            throw;
                        }
                    }
                }
                catch (SocketException exception)
                {
                    switch (exception.ErrorCode)
                    {
                        case 10004: // WSACancelBlockingCall
                            break;
                        default:
                            throw;
                    }
                }
            });
            this._portSetEvent.WaitOne();
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop()
        {
            if (this._tcpListener != null)
            {
                this._cancellationTokenSource.Cancel();
                this._tcpListener.Stop();
                this._portSetEvent.Reset();
            }

            Port = 0;
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose() => this.Dispose(true);

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    try
                    {
                        this._listeningTask?.Dispose();
                        this._portSetEvent?.Dispose();
                    }
                    catch
                    {
                    }
                }

                this._disposed = true;
            }
        }

        #endregion
    }

    public class SocketMessageReceivedEventArgs : EventArgs
    {
        public byte[] MessageBytes;
    }
}
