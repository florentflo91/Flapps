using System;
using System.IO;
using System.Net;

using Android.App;
using Android.Content;
using Android.Widget;
using Android.OS;
using Android.Database.Sqlite;
using System.Collections.Generic;
using SQLite;

namespace PhoneWord
{
    [Activity(Label = "PhoneWord", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get the UI controls from the loaded layout:
            EditText phoneNumberText = FindViewById<EditText>(Resource.Id.PhoneNumberText);
            Button translateButton = FindViewById<Button>(Resource.Id.TranslateButton);
            Button callButton = FindViewById<Button>(Resource.Id.CallButton);
            TextView debugbutton = FindViewById<TextView>(Resource.Id.debugbutton);
            CheckBox checkBoxWifi = FindViewById<CheckBox>(Resource.Id.checkBoxWifi);

           

            // Disable the "Call" button
            callButton.Enabled = false;

            // Add code to translate number
            string translatedNumber = string.Empty;

           
            translateButton.Click += (object sender, EventArgs e) =>
            {

                string ip = null;
                if (checkBoxWifi.Checked)
                {
                    ip = "ftp://192.168.1.19";
                }
                else
                {
                    ip = "ftp://florentflo.ddns.net";
                }
                //debugbutton.Text += checkBoxWifi.Checked;
                //debugbutton.Text += ip;
                WebRequestGetExample.ftp ftpClient = new WebRequestGetExample.ftp(ip, "administrator", "P@ssw0rd");

                // Environment.getRootDirectory();
                //String dirPath = getDataDirectory().getAbsolutePath() + File.separator + "newfoldername";

                //string DataDirectory = Android.OS.Environment.DataDirectory.Path;
                //debugbutton.Text += DataDirectory;
                var path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
                var filename = Path.Combine(path, "test.txt");


                ftpClient.download("/Data/Users/DefaultAccount/AppData/Local/Packages/FloPi_c5m98077hmgxe/LocalState/db.sqlite", filename, debugbutton);

                string text = System.IO.File.ReadAllText(filename);
                debugbutton.Text += filename;




                SQLiteAsyncConnection database = new SQLiteAsyncConnection(filename);
                //int profileCount =  database.ExecuteScalarAsync<int>("select count(*) from " );
               


                debugbutton.Text += text;

                callButton.Text = "RemoteFileOperations";
                    // Translate user's alphanumeric phone number to numeric
                    translatedNumber = Core.PhonewordTranslator.ToNumber(phoneNumberText.Text);
                if (String.IsNullOrWhiteSpace(translatedNumber))
                {
                    callButton.Text = "Call";
                    callButton.Enabled = false;
                }
                else
                {
                    callButton.Text = "Call " + translatedNumber;
                    callButton.Enabled = true;
                }
            };

            callButton.Click += (object sender, EventArgs e) =>
            {
                // On "Call" button click, try to dial phone number.
                var callDialog = new AlertDialog.Builder(this);
                callDialog.SetMessage("Call " + translatedNumber + "?");
                callDialog.SetNeutralButton("Call", delegate {
                    // Create intent to dial phone
                    var callIntent = new Intent(Intent.ActionCall);
                    callIntent.SetData(Android.Net.Uri.Parse("tel:" + translatedNumber));
                    StartActivity(callIntent);
                });
                callDialog.SetNegativeButton("Cancel", delegate { });

                // Show the alert dialog to the user and wait for response.
                callDialog.Show();
            };
        }


       
    }
    
}
