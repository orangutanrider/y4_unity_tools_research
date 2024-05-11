# Pre-Year4 Unity Tools R&D
![Tools Banner](https://github.com/orangutanrider/GDPAbertayUndergrad-UnityToolsResearch/assets/99553929/30068ea5-c42c-4a4c-86dc-5b683d7981c4)
#
This wasn't a university project; it was in the holidays between year3 and 4.
I was developing generically applicable tools, for use in all of my Unity projects, going forward. The tools are as follows:
- Scene/Asset Reference Inspector.
- Help/ReadMe Inspector.

There were more ideas beyond this, but these are the things that you can see in the code.
Both of these were custom inspectors, specifically not editors; They mirrored the behaviour of the default inspector but displayed the inspected differently. I created a base-class that creates this behaviour.

This project was generally my introduction to more complex programming problems. I explored new topics with the Help/ReadMe Inspector. It was my first-time writing code that that interacted with the file system, and my first-time writing code that read files in a pattern; I used Microsoft's code analysis package for C#. 

https://www.nuget.org/packages/Microsoft.CodeAnalysis/

I learned to take things slow, to solve one problem at a time, to progress my understanding; I became familiar with the fact that I couldn't rush this learning. Generally, it developed my problem solving.

### Scene/Asset Reference Inspector
Across my Unity projects, a convention eventually emerged which was the categorisation of reference fields into one of three kinds: Required Reference, Nullable Required, and Component Nullable.
These indicate the strength of the dependency. 
- A required reference, creates a crash when it is null.
- A nullable required is expected but will not crash.
- A component nullable, is optional.

In code, the easiest way to establish these categories I found, was to use Drawer attributes i.e. [Header("Required References")] and so on. The problem with this, is that inheritance breaks it.
When inherited, fields of each category will not be merged under shared headers, it'll repeat itself, creating a messy editor. One way to fix, is to just create custom editors, but this is time consuming.

Not super problematic, but I wanted this categorisation to be formalised in the code, so I decided to create this inspector. 

The editor works via C# attributes, it extracts the fields attributed with the relevant category, and displays them under respective drop-down headers. It works with inherited fields too, no jumbling of headers.

### Help/ReadMe Inspector
This inspector had two functions. It acted as a hub for help-messages and as display for readme/documentation information, about an inspected object. The readme functionality was never completed.

The plan was to link a scriptable object to each inspectable object definition. These scriptable objects could be written to manually or generated from source via extracting documentation comments.

The desired functionality for the help messages, ended up not being possible. I cannot effectively delve into why, forgive me, but my documentation practices were still evolving at the time, and I do not remember the specifics of the problem. It did have some functionality, showing additional information about inspected objects, that could be given by the inspected object, or by other editors. You can investigate the documentation in .docs, if you wish to find out more.

### Scriptable Objects as Structs
Something that isn't a part of the project, but I did want to do. With my use of Unity, scriptable objects eventually became a core part of my workflow. To define parameters of any kind, I would use them, even if for something one-off like a player controller. It helped to keep things organised.

Eventually, I wanted to better manage access to scriptable objects, and the strategy I found was to utilise structs (to pass data by value rather than by reference). This way, a scriptable object cannot be modified during runtime. It was a lot of code though; I wrote a lot of these kinds of scriptable objects, so it needed to be efficient; I tried to find a way to automate it with inheritance but never did. With the knowledge I hold now, I'm unsure if it's a good idea or not; The performance impact is investigated.

