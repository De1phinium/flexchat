﻿using System.Net.Sockets;
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
            public string Inf;
        }

        public SortedSet<int> FilesAsked = new SortedSet<int>();

        private TcpClient tcpClient;
        public NetworkStream netStream;
        private BinaryReader reader;
        private BinaryWriter writer;

        List<tRequest> requests;
        private ushort lastId = 0;

        public Network(string host, int port)
        {
            try
            {
                tcpClient = new TcpClient();
                tcpClient.Connect(host, port);
                netStream = tcpClient.GetStream();
                requests = new List<tRequest>();
                writer = new BinaryWriter(netStream, Encoding.ASCII, true);
                reader = new BinaryReader(netStream, Encoding.ASCII, true);
            }
            catch (Exception)
            {
                Environment.Exit(13);
            }
        }

        public uint SendData(string data, uint mode)
        {
            tRequest t;
            lastId++;
            if (lastId == maxLastRequestId) lastId = 0;
            t.id = lastId;
            t.respond = null;
            t.mode = mode;
            t.Inf = "";
            requests.Add(t);
            string prefix = Convert.ToString((char)0);
            if (Program.session_key != 0)
            {
                prefix = Convert.ToString((char)1) + Convert.ToString(Program.session_key) + Convert.ToString((char)0) + Convert.ToString(Program.Me.ID) + Convert.ToString((char)0);
            }
            data = Convert.ToString('r') + prefix + Convert.ToString(t.id) + Convert.ToString((char)0) + Convert.ToString(mode) + Convert.ToString((char)0) + data + Convert.ToString((char)0);
            byte[] send = Encoding.UTF8.GetBytes(data + "\n");
            /*byte[] send = new byte[data.Length + 1];
            for (int i = 0; i < data.Length; i++)
                send[i] = (byte)data[i];
            send[data.Length] = (byte)'\n';*/
            netStream.Write(send, 0, send.Length);
            netStream.Flush();
            return t.id;
        }

        public void Disconnect()
        {
            netStream.Close();
            tcpClient.Close();
        }

        public void DownloadFile(int id)
        {
            if (id == -1)
                return;
            if (!FilesAsked.Contains(id))
            {
                FilesAsked.Add(id);
                SendData(Convert.ToString(id), 5);
            }
        }

        public void SendVoice(string text)
        {
            int size = Convert.ToInt32(new FileInfo("tosend.wav").Length);
            Program.SendingVoice = true;
            tRequest t;
            lastId++;
            if (lastId == maxLastRequestId) lastId = 0;
            t.id = lastId;
            t.mode = 999;
            t.respond = string.Empty;
            t.Inf = Program.ConvSelected.ToString() + " " + text;
            string prefix =  "ftosend.wav " + t.id.ToString()+ " " + size.ToString() + " " + '\n';
            requests.Add(t);
            using (var fin = File.OpenRead("tosend.wav"))
            {
                byte[] byteArray = new byte[size + prefix.Length];
                for (int i = 0; i < prefix.Length; i++)
                    byteArray[i] = (byte)prefix[i];
                fin.Read(byteArray, prefix.Length, size);
                netStream.Write(byteArray, 0, byteArray.Length);
                netStream.Flush();
            }
        }

        public void SendMessage(Conversations c, string text, string att)
        {
            string data = Convert.ToString(c.id) + Convert.ToString((char)0) + text + Convert.ToString((char)0) + att;
            uint req_id = SendData(data, 7);
            //File.AppendAllText("F:/flexchat_log.txt", data);
            Program.UpdateData(true);
            c.AddMessage(-1 * Convert.ToInt32(req_id), Program.Me.ID, c.id, text, DateTime.Now, att);
        }

        public void WaitForResponse()
        {
            string data;
            string filename = "";
            while (true)
            {
                data = "";
                string s_r_id = "";
                bool f = true;
                try
                {
                    bool was0 = false;
                    f = true;
                    while (true)
                    {
                        byte b = reader.ReadByte();
                        if (!was0 && b == 0)
                        {
                            was0 = true;
                            int rqid = int.Parse(data);
                            int rInd = 0;
                            while (rInd < requests.Count)
                            {
                                tRequest r = requests[rInd];
                                rInd++;
                                if (r.id == rqid)
                                {
                                    if (r.mode == 5)
                                    {
                                        f = false;
                                        filename = "";
                                        b = reader.ReadByte();
                                        while (b != 0)
                                        {
                                            filename += (char)b;
                                            b = reader.ReadByte();
                                        }
                                        string _filesize = "";
                                        b = reader.ReadByte();
                                        while (b != 0)
                                        {
                                            _filesize += (char)b;
                                            b = reader.ReadByte();
                                        }
                                        int filesize = int.Parse(_filesize);
                                        byte[] buffer = new byte[filesize];
                                        for (int i = 0; i < filesize; i++)
                                        {
                                            buffer[i] = reader.ReadByte();
                                        }
                                        File.WriteAllBytes(Content.CACHE_DIR + filename, buffer);
                                        Content.AddFile(filename);
                                        break;
                                    }
                                    break;
                                }
                            }
                        }
                        if (!f) break;
                        if (b == '\n') break;
                        else
                        {
                             data += (char)b;
                        }
                    }
                }
                catch (Exception e)
                {
                    if (e.Data.ToString() != "System.Collections.ListDictionaryInternal")
                        continue;
                }
                if (f && data.Length > 0)
                {
                    int p = 0;
                    while (data[p] != 0)
                    {
                        s_r_id += data[p];
                        p++;
                    }
                    uint r_id = uint.Parse(s_r_id);
                    List<tRequest> toDel = new List<tRequest>();
                    int qInd = 0;
                    while (qInd < requests.Count)
                    {
                        tRequest q = requests[qInd];
                        qInd++;
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
}
