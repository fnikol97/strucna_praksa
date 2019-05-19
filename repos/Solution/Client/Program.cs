using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            TcpClient clientSocket = new TcpClient();
            clientSocket.Connect("127.0.0.1", 8888);
            while (true)
            {
                NetworkStream serverStream = clientSocket.GetStream();
                byte[] outStream = System.Text.Encoding.ASCII.GetBytes(Console.ReadLine());
                serverStream.Write(outStream, 0, outStream.Length);
                serverStream.Flush();
            }
        }
    }
}
