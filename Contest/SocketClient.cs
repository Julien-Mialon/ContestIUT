using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Contest
{
    public class SocketClient
    {
	    private readonly string _host;
	    private readonly int _port;
	    private readonly TcpClient _client;

	    private NetworkStream _stream;

	    public SocketClient(string host, int port)
	    {
		    _host = host;
		    _port = port;

		    _client = new TcpClient();
	    }

	    public bool ConnectAsync()
	    {
		    try
		    {
			    ManualResetEvent e = new ManualResetEvent(false);

				_client.ConnectAsync(_host, _port).ContinueWith(x =>
				{
					e.Set();
				});

			    e.WaitOne();
			    if (_client.Connected)
			    {
				    _stream = _client.GetStream();
				    return true;
			    }
			    return false;
		    }
		    catch (Exception)
		    {
			    return false;
		    }
	    }

		protected int Read(byte[] storage, int offset, int length)
		{
			int readCount = 0;
			int iterationCount;
			do
			{
				iterationCount = _stream.Read(storage, offset + readCount, length - readCount);
				Logger.Debug($"\tRead count for iteration : {iterationCount}");
				if (iterationCount <= 0)
				{
					break;
				}
				readCount += iterationCount;
			} while (iterationCount != 0 && readCount < length);

			return readCount;
		}

	    public char ReadChar()
	    {
		    const int readSize = 1;

		    byte[] size = new byte[readSize];
		    int readCount = Read(size, 0, readSize);
		    if (readCount < readSize)
		    {
			    Logger.Error($"SocketClient.ReadChar : invalid read count {readCount}");
			    throw new Exception("SocketClient.ReadChar");
		    }

		    return BitConverter.ToChar(size, 0);
	    }

		public uint ReadInt()
		{
			const int readSize = sizeof(uint);

			byte[] size = new byte[readSize];
			int readCount = Read(size, 0, readSize);
			if (readCount < readSize)
			{
				Logger.Error($"SocketClient.ReadInt : invalid read count {readCount}");
				throw new Exception("SocketClient.ReadInt");
			}

			return BitConverter.ToUInt32(size, 0);
		}

	    public float ReadFloat()
	    {
			const int readSize = sizeof(float);
			Logger.Info("Float size : " + readSize);

			byte[] size = new byte[readSize];
			int readCount = Read(size, 0, readSize);
			if (readCount < readSize)
			{
				Logger.Error($"SocketClient.ReadInt : invalid read count {readCount}");
				throw new Exception("SocketClient.ReadInt");
			}

			return BitConverter.ToInt32(size, 0);
		}

	    public string ReadLine()
	    {
		    StringBuilder builder = new StringBuilder();
		    while(true)
		    {
			    var c = ReadChar();
			    if (c == '\n')
			    {
				    return builder.ToString();
			    }
			    builder.Append(c);
		    }
	    }

		public string ReadString(int readSize)
		{
			byte[] size = new byte[readSize];
			int readCount = Read(size, 0, readSize);
			if (readCount < readSize)
			{
				Logger.Error($"SocketClient.ReadString : invalid read count {readCount}");
				throw new Exception("SocketClient.ReadString");
			}

			StringBuilder str = new StringBuilder();
			for (int i = 0; i < readCount; ++i)
			{
				str.Append((char) size[i]);
			}
			return str.ToString();
		}

		protected void Write(byte[] packet, int offset, int count)
		{
			_stream.Write(packet, offset, count);
			_stream.Flush();
		}

	    public void WriteInt8(int n)
	    {
		    byte[] b = BitConverter.GetBytes((char)n);

		    Write(b, b.Length - 1, 1);
	    }

	    public void WriteInt(int n)
	    {
		    byte[] b = BitConverter.GetBytes(n);
			Write(b, 0, b.Length);
	    }

	    public void WriteString(string message)
	    {
			byte[] content = new byte[message.Length];
		    for (int i = 0; i < message.Length; ++i)
		    {
			    content[i] = (byte)message[i];
		    }
			
		    WriteInt(content.Length);
		    Write(content, 0, content.Length);
	    }
	}
}
