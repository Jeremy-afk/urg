import socket
import subprocess
import threading
import random
import string

HOST = '127.0.0.1'
LISTEN_PORT = 7777 # Port d'écoute où tourne notre script python pour gérer les connexions des joueurs
SERVER_BASE_PORT = 7778 # Le port de base 
SERVER_EXECUTABLE = "C:/Users/STEEVEN/Desktop/ServerBuild/URG.exe"

server_instances = []

is_running = True


class Server:
    def __init__(self, process, port, sessionCode):
        self.process = process
        self.port = port
        self.sessionCode = sessionCode
        self.isActive = True
    

def handle_client_connection(client_socket, client_address):
    print(f"Connexion reçue de {client_address}")
    # Démarrer l'instance de serveur Mirror
    instantiate_server()

    server = server_instances[len(server_instances) - 1]

    # Informer le client de l'hôte et du port où se connecter
    response = f"{server.port} {server.sessionCode}"
    client_socket.sendall(response.encode())

    # Fermer la connexion avec le client
    client_socket.close()

def start_listening():
    global is_running
    with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as server_socket:
        server_socket.bind((HOST, LISTEN_PORT))
        server_socket.listen()
        print(f"Serveur Python en écoute sur {HOST}:{LISTEN_PORT}")

        while is_running:
            try:
                client_socket, client_address = server_socket.accept()
                # Gérer chaque client dans un thread séparé
                client_thread = threading.Thread(target=handle_client_connection, args=(client_socket, client_address))
                client_thread.start()
            except OSError:
                # Le socket a été fermé, on sort de la boucle
                break

        print("Arrêt du serveur d'écoute.")

def instantiate_server():
    port = SERVER_BASE_PORT + len(server_instances)
    print(f"Lancement du serveur sur le port {port}...")
    try:
        process = subprocess.Popen([SERVER_EXECUTABLE, "--args", "-port", str(port)])
        session_code = ''.join(random.choices(string.ascii_uppercase + string.digits, k=8))
        server_instantiated = Server(process=process, port=port, sessionCode=session_code)
        server_instances.append(server_instantiated)
        print(f"Instance de serveur démarrée sur le port {port}")
    except Exception as e:
        print(f"Erreur lors du démarrage du serveur : {e}")

def stop_all_servers():
    print("Arrêt de tous les serveurs...")
    for port, process in server_instances:
        print(f"Arrêt du serveur sur le port {port}...")
        process.terminate()  # Arrête le processus proprement
        process.wait()       # Attend que le processus se termine
    server_instances.clear()
    print("Tous les serveurs ont été arrêtés.")

if __name__ == "__main__":
    # Lancer le serveur et le moniteur de commande dans des threads séparés
    try:
        start_listening()
    finally:
        stop_all_servers()
        print("Programme terminé.")


