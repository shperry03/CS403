Sam Perry and Erik Buinevicius

This program is designed to interpret and execute files written in the Learn language, created by SP and EB.
The Syntax for Learn is derived from the Lox language found at CraftingInterpreters.com.
The results of output are directed to the command line. 

Due to the fact that this Learn interpreter is written in C#, we opted to create a dotnet application for our program to run on.
This means that dotnet version 3.1 or greater must be installed on the machine to run this Learn interpreter.

CREATING TEST FILES:
    To generate test files, the user must simply create a new .txt file in the same directory as
    the C# files (final_project/), with correct Learn syntax.

RUNNING TEST FILES:
    The following command is used to execute a Learn file in the terminal:
    > dotnet run "filename"

    Example: Testfile --> TestLearn1.txt
    Execution: dotnet run TestLearn1.txt

    The output will be written in the console for the user to view.

RUNNING CODE LINE BY LINE:
    The user may also choose to execute Learn commands one at a time, 
    by running the following command (don't specify a filename):
    > dotnet run 

    Once this command is executed, a Learn prompt will begin running, in which the user can input simple Learn statements.

COMPILATION / RUN INSTRUCTIONS
1. Ensure that an up-to-date dotnet version is installed on your machine (verify using dotnet --list-sdks).
2. Execute the command "dotnet run 'filename'" to run a pre-written Learn file, or "dotnet run" to open the line-by-line Learn prompt.
3. That's it! Your Learn interpreter should be good to go. 

ADDITIONAL INFORMATION
    Additional information about the Learn language, its use cases, and its functionality can be found in the supplemental PowerPoint/Document
