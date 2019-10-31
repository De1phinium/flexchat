using System.Net.Sockets;
using System.IO;
using System.Collections.Generic;
using System;

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

        List<tRequest> requests;
        private ushort lastId = 0;

        public Network(string host, int port)
        {
            tcpClient = new TcpClient();
            tcpClient.Connect(host, port);
            netStream = tcpClient.GetStream();
            requests = new List<tRequest>();
        }

        public ushort SendData(string data)
        {
            tRequest t;
            lastId++;
            if (lastId == maxLastRequestId) lastId = 0;
            t.id = lastId;
            t.respond = null;
            data = Convert.ToString((char)(t.id / 256)) + Convert.ToString((char)(t.id % 256)) + data;
            requests.Add(t);
            StreamWriter writer = new StreamWriter(netStream);
            writer.WriteLine(data);
            writer.Flush();
            writer.Close();
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
            tcpClient.Close();
        }
    }
}
