#addin "Cake.Npm"
var target = Argument("target", "Default");

Task("Default").IsDependentOn("Build");

Task("Build")
	.IsDependentOn("Gulp")
	.IsDependentOn("DNU")
	.Does(() =>
	{
		DNUBuild("./IntranetGJAK/*");
	});

Task("Gulp")
	.IsDependentOn("Bower")
	.IsDependentOn("NPM")
	.Does(() =>
	{
		StartProcess("gulp" + (IsRunningOnWindows() ? ".cmd"  : "") , new ProcessSettings{ WorkingDirectory = "./IntranetGJAK" });
	});

Task("Bower")
	.IsDependentOn("NPM")
	.Does(() =>
	{
		var settings = new ProcessSettings{ Arguments = "install", WorkingDirectory = "./IntranetGJAK" };
		StartProcess("bower" + (IsRunningOnWindows() ? ".cmd"  : "") , settings);
	});

Task("NPM")
	.Does(() =>
	{
		Npm.Install();
		Npm.Install(settings => settings.Package("gulp").Globally());
		Npm.Install(settings => settings.Package("bower").Globally());
	});

Task("DNU")
	.Does(() =>
	{
		DNURestore("./IntranetGJAK/project.json");
	});

RunTarget(target);
