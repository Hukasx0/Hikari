/*
    MIT License

    Copyright (c) 2023 Hubert Kasperek

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
*/

using MaybeSharp;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MaybeDotnet {
    public static class MaybeFile {
        public static MaybeErr<FileStream, Exception> Open(string file_path, FileMode mode) {
            try {
                FileStream fs = new FileStream(file_path, mode);
                return MaybeErr<FileStream, Exception>.ok(fs);
            } catch (Exception ex) {
                return MaybeErr<FileStream, Exception>.err(ex);
            }
        }

        public static MaybeErr<string, Exception> Read(string file_path) {
            try {
                string content = System.IO.File.ReadAllText(file_path);
                return MaybeErr<string, Exception>.ok(content);
            } catch (Exception ex) {
                return MaybeErr<string, Exception>.err(ex);
            }
        }

        public static MaybeErr<Unit, Exception> Write(string file_path, string content) {
            try {
                System.IO.File.WriteAllText(file_path, content);
                return MaybeErr<Unit, Exception>.ok(Unit.Value);
            } catch (Exception ex) {
                return MaybeErr<Unit, Exception>.err(ex);
            }
        }

        public static MaybeErr<Unit, Exception> Append(string file_path, string content) {
            try {
                System.IO.File.AppendAllText(file_path, content);
                return MaybeErr<Unit, Exception>.ok(Unit.Value);
            } catch (Exception ex) {
                return MaybeErr<Unit, Exception>.err(ex);
            }
        }

        public static MaybeErr<Unit, Exception> Remove(string file_path) {
            try {
                if (System.IO.File.Exists(file_path)) {
                    System.IO.File.Delete(file_path);
                }
                return MaybeErr<Unit, Exception>.ok(Unit.Value);
            } catch (Exception ex) {
                return MaybeErr<Unit, Exception>.err(ex);
            }
        }
    }

    public static class Http {
        public static MaybeErr<string, Exception> GetSync(string url) {
            using (HttpClient cl = new HttpClient())
            {
                try {
                    HttpResponseMessage response = cl.GetAsync(url).Result;
                    if (response.IsSuccessStatusCode) {
                        string content = response.Content.ReadAsStringAsync().Result;
                        return MaybeErr<string, Exception>.ok(content);
                    } else {
                        return MaybeErr<string, Exception>.err(new Exception($"HTTP GET request failed with status code {response.StatusCode}"));
                    }
                } catch (Exception ex) {
                    return MaybeErr<string, Exception>.err(ex);
                }
            }
        }

        public static async Task<MaybeErr<string, Exception>> Get(string url) {
            using (HttpClient cl = new HttpClient())
            {
                try {
                    HttpResponseMessage response = await cl.GetAsync(url);
                    if (response.IsSuccessStatusCode) {
                        string content = await response.Content.ReadAsStringAsync();
                        return MaybeErr<string, Exception>.ok(content);
                    } else {
                        return MaybeErr<string, Exception>.err(new Exception($"HTTP GET request failed with status code {response.StatusCode}"));
                    }
                } catch (Exception ex) {
                    return MaybeErr<string, Exception>.err(ex);
                }
            }
        }

        public static MaybeErr<string, Exception> PostSync(string url, string request_body, string content_type) {
            using (HttpClient cl = new HttpClient())
            {
                try {
                    StringContent post_content = new StringContent(request_body, Encoding.UTF8, content_type);
                    HttpResponseMessage response = cl.PostAsync(url, post_content).Result;
                    if (response.IsSuccessStatusCode) {
                        string content = response.Content.ReadAsStringAsync().Result;
                        return MaybeErr<string, Exception>.ok(content);
                    } else {
                        return MaybeErr<string, Exception>.err(new Exception($"HTTP POST request failed with status code {response.StatusCode}"));
                    }
                } catch (Exception ex) {
                    return MaybeErr<string, Exception>.err(ex);
                }  
            }
        }

        public static async Task<MaybeErr<string, Exception>> Post(string url, string request_body, string content_type) {
            using (HttpClient cl = new HttpClient()) 
            {
                try {
                    StringContent post_content = new StringContent(request_body, Encoding.UTF8, content_type);
                    HttpResponseMessage response = await cl.PostAsync(url, post_content);
                    if (response.IsSuccessStatusCode) {
                        string content = await response.Content.ReadAsStringAsync();
                        return MaybeErr<string, Exception>.ok(content);
                    } else {
                        return MaybeErr<string, Exception>.err(new Exception($"HTTP POST request failed with status code {response.StatusCode}"));
                    }
                } catch (Exception ex) {
                    return MaybeErr<string, Exception>.err(ex);
                }
            }
        }
    }
}
