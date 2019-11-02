using System.Net.Sockets;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;

namespace flexchat
{
    class Network
    {
        const  ushort maxLastRequestId = 65535;

        public struct tRequest
        {
            public ushort id;
            public string respond;
            public uint mode;
        }

        private TcpClient tcpClient;
        private NetworkStream netStream;
        private BinaryReader reader;
        private BinaryWriter writer;

        List<tRequest> requests;
        private ushort lastId = 0;

        public Network(string host, int port)
        {
            tcpClient = new TcpClient();
            tcpClient.Connect(host, port);
            netStream = tcpClient.GetStream();
            requests = new List<tRequest>();
            writer = new BinaryWriter(netStream, Encoding.ASCII, true);
            reader = new BinaryReader(netStream, Encoding.ASCII, true);
        }

        public uint SendData(string data, uint mode)
        {
            tRequest t;
            lastId++;
            if (lastId == maxLastRequestId) lastId = 0;
            t.id = lastId;
            t.respond = null;
            t.mode = mode;
            requests.Add(t);
            data = Convert.ToString(t.id) + Convert.ToString((char)0) + data + Convert.ToString((char)0);
            byte[] send = Encoding.ASCII.GetBytes(data + "\n");
            netStream.Write(send, 0, send.Length);
            netStream.Flush();
            return t.id;
        }

        public void Disconnect()
        {
            netStream.Close();
            tcpClient.Close();
        }

        public void WaitForResponse()
        {
            string data;
            while (!Program.Closed)
            {
                data = "";
                try
                {
                    while (true)
                    {
                        byte b = reader.ReadByte();
                        if (b == '\n') break;
                        else data += (char)b;
                    }
                }
                catch (Exception)
                {
                    continue;
                }
                string s_r_id = "";
                int p = 0;
                while (data[p] != 0)
                {
                    s_r_id += data[p];
                    p++;
                }
                uint r_id = uint.Parse(s_r_id);
                List<tRequest> toDel = new List<tRequest>();
                foreach (tRequest q in requests)
                {
                    if (q.id == r_id)
                    {
                        tRequest t = q;
                        t.respond = data;
                        Program.Resp.Add(t);
                        toDel.Add(q);
                        break;
                    }
                }
                foreach (tRequest q in toDel)
                {
                    requests.Remove(q);
                }
            }
        }
    }
}
