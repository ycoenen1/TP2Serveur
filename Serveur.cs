using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace TP2Serveur
{
    public class Serveur
    {
        public async Task Demarrer(TcpListener listener)
        {
            Console.WriteLine("Serveur TCP");
            TcpClient client = await listener.AcceptTcpClientAsync();


            Console.WriteLine("Client connecte" + client.Client.RemoteEndPoint);
            Session ce = new Session(client);
            
            Task.Run(() => ce.TraitedClient());

        }

        public async Task Arreter(TcpListener listener)
        {
            listener.Stop();
        }

    }
}
