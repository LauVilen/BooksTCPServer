using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace BooksTCPServer
{
    class TCPServer
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Server start");

            TcpListener listener = new TcpListener(IPAddress.Any, 4646);
            listener.Start();

            while (true)
            {
                TcpClient socket = listener.AcceptTcpClient();
                Console.WriteLine("Incoming client...");
                Task.Run(() => { ClientHandler.HandleClient(socket); });
            }

        }

    }
}
