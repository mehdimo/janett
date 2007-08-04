$!Output=Xml!$
$!Root:Projekt!$
Microsoft Visual Studio Solution File, Format Version 8.00$Root.Projects:{
Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "$it.Name$", "$it.Folder$\\$it.Name$.csproj", "\{$it.GUID$\}"
	ProjectSection(ProjectDependencies) = postProject
	EndProjectSection
EndProject}$
Global
	GlobalSection(SolutionConfiguration) = preSolution
		Debug = Debug
		Release = Release
	EndGlobalSection
	GlobalSection(ProjectConfiguration) = postSolution$Root.Projects:{
		\{$it.GUID$\}.Debug.ActiveCfg = Debug|.NET
		\{$it.GUID$\}.Debug.Build.0 = Debug|.NET
		\{$it.GUID$\}.Release.ActiveCfg = Release|.NET
		\{$it.GUID$\}.Release.Build.0 = Release|.NET}$
	EndGlobalSection
	GlobalSection(SolutionItems) = postSolution
	EndGlobalSection
	GlobalSection(ExtensibilityGlobals) = postSolution
	EndGlobalSection
	GlobalSection(ExtensibilityAddIns) = postSolution
	EndGlobalSection
EndGlobal
