/*
  _____                                   _       _   _____                      _       
 |  __ \                                 (_)     | | |  __ \                    | |      
 | |__) |____      _____ _ __ _ __   ___  _ _ __ | |_| |__) |___ _ __ ___   ___ | |_ ___ 
 |  ___/ _ \ \ /\ / / _ \ '__| '_ \ / _ \| | '_ \| __|  _  // _ \ '_ ` _ \ / _ \| __/ _ \
 | |  | (_) \ V  V /  __/ |  | |_) | (_) | | | | | |_| | \ \  __/ | | | | | (_) | ||  __/
 |_|   \___/ \_/\_/ \___|_|  | .__/ \___/|_|_| |_|\__|_|  \_\___|_| |_| |_|\___/ \__\___|
                             | |                                                         
                             |_|                                                         
 
 */

using System;
using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;
using Android.Content.PM;

namespace Client.Android
{
    [Activity(MainLauncher = true, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : Activity
    {
        private Button connectButton;
        private EditText ipText;
        private EditText portText;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            this.connectButton = this.FindViewById<Button>(Resource.Id.connectButton);
            this.ipText = this.FindViewById<EditText>(Resource.Id.ipText);
            this.portText = this.FindViewById<EditText>(Resource.Id.portText);

            this.connectButton.Click += ConnectButton_Click;
        }

        private void ConnectButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.ipText.Text) || string.IsNullOrWhiteSpace(this.portText.Text))
            {
                Toast.MakeText(this, GetString(Resource.String.InvalidInput), ToastLength.Short).Show();
                return;
            }

            // Get ip and port
            string ip = this.ipText.Text.Trim();
            int port = int.Parse(this.portText.Text.Trim());

            // TODO: ip validation!
            
            Intent i = new Intent(this, typeof(RemoteActivity));
            i.PutExtra("ip:port", string.Format("{0}:{1}", ip, port));

            // Go!
            StartActivity(i);
        }
    }
}

