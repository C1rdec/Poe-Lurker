using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Lurker.Services
{
    class SocketMessageService
    {
        private int _messageByteSize;
        private TcpListener _tcpListener;
        private CancellationTokenSource _cancellationTokenSource;
        private CancellationToken _cancellationToken;
        private Task _listeningTask;
        private ManualResetEvent _portSetEvent;

        public event EventHandler<SocketMessageReceivedEventArgs> SocketMessageReceived;

        public int Port { get; private set; }
        public bool IsListening => _listeningTask != null && !_listeningTask.IsCompleted;

        public SocketMessageService(int messageByteSize)
        {
            _messageByteSize = messageByteSize;
        }

        public void StartListening()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;
            _portSetEvent = new ManualResetEvent(false);
            _listeningTask = Task.Run(() =>
            {
                _tcpListener = new TcpListener(IPAddress.Loopback, 0);
                _tcpListener.Start(1);
                Port = ((IPEndPoint)_tcpListener.LocalEndpoint).Port;
                _portSetEvent.Set();
                try
                {
                    var bytes = new byte[this._messageByteSize];
                    using var client = _tcpListener.AcceptTcpClient();
                    using var stream = client.GetStream();
                    int bytecount, offset = 0;
                    while ((bytecount = stream.Read(bytes, offset, bytes.Length - offset)) != 0)
                    {
                        if (bytecount + offset == _messageByteSize)
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

                        if (_cancellationToken.IsCancellationRequested)
                        {
                            break;
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
            _portSetEvent.WaitOne();
        }

        public void Stop()
        {
            if (_tcpListener != null)
            {
                _cancellationTokenSource.Cancel();
                _tcpListener.Stop();
                _portSetEvent.Reset();
            }

            Port = 0;
        }
    }

    public class SocketMessageReceivedEventArgs : EventArgs
    {
        public byte[] MessageBytes;
    }
}
