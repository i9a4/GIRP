using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Threading;

namespace WpfAppIpRange
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>


    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }




        static readonly HttpClient client = new HttpClient();
        static readonly UriBuilder uriBuilder = new UriBuilder();

        private String City;
        private static async void Log(string input)
        {
            System.Diagnostics.Debug.WriteLine(input);
        }

        private async Task<JArray> StringToJsonParse(string json)
        {
            JArray result = new JArray();
            if (json.Length > 0)
            {
                result = JArray.Parse(json);
            }
            return result;
        }

        private async void HandlerExpection(int type, string Error)
        {
            switch (type)
            {
                case 0:
                    AddTextToOut("[Ошибка] " + Error, true);
                    break;
            }

        }
        private async void AddTextToOut(string data, bool clear)
        {
            if (clear)
            {
                TB_OUT.Document.Blocks.Clear();
            }
            var tex = new Run(data);
            Paragraph ert = new Paragraph(tex);
            ert.TextAlignment = TextAlignment.Center;
            ert.LineHeight = 1;
            TB_OUT.Document.Blocks.Add(ert);
        }

        private async void EnableButtons()
        {
            TB_SAVE.IsEnabled = true;
            BT_COPY.IsEnabled = true;
        }

        private static readonly Dictionary<char, string> ConvertedLetters = new Dictionary<char, string>
    {
        {'а', "a"},
        {'б', "b"},
        {'в', "v"},
        {'г', "g"},
        {'д', "d"},
        {'е', "e"},
        {'ё', "yo"},
        {'ж', "zh"},
        {'з', "z"},
        {'и', "i"},
        {'й', "j"},
        {'к', "k"},
        {'л', "l"},
        {'м', "m"},
        {'н', "n"},
        {'о', "o"},
        {'п', "p"},
        {'р', "r"},
        {'с', "s"},
        {'т', "t"},
        {'у', "u"},
        {'ф', "f"},
        {'х', "h"},
        {'ц', "c"},
        {'ч', "ch"},
        {'ш', "sh"},
        {'щ', "sch"},
        {'ъ', "j"},
        {'ы', "i"},
        {'ь', "j"},
        {'э', "e"},
        {'ю', "yu"},
        {'я', "ya"},
        {'А', "A"},
        {'Б', "B"},
        {'В', "V"},
        {'Г', "G"},
        {'Д', "D"},
        {'Е', "E"},
        {'Ё', "Yo"},
        {'Ж', "Zh"},
        {'З', "Z"},
        {'И', "I"},
        {'Й', "J"},
        {'К', "K"},
        {'Л', "L"},
        {'М', "M"},
        {'Н', "N"},
        {'О', "O"},
        {'П', "P"},
        {'Р', "R"},
        {'С', "S"},
        {'Т', "T"},
        {'У', "U"},
        {'Ф', "F"},
        {'Х', "H"},
        {'Ц', "C"},
        {'Ч', "Ch"},
        {'Ш', "Sh"},
        {'Щ', "Sch"},
        {'Ъ', "J"},
        {'Ы', "I"},
        {'Ь', "J"},
        {'Э', "E"},
        {'Ю', "Yu"},
        {'Я', "Ya"}
    };

        public static string ConvertToLatin(string source)
        {
            var result = new StringBuilder();
            foreach (var letter in source)
            {
                try
                {
                    result.Append(ConvertedLetters[letter]);
                }
                catch (Exception e)
                {
                    result.Append(letter).Replace("-", "_").Replace(" ", "_");
                }
            }
            return result.ToString();
        }

        private async Task<JArray> GetRequest(string city)
        {
            string responseBody = "null";
            try
            {
                uriBuilder.Host = "4it.me";
                uriBuilder.Path = "/api/getcitylist";
                uriBuilder.Port = -1;
                uriBuilder.Scheme = "https";
                var query = HttpUtility.ParseQueryString(uriBuilder.Query);
                query["city"] = city;
                uriBuilder.Query = query.ToString();
                string url = uriBuilder.ToString();
                Log(url);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("utf-8"));
                int i = 0;
                int b = 0;

                do
                {
                    var response = Task.Run(() => client.GetAsync(url));
                    // CONTENT CHECK CONTENT TYPE AND HEADER
                    bool resp;
                    try
                    {
                        resp = response.Wait(TimeSpan.FromSeconds(0.5));
                    }
                    catch (Exception e)
                    {
                        HandlerExpection(0, e.Message);
                        resp = false;
                    }


                    if (resp)
                    {
                        HttpResponseMessage response_r = response.Result;
                        response_r.EnsureSuccessStatusCode();
                        responseBody = Task.Run(() => response_r.Content.ReadAsStringAsync()).Result;
                        Log(responseBody);
                        i = 1;
                    }
                    else
                    {
                        if (b > 10)
                        {
                            break;
                        }
                        i = 0;
                        b = b + 1;

                    }
                }
                while (i == 0);




            }
            catch (HttpRequestException e)
            {
                Log(e.Message);
                return null;

            }
            if (responseBody != "null")
            {

                JArray JAresponseBody = Task.Run(() => StringToJsonParse(responseBody)).Result;


                return JAresponseBody;
            }
            else
            {
                return null;
            }
        }

        static async Task<string> ToAddr(long address)
        {
            return IPAddress.Parse(address.ToString()).ToString();
        }

        private async void SetDataBord(string timezone, int found)
        {
            if (timezone == null)
            {
                timezone = "Неизвестно";
            }
            TB_TIMEZONE.Text = timezone;
            TB_FOUND.Text = found.ToString();
        }
        string StringFromRichTextBox(RichTextBox rtb)
        {
            TextRange textRange = new TextRange(
                // TextPointer to the start of content in the RichTextBox.
                rtb.Document.ContentStart,
                // TextPointer to the end of content in the RichTextBox.
                rtb.Document.ContentEnd
            );

            // The Text property on a TextRange object returns a string
            // representing the plain text content of the TextRange.
            return textRange.Text;
        }

        private async Task<JArray> GetIp(JObject data)
        {
            City = (string)data["name_ru"];
            string responseBody = "null";
            try
            {
                uriBuilder.Host = "4it.me";
                uriBuilder.Path = "/api/getlistip";
                uriBuilder.Port = -1;
                uriBuilder.Scheme = "https";
                string basee;
                string cityid;
                await Task.Run(() => Log((string)data["id_nic"]));
                if ((string)data["id_nic"] == null)
                {
                    basee = "net";
                    cityid = (string)data["id_net"];
                }
                else
                {
                    basee = "nic";
                    cityid = (string)data["id_nic"];
                }
                var query = HttpUtility.ParseQueryString(uriBuilder.Query);
                query["base"] = (string)basee;
                query["cityid"] = (string)cityid;
                query["city"] = (string)data["name_ru"];
                uriBuilder.Query = query.ToString();
                string url = uriBuilder.ToString();
                await Task.Run(() => Log(url));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("utf-8"));
                int i = 0;
                do
                {
                    var response = Task.Run(() => client.GetAsync(url));
                    // CONTENT CHECK CONTENT TYPE AND HEADER
                    if (response.Wait(TimeSpan.FromSeconds(5)))
                    {
                        HttpResponseMessage response_r = response.Result;
                        response_r.EnsureSuccessStatusCode();
                        responseBody = Task.Run(() => response_r.Content.ReadAsStringAsync()).Result;
                        Log(responseBody);
                        i = 1;
                    }
                    else
                    {
                        i = 0;
                    }
                }
                while (i == 0);



            }
            catch (HttpRequestException e)
            {
                Log(e.Message);
            }

            JArray JAresponseBody = Task.Run(() => StringToJsonParse(responseBody)).Result;


            return JAresponseBody;
        }



        private void TB_GOROD_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var pattern = @"^[А-Я, A-Z]*$";
            //Log(e.Text);
            if (Regex.IsMatch(e.Text, pattern, RegexOptions.IgnoreCase))
            {
                //  Log("Ok");
            }
            else
            {
                e.Handled = true;
            }

        }

        private async void BTN_SEARCH_Click(object sender, RoutedEventArgs e)
        {


            BTN_SEARCH.IsEnabled = false;
            JArray request = await Dispatcher.Invoke(() => GetRequest(TB_GOROD.Text));

            if (request == null)
            {
                e.Handled = true;
                HandlerExpection(0, "Ошибка соединения с сервером... Проверьте подключение к сети.");
                BTN_SEARCH.IsEnabled = true;
            }
            else
            {

                if (request.Count() > 1)
                {
                    Log("More 2");

                    var taskWindow = new Window1(request);
                    taskWindow.ShowDialog();
                    JObject taskdata = Window1.selected;
                    if (taskdata == null)
                    {
                        e.Handled = true;
                    }
                    else
                    {
                        Log((string)taskdata["name_ru"]);
                        Log((string)taskdata["id"]);
                        JArray request_ip = await Dispatcher.Invoke(() => GetIp(taskdata));
                        Log((string)request_ip[0]["e"]);
                        var ip = await ToAddr((long)request_ip[0]["e"]);
                        Log(ip);
                        TB_GOROD.Text = (string)taskdata["name_ru"];
                        TB_OUT.Document.Blocks.Clear();
                        SetDataBord((string)taskdata["timezone"], request_ip.Count());
                        GC.WaitForPendingFinalizers();
                        for (int i = 0; i < request_ip.Count(); i++)
                        {
                            var tex = new Run(await ToAddr((long)request_ip[i]["b"]) + "-" + await ToAddr((long)request_ip[i]["e"]));
                            Paragraph ert = new Paragraph(tex);
                            ert.TextAlignment = TextAlignment.Center;
                            ert.LineHeight = 1;
                            TB_OUT.Document.Blocks.Add(ert);
                        }
                        BTN_SEARCH.IsEnabled = true;
                        EnableButtons();
                        GC.Collect();

                    }



                }
                else if (request.Count() < 1)
                {
                    AddTextToOut("Не найдено...", true);
                    e.Handled = true;
                    GC.Collect();
                }
                else
                {
                    JArray request_ip = await Dispatcher.Invoke(() => GetIp((JObject)request[0]));
                    SetDataBord((string)request[0]["timezone"], request_ip.Count());
                    Log(ConvertToLatin((string)request[0]["name_ru"]));
                    TB_OUT.Document.Blocks.Clear();
                    GC.WaitForPendingFinalizers();
                    for (int i = 0; i < request_ip.Count(); i++)
                    {
                        var tex = new Run(await ToAddr((long)request_ip[i]["b"]) + "-" + await ToAddr((long)request_ip[i]["e"]));
                        Paragraph ert = new Paragraph(tex);
                        ert.TextAlignment = TextAlignment.Center;
                        ert.LineHeight = 1;
                        TB_OUT.Document.Blocks.Add(ert);
                    }
                    EnableButtons();
                    BTN_SEARCH.IsEnabled = true;

                    Log("Less 2");
                    if ((string)request[0]["id_net"] == null)
                    {
                        Log("id_nic: " + (string)request[0]["id_nic"] + "\nName: " + (string)request[0]["name_ru"]);
                    }
                    else
                    {
                        Log("id_net: " + (string)request[0]["id_net"] + "\nName: " + (string)request[0]["name_ru"]);
                    }
                    GC.Collect();
                }
            }



        }

        private async void BTN_Random_Click(object sender, RoutedEventArgs e)
        {
            JArray request;
            IsEnabled = false;

            request = await Dispatcher.Invoke(() => GetRequest(null));
            if (request == null)
            {
                e.Handled = true;
                HandlerExpection(0, "Ошибка соединения с сервером... Проверьте подключение к сети.");
            }
            else
            {
                TB_GOROD.Text = (string)request[0]["name_ru"];
                JArray request_ip = await Dispatcher.Invoke(() => GetIp((JObject)request[0]));
                TB_OUT.Document.Blocks.Clear();
                SetDataBord((string)request[0]["timezone"], request_ip.Count());
                GC.WaitForPendingFinalizers();
                for (int i = 0; i < request_ip.Count(); i++)
                {
                    var tex = new Run(await ToAddr((long)request_ip[i]["b"]) + "-" + await ToAddr((long)request_ip[i]["e"]));
                    Paragraph ert = new Paragraph(tex);
                    ert.TextAlignment = TextAlignment.Center;
                    ert.LineHeight = 1;
                    TB_OUT.Document.Blocks.Add(ert);
                }


                Log(StringFromRichTextBox(TB_OUT));
                EnableButtons();
            }
            IsEnabled = true;
            GC.Collect();


        }

        private async void BT_COPY_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(StringFromRichTextBox(TB_OUT));
            GC.Collect();
        }

        private void TB_SAVE_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = ConvertToLatin(City) + "_ipRange"; // Default file name
            dlg.DefaultExt = ".txt"; // Default file extension
            dlg.Filter = "I'm like Lady Gaga (.txt)|*.txt"; // Filter files by extension

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                string filename = dlg.FileName;
                var ip_list = StringFromRichTextBox(TB_OUT);
                System.IO.File.WriteAllText(filename, ip_list);
            }
            GC.Collect();
        }

        private void TB_GOROD_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TB_GOROD.Text.Length != 0)
            {
                BTN_SEARCH.IsEnabled = true;
            }
            else
            {
                BTN_SEARCH.IsEnabled = false;
            }
        }

        private void BT_ABOUT_Click(object sender, RoutedEventArgs e)
        {
            new Window2().ShowDialog();
            GC.Collect();
        }

        private void TB_GOROD_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (BTN_SEARCH.IsEnabled)
                {
                    BTN_SEARCH_Click(sender, e);
                }
            }
        }
    }
}
