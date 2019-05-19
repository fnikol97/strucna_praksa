using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            TcpListener serverSocket = new TcpListener(ip, 8888);
            TcpClient clientSocket = default(TcpClient);
            serverSocket.Start();
            Console.WriteLine(" *** Server Started *** ");

            while ((true))
            {
                try
                {
                    clientSocket = serverSocket.AcceptTcpClient();
                    Console.WriteLine("Accepted connection from client");
                    HandleClient client = new HandleClient();
                    client.startChat(clientSocket);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
   
    }
    public class HandleClient
    {
        TcpClient clientSocket;
        private void doChat()
        {
            while (true)
            {
                try
                {
                    NetworkStream networkStream = clientSocket.GetStream();
                    byte[] bytesFrom = new byte[10025];
                    networkStream.Read(bytesFrom, 0, bytesFrom.Length);
                    string dataFromClient = System.Text.Encoding.ASCII.GetString(bytesFrom);
                    dataFromClient = dataFromClient.Substring(0, dataFromClient.IndexOf('\0'));
                    Console.WriteLine("Data from client - " + dataFromClient);
                    networkStream.Flush();
                }
                catch (Exception ex)
                {
                    clientSocket.Close();
                }
            }
        }
        public void startChat(TcpClient clientSocket)
        {
            this.clientSocket = clientSocket;
            Thread ctThread = new Thread(doChat);
            ctThread.Start();
        }
    }
}
