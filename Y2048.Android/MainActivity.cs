using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Birch.Android;
using Birch.Android.Extensions;
using Birch.System;
using Y2048.Core;

namespace Y2048.Android
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : BirchActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource

            RequestWindowFeature(WindowFeatures.NoTitle);
            Window?.SetFlags(WindowManagerFlags.Fullscreen, WindowManagerFlags.Fullscreen);

            SetContentView(Resource.Layout.activity_main);

            var layout = FindViewById<RelativeLayout>(Resource.Id.skCanvasSurface);
            InitializeBirchEngine(layout!);

            new ApplicationBuilder()
                .CreateAndroid()
                .UseStartup<Startup>()
                .Build(this);
        }
    }
}