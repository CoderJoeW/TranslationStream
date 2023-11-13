using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslationStream
{
    internal class AudioRecording
    {
        public bool IsRecording = false;

        private WaveInEvent _waveSource = null;
        private WaveFileWriter _waveFile = null;

        private bool _waveSourceHasStopped = false;

        public AudioRecording()
        {
            _waveSource = new WaveInEvent();
            _waveSource.WaveFormat = new WaveFormat(44100, 1);
            _waveSource.DataAvailable += new EventHandler<WaveInEventArgs>(waveSource_DataAvailable);
            _waveSource.RecordingStopped += new EventHandler<StoppedEventArgs>(waveSource_RecordingStopped);
        }

        public void StartRecording()
        {
            IsRecording = true;
            _waveSourceHasStopped = false;

            string fileName = "audio.wav";
            _waveFile = new WaveFileWriter(fileName, _waveSource.WaveFormat);

            _waveSource.StartRecording();

            Console.WriteLine("Recording Started...");
        }

        public void StopRecording()
        {
            _waveSource.StopRecording();
        }

        private void waveSource_DataAvailable(object sender, WaveInEventArgs e)
        {
            if(_waveFile != null)
            {
                _waveFile.Write(e.Buffer, 0, e.BytesRecorded);
                _waveFile.Flush();
            }
        }

        private void waveSource_RecordingStopped(object sender, StoppedEventArgs e)
        {
            if (!_waveSourceHasStopped)
            {
                _waveSourceHasStopped = true;

                if (_waveSource != null)
                {
                    _waveSource.Dispose();
                    _waveSource = null;
                }

                if (_waveFile != null)
                {
                    _waveFile.Dispose();
                    _waveFile = null;
                }

                IsRecording = false;
                Console.WriteLine("Recording stopped.");
            }
        }
    }
}
