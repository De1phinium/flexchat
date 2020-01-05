using NAudio;
using NAudio.Wave;
using System;
using System.IO;

namespace flexchat
{
    class Audio
    {
        private WaveInEvent _waveSource;
        private WaveFileWriter _waveWriter;
        private string _filename;
        private string _tempfilename;
        private bool Recording = false;
        public event EventHandler RecordingFinished;

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
    }
}
