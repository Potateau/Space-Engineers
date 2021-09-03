//ALL TEXTPANELS
        List<IMyTextPanel> allTextPanels = new List<IMyTextPanel>();


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
            //puts the directory/naming convention guide onto a textpanel in game
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
                    //Reference length for general max length of a text line on an LCD
                    //"*******************************************"
                    allTextPanels[j].WriteText("MAIN DIRECTORY\n" +
                        "Change the custom data of an LCD to the \n" +
                        "following to learn how to adjust the\n" +
                        "custom data of cargo containers for auto\n" +
                        "sorting, stock maintanance, and display\n" +
                        "Requires related script to function\n" +
                        "Sorting Directory: \"sorting directory\"\n" +
                        "Display Power Levels_WIP: \"power directory\"\n" +
                        "Cargo Display: \"cargo directory\"\n" +
                        "Page 2: WIP\n" +
                        "Some displays contain more information than\n" +
                        "displayed on screen, show screen for more");
                }


                //checks if the text panel custom data contains the term sorting directory
                if (allTextPanels[j].CustomData.ToLower().Replace(" ", "").Contains("sortingdirectory"))
                {
                    //adds the textpanel to the list of directory pannels, technically redundant
                    FormatTextPanel(allTextPanels[j]);
                                             //"*******************************************"
                    allTextPanels[j].WriteText("CARGO CONTAINER SORTING\n" +
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
                                               "Hydrogen and Oxygen Bottles are not sorted\n" +
                                               "\n" +
                                               "Items move as a stack and only if space\n" +
                                               "for the whole stack is availabe\n" +
                                               "\n" +
                                               "To display inventories on LCD screens\n" +
                                               "refer to Cargo Display\n"
                                               );
                }
                if (allTextPanels[j].CustomData.ToLower().Replace(" ", "").Contains("cargodirectory"))
                {
                    FormatTextPanel(allTextPanels[j]);
                    allTextPanels[j].WriteText("CARGO DIRECTORY\n" +
                                               "To display the inventories of items the\n" +
                                               "first line in the Custom Data field must\n" +
                                               "be \"cargodisplay\"\n" +
                                               "\n" +
                                               "Enter the items you want to see the\n" +
                                               "inventory of on the following lines\n" +
                                               "as filename/displayname\n" +
                                               "Examples: \"Steel Plate/Steel Plate\"\n" +
                                               "and \"Large Tube/Large Steel Tubes\"\n" +
                                               "\"line break\" to get a line break" +

                                               "\n" +
                                               "Refer to .sbc files in \n" +
                                               "SpaceEngineers\\Content\\Data for names\n" +

                                               "\n" +
                                               "Ores and Ingots cause an issue since they\n" +
                                               "share the same name. Include Ore or Ingot\n" +
                                               "in the filename to distinguish.\n" +
                                               "\n" +
                                               "Hydrogen and Oxygen displays are custom\n" +
                                               "hydrogen storage/\n" +
                                               "hydrogen capacity/\n" +
                                               "hydrogen tank count/\n" +
                                               "and the same for Oxygen are the filename\n" +
                                               "equivalents for use.\n"
                                               );
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
