$!Output=Xml!$
$!Root:Project!$
<Project DefaultTargets="Build" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <PropertyGroup>
    <ProjectType>Local</ProjectType>
    <ProductVersion>8.0.50727</ProductVersion>
    <SchemaVersion>2.0</SchemaVersion>
    <ProjectGuid>{$Root.GUID$}</ProjectGuid>
    <Configuration Condition=" '\$(Configuration)' == '' ">Debug</Configuration>
    <Platform Condition=" '\$(Platform)' == '' ">AnyCPU</Platform>
    <ApplicationIcon>$Root.ApplicationIcon$</ApplicationIcon>
    <AssemblyKeyContainerName>
    </AssemblyKeyContainerName>
    <AssemblyName>$Root.AssemblyName$</AssemblyName>
	<AssemblyOriginatorKeyFile>
    </AssemblyOriginatorKeyFile>
    <DefaultClientScript>JScript</DefaultClientScript>
    <DefaultHTMLPageLayout>Grid</DefaultHTMLPageLayout>
    <DefaultTargetSchema>IE50</DefaultTargetSchema>
    <DelaySign>false</DelaySign>
    <OutputType>$Root.OutputType$</OutputType>
    <RootNamespace>$Root.RootNamespace$</RootNamespace>
	<RunPostBuildEvent>OnBuildSuccess</RunPostBuildEvent>
    <StartupObject>
    </StartupObject>
    <FileUpgradeFlags>
    </FileUpgradeFlags>
    <UpgradeBackupLocation>
    </UpgradeBackupLocation>
  </PropertyGroup>
  <PropertyGroup Condition=" '\$(Configuration)|\$(Platform)' == 'Debug|AnyCPU' ">
    <OutputPath>bin\Debug\</OutputPath>
    <AllowUnsafeBlocks>false</AllowUnsafeBlocks>
    <BaseAddress>285212672</BaseAddress>
    <CheckForOverflowUnderflow>false</CheckForOverflowUnderflow>
    <ConfigurationOverrideFile>
    </ConfigurationOverrideFile>
    <DefineConstants>DEBUG;TRACE</DefineConstants>
    <DocumentationFile>
    </DocumentationFile>
    <DebugSymbols>true</DebugSymbols>
    <FileAlignment>4096</FileAlignment>
    <NoStdLib>false</NoStdLib>
    <NoWarn>
    </NoWarn>
    <Optimize>false</Optimize>
    <RegisterForComInterop>false</RegisterForComInterop>
    <RemoveIntegerChecks>false</RemoveIntegerChecks>
    <TreatWarningsAsErrors>false</TreatWarningsAsErrors>
    <WarningLevel>4</WarningLevel>
    <DebugType>full</DebugType>
    <ErrorReport>prompt</ErrorReport>
  </PropertyGroup>
  <PropertyGroup Condition=" '\$(Configuration)|\$(Platform)' == 'Release|AnyCPU' ">
    <OutputPath>bin\Release\</OutputPath>
    <AllowUnsafeBlocks>false</AllowUnsafeBlocks>
    <BaseAddress>285212672</BaseAddress>
    <CheckForOverflowUnderflow>false</CheckForOverflowUnderflow>
    <ConfigurationOverrideFile>
    </ConfigurationOverrideFile>
    <DefineConstants>TRACE</DefineConstants>
    <DocumentationFile>
    </DocumentationFile>
    <DebugSymbols>false</DebugSymbols>
    <FileAlignment>4096</FileAlignment>
    <NoStdLib>false</NoStdLib>
    <NoWarn>
    </NoWarn>
    <Optimize>true</Optimize>
    <RegisterForComInterop>false</RegisterForComInterop>
    <RemoveIntegerChecks>false</RemoveIntegerChecks>
    <TreatWarningsAsErrors>false</TreatWarningsAsErrors>
    <WarningLevel>4</WarningLevel>
    <DebugType>none</DebugType>
    <ErrorReport>prompt</ErrorReport>
  </PropertyGroup>
  <ItemGroup>
    $Root.References: {
			$if(it.Project)$
				<ProjectReference Include="$it.Folder$\\$it.Project$.csproj">
				  <Project>{$it.GUID$}</Project>
				  <Name>$it.Project$</Name>
				</ProjectReference>
			$else$
				<Reference Include="$it.Assembly$">
				$if(it.HintPath)$  
					<SpecificVersion>False</SpecificVersion>
					<HintPath>$it.HintPath$</HintPath>
				$endif$
				</Reference>
			$endif$
    }$
  </ItemGroup>
  <ItemGroup>
		$Root.Items: {
			<$it.BuildAction$ Include="$it.Name$">
				$if (it.Link)$
					<Link>$it.Link$</Link>
				$endif$
				$if (it.SubType)$
					<SubType>$it.SubType$</SubType>
				$endif$
				$if (it.DependentUpon)$
					<DependentUpon>$it.DependentUpon$</DependentUpon>
				$endif$
			</$it.BuildAction$>
        }$
  </ItemGroup>
  <ItemGroup>
    <Service Include="{B4F97281-0DBD-4835-9ED8-7DFB966E87FF}" />
  </ItemGroup>
  <Import Project="\$(MSBuildBinPath)\Microsoft.CSharp.targets" />
  <PropertyGroup>
    <PreBuildEvent>$Root.PreBuildEvent$
    </PreBuildEvent>
    <PostBuildEvent>$Root.PostBuildEvent$
    </PostBuildEvent>
  </PropertyGroup>
</Project>
