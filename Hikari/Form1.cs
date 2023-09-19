using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Speech.Synthesis;
using Newtonsoft.Json;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using System.IO;
using System.Threading;
using System.Runtime.InteropServices.ComTypes;
using System.Diagnostics;

namespace Hikari
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void companion_test()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string json_to_send = $"{{ \"prompt\": \"Hello, how are you today?\" }}";
                    var content = new StringContent(json_to_send, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync("http://localhost:3000/api/prompt", content);
                    if (response.IsSuccessStatusCode)
                    {
                        string response_body = await response.Content.ReadAsStringAsync();
                        var response_object = JsonConvert.DeserializeObject<companion_prompt_response>(response_body);
                        example_tts(response_object.text);
                    }
                    else
                    {
                        MessageBox.Show($"Error while doing http request to ai-companion backend: {response.StatusCode}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void example_tts(string text)
        {
            int virtualCableDeviceIndex = 0; // VB-Audio Virtual Cable device number

            MMDeviceEnumerator enumerator = new MMDeviceEnumerator();
            MMDeviceCollection playbackDevices = enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);

            if (virtualCableDeviceIndex >= playbackDevices.Count)
            {
                Console.WriteLine("Error: Cannot connect to VB-Audio Virtual Cable, you can install it from: https://vb-audio.com/Cable/");
                DialogResult result = MessageBox.Show(
                    "Cannot connect to VB-Audio Virtual Cable, you can install it from: https://vb-audio.com/Cable/",
                    "Error",
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Error
                );

                if (result == DialogResult.OK)
                {
                    Process.Start("https://vb-audio.com/Cable/");
                }
                return;
            }

            MMDevice virtualCableDevice = playbackDevices[virtualCableDeviceIndex];

            SpeechSynthesizer synth = new SpeechSynthesizer();

            MemoryStream audioStream = new MemoryStream();

            foreach (InstalledVoice voice in synth.GetInstalledVoices())
            {
                      // if english-US tts is not available then use default installed
                if (voice.VoiceInfo.Culture.Name == "en-US")
                {
                    synth.SelectVoice(voice.VoiceInfo.Name);
                    break;
                }
            }

            synth.SetOutputToWaveStream(audioStream);

            synth.Speak(text);

            audioStream.Seek(0, SeekOrigin.Begin);

            using (var waveOut = new WasapiOut(virtualCableDevice, AudioClientShareMode.Shared, true, 100))
            {
                using (var waveStream = new RawSourceWaveStream(audioStream, new WaveFormat(16000, 16, 1)))
                {
                    waveOut.Init(waveStream);
                    waveOut.Play();

                    while (waveOut.PlaybackState == PlaybackState.Playing)
                    {
                        Thread.Sleep(500);
                    }
                }
            }

            audioStream.Dispose();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            companion_test();
        }

        private bool is_server_running(string host, int port)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = client.GetAsync($"http://{host}:{port}").Result;
                    if (response.IsSuccessStatusCode)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (!is_server_running("localhost", 3000))
            {
                DialogResult result = MessageBox.Show(
                "AI-companion server is not running. You can download the program from GitHub or start it if you have it installed locally.",
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
                );
                if (result == DialogResult.OK)
                {
                    Process.Start("https://github.com/Hukasx0/ai-companion/releases/tag/0.9.5");
                }
            }
        }
    }
}

class companion_prompt_response
{
    public int id { get; set; }
    public bool ai { get; set; }
    public string text { get; set; }
    public string date { get; set; }
}
