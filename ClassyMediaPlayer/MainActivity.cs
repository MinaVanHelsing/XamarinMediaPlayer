using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Content;

using Android.Widget;
using Android.Media;

using System.Collections.Generic;
using System;
using System.Collections;

namespace ClassyMediaPlayer
{
    [Activity(Label = "@string/app_name", Theme = "@android:style/Theme.Holo.NoActionBar.TranslucentDecor",
        ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, MainLauncher = true)]
    public class MainActivity : Activity
    {
        private MediaPlayer mediaPlayer;
         private ArrayAdapter adapter;
        
        public static IList<string> list { get; private set; }

        int pos;
        bool videoCompleted = false;

        VideoView videoView;
        Button btnOpen, btnPlay, btnStop, btnRepeat, btnNext, btnPrevious;
        Android.Net.Uri uri;        

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            btnOpen = FindViewById<Button>(Resource.Id.open);
            btnPlay = FindViewById<Button>(Resource.Id.play);
            btnStop = FindViewById<Button>(Resource.Id.stop);
            btnRepeat = FindViewById<Button>(Resource.Id.repeat);
            btnPrevious = FindViewById<Button>(Resource.Id.previous);
            btnNext = FindViewById<Button>(Resource.Id.next);           
            
            videoView = FindViewById<VideoView>(Resource.Id.videoView);
            videoView.SetMediaController(new MediaController(this));
            list = new List<string>();
            adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1);
            pos = 0;

            btnOpen.Click += delegate {

                adapter.Clear();
                Intent mediaIntent = new Intent(Intent.ActionGetContent);
                mediaIntent.SetType("*/*");
                mediaIntent.PutExtra(Intent.ExtraAllowMultiple, true);
                StartActivityForResult(Intent.CreateChooser(mediaIntent, "Choose a file"), 0);  
				
                btnPlay.Enabled = true;
				btnStop.Enabled = false;
                btnNext.Enabled = false;
                btnRepeat.Enabled = false;
                btnPrevious.Enabled = false;
            };

            btnPlay.Click += delegate {
                btnPlay.Enabled = !btnPlay.Enabled;

                pos = 0;

                if (adapter.Count != 0)
                {
                    videoView.SetVideoURI(Android.Net.Uri.Parse(adapter.GetItem(pos).ToString()));
                    videoView.SetMediaController(new MediaController(this));
                    videoView.RequestFocus();
                    videoView.Start();
                    btnRepeat.Enabled = true;
                    btnStop.Enabled = true;
                }
                else
                {
                    Toast.MakeText(ApplicationContext, "There is no track to play", ToastLength.Short).Show();
                }                       

                if (pos < (adapter.Count - 1))
                {
                    btnNext.Enabled = true;
                    btnRepeat.Enabled = true;
                }
            };

            btnStop.Click += delegate {
                btnStop.Enabled = !btnStop.Enabled;
                videoView.StopPlayback();
                videoView.ClearFocus();
                
                btnPlay.Enabled = true;
                btnNext.Enabled = false;
                btnRepeat.Enabled = false;
                btnPrevious.Enabled = false;
            };

            btnNext.Click += delegate {
                PlayNextTrack(pos);
            };

            btnPrevious.Click += delegate {
                Previous(pos);
            };

            btnRepeat.Click += delegate {
                Repeat(pos);
            };

            videoView.Completion += delegate {

                videoCompleted = true;
                btnPrevious.Enabled = true;
                PlayNextTrack(pos);
            };

        }        

        public void PlayNextTrack(int trackIndex)
        {
            pos = trackIndex;

            if (pos < (adapter.Count - 1))
            {
                pos = pos + 1;

                videoView.SetVideoURI(Android.Net.Uri.Parse(adapter.GetItem(pos).ToString()));
                videoView.RequestFocus();
                videoView.Start();
                btnPrevious.Enabled = true;
            }
            else
            {
                videoView.StopPlayback();
                videoView.ClearFocus();
                btnStop.Enabled = false;
                btnNext.Enabled = false;
                btnRepeat.Enabled = false;
                btnPrevious.Enabled = false;
            }
        }

        public void Repeat(int trackIndex)
        {
            pos = trackIndex;
            
                videoView.SetVideoURI(Android.Net.Uri.Parse(adapter.GetItem(pos).ToString()));
                videoView.RequestFocus();
                videoView.Start();            
        }

        public void Previous(int trackIndex)
        {
            pos = trackIndex;

            if (pos < (adapter.Count - 1) && pos != 0)
            {
                pos = pos - 1;

                videoView.SetVideoURI(Android.Net.Uri.Parse(adapter.GetItem(pos).ToString()));
                videoView.RequestFocus();
                videoView.Start();
            }
            else
            {
                Toast.MakeText(ApplicationContext, "There is no previous track to play", ToastLength.Long).Show();
            }
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (resultCode == Result.Ok)
            {
                if (null != data)
                { // checking empty selection
                    if (null != data.ClipData)
                    { // checking multiple selection
                        for (int i = 0; i < data.ClipData.ItemCount; i++)
                        {
                            uri = data.ClipData.GetItemAt(i).Uri;                            
                            adapter.Add(uri);
                        }
                    }
                    else
                    {
                        uri = data.Data;
                        adapter.Add(uri);
                    }                    
                }
            }
            base.OnActivityResult(requestCode, resultCode, data);
        }             

        protected void OnBackPressed()
        {
            base.OnBackPressed();
        }

        protected void OnDestroy()
        {
            base.OnDestroy();
        }

        protected void OnPause()
        {
            base.OnPause();
        }

        protected void OnResume()
        {
            base.OnResume();
        }       
    }
}

