using Xamarin.Forms;

namespace CircleButtonMenuSample
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            BindingContext = new
            {
                Controls = new[] { "waves", "save", "sound" }
            };
        }
    }
}
