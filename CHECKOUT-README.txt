Software prerequirements:
* Visual Studio 2015                
* Windows Azure SDK for .NET 2.8+   Search in Web Platform Installer
* Ruby 1.9.3+                       http://rubyinstaller.org/downloads/
* Node.js 0.12+                     https://nodejs.org/en/download/
* GitBash for Windows               https://git-scm.com/download/win

Post checkout steps:
* gem install compass
* npm install -g grunt-cli
* npm install -g bower

Post update steps:
* npm install
* npm run-script bower
* grunt post-checkout
