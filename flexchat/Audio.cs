using NAudio;
using NAudio.Wave;
using System;
using System.IO;

namespace flexchat
{
    class Audio
    {
        struct VoiceMessageInf
        {
            public int conv_id;
            public int msg_id;
            public int file_id;
            public TimeSpan msg_len;
            public bool paused;
        }

        private VoiceMessageInf VoiceMessage;
        private WaveInEvent _waveSource;
        private WaveFileWriter _waveWriter;
        private WaveOutEvent waveOut;
        private WaveStream wavStream;
        private WaveChannel32 wavChannel;
        private string _filename;
        private string _tempfilename;
        private bool Recording = false;
        public event EventHandler RecordingFinished;

        public Audio()
        {
            waveOut = new WaveOutEvent();
            VoiceMessage.conv_id = -1;
            VoiceMessage.msg_id = -1;
            VoiceMessage.paused = false;
            VoiceMessage.file_id = -1;
        }
        public Audio(string filename)
        {
            _filename = filename;
            _tempfilename = $"{Path.GetFileNameWithoutExtension(filename)}.wav";
        }

        public void StartRecording()
        {
            if (Recording)
                return;

            _waveSource = new WaveInEvent
            {
                WaveFormat = new WaveFormat(44100, 1)
            };

            _waveSource.DataAvailable += DataAvailable;
            _waveSource.RecordingStopped += RecordingStopped;
            _waveWriter = new WaveFileWriter(_tempfilename, _waveSource.WaveFormat);

            _waveSource.StartRecording();
        }

        public void StopRecording()
        {
            Recording = false;
            _waveSource.StopRecording();
            _waveSource.DataAvailable -= DataAvailable;
            _waveSource.RecordingStopped -= RecordingStopped;
            _waveSource?.Dispose();
            _waveWriter?.Dispose();

            PrepareVoice();
        }

        private void RecordingStopped(object sender, StoppedEventArgs e)
        {
        }

        private void DataAvailable(object sender, WaveInEventArgs e)
        {
            if (_waveWriter != null)
            {
                _waveWriter.Write(e.Buffer, 0, e.BytesRecorded);
                _waveWriter.Flush();
            }
        }

        public void PrepareVoice()
        {
            using (var waveStream = new WaveFileReader(_tempfilename))
            using (var writer = new WaveFileWriter("tosend.wav", waveStream.WaveFormat))
            {
                long size = waveStream.Length;
                waveStream.CopyTo(writer);
                waveStream.Flush();
            }
        }

        public double VoiceMessageProgress()
        {
            TimeSpan current = wavStream.CurrentTime;
            return Convert.ToDouble(current.TotalMilliseconds) / Convert.ToDouble(VoiceMessage.msg_len.TotalMilliseconds);
        }

        public void StopVoiceMessage()
        {
            if (VoiceMessage.msg_id >= 0)
            {
                waveOut.Stop();
                wavChannel.Dispose();
                wavStream.Dispose();
                for (int src_conv = 0; src_conv < Program.convs.Count; src_conv++)
                {
                    if (Program.convs[src_conv].id == VoiceMessage.conv_id)
                    {
                        for (int src_msg = 0; src_msg < Program.convs[src_conv].messages.Count; src_msg++)
                        {
                            if (Program.convs[src_conv].messages[src_msg].id == VoiceMessage.msg_id)
                            {
                                Program.convs[src_conv].messages[src_msg].buttons[0].Status = StatusType.BLOCKED;
                                Program.convs[src_conv].messages[src_msg].buttons[1].Status = StatusType.BLOCKED;
                                Program.convs[src_conv].messages[src_msg].PlaybackActive = false;
                                break;
                            }
                        }
                    }
                }
                VoiceMessage.conv_id = -1;
                VoiceMessage.msg_id = -1;
                VoiceMessage.file_id = -1;
                VoiceMessage.paused = false;
            }
        }

        public void PlayVoiceMessage(int conv_id, int msg_id, int file_id)
        {
            StopVoiceMessage();
            VoiceMessage.conv_id = conv_id;
            VoiceMessage.msg_id = msg_id;
            VoiceMessage.file_id = file_id;
            VoiceMessage.paused = false;
            // Play Content.GetFilename(file_id)
            //WaveFileReader wavReader = new WaveFileReader(Content.GetFilePath(file_id));
            wavStream = new WaveFileReader(Content.GetFilePath(file_id));
            wavChannel = new WaveChannel32(wavStream);
            waveOut.Init(wavChannel);
            VoiceMessage.msg_len = wavChannel.TotalTime;
            waveOut.Volume = 1.0F;
            waveOut.Play();
        }

        public void PauseVoiceMessage()
        {
            if (VoiceMessage.msg_id != -1)
            {
                if (VoiceMessage.paused)
                {
                    waveOut.Play();
                    VoiceMessage.paused = false;
                }
                else
                {
                    waveOut.Stop();
                    VoiceMessage.paused = true;
                }
            }
        }
    }
}
