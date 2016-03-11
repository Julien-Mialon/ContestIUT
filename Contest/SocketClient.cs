using System;
using System.Net.Sockets;
using System.Text;

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
				_client.Connect(_host, _port);
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

		public int ReadInt()
		{
			const int readSize = sizeof(int);

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
			return Encoding.ASCII.GetString(size, 0, readSize);
		}

		protected void Write(byte[] packet, int offset, int count)
		{
			_stream.Write(packet, offset, count);
			_stream.Flush();
		}

	    public void WriteString(string message)
	    {
		    byte[] content = Encoding.ASCII.GetBytes(message);

		    Write(content, 0, content.Length);
	    }
	}
}
