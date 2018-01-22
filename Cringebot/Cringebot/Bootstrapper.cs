﻿using Cringebot.Wrappers;
using Cringebot.ViewModel;
using FreshMvvm;
using Xamarin.Forms;

namespace Cringebot
{
    public class Bootstrapper
    {
        public Bootstrapper()
        {
            FreshIOC.Container.Register<IAppDataStore, StorageWrapper>();
            FreshIOC.Container.Register(DependencyService.Get<INotificationManager>());
        }

        // ReSharper disable once MemberCanBeMadeStatic.Global - if static, no instantiation, if no instantiation, no registrations!
        public Xamarin.Forms.Page GetStartingPage()
        {
            return new FreshNavigationContainer(FreshPageModelResolver.ResolvePageModel<MainViewModel>());
        }
    }
}
