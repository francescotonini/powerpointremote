using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using System.Net.Sockets;
using System.Threading;
using Android.Runtime;
using Android.Views;
using Android.Content.PM;

namespace Client.Android
{
    [Activity(Label = "Remote", ScreenOrientation = ScreenOrientation.Portrait)]
    public class RemoteActivity : Activity
    {
        private const string NEXT_SIG = "NEXT";
        private const string PREV_SIG = "PREV";
        private const string END_SIG = "ENDP";

        private string ip;
        private int port;
        private Button nextButton;
        private Button prevButton;
        private Button endButton;
        private TcpClient client;
        private TextView statusText;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.Remote);

            this.nextButton = this.FindViewById<Button>(Resource.Id.nextButton);
            this.prevButton = this.FindViewById<Button>(Resource.Id.prevButton);
            this.endButton = this.FindViewById<Button>(Resource.Id.endButton);
            this.statusText = this.FindViewById<TextView>(Resource.Id.statusText);

            this.nextButton.Click += NextButton_Click;
            this.prevButton.Click += PrevButton_Click;
            this.endButton.Click += EndButton_Click;

            string fromIntent = Intent.GetStringExtra("ip:port");
            this.ip = fromIntent.Split(':')[0];
            this.port = int.Parse(fromIntent.Split(':')[1]);

            Thread remoteT = new Thread(RemoteThread);
            remoteT.Start();
        }

        public override bool OnKeyUp([GeneratedEnum] Keycode keyCode, KeyEvent e)
        {
            if (keyCode == Keycode.VolumeUp)
            {
                SendToClient(NEXT_SIG);
                return true;
            }
            else if (keyCode == Keycode.VolumeDown)
            {
                SendToClient(PREV_SIG);
                return true;
            }

            return base.OnKeyUp(keyCode, e);
        }

        public override bool OnKeyDown([GeneratedEnum] Keycode keyCode, KeyEvent e)
        {
            if (keyCode == Keycode.VolumeUp) return true;
            else if (keyCode == Keycode.VolumeDown) return true;

            return base.OnKeyDown(keyCode, e);
        }

        private void EndButton_Click(object sender, EventArgs e)
        {
            SendToClient(END_SIG);
        }

        private void PrevButton_Click(object sender, EventArgs e)
        {
            SendToClient(PREV_SIG);
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            SendToClient(NEXT_SIG);
        }

        private void SendToClient(string message)
        {
            if (this.client == null)
            {
                Toast.MakeText(this, GetString(Resource.String.NoConnection), ToastLength.Short).Show();
                return;
            }

            NetworkStream stream = this.client.GetStream();
            byte[] buffer = System.Text.Encoding.ASCII.GetBytes(message);
            stream.Write(buffer, 0, buffer.Length);
        }

        private void RemoteThread()
        {
            try
            {
                this.client = new TcpClient(this.ip, this.port);
                
                this.statusText.Text = GetString(Resource.String.YesConnection);
            }
            catch(Exception ex)
            {
                RunOnUiThread(() => Toast.MakeText(this, ex.ToString(), ToastLength.Long).Show());
            }
        }
    }
}