﻿Sitecore - DesignBot
==============

Summary
--------------
Automate bulk design changes to Sitecore presentation details. This leverages Sitecore's rule editor field for savvy users to define design changes with pre-loaded Sitecore rule editor. These user programs can be applied to a single item or include any number of child levels, available in the Context menu of a selected item.

Dependencies
--------------
This module requires previous installation of Sitecore Powershell Extension (SPE) Module. SPE is used for the simplicity of adding a context menu option and dialogs.

Usage
--------------
1. Define Design Program
![alt text](https://github.com/digitalParkour/Community.Foundation.DesignBot/raw/master/screenshots/Define.png "Define Design Program")

2. Apply Program to select items
![alt text](https://github.com/digitalParkour/Community.Foundation.DesignBot/raw/master/screenshots/Apply.png "Apply Design Program")

Installation
--------------
Either:
* Install Sitecore package. [See Releases](https://github.com/digitalParkour/Community.Foundation.DesignBot/releases).
	> For CM or Standalone instances only.

Or:
1. Include this project in your Helix style solution
2. Update NuGet references to match target sitecore version
3. Sync unicorn data or install sitecore package
    > Expects Unicorn 4+ in your solution.
    > If not using Unicorn, disable Foundation.RemoteCacheKick.Serialization.config