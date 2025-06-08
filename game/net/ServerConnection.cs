using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace ShGame.Game.Net;

internal class ServerConnection {
    private readonly Socket socket;
    private bool stop = false;
    private readonly Logger logger;
    internal int disposalCooldown = 100;
    internal readonly int id;

    internal ServerConnection(Socket s, GameServer gs, int id_) {
        socket = s;
        logger = new Logger(new LoggingLevel("ServerConnection"));
        logger.Log("Constructor");
        id = id_;
        Task.Run(() => Run(gs));
    }

    private bool ReceivePacket(ref byte[] buffer) {
        if (!socket.Connected)
            return false;

        Array.Resize(ref buffer, Protocoll.PACKET_BYTE_LENGTH);
        buffer.Initialize();

        int received = 0;
        while (received < Protocoll.PACKET_BYTE_LENGTH && !stop) {
            int bytes;
            try {
                bytes = socket.Receive(buffer, received, Protocoll.PACKET_BYTE_LENGTH - received, SocketFlags.None);
            } catch (Exception) {
                return false;
            }
            if (bytes == 0)
                break;

            received += bytes;
        }
        return received >= Protocoll.PACKET_BYTE_LENGTH;
    }

    private void SendPacket(byte[]? send) {
        if (send == null)
            return;
        _ = socket.Send(send);
    }

    private void Run(GameServer gs) {
        logger.Log("run");
        byte[] buffer = new byte[Protocoll.PACKET_BYTE_LENGTH];
        while (!stop) {
            try {
                if (!ReceivePacket(ref buffer))
                    continue;

                switch (Protocoll.AnalyzePacket(buffer)) {
					case Headers.BULLET:
						SendPacket(gs.OnBulletRequest(buffer));
                        break;
                    case Headers.PLAYER:
                        SendPacket(gs.OnExchangePlayerRequest(buffer));
                        break;
                    case Headers.PING:
                        SendPacket(gs.OnPingRequest(buffer));
                        break;
                    case Headers.MAP:
                        SendPacket(gs.OnMapRequest());
						break;
                    case Headers.REGISTER_PLAYER:
                        SendPacket(gs.OnRegisterPlayerRequest(buffer));
                        break;
					default:
                        Console.WriteLine("[ServerConnection]: Unknown protocol type (protocol.type=" + Protocoll.AnalyzePacket(buffer) + ")");
                        break;
                }
            } catch (SocketException e) {
                logger.Error(e.Message);
                disposalCooldown--;
                break;
            }
        }
    }

    public override string? ToString() => socket.ToString();

    internal void Stop() {
        logger.Log("stopping");
        stop = true;
        socket.Close();
        socket.Dispose();
    }
}