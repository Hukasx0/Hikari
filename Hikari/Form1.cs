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
            SpeechSynthesizer synth = new SpeechSynthesizer();
            synth.SetOutputToDefaultAudioDevice();
            synth.SpeakAsync(text);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            companion_test();
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
