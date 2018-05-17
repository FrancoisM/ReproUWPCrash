using System;
using System.Reactive.Concurrency;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Reactive.Linq;
using System.Threading;

[assembly: XamlCompilation (XamlCompilationOptions.Compile)]
namespace ReproUWPCrash
{
	public partial class App
	{
	    private ExNavigationPage _currentExNavigationPage;

        public App ()
		{
		    InitializeComponent();
		    MainPage = new ContentPage { Content = new ActivityIndicator { IsRunning = true, IsVisible = true, WidthRequest = 25, HeightRequest = 25 } };
		    var schedulerProvider = new SchedulerProvider();
            Observable.Timer(TimeSpan.Zero)
                .SubscribeOn(schedulerProvider.Background)
                .ObserveOn(schedulerProvider.Foreground)
                .Subscribe(_ =>
		        {
		            var stack = new Stack();
		            var navPage = new ExNavigationPage(new ContentPage { Content = new Label { Text = "YO" } }, "");
		            stack.Children.Add(navPage);
		            _currentExNavigationPage = navPage;
		            MainPage = stack;
                });
        }
	}

    public class ExNavigationPage : NavigationPage
    {
        public string StackKey { get; }

        public ExNavigationPage(Page page, string stackKey) : base(page)
        {
            StackKey = stackKey;
            BarTextColor = Color.White;
            SetHasNavigationBar(CurrentPage, false);
        }

        protected override void OnPropertyChanged(string propertyName = null)
        {
            // ReSharper disable once ExplicitCallerInfoArgument
            base.OnPropertyChanged(propertyName);
            if (propertyName != CurrentPageProperty.PropertyName) return;
            SetHasNavigationBar(CurrentPage, false);
        }
    }

    public class Stack : MultiPage<ExNavigationPage>
    {
        protected override ExNavigationPage CreateDefault(object item) => null;
    }

    public sealed class SchedulerProvider
    {
        public IScheduler Foreground { get; } = new SynchronizationContextScheduler(SynchronizationContext.Current);
        public IScheduler Background => TaskPoolScheduler.Default;
    }
}
