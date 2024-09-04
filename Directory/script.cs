//puts the directory/naming convention guide onto a textpanel in game
//Reference length for general max length of a text line on an LCD
//"*******************************************"
public Program()
{
    //Controls the frequency at which the script is called possible choices are Update1, Update10, Update100
    //Recommended to stay at Update100 to have minimal impact from script, delay is not noticed
    Runtime.UpdateFrequency = UpdateFrequency.Update100;
}
//the main program
public void Main(string argument, UpdateType updateSource)
{
    //As this is the directory display script only calls one subfunction
    DisplayDirectories();
}


//methods called directly by the main program
public void DisplayDirectories()
{
    //ALL TEXTPANELS
    List<IMyTextPanel> allTextPanels = new List<IMyTextPanel>();
    //checks all textpanels by first clearing all data from the text panels and then finding all the current text panels in the grid
    allTextPanels.Clear();
    GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(allTextPanels, b => b.CubeGrid == Me.CubeGrid);
    for (int j = 0; j < allTextPanels.Count(); j++)
    {
        //check if the textpanel custom data is requesting the main directory
        if (allTextPanels[j].CustomData.ToLower().Replace(" ", "") == "directory")
        {
            //providing the list of available glossaries
            FormatTextPanel(allTextPanels[j]);
            allTextPanels[j].WriteText("MAIN DIRECTORY\n" +
                                     //"*******************************************"
                                       "Change the custom data of an LCD to the \n" +
                                       "following to learn how to adjust the\n" +
                                       "custom data of blocks for display,\n" +
                                       "stock maintanance, and auto sorting\n" +
                                       "Requires related scripts to function\n" +
                                       "Display: \"cargo display directory\"\n" +
                                       "Display and Assembly:\n" +
                                       "\"cargo display and assembly directory\"\n" +
                                       "Sorting: \"cargo sorting directory\"\n" +
                                       "Space Arm: \"space arm directory\"\n" +
                                       "Displays contain more information if\n" +
                                       "you open the screen text");
        }
        //check if the text panel custom data contains the term for the cargo display directory
        if (allTextPanels[j].CustomData.ToLower().Replace(" ", "").Contains("cargodisplaydirectory"))
        {
            FormatTextPanel(allTextPanels[j]);
            allTextPanels[j].WriteText("CARGO DISPLAY DIRECTORY\n" +
                                       "To display the inventories of items the\n" +
                                       "first line in the Custom Data field must\n" +
                                       "be \"cargodisplay\"\n" +
                                       
                                       "\n" +
                                       "Enter the items you want to see the\n" +
                                       "inventory of on the following lines\n" +
                                       "as subtypeId/displayname\n" +
                                       "Examples: \"Steel Plate/Steel Plate\"\n" +
                                       "and \"Large Tube/Large Steel Tubes\"\n" +
                                       "\"line break\" to get a line break." +

                                       "\n" +
                                       "Refer to .sbc files in \n" +
                                       "SpaceEngineers\\Content\\Data for subtypeId.\n" +

                                       "\n" +
                                       "Ores and Ingots cause an issue since they\n" +
                                       "share the same name. Include Ore or Ingot\n" +
                                       "as a suffix to the subtypeId to distinguish.\n" +
                                       
                                       "\n" +
                                       "Hydrogen and Oxygen displays are custom\n" +
                                       "hydrogen storage/\n" +
                                       "hydrogen capacity/\n" +
                                       "hydrogen tank count/\n" +
                                       "and the same for Oxygen as the subtypeId\n" +
                                       "equivalents for use.\n"+
                                     //"*******************************************"
                                       "\n"+
                                       "Production goals can be displayed by using\n"+
                                       "a \"=\"\n" +
                                       "Example:\n" +
                                       "Steel Plate/Steel Plate=1000\n" +
                                       "Displays as:\n" +
                                       "Steel Plate: 0/1000\n" +
                                       
                                       "\n" +
                                       "You may also refer to my list I made found\n" +
                                       "here: https://github.com/Potateau/Space-Engineers/wiki/SubtypeId-Reference\n" +
                                       "for subtypeId.\n"+
                                       "");
        }
        //checks if the text panel custom data contains the term for cargo display and assembly directory
        if (allTextPanels[j].CustomData.ToLower().Replace(" ", "").Contains("cargodisplayandassemblydirectory"))
        {
            FormatTextPanel(allTextPanels[j]);
            allTextPanels[j].WriteText("CARGO DISPLAY AND ASSEMBLY DIRECTORY\n" +
                                     //"*******************************************"
                                       "To display the inventories of items and\n"+
                                       "control stock of those items the first line\n"+
                                       "in the Custom Data field must be\n"+
                                       "\"cargodisplayandassembly\"\n"+
                                       
                                       "\n"+
                                       "Enter the items you want to see the\n" +
                                       "inventory of on the following lines\n" +
                                       "as subtypeId/displayname=number\n" +
                                       "Examples: \"Steel Plate/Steel Plate=1000\"\n" +
                                       "and \"Large Tube/Large Steel Tubes=100\"\n" +
                                       "\"line break\" to get a line break." +
                                       
                                       "\n" +
                                       "Refer to .sbc files in \n" +
                                       "SpaceEngineers\\Content\\Data for subtypeId.\n" +
                                       
                                       "\n" +
                                       "Ores and Ingots cause an issue since they\n" +
                                       "share the same name. Include Ore or Ingot\n" +
                                       "as a suffix to the subtypeId to distinguish.\n" +
                                       
                                       "\n"+
                                       "Highly recommended to have all but one\n"+
                                       "assembler set to cooperative mode.  Script\n"+
                                       "will not auto queue to assemblers in\n"+
                                       "cooperative mode.\n"+
                                       
                                       "\n" +
                                       "Hydrogen and Oxygen displays are custom\n" +
                                       "hydrogen storage/\n" +
                                       "hydrogen capacity/\n" +
                                       "hydrogen tank count/\n" +
                                       "and the same for Oxygen as the subtypeId\n" +
                                       "equivalents for use.\n"+
                                     //"*******************************************"
                                       "\n"+
                                       "Production goals do not need to be set to\n"+
                                       "display inventory.  Simply do not enter the\n"+
                                       "= sign or write a number on the line to\n"+
                                       "only use the display functionality.\n"+
                                       
                                       "\n"+
                                       "You may also refer to my list I made found\n" +
                                       "here: https://github.com/Potateau/Space-Engineers/wiki/SubtypeId-Reference\n" +
                                       "for subtypeId.\n"+
                                       "");
        }
        //checks if the text panel custom data contains the term for the sorting directory
        if (allTextPanels[j].CustomData.ToLower().Replace(" ", "").Contains("cargosortingdirectory"))
        {
            FormatTextPanel(allTextPanels[j]);
                                     //"*******************************************"
            allTextPanels[j].WriteText("CARGO SORTING DIRECTORY\n" +
                                       "For any container to interact with the\n" +
                                       "script the custom data field must be set \n" +
                                       "properly.  It is not case sensitive\n" +
                                       
                                       "\n" +
                                       "Containers are only sorted within a grid\n" +
                                       
                                       "\n" +
                                       "Ore Storage => ore\n" +
                                       "Ingot Storage => ingot\n" +
                                       "Component Storage => component\n" +
                                       "Ammo Storage => ammo\n" +
                                       "Tool Storage => tool\n" +
                                       "Miscelaneous Storage => misc\n" +
                                       
                                       "\n" +
                                       "Hydrogen and Oxygen Bottles are sorted as\n" +
                                       "tools.  They will be automatically taken out\n" +
                                       "of O2/H2 generators and tanks\n"+
                                       
                                       "\n"+
                                       "Items move as a stack and only if space\n" +
                                       "for the whole stack is availabe\n" +
                                     //"*******************************************"
                                       "\n"+
                                       "Assembler and Refinery output inventories\n"+
                                       "are automatically emptied into cargo\n"+
                                       "containers if there are assigned cargo\n"+
                                       "containers available.\n"+
                                       
                                       "\n" +
                                       "To display inventories on LCD screens\n" +
                                       "refer to Cargo Display\n"+
                                       "");
        }
        //checks if the text panel custom data contains the term for the space arm directory
        if(allTextPanels[j].CustomData.ToLower().Replace(" ", "").Contains("spacearmdirectory"))
        {
            FormatTextPanel(allTextPanels[j]);
                                     //"*******************************************"
            allTextPanels[j].WriteText("SPACE ARM DIRECTORY\n"+
                                       "All space arms require a dedicated cockpit.\n"+
                                       "Cockpit custom data field must be set to\n"+
                                       "\"spacearm name\" where \"name\" is custom and\n"+
                                       "unique.\n"+
                                       
                                       "\n"+
                                       "Pistons must contain the associated\n"+
                                       "\"spacearm name\" as the control cockpit.\n"+
                                       "Following line needs \"vertical\", \"lateral\",\n"+
                                       "or \"longitudinal\" to move."+
                                       "Enter \"-1\" on another line to reverse\n"+
                                       "piston movement."+
                                     //"*******************************************"
                                       "\n"+
                                       "Rotors and hinges must contain the\n"+
                                       "associated \"spacearm name\" as the control\n"+
                                       "cockpit.  Following line needs \"pitch\"\n"+
                                       "\"yaw\", or \"roll\" to move."+
                                       "Enter \"-1\" on another line to reverse\n"+
                                       "rotor and hinge movement."+
                                       
                                       "\n"+
                                       "Enter the sensitivity factor into the run\n"+
                                       "argument of the programmable block.  It\n"+
                                       "takes any number.  The pistons and rotors\n"+
                                       "use the same sensitivity factor.\n"+
                                       
                                       "\n"+
                                       "Cockpit, pistons, rotors, and hinges will\n"+
                                       "all have \"spacearm name\" from the\n"+
                                       "cockpit's custom data field added to their\n"+
                                       "name.\n"+
                                       "");
        }
            
    }
}


public void FormatTextPanel(IMyTextPanel textPanel)
{
    textPanel.FontSize = 1f;
    textPanel.TextPadding = 2f;
    textPanel.Alignment = TextAlignment.CENTER;
    textPanel.ContentType = ContentType.TEXT_AND_IMAGE;
}
