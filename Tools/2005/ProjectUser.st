$!Output=Xml!$
$!Root:Project!$
<Project xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <PropertyGroup Condition=" '\$(Configuration)|\$(Platform)' == 'Debug|AnyCPU' ">
    $if(Root.StartProgram)$
    <StartAction>Program</StartAction>
    <StartProgram>$Root.StartProgram$</StartProgram>
    $endif$
  </PropertyGroup>
</Project>
