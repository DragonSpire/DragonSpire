DragonSpire
===========

An open source (Apache V2) C# Minecraft Server Software

------------------------------------------
-----------On the Community---------------
------------------------------------------
DragonSpire is also the name of the community, we will be working on a site and forums soon! We are commited to an open-source and friendly community of developers and players alike. We look forward to *any* ideas you have, be creative and have fun!

If you want to join our personal Developer team, upload a pull request and state that you would like to be reviewed for a position inside the group, we will review your pull request. Even if you don't make it into the internal team any dev can join in on our public development chat, however note that if we are working we would prefer the chat stay mostly on topic, our private Developers chat will most likely only be used to withdraw from the public one if there is too much chatter and we cannot properly discuss the software in detail.


------------------------------------------
---------------On Plugins-----------------
------------------------------------------
Note that if you wish to have a plugin featured *WITH* the server it must also fall under the Apache v2 license (The same license as the server), it will also be scrutinized from top to bottom and possibly optimized before being included into the server distributable.

This does not, however, mean that it becomes ours, a full-fledged plugin that is included in the software remains yours, if we like the *IDEA* of your plugin, but not the way it is written or the setup of it, we may decide to scrap the plugin and write our own, *IF* that happens you will be credited as the base for the plugin, if you so wish.

We will do our best to avoid taking code from your uploaded plugins and putting it into our own server without giving you credit, however, this is a very touchy area, we know that, and we have every intention of staying in contact with the original creator through the process if possible.


------------------------------------------
------------On the Source-----------------
------------------------------------------
In order to build and test the server effectively we moved all DLL's to build into C:\DragonSpire and moved the working directory of all the projects to C:\DragonSpire

You *shouldn't* have to create this folder yourself, but if you run into errors, check and make sure that it is created

Once you update a dll (and when you *FIRST* build the server) you need to run a ```REGULAR``` build, not a ```DEBUG``` build, this will create the dll's and put them into the C:\DragonSpire folder, if you do not build the entire project it will not place them there, the server will start as normal but will load 0 items from the dll's (if the dll's are the wrong version then it will error when trying to load them.)

Once you have built it, it should debug fine, pulling the dll's from the folder as the working directory, if anyone has a better way to do this (we dont want to have to manually copy the dll's from 5 different sub folders each time they are updated) feel free to add change it!

We will accept pull requests, however, do not be offended if we change your code, if we use your code you may continue to take credit for it.

Note that any pull requests will be held under our license and copyright, you personally may of course use your own code as you see fit. You may not however reclaim it from our software, once it us uploaded your are giving us full rights to your code.

If you wish to help, feel free to submit pull requests, work on a wiki, documentation, plugin or anything that you want! The api will change quite often early in development, but once we nail down what we want to have in our api it will stablize and you will be able to rely on it!


------------------------------------------
-------------Source Rules-----------------
------------------------------------------
Unfortunately we do not abide by 'common' coding standards, we have our own set of rules we follow and even then each of the main developers has their own 'flair' that they use when coding. Many of the developers come from Java backgrounds and have brought some of the java coding style to our C# Server, this isn't a bad thing, but it can cause some confusion.

You are free to code in the style that you find suits you best, and in return, we may tweak the style and code in your commit before passing it onto the main code.

This is not because we don't like you (WE DO!) and it's not because we're trying to steal your code (WE'RE NOT!), see heres the deal; When we write code (and by we I mean any developer) it's a way to express out thoughts, our ideas and our schemes in a way that can AFFECT the computer we are working on. Our complex thought gets put into a system of chracters and those in turn form commands for the computer to follow.

Complex thought is like a train, it takes a while for the train to get up to a speed at which it accomplishes something, and it takes a while for the train to slow down enough to take corners, if the train DOES NOT slow down for a corner then one of two things happen:

1. The contents of the train are shoved aside and may be broken or jumbled
2. The train derails and a lot of havoc ensues

Both of those things are usually bad, and that concept can be brought into software development in the same way. When we go through and rewrite your code, or tweak it, were slowing the train down and examining something that we are not familiar with, by doing this before we add it to the main code we avoid a possible train wreck or 'Priority Shift' later if we have to stop development to figure out how your code works.

It's not that we don't like your code, we wouldn't add it to the server if we didn't like it! It might just be that you think in a more complex way than we do, and we have to take time to understand your thought pattern. That's not something we want to do when were between lines of code later on!
