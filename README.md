# PowerApps Plugins


These Dynamics 365 PowerApps plugins are created as an example to show how to create, register, and deploy Plugins in a PowerApps hosted on Dynamics 365 using VS extensions and the Plugin Registration Tool (PRT).

### Required Tools & Assemblies
- [ ] .NET Framework 4.6.2 (Right-click Project > Select Properties > Application Target Framework .NET 4.6.2)
- [ ] Microsoft.CrmSdk.CoreAssemblies (Use NuGet Package Manager)
- [ ] Power Dev Tools - Extensions > Manage Extensions. Search for Power Platform Tools for VS. Install and restart VS.
- [ ] Plugin Registration Tool (PRT). There are a few different ways to obtain this Tool. I had difficulty and the one that worked for me was the CLI download
```dotnet tool install --global Microsoft.PowerApps.CLI.Tool```
> To access PRT in terminal type ```pac tool prt```

### Instructions - Creating C# Plugin
Create a new Project in VS.
- Use template: Power Platforms Solutions
- Connect to Dataverse. Tools > Connect to Dataverse. Enter your login credentials. If multiple environments be sure to click the checkbox to show all environments.
- Scroll to entity in PowerApp Explorer and right click create Plugin. This will create the boilerplate code.
- Add your personalized Business Logic under the TODO comment in ExecuteCDSPlugin method.
- Save.
- Right click the project in the Solution Explorer.
- In the main window on the left you will see various options. Select "signing".
- Choose a strong name key file. Click OK.
- With Plugin project highlighted in Solution Explorer
- Go to Build > Deploy Solution

### Instructions - Registering a Plugin

With the Power Platform Solution Template after clicking Deploy a window will pop up to setup which Message(Event) triggers the Plugin along with some other options.
If you get this right the first time. You are done. Congratulations!

> However, if you need to make changes to the registration of the event you will need to use the Public Registration Tool listed above.
- run the terminal command ```pac tool prt```
- connect to dataverse
- wait for everything to load
- find your assembly in the list.
- Expand your assembly.
- Select Step.
- Click the update button at the top of the screen.
- Make any necessary changes.
- Save. Close the window.

### Log Changes to Student Data | [cs file](https://github.com/theGaryLarson/PowerApps/blob/main/Plugins/PostOperationin23gl_studentUpdate.cs)
On lines 78 and 79 there is a reference to preimage and postimage entity objects. Before this will work the PRT must be used on the step to register PreImage and PostImages with the Step.
1. Open PRT ```pac tool prt```
2. Login

 &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;![](resources/prt-login.png)<br></br>
Be sure to select 
- [ ] Office 365
- [ ] Display list of available organizations

3.  Choose organization

 &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;![](resources/choose-org.png)<br></br>
 
5. Find your assembly. Expand. Right-click step
   
 &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;![](resources/register-new-image.png)<br></br>
6. Selecting Register new image will give this window

 &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;![](resources/new-image-options.png)<br></br>
 - [ ] **Pre Image:** The state of the data pre-operations
 - [ ] **Post Image:** The state of the data post-operations
- Name: is the display name
- **Entity Alias:** is the name you will use in code. It is case-sensitive.
- **Parameters:** By default all parameters are selected. You want to change this by pressing the ```...``` button and selecting the fields that are relevant for your process. Or else there will be performance issues.

7. Update Existing image
  
 &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;![](resources/update-existing-image.png)<br></br>
 
 - This may not be needed but it is good to know you have an option to make changes if needed.
 - After clicking update. I recommend selecting the assembly and clicking the register button just to be sure it is updated to the organization.
