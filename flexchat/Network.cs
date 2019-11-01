using System.Net.Sockets;
using System.Collections.Generic;
using System;
using System.IO;

namespace flexchat
{
    class Network
    {
        const  ushort maxLastRequestId = 65535;

        private struct tRequest
        {
            public ushort id;
            public byte[] respond;
        }

        private TcpClient tcpClient;
        private NetworkStream netStream;
        private StreamWriter writer;

        List<tRequest> requests;
        private ushort lastId = 0;

        public Network(string host, int port)
        {
            tcpClient = new TcpClient();
            tcpClient.Connect(host, port);
            netStream = tcpClient.GetStream();
            requests = new List<tRequest>();
            writer = new StreamWriter(netStream);
        }

        public ushort SendData(string data)
        {
            tRequest t;
            lastId++;
            if (lastId == maxLastRequestId) lastId = 0;
            t.id = lastId;
            t.respond = null;
            data = Convert.ToString(t.id) + Convert.ToString((char)0) + data + Convert.ToString((char)0);
            writer.WriteLine(data + "\n");
            writer.Flush();
            return t.id;
        }

        public bool Responded(ushort request_id)
        {
            foreach (tRequest q in requests)
            {
                if (q.id == request_id && q.respond != null)
                    return true;
            }
            return false;
        }

        public void Disconnect()
        {
            writer.Close();
            tcpClient.Close();
        }
    }
}
