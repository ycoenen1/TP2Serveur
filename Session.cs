using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace TP2Serveur
{
    public class Session
    {
        TcpClient client;
        int nbrCommandes = 0;
        int nbrClients = 0;
        public Session(TcpClient client)
        {
            this.client = client;
            nbrClients++;
        }

        public async Task TraitedClient()
        {
            NetworkStream stream = client.GetStream();
            await stream.WriteAsync(Encoding.UTF8.GetBytes("200 Bienvenue sur ServeurTP v1.0 - tapez AIDE\n"));
            byte[] data = new byte[1024];
            string messageRecu;
            string commande;
            do
            {
                int nbBytes = await stream.ReadAsync(data, 0, data.Length);
                messageRecu = System.Text.Encoding.UTF8.GetString(data, 0, nbBytes).Trim();
                nbrCommandes++;
                commande = messageRecu.Split(' ')[0].ToUpper();
                Console.WriteLine($"Commande reçue : {commande}");
                if (nbBytes == 0)
                {
                    break;
                }
                switch (commande)
                {
                    case "AIDE":
                        await stream.WriteAsync(Encoding.UTF8.GetBytes("200 AIDE : Commandes disponibles : AIDE, ECHO <message>, DATE, HEURE, MAj <message>, CALC <a> <op> <b>, NOM <nom>, QUI, STATS,QUIT\n"));
                        break;
                    case "QUIT":
                        await stream.WriteAsync(Encoding.UTF8.GetBytes("221 Au revoir !\n"));
                        messageRecu = "";
                        break;
                    case "ECHO":
                        await stream.WriteAsync(Encoding.UTF8.GetBytes($"200 ECHO : {messageRecu.Substring(5)}\n"));
                        break;
                    case "DATE":
                        await stream.WriteAsync(Encoding.UTF8.GetBytes($"200 DATE : {DateTime.Now}\n"));
                        break;
                    case "MAj":
                        await stream.WriteAsync(Encoding.UTF8.GetBytes($"200 MAJ : {messageRecu.Substring(5).ToUpper()}\n"));
                        break;
                    case "HEURE":
                        await stream.WriteAsync(Encoding.UTF8.GetBytes($"200 HEURE : {DateTime.Now.ToShortTimeString()}\n"));
                        break;
                    case "CALC":
                        int a, b, result;
                        string op;
                        a = int.Parse(messageRecu.Split(' ')[1]);
                        b = int.Parse(messageRecu.Split(' ')[3]);
                        op = messageRecu.Split(' ')[2];
                        switch (op)
                        {
                            case "+":
                                result = a + b;
                                await stream.WriteAsync(Encoding.UTF8.GetBytes($"200 CALC : {a} + {b} = {result}\n"));
                                break;
                            case "-":
                                result = a - b;
                                await stream.WriteAsync(Encoding.UTF8.GetBytes($"200 CALC : {a} - {b} = {result}\n"));
                                break;
                            case "*":
                                result = a * b;
                                await stream.WriteAsync(Encoding.UTF8.GetBytes($"200 CALC : {a} * {b} = {result}\n"));
                                break;
                            case "/":
                                if (b == 0)
                                {
                                    await stream.WriteAsync(Encoding.UTF8.GetBytes($" 400 CALC : Division par zéro impossible\n"));
                                }
                                else
                                {
                                    result = a / b;
                                    await stream.WriteAsync(Encoding.UTF8.GetBytes($"200 CALC : {a} / {b} = {result}\n"));
                                }
                                break;
                            default:
                                await stream.WriteAsync(Encoding.UTF8.GetBytes($" 400 CALC : Opérateur inconnu\n"));
                                break;
                        }
                        break;
                    case "NOM":
                        await stream.WriteAsync(Encoding.UTF8.GetBytes($"200 NOM : {messageRecu.Split(' ')[1]}\n"));
                        break;
                    case "QUI":
                        await stream.WriteAsync(Encoding.UTF8.GetBytes($"200 QUI : {Environment.MachineName}\n"));
                        break;
                    case "STATS":

                        await stream.WriteAsync(Encoding.UTF8.GetBytes($"200 STATS : clients:{nbrClients} commandes:{nbrCommandes} \n"));
                        break;
                    default:
                        await stream.WriteAsync(Encoding.UTF8.GetBytes($"404 Commande inconnue.\n"));
                        break;
                }
                Console.WriteLine($"Message reçu : {messageRecu}");
                commande = "";
            } while (messageRecu != "");
            Fermer(client, stream);
        }

        public async Task Fermer(TcpClient client, NetworkStream stream)
        {
            client.Close();
            stream.Close();
            nbrClients--;
            Console.WriteLine("Client déconnecté. Nombre de clients connectés : " + nbrClients);
        }
    }
}
