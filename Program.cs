using System.Net.Sockets;

namespace TP2Serveur
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Serveur se = new Serveur();
            TcpClient client = new TcpClient();

            try
            {
                TcpListener listener = new TcpListener(System.Net.IPAddress.Any, 5000);
                do
                {
                    Console.WriteLine("En attente d'un client...");
                    listener.Start();
                    await se.Demarrer(listener);

                } while (true);




            }
            catch (IOException ex)
            {
                Console.WriteLine("Erreur de connexion : " + ex.Message);
                return;
            }
        }
    }
}

