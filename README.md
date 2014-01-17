nHydrate Designer Only
======================

The DSL Designer from v5 of nHydrate ORM Modeler from codeplex,
[https://nhydrate.codeplex.com/](https://nhydrate.codeplex.com/)

This is not a clone, or fork, or alternative.
It is just the minimum to make the Designer work. It was done as a learning exercise on Microsoft's DSL Tools.

There is a good DSL Tools explanational video on YouTube,
[http://www.youtube.com/watch?v=8NElPc3I7Q4](http://www.youtube.com/watch?v=8NElPc3I7Q4)

DSL Project
This is a new VS2012 DSL project, with nHydrate's code re-added step by step.

DSL Package Project
Also a new VS2012 DSL Package but does not need any nHydrate's code to make work. Just some resources etc.
It is basically a VSIX host to load the DSL easily into VS 2012.

The solution is only designed to work in the Experimental instance of VS2012, although I have tried to avoid registry clashes.
In both projects I have changed the namespaces to nHydrate2 from nHydrate, and changed company and versions.

Outstanding Bug - now Fixed
*There is one outstanding bug when running Debugging.sln and trying to add a Field to an Entity.*
Was due to DSL project png bitmaps not being Embedded Resources.

Lacking Feature
When adding an association between two Entities, there is no option to change 1:1, 1:Many.

Jan 2014
