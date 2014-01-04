Janett
======

Janett translates Java syntax, constructs and calls to Java libraries to C# / .Net counterparts. Janett tries to automate this process as much as possible and preserve manual changes, making re-translation possible.

Janett takes a different way compared to [JLCA](https://github.com/mehdimo/janett/wiki/JLCAssistant):

- It helps you to keep your main code-base on Java and translate it to .Net. It is not a tool for JUMPing.
- It aims to translate libraries and frameworks rather than applications (Swing, JSP, EJB, ...) , however it caries out this task right.
- It released as open source (under LGPL license), and to support open source projects.

Currently, Janett can translate [Classifier4J](Classifier4J) and a lot of infrastructures are in place. Version number shows that Janett has beta quality and that there are [KnownIssues](KnownIssues), but we recommend [trying](https://github.com/mehdimo/janett/releases/tag/v0.7.0) Janett.

While supporting Janett, we try to translate some projects as a case. For the next release, we are going to work on some parts of [Spring framework](http://projects.spring.io/spring-framework).

We invite developers to contribute and users to help us with extensibility mechanisms.

Why
---

We predict Java language will remain source of inspiration in open-source space for a while:

- Mature ecosystem involves open source foundations like Apache as well as multi-billion dollar businesses
- There are more professional developers who do open-source as a day-job
- More universities and research institutes use Java as their projects language
- Open source Java will probably influence this situation more

Large body of Java open source projects seems compelling in .Net. Many Java projects translated to .Net in recent years but translating manually takes days and months even for experienced developers.

Virtual machines like [IKVM](https://github.com/mehdimo/janett/wiki/IKVM) can not solve this problem (see [IKVMCompiler](https://github.com/mehdimo/janett/wiki/IKVMCompiler))

Current Java to .Net translation tool (JLCA) only translates nearly 80% of your code so preserving manual changes is possible only for small projects (see [JLCAssistant](https://github.com/mehdimo/janett/wiki/JLCAssistant))
As a result:

- Some projects were discontinued when developers lose interest or has not enough time to contribute (e.g. NVelocity or older Lucene ports)
- Major version changes again take long time to port (e.g. Hibernate 3)
- Lots of interesting and well-known projects are not ported (e.g. FreeMarker)

How
---

Janett translates your source code through these steps:

- Parsing Java source code to AST using infrastructure provided by [NRefactory](https://github.com/mehdimo/janett/wiki/NRefactory)
- [Transforming](https://github.com/mehdimo/janett/wiki/Transformations) constructs in Java which have no counterparts in C# (e.g. anonymous classes)
- Refactoring code to adhere .Net conventions (making methods PascalStyle, Properties instead of accessors, ...)
- Mapping: Replacing call to Java or other third party libraries
  * [Virtualization](https://github.com/mehdimo/janett/wiki/VirtualizationMode): using java libraries provided by IKVM or J#
  * [Emulation](https://github.com/mehdimo/janett/wiki/EmulationMode): stubbing java libraries using delegation or inheritance of .Net libraries
  * [Native](https://github.com/mehdimo/janett/wiki/NativeMode): replacing java libraries with .Net ones
- In Native mode test cases may failed because replaced methods have different behavior so Correction is required:
  * Inspection and modification of resulting code with Adapter classes (see [ReaderRead](https://github.com/mehdimo/janett/wiki/ReaderRead))
  * Stubbing some classes to be implemented from scratch (see [ClassifierResourceClass](https://github.com/mehdimo/janett/wiki/ClassifierResourceClass))
  * Using helper classes instead of calling .Net libraries directly (see [StringSplit](https://github.com/mehdimo/janett/wiki/StringSplit))
- Preserving manual changes automatically (by diff and patch) while you re-translate