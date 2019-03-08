
using Android.App;
using Android.Content.PM;
using Android.OS;
using CircleButtonMenu.Android;

namespace CircleButtonMenuSample.Droid
{
    [Activity(Label = "CircleButtonMenuSample", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            CircleButtonMenuRenderer.Init();
            LoadApplication(new App());
        }
    }
}