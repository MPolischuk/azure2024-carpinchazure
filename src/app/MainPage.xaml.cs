namespace Carpinchazure
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnCarpinchosClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CarpinchosPage());
        }

        private async void OnCarpinchosPremiumClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CarpinchosPage(isPrivate:true));
        }
    }

}
