$!Output=Xml!$
$!Root:Projekt!$
Microsoft Visual Studio Solution File, Format Version 9.00
# Visual Studio 2005$Root.Projects:{
Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "$it.Name$", "$it.Folder$\\$it.Name$.csproj", "\{$it.GUID$\}"
EndProject}$
Global
	GlobalSection(TextTemplating) = postSolution
		TextTemplating = 1
	EndGlobalSection
	GlobalSection(SolutionConfigurationPlatforms) = preSolution
		Debug|Any CPU = Debug|Any CPU
		Release|Any CPU = Release|Any CPU
	EndGlobalSection
	GlobalSection(ProjectConfigurationPlatforms) = postSolution$Root.Projects:{
		\{$it.GUID$\}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		\{$it.GUID$\}.Debug|Any CPU.Build.0 = Debug|Any CPU
		\{$it.GUID$\}.Release|Any CPU.ActiveCfg = Release|Any CPU
		\{$it.GUID$\}.Release|Any CPU.Build.0 = Release|Any CPU}$
	EndGlobalSection
	GlobalSection(SolutionProperties) = preSolution
		HideSolutionNode = FALSE
	EndGlobalSection
EndGlobal
