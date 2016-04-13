#addin "Cake.Npm"
#addin "Cake.Gulp"
var target = Argument("target", "Default");

Task("Default").IsDependentOn("Build");

Task("Build")
	.IsDependentOn("Gulp")
	.IsDependentOn("DNU")
	.Does(() =>
	{
		DNUBuild("./*");
	});
	
Task("Gulp")
	.IsDependentOn("Bower")
	.IsDependentOn("NPM")
	.Does(() =>
	{
		Gulp.Local.Execute();
	});

Task("Bower")
	.IsDependentOn("NPM")
	.Does(() =>
	{
		try
		{
			StartProcess("bower.cmd", new ProcessSettings{ Arguments = "install" });
		}
		catch
		{
			StartProcess("bower", new ProcessSettings{ Arguments = "install" });
		}
		
	});
	
Task("NPM")
	.Does(() =>
	{
		Npm.Install();
		Npm.Install(settings => settings.Package("bower").Globally());
	});
	
Task("DNU")
	.Does(() =>
	{
		DNURestore();
	});

RunTarget(target);