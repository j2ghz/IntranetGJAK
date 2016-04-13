#addin "Cake.Npm"
var target = Argument("target", "Default");

Task("Default").IsDependentOn("Build");

Task("Build")
	.IsDependentOn("Gulp")
	.Does(() =>
	{
		
	});
	
Task("Gulp")
	.IsDependentOn("Bower")
	.IsDependentOn("NPM")
	.Does(() =>
	{
		
	});

Task("Bower")
	.IsDependentOn("NPM")
	.Does(() =>
	{
		
	});
	
Task("NPM")
	.Does(() =>
	{
		Npm.Install();
	});

RunTarget(target);