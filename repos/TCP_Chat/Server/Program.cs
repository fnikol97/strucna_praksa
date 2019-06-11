using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Collections;

namespace Server
{
    class Program
    {
        public static Hashtable clientsList = new Hashtable();
        static void Main(string[] args)
        {
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            TcpListener serverSocket = new TcpListener(ip, 8888);
            TcpClient clientSocket = default(TcpClient);
            serverSocket.Start();
            Console.WriteLine(" *** Server Started *** ");

            while (true)
            {
                try
                {
                    clientSocket = serverSocket.AcceptTcpClient();
                    Console.WriteLine("Accepted connection from client");

                    NetworkStream networkStream = clientSocket.GetStream();
                    byte[] bytesFrom = new byte[10025];
                    networkStream.Read(bytesFrom, 0, bytesFrom.Length);
                    string uName = Encoding.ASCII.GetString(bytesFrom);
                    uName = uName.Substring(0, uName.IndexOf('\0'));
                    Console.WriteLine(uName + " joined the server");
                    networkStream.Flush();

                    clientsList.Add(uName, clientSocket);
                    broadcast(uName + " joined the server");

                    HandleClient client = new HandleClient();
                    client.startChat(clientSocket, uName, clientsList);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
        public static void broadcast (string msg)
        {
            foreach (DictionaryEntry Pair in clientsList)
            {
                TcpClient broadcastSocket;
                broadcastSocket = (TcpClient)Pair.Value;
                NetworkStream broadcastStream = broadcastSocket.GetStream();
                byte[] outStream = new byte[10025];
                outStream = Encoding.ASCII.GetBytes(msg);
                broadcastStream.Write(outStream, 0, outStream.Length);
                broadcastStream.Flush();
            }
        }
    }
    public class HandleClient
    {
        TcpClient clientSocket;
        string uName;
        Hashtable clientsList;
        private void doChat()
        {
            while (true)
            {
                try
                {
                    NetworkStream networkStream = clientSocket.GetStream();
                    byte[] inStream = new byte[10025];
                    networkStream.Read(inStream, 0, inStream.Length);
                    string msg = Encoding.ASCII.GetString(inStream);
                    msg = msg.Substring(0, msg.IndexOf('\0'));
                    Console.WriteLine(uName + ": " + msg);
                    Program.broadcast(uName + ": " + msg);
                    networkStream.Flush();
                   
                }
                catch (Exception ex)
                {
                    clientSocket.Close();
                    clientsList.Remove(uName);
                }
            }
        }
        public void startChat(TcpClient clientSocket, string uName, Hashtable clientsList)
        {
            this.uName = uName;
            this.clientsList = clientsList;
            this.clientSocket = clientSocket;
            Thread ctThread = new Thread(doChat);
            ctThread.Start();
        }
    }
}
