using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
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
            Console.WriteLine("Connection to server successful!\nEnter your nick: ");
            while (true)
            {
                NetworkStream serverStream = clientSocket.GetStream();
                byte[] outStream = Encoding.ASCII.GetBytes(Console.ReadLine());
                serverStream.Write(outStream, 0, outStream.Length);
                serverStream.Flush();

                handleServer server = new handleServer();
                server.startChat(clientSocket);

            }
        }
    }
    public class handleServer
    {
        TcpClient clientSocket;
        private void getMessage()
        {
            while (true)
            {
                NetworkStream serverStream = clientSocket.GetStream();
                byte[] inStream = new byte[10025];
                serverStream.Read(inStream, 0, inStream.Length);
                string msg = Encoding.ASCII.GetString(inStream);
                msg = msg.Substring(0, msg.IndexOf('\0'));
                Console.WriteLine(msg);
                serverStream.Flush();
            }
        }
        public void startChat(TcpClient clientSocket)
        {
            this.clientSocket = clientSocket;
            Thread ctThread = new Thread(getMessage);
            ctThread.Start();

        }
    }
}
