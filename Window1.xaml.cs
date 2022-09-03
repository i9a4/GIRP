using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Configuration;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Newtonsoft.Json;
using static System.Net.WebRequestMethods;
using System.Security.Authentication;
using System.Net.Http.Headers;
using System.IO;
using System.Net;
using System.Windows.Markup;

namespace WpfAppIpRange
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    
    public partial class Window1 : Window
    {
       static public JObject selected { set; get; }
        public JArray Data { get; set; }
        public Window1(JArray data)
        {
            InitializeComponent();
            Data = data;
            Parse_to_list(Data);
        }

        static readonly HttpClient client = new HttpClient();
        static readonly UriBuilder uriBuilder = new UriBuilder();
        private static async void Log(string input)
        {
            System.Diagnostics.Debug.WriteLine(input);
        }


        

        private async void Parse_to_list(JArray a)
        {
            JArray request = a;
            selected = null;
            if (Data.Count != 0)
            {
                LB_CITY.Items.Clear();
                
                for (int i = 0; i < request.Count(); i++)
                {
                    string timezone;
                    ComboBoxItem cbx = new ComboBoxItem();
                    if((string)request[i]["timezone"] == null)
                    {
                        timezone = "Неизвестно";
                    }
                    else
                    {
                        timezone = (string)request[i]["timezone"];
                    }
                    cbx.Content = (string)request[i]["name_ru"] + " (" + timezone + ")";
                    cbx.Name = "CBX_" + i.ToString();
                    cbx.BorderBrush = (Brush)new System.Windows.Media.BrushConverter().ConvertFromString("#FFB4B4B4");
                    cbx.Height = 65;
                    cbx.VerticalContentAlignment = VerticalAlignment.Center;
                    cbx.VerticalAlignment = VerticalAlignment.Stretch;
                    cbx.HorizontalAlignment = HorizontalAlignment.Stretch;
                    cbx.HorizontalContentAlignment = HorizontalAlignment.Stretch;
                    LB_CITY.Items.Add(cbx);


                }
            }
            else
            {
                ComboBoxItem cbx = new ComboBoxItem();
                cbx.Content = "Упс... Что-то пошло не так...";
                cbx.Name = "CBX_";
                cbx.BorderBrush = (Brush)new System.Windows.Media.BrushConverter().ConvertFromString("#FFB4B4B4");
                cbx.VerticalContentAlignment = VerticalAlignment.Stretch;
                cbx.HorizontalContentAlignment = HorizontalAlignment.Stretch;
                cbx.VerticalAlignment = VerticalAlignment.Stretch;
                cbx.HorizontalAlignment = HorizontalAlignment.Stretch;
                cbx.Height = 65;
                LB_CITY.Items.Add(cbx);

            }
        }


        private void BTN_OK_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SelectItem(int a)
        {
            Log(a.ToString());
        }

        private void LB_CITY_Selected(object sender, RoutedEventArgs e)
        {

        }

        private void LB_CITY_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Log(int.Parse(((System.Windows.FrameworkElement)e.Source).Name.Replace("CBX_", "")).ToString());
                JArray request = Data;
                int i = int.Parse(((System.Windows.FrameworkElement)e.Source).Name.Replace("CBX_", ""));
                selected = (JObject)request[i];
                Close();

            }
            catch (Exception)
            {
                Window1.Log("err");
            }
        }
    }
}
