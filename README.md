
![Logo](My%20Project/media/logo.png)
<p align=center>Robert McAnany 2022

**Contributors**:
@farfilli (aka @Fiorini), @daysanduski

**Helpful feedback and bug reports:**
@Satyen, @n0minus38, @wku, @aredderson, @bshand, @TeeVar, @SeanCresswell, 
@Jean-Louis, @Jan_Bos, @MonkTheOCD_Engie, @[mike miller], @Fiorini, 
@[Martin Bernhard], @Derek G, @Chris42, @Jason1607436093479, @Bob Henry, 
@JayJay101

![Interface_Large](My%20Project/media/interface_1100x830.png)

## DESCRIPTION

Solid Edge Housekeeper helps you find annoying little errors in your project. 
It can identify failed features in 3D models, detached dimensions in drawings, 
missing parts in assemblies, and more.  It can also update certain individual 
file settings to match those in a template you specify.

## GETTING HELP

Ask questions or suggest improvements on the 
[**Solid Edge Forum**](https://community.sw.siemens.com/s/topic/0TO4O000000MihiWAC/solid-edge)

To subscribe to update notices or volunteer to be a beta tester, 
message me, RobertMcAnany, on the forum. 
(Click your profile picture, then `My Messages`, then `Create`). 
Unsubscribe the same way.  To combat bots and spam, I will probably 
ignore requests from `User16612341234...`. 
(Change your nickname by clicking your profile picture, then 
`My Profile`, then `Edit`). 


If you want to help out on Housekeeper, the easiest way may be to 
become a beta tester.  If you know .NET, or want to learn, there's more
to do!  To get started on GitHub collaboration, head over to
[**ToyProject**](https://github.com/rmcanany/ToyProject). 
There are instructions and links to get you up to speed.


## INSTALLATION

There is no installation per se.  The preferred method is to download or clone 
the project and compile it yourself.

The other option is to use the latest 
[**Release**](https://github.com/rmcanany/SolidEdgeHousekeeper/releases). 
It will be the top entry on the page. 


![Release Page](My%20Project/media/release_page.png)

Click the 
file SolidEdgeHousekeeper-vx.x.x.zip 
(sometimes hidden under the Assets dropdown). 
It should prompt you to save it. 
Choose a convenient location on your machine. 
Extract the zip file (probably by right-clicking and selecting Extract All). 
Double-click the .exe file to run.

If you are upgrading from a previous release, you should be able to copy 
the settings files from the old version to the new. 
The files are 'defaults.txt', 'property_filters.txt', and 'filename_charmap.txt'. 
If you haven't used Property Filter, 'property_filters.txt' won't be there. 
Versions prior to 0.1.10 won't have 'filename_charmap.txt' either.

## OPERATION

![Tabs](My%20Project/media/tabs.png)

On each file type's tab, select which errors to detect. 
On the General tab, browse to the desired input folder, 
then select the desired file search option. 
You can refine the search using a file filter, a property filter, or both. 
See the **File Selection** section below. 

If any errors are found, a log file will be written to the input folder. 
It will identify each error and the file in which it occurred. 
When processing is complete, the log file is opened in Notepad for review.

The first time you use the program, some site-specific information is needed. 
This includes the location of your templates, material table, etc. 
These are populated on the Configuration Tab.

![Tabs](My%20Project/media/stop_button.png)

You can interrupt the program before it finishes.  While processing, 
the Cancel button changes to a Stop button.  Just click that to halt 
processing.  It may take several seconds to register the request.  It 
doesn't hurt to click it a couple of times.

## CAVEATS

Since the program can process a large number of files in a short amount of time, 
it can be very taxing on Solid Edge. 
To maintain a clean environment, the program restarts Solid Edge periodically. 
This is by design and does not necessarily indicate a problem.

However, problems can arise. 
Those cases will be reported in the log file with the message 'Error processing file'. 
A stack trace will be included.  The stack trace looks scary, but may be useful for program debugging. 
If four of these errors are detected in a run, the programs halts with the 
Status Bar message 'Processing aborted'.

Please note this is not a perfect program.  It is not guaranteed not to mess up your files.  Back up any files before using it.

## KNOWN ISSUES

#### Does not support managed files

Cause: Unknown.  Possible workaround: Process the files in an unmanaged 
workspace.  

*Update 10/10/2021* Some users have reported success with BiDM managed files. 

*Update 1/25/2022* One user has reported success with Teamcenter 'cached' files. 

#### Older Solid Edge versions

Some tasks may not support versions of Solid Edge prior to SE2020. 
Cause: Maybe an API call not available in previous versions. 
Possible workaround: Use SE2020 or later. 

#### Multiple installed Solid Edge versions

May not support multiple installed versions on the same machine. 
Cause: Unknown. 
Possible workaround: Use the version that was 'silently' installed. 

#### Printer settings

Does not support all printer settings, e.g., duplexing, collating, etc. 
Cause: Not exposed in the DraftPrintUtility() API. 
Possible workaround: Create a new Windows printer with the desired settings. 
Refer to the TESTS AND ACTIONS topic below for more details. 

#### Pathfinder during Interactive Edit

Pathfinder is sometimes blank when running the 'Interactive Edit' task. 
Cause: Unknown. 
Possible workaround: Refresh the screen by minimizing and maximizing the Solid Edge window. 



## SELECTION TOOLBAR

The Selection Toolbar is where you select what files to process. 
With the toolbar, you can select by folder, by top-level assembly, 
by list, or by files with errors from a previous run. 

Another option is to drag and drop from Windows File Explorer. 
You can use drag and drop and the toolbar in combination.

The toolbar functions are explained below.

![Toolbar](My%20Project/media/folder_toolbar.png)

### 1. Select by Folder

Choose this option if you want to select files within a single folder, 
or a folder and its subfolders.  You can select any number of each.
Referring to the diagram, click the the icon marked **a** to select a 
single folder, click the icon marked **b** to include sub folders.

### 2. Select by Top-Level Assembly

Choose this option if you want to select files linked to an assembly.
Again referring to the diagram, click **a** to choose the assembly, 
click **b** to choose where to look for _where used_ files for 
the assembly.  You can select any number of _where used_ folders.

If you don't specify any folders, Housekeeper simply finds
files contained in the specified assembly and subassemblies without 
performing a _where used_ on them.

If you _do_ specify one or more folders, there are two options on how 
the _where used_ is performed, **Top Down** or **Bottom Up**.  Make 
this selection on the **Configuration Tab**.

#### Bottom Up

Bottom up is meant for general purpose directories 
(e.g., `\\BIG_SERVER\all_parts\`), where the number of files 
in the folder(s) far exceed the number of files in the assembly. 
The program gets links by recursion, then 
finds draft files with _where used_. 

A bottom up search requires a valid Fast Search Scope filename, 
(e.g., `C:\Program Files\...\Preferences\FastSearchScope.txt`), 
which tells the program if the specified folder is on an indexed drive. 
Set the Fast Search Scope filename on the **Configuration Tab**.

#### Top Down

Top down is meant for self-contained project directories 
(e.g., `C:\Projects\Project123\`), where most of the files 
in the folder(s) are related to the assembly. 
The program opens every file within and below the input directory. 
As it does, it creates a graph representation of the links. 
The graph is subsequently traversed to find related files. 
I don't know how it works; my son did that part. 

A top down search can optionally report files with no links to the 
top level assembly.  It is set on the **Configuration Tab**.

### 3. Select by list

Referring to the diagram, click **a** to import a list, 
click **b** export one.  

If you are importing a list from another source, be aware that the 
file names must contain the full path.  E.g.,
`D:\Projects\Project123\Partxyz`, not just `Project123\Partxyz`.

### 4. List operations

#### Select files with errors from the previous run

Click **a** to select only files that encountered an error. 
All other files will be removed from the list.  To reproduce the 
previous TODO list functionality, you can export the list if
needed.

#### Remove all

Click **b** to remove all folders and files from the list.

#### RMB shortcut menu

![RMB Shortcut Menu](My%20Project/media/RMB_shortcut_menu.png)

If you select one or more files on the list, you can click the right 
mouse button for more options.  Use **Open** to view the files in 
Solid Edge, **Open folder** to view them in File Explorer, 
**Process selected** to run selected Tasks on only those files, 
and finally **Remove from list** to move them to the *Excluded files* 
section of the list.

### 5. Update

The update button processes the selections made above.  If a change is
made to the selections or a file filter (see next), an update
is required.

In the above image, you will notice a checkbox
![Error](Resources/icons8_unchecked_checkbox_16.png) to the left of
the file name.  When a file is processed successfully, a checkmark 
![Error](Resources/icons8_Checked_Checkbox_16.png) is shown. 
If some task encountered an issue, an error indicator 
![Error](Resources/icons8_Error_16.png) is shown instead.

## FILTER TOOLBAR

![Filter Toolbar](My%20Project/media/filter_toolbar.png)

Filters are a way to refine the list of files to process.  You can filter 
on file properties, filenames (with a wildcard search), or file type. 
They can be used alone or in any combination.

### Property Filter

To configure a property filter, click the tool icon to the right of
the Property filter checkbox.  For details on the property search, 
see the Readme tab on the Property Filter dialog. 

### Wildcard Filter

Filtering by wildcard is done by entering the wildcard pattern in the 
provided combobox.  Wildcard patterns are automatically saved for 
future use.  Delete a pattern that is no longer needed by selecting it 
and clicking the red 'X' icon. 

Internally, the search is implemented with the VB `Like` operator, 
which is similar to the old DOS wildcard search, but with a few more options. 
For details and examples, see 
[**VB Like Operator**](https://docs.microsoft.com/en-us/dotnet/visual-basic/language-reference/operators/like-operator).

### File Type Filter

Filtering by file type is done by checking/unchecking the appropriate 
 Type filter icon. 

...


## TASK DESCRIPTIONS

### Assembly

#### Open/Save
Open a document and save in the current version.

#### Activate and update all
Loads all assembly occurrences' geometry into memory and does an update. Used mainly to eliminate the gray corners on assembly drawings. 

Can run out of memory for very large assemblies.

#### Property find replace
Searches for text in a specified property and replaces it if found. The property, search text, and replacement text are entered on the task tab, below the task list. 

A `Property set`, either `System` or `Custom`, is required. System properties are in every Solid Edge file. They include Material, Manager, Project, etc. At this time, they must be in English. Custom properties are ones that you create, probably in a template. 

The search is case insensitive, the replace is case sensitive. For example, say the search is `aluminum`, the replacement is `ALUMINUM`, and the property value is `Aluminum 6061-T6`. Then the new value would be `ALUMINUM 6061-T6`. 

#### Expose variables missing
Checks to see if all the variables listed in `Variables to expose` are present in the document.

#### Expose variables
Enter the names as a comma-delimited list in the `Variables to expose` textbox. Optionally include a different Expose Name, set off by the colon `:` character. 

For example

`var1, var2, var3`

Or

`var1: Variable Name One, var2: Variable Name 2, var3: Variable Name 3`

Or a combination

`var1: Variable Name One, var2, var3`

Note: You cannot use either a comma `,` or a colon `:` in the Expose Name. Actually you can, but it will not do what you expect. 

#### Remove face style overrides
Face style overrides change a part's appearance in the assembly. This command causes the part to appear the same in the part file and the assembly.

#### Update face and view styles from template
Updates the file with face and view styles from a file you specify on the Configuration tab. 

Note, the view style must be a named style.  Overrides are ignored. To create a named style from an override, use `Save As` on the `View Overrides` dialog.

#### Hide constructions
Hides all non-model elements such as reference planes, PMI dimensions, etc.

#### Fit pictorial view
Maximizes the window, sets the view orientation, and does a fit.

Select the desired orientation on the Configuration Tab.

#### Part number does not match file name
Checks if a file property, that you specify on the Configuration tab, matches the file name.

#### Missing drawing
Assumes drawing has the same name as the model, and is in the same directory

#### Broken links
Checks to see if any assembly occurrence is pointing to a file not found on disk.

#### Links outside input directory
Checks to see if any assembly occurrence resides outside the top level directories specified on the General tab. 

#### Failed relationships
Checks if any assembly occurrences have conflicting or otherwise broken relationships.

#### Underconstrained relationships
Checks if any assembly occurrences have missing relationships.

#### Run external program
Runs an `\*.exe` or `\*.vbs` file.  Select the program with the `Browse` button. It is located on the task tab below the task list. 

Several rules about the program implementation apply. See **[HousekeeperExternalPrograms](https://github.com/rmcanany/HousekeeperExternalPrograms)** for details and examples. 

#### Interactive edit
Brings up files one at a time for manual processing.  Some rules apply.

It is important to leave Solid Edge in the state you found it when the file was opened. For example, if you open another file, such as a drawing, you need to close it. If you add or modify a feature, you need to click Finish. 

Also, do not Close the file or do a Save As on it. Housekeeper maintains a `reference` to the file. Those two commands cause the reference to be lost, resulting in an exception. 

#### Save as
Exports the file to a non-Solid Edge format. 

Select the file type using the `Save As` combobox. Select the directory using the `Browse` button, or check the `Original Directory` checkbox. These controls are on the task tab below the task list. 

Images can be saved with the aspect ratio of the model, rather than the window. The option is called `Save as image -- crop to model size`. It is located on the Configuration tab. 

You can optionally create subdirectories using a formula similar to the Property Text Callout. For example `Material %{System.Material} Thickness %{Custom.Material Thickness}`. The PropertySet designation, `System.` or `Custom.` is required. These refer to where the property is stored in a Solid Edge file. 

System properties are in every Solid Edge file. They include Material, Project, etc. Note, at this time, the System property names must be specified in English. Custom properties are ones that you create, probably in a template. The custom property names can be in any language.  (In theory, at least -- not tested at this time.)

It is possible that a property contains a character that cannot be used in a file name. If that happens, a replacement is read from filename_charmap.txt in the same directory as Housekeeper.exe. You can/should edit it to change the replacement characters to your preference. The file is created the first time you run Housekeeper.  For details, see the header comments in that file. 

### Part

#### Open/Save
Same as the Assembly command of the same name.

#### Property find replace
Same as the Assembly command of the same name.

#### Expose variables missing
Same as the Assembly command of the same name.

#### Expose variables
Same as the Assembly command of the same name.

#### Update face and view styles from template
Same as the Assembly command of the same name.

#### Update material from material table
Checks to see if the part's material name and properties match any material in a file you specify on the Configuration tab. 

If the names match, but their properties (e.g., face style) do not, the material is updated. If the names do not match, or no material is assigned, it is reported in the log file.

#### Hide constructions
Same as the Assembly command of the same name.

#### Fit pictorial view
Same as the Assembly command of the same name.

#### Update insert part copies
In conjuction with `Assembly Activate and update all`, used mainly to eliminate the gray corners on assembly drawings.

#### Broken links
Same as the Assembly command of the same name.

#### Part number does not match file name
Same as the Assembly command of the same name.

#### Missing drawing
Same as the Assembly command of the same name.

#### Failed or warned features
Checks if any features of the model are in the Failed or Warned status.

#### Suppressed or rolled back features
Checks if any features of the model are in the Suppressed or Rolledback status.

#### Underconstrained profiles
Checks if any profiles are not fully constrained.

#### Insert part copies out of date
If the file has any insert part copies, checks if they are up to date.

#### Material not in material table
Checks the file's material against the material table. The material table is chosen on the Configuration tab. 

#### Run external program
Same as the Assembly command of the same name.

#### Interactive edit
Same as the Assembly command of the same name.

#### Save As
Same as the Assembly command of the same name.

### Sheetmetal

#### Open/Save
Same as the Assembly command of the same name.

#### Property find replace
Same as the Assembly command of the same name.

#### Expose variables missing
Same as the Assembly command of the same name.

#### Expose variables
Same as the Assembly command of the same name.

#### Update face and view styles from template
Same as the Part command of the same name.

#### Update material from material table
Same as the Part command of the same name.

#### Hide constructions
Same as the Assembly command of the same name.

#### Fit pictorial view
Same as the Assembly command of the same name.

#### Update insert part copies
Same as the Part command of the same name.

#### Update design for cost
Updates DesignForCost and saves the document.

An annoyance of this command is that it opens the DesignForCost Edgebar pane, but is not able to close it. The user must manually close the pane in an interactive Sheetmetal session. The state of the pane is system-wide, not per-document, so closing it is a one-time action. 

#### Broken links
Same as the Assembly command of the same name.

#### Part number does not match file name
Same as the Part command of the same name.

#### Missing drawing
Same as the Assembly command of the same name.

#### Failed or warned features
Same as the Part command of the same name.

#### Suppressed or rolled back features
Same as the Part command of the same name.

#### Underconstrained profiles
Same as the Part command of the same name.

#### Insert part copies out of date
Same as the Part command of the same name.

#### Flat pattern missing or out of date
Checks for the existence of a flat pattern. If one is found, checks if it is up to date. 

#### Material not in material table
Same as the Part command of the same name.

#### Run external program
Same as the Assembly command of the same name.

#### Interactive edit
Same as the Assembly command of the same name.

#### Save As
Same as the Assembly command of the same name, except two additional options -- `DXF Flat (\*.dxf)` and `PDF Drawing (\*.pdf)`. 

The `DXF Flat` option saves the flat pattern of the sheet metal file. 

The `PDF Drawing` option saves the drawing of the sheet metal file. The drawing must have the same name as the model, and be in the same directory. A more flexible option may be to use the Draft `Save As`, using a `Property Filter` if needed. 

### Draft

#### Open/Save
Same as the Assembly command of the same name.

#### Update drawing views
Checks drawing views one by one, and updates them if needed.

#### Update styles from template
Creates a new file from a template you specify on the Configuration tab. Copies drawing views, dimensions, etc. from the old file into the new one. If the template has updated styles, a different background sheet, or other changes, the new drawing will inherit them automatically. 

This task has the option to `Allow partial success`.  It is set on the Configuration tab. If the option is set, and some drawing elements were not transferred, it is reported in the log file. Also reported in the log file are instructions for completing the transfer. 

Note, because this task needs to do a `Save As`, it must be run with no other tasks selected.

#### Update drawing border from template
Replaces the background border with that of the Draft template specified on the Configuration tab.

In contrast to `UpdateStylesFromTemplate`, this command only replaces the border. It does not attempt to update styles or anything else.

#### Fit view
Same as the Assembly command of the same name.

#### File name does not match model file name
Same as the Assembly command of the same name.

#### Broken links
Same as the Assembly command of the same name.

#### Drawing views out of date
Checks if drawing views are not up to date.

#### Detached dimensions or annotations
Checks that dimensions, balloons, callouts, etc. are attached to geometry in the drawing.

#### Parts list missing or out of date
Checks is there are any parts list in the drawing and if they are all up to date.

#### Run external program
Same as the Assembly command of the same name.

#### Interactive edit
Same as the Assembly command of the same name.

#### Print
Print settings are accessed on the Configuration tab.

Note, the presence of the Printer Settings dialog is somewhat misleading. The only settings taken from it are the printer name, page height and width, and the number of copies. Any other selections revert back to the Windows defaults when printing. A workaround is to create a new Windows printer with the desired defaults. 

Another quirk is that, no matter the selection, the page width is always listed as greater than or equal to the page height. In most cases, checking `Auto orient` should provide the desired result. 

#### Save As
Same as the Assembly command of the same name, except as follows.

Optionally includes a watermark image on the output.  For the watermark, set X/W and Y/H to position the image, and Scale to change its size. The X/W and Y/H values are fractions of the sheet's width and height, respectively. So, (`0,0`) means lower left, (`0.5,0.5`) means centered, etc. Note some file formats may not support bitmap output.

The option `Use subdirectory formula` can use an Index Reference designator to select a model file contained in the draft file. This is similar to Property Text in a Callout, for example, `%{System.Material|R1}`. To refer to properties of the draft file itself, do not specify a designator, for example, `%{Custom.Last Revision Date}`. 


## CODE ORGANIZATION

Processing starts in Form1.vb.  A short description of the code's organization can be found there.

