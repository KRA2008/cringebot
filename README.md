# Cringebot

Cringebot explores the tendency many people have to make themselves cringe by remembering embarrassing events in their past and to generate recurrent negative thoughts. I hope that by using Cringebot, users will become more aware of these tendencies and will be more empowered to live happier lives.

Cringebot helps you to see that you are the Cringebot.

---

Cringebot can do a couple things:
* Track occurrences of cringes and negative thoughts in real time
* Simulate involuntary recall and negative thoughts from a user-generated list of prompts
    
By tracking occurrences, Cringebot helps you to be more aware of how much these occurrences affect your daily life. By simulating, Cringebot helps you to realize the absurdity of the phenomena and perhaps build up a psychological tolerance. Simulation could also be used by people who don't experience such things so they can better understand people who do.

(I am not a doctor. Cringebot is not therapy.)

---

The above describes Cringebot in its value as a running application. As a repository, Cringebot fulfills two main functions. First, it acts as a bit of a résumé booster for its creator. Second, it demonstrates the ease of building a non-trivial, useful, robust, and maintainable Xamarin.Forms application for those who might be interested in pursuing the technology.

Some of the niceties incorporated here that I would encourage others to use:
* https://www.nuget.org/packages/PropertyChanged.Fody/
    * Automatically weaves in all that PropertyChanged event handler boilerplate
* https://www.nuget.org/packages/FreshMvvm/
    * Makes the MVVM pattern a breeze, makes navigation super easy and unit testable
* https://www.nuget.org/packages/Corcav.Behaviors/
    * Eliminates codebehind by allowing ViewModels to handle every UI event directly
* https://www.nuget.org/packages/moq/
    * Friendly mocking for unit tests
* https://www.nuget.org/packages/SharpTestsEx/
    * Testing assertions that read like sentences
* http://www.ncrunch.net/
	* Automatic smart test execution
* https://github.com/aloisdeniel/Microcharts
	* Nice charts
* ~~https://www.nuget.org/packages/xcc/~~
    * ~~When combined with ReSharper, adds auto-complete/intellisense to Xamarin.Forms XAML pages for your binding context in Visual Studio on Windows~~
	* Xamarin.Forms is now able to understand 'xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006" mc:Ignorable="d"' which makes xcc unneeded, so I'd suggest using xcc if you're not up to that version of Xamarin.Forms yet