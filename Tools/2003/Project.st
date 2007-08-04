$!Output=Xml!$
$!Root:Project!$
<VisualStudioProject>
  <CSHARP ProjectType="Local" ProductVersion="7.10.3077" SchemaVersion="2.0" 
	ProjectGuid="{$Root.GUID$}">
    <Build>
      <Settings ApplicationIcon="$Root.ApplicationIcon$" AssemblyKeyContainerName="" AssemblyName="$Root.AssemblyName$" 
	AssemblyOriginatorKeyFile="" DefaultClientScript="JScript" DefaultHTMLPageLayout="Grid" 
	DefaultTargetSchema="IE50" DelaySign="false" OutputType="$Root.OutputType$" 
	PreBuildEvent="$Root.PreBuildEvent$" PostBuildEvent="$Root.PostBuildEvent$" 
	RootNamespace="$Root.RootNamespace$" RunPostBuildEvent="OnBuildSuccess" 
	StartupObject="">
        <Config Name="Debug" AllowUnsafeBlocks="false" BaseAddress="285212672" 
		CheckForOverflowUnderflow="false" ConfigurationOverrideFile="" DefineConstants="DEBUG;TRACE" 
		DocumentationFile="" DebugSymbols="true" FileAlignment="4096" IncrementalBuild="false" 
		NoStdLib="false" NoWarn="" Optimize="false" OutputPath="bin\Debug\" RegisterForComInterop="false" 
		RemoveIntegerChecks="false" TreatWarningsAsErrors="false" WarningLevel="4" />
        <Config Name="Release" AllowUnsafeBlocks="false" BaseAddress="285212672" CheckForOverflowUnderflow="false" 
		ConfigurationOverrideFile="" DefineConstants="TRACE" DocumentationFile="" DebugSymbols="false" 
		FileAlignment="4096" IncrementalBuild="true" NoStdLib="false" NoWarn="" Optimize="true" 
		OutputPath="bin\Release\" RegisterForComInterop="false" RemoveIntegerChecks="false" 
		TreatWarningsAsErrors="false" WarningLevel="4" />
      </Settings>
      <References>
        $Root.References: {
			$if(it.Assembly)$
				<Reference Name="$it.Assembly$" AssemblyName="$it.Assembly$" 
				$if(it.HintPath)$  
					HintPath="$it.HintPath$" />
				$else$
					/>
				$endif$
			$endif$
			$if(it.Project)$
				<Reference Name="$it.Project$" Project = "{$it.GUID$}" Package = "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}"/>
			$endif$
        }$
        
      </References>
    </Build>
    <Files>
      <Include>
		$Root.Items: {
			<File RelPath="$it.Name$" 
			$if(it.Link)$Link="$it.Link$"$endif$
			$if(it.SubType)$SubType="$it.SubType$"$endif$
			$if(it.DependentUpon)$DependentUpon="$it.DependentUpon$"$endif$
			BuildAction="$it.BuildAction$" />
        }$
      </Include>
    </Files>
  </CSHARP>
</VisualStudioProject>
