function Create-Nuspec
{
	<#
	.Synopsis
		Creates nuspec-files from projects files.
	#>
	param
	(
		[Parameter(HelpMessage = "Path to the solution directory.")]
		[String] $solutionDir = '.',

		[Parameter(HelpMessage = "Path to the solution output directory.")]
		[String] $outputDir = 'Assemblies',

		[Parameter(HelpMessage = "Path to GlobalAssemblyInfo.cs.")]
		[String] $assemblyInfo = '.files\Packaging\GlobalAssemblyInfo.cs',

		[Parameter(HelpMessage = "VCS branch name.")]
		[String] $branchName = '',

		[Parameter(HelpMessage = "VCS commit hash.")]
		[String] $commitHash = '',

		[Parameter(HelpMessage = ".NET version.")]
		[String] $framework = 'net45'
	)

	process
	{
		### Build the version number

		$version = Get-Content $assemblyInfo `
			| Select-String -Pattern 'AssemblyVersion\s*\(\s*\"(?<version>.*?)\"\s*\)' `
			| ForEach-Object { $_.Matches[0].Groups['version'].Value }

		Write-Host $version
		Write-Host $branchName

		### Create nuspec-file for project

		$project = Get-ChildItem -Path $solutionDir -Filter 'InfinniPlatform.Watcher.csproj' -Recurse | Select-Object -First 1

		[xml] $projectXml = Get-Content $project.FullName

		$projectRefs = @()
		$projectName = $project.BaseName
		$projectAssemblyName = ($projectXml.Project.PropertyGroup.AssemblyName[0])

		Write-Host "Create $projectName.nuspec"

		$projectNuspec =
			"<?xml version=""1.0"" encoding=""utf-8""?>`r`n" + `
			"<package xmlns=""http://schemas.microsoft.com/packaging/2011/08/nuspec.xsd"">`r`n" + `
			"    <metadata>`r`n" + `
			"        <id>$projectName</id>`r`n" + `
			"        <version>$version</version>`r`n" + `
			"        <authors>Infinnity Solutions</authors>`r`n" + `
			"        <owners>Infinnity Solutions</owners>`r`n" + `
			"        <requireLicenseAcceptance>false</requireLicenseAcceptance>`r`n" + `
			"        <description>Commit $commitHash</description>`r`n" + `
			"        <copyright>Infinnity Solutions $(Get-Date -Format yyyy)</copyright>`r`n" + `
			"        <dependencies>`r`n"

		# Add external dependencies

		$packagesConfigPath = Join-Path $project.Directory.FullName 'packages.config'

		if (Test-Path $packagesConfigPath)
		{
			[xml] $packagesConfigXml = Get-Content $packagesConfigPath

			$packages = $packagesConfigXml.packages.package

			if ($packages)
			{
				foreach ($package in $packages)
				{
					$projectNuspec = $projectNuspec + "            <dependency id=""$($package.id)"" version=""$($package.version)"" />`r`n"
				}
			}
		}

		$projectRefs += $projectXml.Project.ItemGroup.Reference.HintPath | Where { $_ -like '..\packages\*.dll' } | % { $_ -replace '^\.\.\\packages\\', '' }

		# Add internal dependencies

		$projectReferences = $projectXml.Project.ItemGroup.ProjectReference.Name | Sort-Object | Get-Unique -AsString

		if ($projectReferences)
		{
			foreach ($projectReference in $projectReferences)
			{
				$projectNuspec = $projectNuspec + "            <dependency id=""$projectReference"" version=""[$version]"" />`r`n"
			}
		}

		$projectNuspec = $projectNuspec + `
			"        </dependencies>`r`n" + `
			"    </metadata>`r`n" + `
			"    <files>`r`n"

		# Add project assembly

		$projectNuspec = $projectNuspec + "        <file target=""lib\$framework"" src=""$projectAssemblyName.dll"" />`r`n"
		$projectRefs += "$projectName.$version\lib\$framework\$projectAssemblyName.dll"

		# Add resources for ru-RU (if exists)

		$projectResourcesRu = $projectXml.Project.ItemGroup.EmbeddedResource.Include | Where { $_ -like '*.ru-RU.*' }

		if ($projectResourcesRu -and $projectResourcesRu.Count -gt 0 -and $projectResourcesRu[0])
		{
			$projectNuspec = $projectNuspec + "        <file target=""lib\$framework\ru-RU"" src=""ru-RU\$projectAssemblyName.resources.dll"" />`r`n"
			$projectRefs += "$projectName.$version\lib\$framework\ru-RU\$projectAssemblyName.resources.dll"
		}

		# Add resources for en-US (if exists)

		$projectResourcesEn = $projectXml.Project.ItemGroup.EmbeddedResource.Include | Where { $_ -like '*.en-US.*' }

		if ($projectResourcesEn -and $projectResourcesEn.Count -gt 0 -and $projectResourcesEn[0])
		{
			$projectNuspec = $projectNuspec + "        <file target=""lib\$framework\en-US"" src=""en-US\$projectAssemblyName.resources.dll"" />`r`n"
			$projectRefs += "$projectName.$version\lib\$framework\en-US\$projectAssemblyName.resources.dll"
		}

		# Add symbol file

		$projectNuspec = $projectNuspec + "        <file target=""lib\$framework"" src=""$projectAssemblyName.pdb"" />`r`n"
		$projectRefs += "$projectName.$version\lib\$framework\$projectAssemblyName.pdb"

		# Add XML-documentation (if exists)

		$projectDocs = $projectXml.Project.PropertyGroup.DocumentationFile

		if ($projectDocs -and $projectDocs.Count -gt 0 -and $projectDocs[0])
		{
			$projectNuspec = $projectNuspec + "        <file target=""lib\$framework"" src=""$projectAssemblyName.xml"" />`r`n"
		}

		$projectNuspec = $projectNuspec + `			
			"    </files>`r`n" + `
			"</package>"
		
		Set-Content (Join-Path $outputDir "$projectName.nuspec") -Value $projectNuspec
	}
}