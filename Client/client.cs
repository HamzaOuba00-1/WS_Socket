using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Client
{
    class Client
    {
        static void Main(string[] args)
        {
            
            Socket socketClient = ConnectToServer();

            if (socketClient != null)
            {
                ListenToServer(socketClient);
                Disconnect(socketClient);
            }
        }

        private static Socket ConnectToServer()
        {
            try
            {
                // 1. Création du socket
                Socket socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                // 2. Définition de l'endpoint du serveur (localhost:12345)
                IPAddress ipServeur = IPAddress.Parse("127.0.0.1"); // ou IP réelle du serveur
                int port = 12345;
                IPEndPoint remoteEP = new IPEndPoint(ipServeur, port);

                // 3. Connexion au serveur
                socketClient.Connect(remoteEP);

                Console.WriteLine("Connecté au serveur !");
                return socketClient;
            }
            catch (SocketException e)
            {
                Console.WriteLine("Erreur de connexion : " + e.Message);
                return null;
            }
        }

        private static void ListenToServer(Socket client)
        {
            try
            {
                while (true)
                {
                    Console.Write("Vous: ");
                    string message = Console.ReadLine();

                    if (string.IsNullOrEmpty(message) || message.ToLower() == "exit")
                        break;

                    // Envoi du message
                    byte[] data = Encoding.UTF8.GetBytes(message);
                    client.Send(data);

                    // Réception de la réponse
                    byte[] buffer = new byte[1024];
                    int bytesReceived = client.Receive(buffer);

                    string response = Encoding.UTF8.GetString(buffer, 0, bytesReceived);
                    Console.WriteLine("Serveur: " + response);
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("Erreur réseau : " + e.Message);
            }
        }

        private static void Disconnect(Socket socket)
        {
            if (socket != null && socket.Connected)
            {
                try
                {
                    socket.Shutdown(SocketShutdown.Both);
                }
                catch (SocketException e)
                {
                    Console.WriteLine("Erreur lors de l'arrêt du socket : " + e.Message);
                }
                finally
                {
                    socket.Close();
                    Console.WriteLine("Connexion fermée.");
                }
            }
        }


    }
}
