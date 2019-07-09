# chess.blazor

Simple client-side [Blazor](https://docs.microsoft.com/en-us/aspnet/core/blazor/?view=aspnetcore-3.0) site thats fronts my [chess.webapi](..\chess.webapi) service to play a game of chess.

Created for direct comparison of approaches to creating single-page-applications.

See also;

* [chess.webapi](..\chess.webapi)
* [chess.reactredux](..\chess.reactredux)

## Notes

As time of writing (02/07/19) the blazor framework is still in preview (solution requires .NET Core 3.0 preview6 to work). 

# DEPLOYMENT
Deployment to Azure Static Storage currently accomplished via using dotnet to publish to a file and then using the Azure Storage extension in VS Code to up load the file to my static storage
* VS: Right click on chess.blazor projectand select publish...
* VS: Create a profile to publish to a Folder somewhere and publish to it
* VSCODE: Open the publish folder publish location in VS Code and having installed Azure Storage extension, right click on the `dist` folder found under `...\publish\chess.blazor` and select Deploy to Static Website...
* VSCODE: A dropdown appears at the top of VS code to select which subscription, select the correct subscription and storage account. *NOTE* Transfer will start immediately so make sure you click the right things!
