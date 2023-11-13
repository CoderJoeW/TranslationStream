using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslationStream
{
    internal class Mp3Player
    {
        public static void ConvertMp3ToWav(string mp3FilePath, string wavFilePath)
        {
            using(Mp3FileReader mp3Reader = new Mp3FileReader(mp3FilePath))
            {
                using(WaveStream pcmStream = WaveFormatConversionStream.CreatePcmStream(mp3Reader))
                {
                    WaveFileWriter.CreateWaveFile(wavFilePath, pcmStream);
                }
            }
        }

        public static void PlayWavFile(string wavFilePath)
        {
            using(WaveOutEvent waveOut = new WaveOutEvent())
            {
                using(AudioFileReader reader = new AudioFileReader(wavFilePath))
                {
                    waveOut.Init(reader);
                    waveOut.Play();
                    while(waveOut.PlaybackState == PlaybackState.Playing)
                    {
                        Thread.Sleep(100);
                    }
                }
            }
        }
    }
}
