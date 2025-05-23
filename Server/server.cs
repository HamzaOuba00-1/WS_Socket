using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    class Server
    {
        static void Main(string[] args)
        {
            Socket socketServeur = StartServer();
            Socket socketClient = AcceptConnection(socketServeur);
            ListenToClient(socketClient);
            Disconnect(socketClient);
        }

        private static Socket StartServer()
        {
            // 1. Création du socket
            Socket socketServeur = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // 2. Création de l'adresse IP et du port
            IPAddress ipAdresse = IPAddress.Any; // Accepte toutes les interfaces réseau
            int port = 12345; // Choisis un port libre (> 1024)
            IPEndPoint pointDeConnexion = new IPEndPoint(ipAdresse, port);

            // 3. Association du socket à l'adresse IP et port
            socketServeur.Bind(pointDeConnexion);

            // 4. Mise en écoute du socket
            socketServeur.Listen(10); // 10 connexions max en file d'attente

            Console.WriteLine($"Serveur démarré sur {ipAdresse}:{port} et en attente de connexion!");

            return socketServeur;
        }

        private static Socket AcceptConnection(Socket socket)
        {
            // Attend et accepte une connexion entrante
            Socket clientSocket = socket.Accept();

            // Récupère les informations du client
            IPEndPoint remoteEndPoint = clientSocket.RemoteEndPoint as IPEndPoint;
            string clientIP = remoteEndPoint.Address.ToString();
            int clientPort = remoteEndPoint.Port;

            // Affiche l'adresse IP et le port du client
            Console.WriteLine($"Connexion acceptée : Client {clientIP}:{clientPort}");

            return clientSocket;
        }

        private static void ListenToClient(Socket client)
        {
            try
            {
                byte[] buffer = new byte[1024]; // Buffer pour recevoir les données
                int receivedBytes;

                while (true)
                {
                    // 1. Réception des données
                    receivedBytes = client.Receive(buffer);
                    if (receivedBytes == 0) break; // Connexion terminée

                    // 2. Conversion en string
                    string messageClient = Encoding.UTF8.GetString(buffer, 0, receivedBytes);
                    Console.WriteLine($"Client: {messageClient}");

                    // 3. Réponse du serveur : message en majuscules
                    string messageServeur = messageClient.ToUpper();
                    byte[] dataServeur = Encoding.UTF8.GetBytes(messageServeur);

                    log(messageServeur);

                    client.Send(dataServeur); // Envoi de la réponse
                    Console.WriteLine($"Serveur: {messageServeur}");
                }
            }
            catch (SocketException)
            {
                Console.WriteLine("Connexion perdue avec le client.");
            }
            finally
            {
                client.Close();
                Console.WriteLine("Socket client fermé.");
            }
        }

        private static void log(string message)
        {
            string cheminFichier = Path.Combine(GetLogDirectory(), "log.txt");
            string ligne = $"[{DateTime.Now}] {message}";

            try
            {
                File.AppendAllText(cheminFichier, ligne + Environment.NewLine);
            }
            catch (IOException e)
            {
                Console.WriteLine("Erreur lors de l’écriture dans le fichier de log : " + e.Message);
            }
        }

        private static string GetLogDirectory()
        {
            string logDir = "logs";

            // Si répertoire inexistant, le créer
            if (!Directory.Exists(logDir))
                Directory.CreateDirectory(logDir);

            return logDir;
        }

        private static void Disconnect(Socket socket)
        {
            if (socket != null && socket.Connected)
            {
                try
                {
                    socket.Shutdown(SocketShutdown.Both); // Termine la communication dans les deux sens
                }
                catch (SocketException e)
                {
                    Console.WriteLine("Erreur lors de l’arrêt de la connexion : " + e.Message);
                }
                finally
                {
                    socket.Close(); // Ferme et libère les ressources
                    Console.WriteLine("Connexion client fermée.");
                }
            }
        }




    }
}
