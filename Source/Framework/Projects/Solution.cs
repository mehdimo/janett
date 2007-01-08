namespace Janett.Framework
{
	using System.Collections;
	using System.IO;
	using System.Reflection;

	using Janett.Commons;

	public class Solution
	{
		public string Folder;
		public string SolutionName;
		public IList Projects = new ArrayList();

		public Solution(string folder, string solutionName)
		{
			Folder = folder;
			SolutionName = solutionName;
		}

		public Project GetProject(string name)
		{
			foreach (Project project in Projects)
			{
				if (project.Name == name)
					return project;
			}
			return null;
		}

		public void Save()
		{
			Discovery dis = new Discovery();
			dis.AddAssembly(Assembly.GetExecutingAssembly());
			Stream stream = dis.GetResource("SolutionTemplate.txt");

			using (StreamReader reader = new StreamReader(stream))
			{
				string solutionContents = reader.ReadToEnd();

				string solutionPath = Path.Combine(Folder, SolutionName + ".sln");
				string projectsSection = "";
				foreach (Project project in Projects)
				{
					project.RelPath = Path.Combine(project.OutputFolder.Replace(Folder + Path.DirectorySeparatorChar, ""), project.Name + ".csproj");
					projectsSection += string.Format("Project(\"{{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}}\") = \"{0}\", \"{1}\", \"{2}\"\r\n" +
					                                 "\tProjectSection(ProjectDependencies) = postProject\r\n" +
					                                 "\tEndProjectSection\r\n" +
					                                 "EndProject\r\n", project.Name, project.RelPath, "{" + project.Guid + "}");
				}

				solutionContents = solutionContents.Replace("#Projects#", projectsSection.TrimEnd('\r', '\n'));
				string configurationTemplate = "\t\t{0}.Debug.ActiveCfg = Debug|.NET\r\n" +
				                               "\t\t{0}.Debug.Build.0 = Debug|.NET\r\n" +
				                               "\t\t{0}.Release.ActiveCfg = Release|.NET\r\n" +
				                               "\t\t{0}.Release.Build.0 = Release|.NET\r\n";
				string configuration = "";
				foreach (Project project in Projects)
				{
					configuration += string.Format(configurationTemplate, "{" + project.Guid + "}");
				}
				solutionContents = solutionContents.Replace("#Configuration#", "\r\n" + configuration + "\t");
				FileSystemUtil.WriteFile(solutionPath, solutionContents);
			}
		}
	}
}