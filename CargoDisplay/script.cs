


//ALL CARGO CONTAINERS
List<IMyCargoContainer> allCargoContainers = new List<IMyCargoContainer>();
//ALL GAS TANKS(OXYGEN AND HYDROGEN)
List<IMyGasTank> allGasTanks = new List<IMyGasTank>();
double hydrogenCapacity;
double hydrogenStorage;
int hydrogenTankCount;
double oxygenCapacity;
double oxygenStorage;
int oxygenTankCount;

//ALL TEXTPANELS
List<IMyTextPanel> allTextPanels = new List<IMyTextPanel>();

public Program()
{
    //updating only once every 100 ticks since timing is not critical
    Runtime.UpdateFrequency = UpdateFrequency.Update100;
}

public void Main(string argument, UpdateType updateSource)
{
    //giving an in game directory to how to enter the correct information into the custom data of text panels to get inventory display
    DisplayDirectories();
    //clearing the contents to make sure new list is created and getting all the cargo containers of the same grid as the programmable block
    allCargoContainers.Clear();
    GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(allCargoContainers, b => b.CubeGrid == Me.CubeGrid);

    //clearing and getting all the gas containers on the ship
    allGasTanks.Clear();
    GridTerminalSystem.GetBlocksOfType<IMyGasTank>(allGasTanks, b => b.CubeGrid == Me.CubeGrid);

    //clearing the contents to make sure new list is created and getting all the text panels of the same grid as the programmable block
    allTextPanels.Clear();
    GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(allTextPanels, b => b.CubeGrid == Me.CubeGrid);
    //displaying the inventories on the text panels with the correct custom data fields
    DisplayInventory();
}

//methods called directly by the main program
public void DisplayDirectories()
{
    //puts the directory/naming convention guide onto a textpanel in game


    for (int j = 0; j < allTextPanels.Count(); j++)
    {
        //check if the textpanel custom data is requesting the main directory
        if (allTextPanels[j].CustomData.ToLower() == "directory")
        {
            //providing the list of available glossaries
            FormatTextPanel(allTextPanels[j]);
            allTextPanels[j].WriteText("MAIN DIRECTORY\n" +
                                        "Change the custom data of an LCD to the \n" +
                                        "following to learn how to adjust the\n" +
                                        "custom data of cargo containers for auto\n" +
                                        "sorting, stock maintanance, and display\n" +
                                        "Inventory Sorting: sorting directory\n" +
                                        "Inventory Display: inventory directory");


            //allTextPanels[j].CustomName="";
        }


        //checks if the text panel custom data contains the term sorting directory
        if (allTextPanels[j].CustomData.ToLower().Contains("sorting directory"))
        {
            //adds the textpanel to the list of directory pannels, technically redundant
            FormatTextPanel(allTextPanels[j]);
            allTextPanels[j].WriteText("For any block to interact with the script\n" +
                                       "the custom data field must be set \n" +
                                       "properly.  It is not case sensitive\n" +
                                       "\n" +
                                       "CARGO CONTAINER SORTING\n" +
                                       "Ore Storage => ore\n" +
                                       "Ingot Storage => ingot\n" +
                                       "Component Storage => component\n" +
                                       "Ammo Storage => ammo\n" +
                                       "Tool Storage => tool\n" +
                                       "Miscelaneous Storage => misc\n" +
                                       "\n" +
                                       "To display inventories on LCD screens\n" +
                                       "use the relative custom data fields\n" +
                                       "+ \"display\" example: ore display"
                                       );
        }
        if (allTextPanels[j].CustomData.ToLower().Contains("inventory directory"))
        {
            FormatTextPanel(allTextPanels[j]);
            allTextPanels[j].WriteText("INVENTORY DIRECTORY\n" +
                                       "To display the inventories of items the\n" +
                                       "first line in the Custom Data field must\n" +
                                       "be \"inventorydisplay\"\n" +
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
public void DisplayInventory()
{
    //method to display all the ores, ingots, components, ammo, hydrogen, oxygen, and other things in the cargo containers


    //clearing the list of TextPanels to make sure it is empty from previous runs
    allTextPanels.Clear();
    //adding all TextPanels accessable to the prgramable block to the list of TextPanels
    GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(allTextPanels, b => b.CubeGrid == Me.CubeGrid);
    //getting the capacity and storage levels of all the gas(hydrogen and oxygen) on the grid
    GetGasInventory();

    //string of the text panel's custom data
    string[] displayRequests;
    //null assignment on separate line to make sure it is explicit over multiple runs
    displayRequests = null;

    string itemCategory = null;

    //variable for alternate display
    //int stringLength;
    //string tempString = null;
    //string tempStringPad = null;

    //"*******************************************"
    string inventoryDisplay;


    //side idea is if there is a way for the game to go through the list of string values of items types for me
    for (int k = 0; k < allTextPanels.Count(); k++)
    {
        //following if statements check the custom data field of the textpanel and display the categories that are called for
        displayRequests = allTextPanels[k].CustomData.Split(new string[] { "\n" }, StringSplitOptions.None);
        if (displayRequests[0].ToLower().Replace(" ", "") == "inventorydisplay")
        {

            inventoryDisplay = "GRID INVENTORY\n";
            FormatTextPanel(allTextPanels[k]);
            //add changing of background if necessary




            //for every line except the first two, since it is the text panel identifier, check to see if they can be counted and displayed
            for (int n = 1; n < displayRequests.Count(); n++)
            {



                //displayRequests[n].Substring(0, displayRequests[n].IndexOf("/")-1);

                //inventoryDisplay = inventoryDisplay + displayRequests[n] + ": " + (GetNumberOfItems(allCargoContainers, displayRequests[n].Replace(" ", "").Replace("Ore", "").Replace("ore", "").Replace("Ingot", "").Replace("ingot", ""), itemCategory) + "\n");
                if(displayRequests[n].ToLower().Replace(" ","").Contains("linebreak"))
                {
                    inventoryDisplay = inventoryDisplay + "\n";
                }
                else if (displayRequests[n].IndexOf("/") > 0)
                {


                    if (displayRequests[n].ToLower().Contains("hydrogen"))
                    {
                        itemCategory = "hydrogen";
                    }
                    else if (displayRequests[n].ToLower().Contains("oxygen"))
                    {
                        itemCategory = "oxygen";
                    }
                    else if (displayRequests[n].ToLower().Contains("ore"))
                    {
                        itemCategory = "ore";
                    }
                    else if (displayRequests[n].ToLower().Contains("ingot"))
                    {
                        itemCategory = "ingot";
                    }
                    else
                    {
                        itemCategory = "component tool or ammo";
                    }

                    /*alternate display type that works better with monospace
                            //finding the length of the text line that will be added to the display
                            tempString = displayRequests[n].Substring(displayRequests[n].IndexOf("/") + 1, displayRequests[n].Length - displayRequests[n].IndexOf("/") - 1) + "." + GetNumberOfItems(allCargoContainers, displayRequests[n].Substring(0, displayRequests[n].IndexOf("/")).Replace(" ", "").Replace("Ore", "").Replace("ore", "").Replace("Ingot", "").Replace("ingot", ""), itemCategory);
                            stringLength = tempString.Length;
                            //adding the first part of the text line, the name of the item
                            inventoryDisplay = inventoryDisplay + displayRequests[n].Substring(displayRequests[n].IndexOf("/") + 1, displayRequests[n].Length - displayRequests[n].IndexOf("/") - 1);
                            //adding . so all the lines of text are aligned the same
                            tempStringPad = ".";
                            //custom padding loop since .PadLeft or .PadRight was not working
                            for(int i = 0;i+stringLength<30; i++)
                            {
                                tempStringPad = tempStringPad + ".";
                            }


                            //putting the number of items onto the text line
                            inventoryDisplay = inventoryDisplay +tempStringPad + (GetNumberOfItems(allCargoContainers, displayRequests[n].Substring(0, displayRequests[n].IndexOf("/")).Replace(" ", "").Replace("Ore", "").Replace("ore", "").Replace("Ingot", "").Replace("ingot", ""), itemCategory) + "\n");
                            */
                    //there is nothing on that line so add a linebreak to the display

                    //adding to the display if it is an item and not a gas type
                    if (itemCategory != "hydrogen" && itemCategory != "oxygen")
                    {
                        inventoryDisplay = inventoryDisplay + displayRequests[n].Substring(displayRequests[n].IndexOf("/") + 1, displayRequests[n].Length - displayRequests[n].IndexOf("/") - 1) + ": " + FormatNumber(GetNumberOfItems(allCargoContainers, displayRequests[n].Substring(0, displayRequests[n].IndexOf("/")).Replace(" ", "").Replace("Ore", "").Replace("ore", "").Replace("Ingot", "").Replace("ingot", ""), itemCategory)) + "\n";
                    }
                    else //it is requesting a gas type
                    {
                        if(displayRequests[n].ToLower().Replace(" ","").Contains("hydrogenstorage"))
                        {
                            inventoryDisplay = inventoryDisplay + displayRequests[n].Substring(displayRequests[n].IndexOf("/") + 1, displayRequests[n].Length - displayRequests[n].IndexOf("/") - 1) + ": " + FormatNumber(hydrogenStorage) + "L" + "\n";
                        }
                        else if (displayRequests[n].ToLower().Replace(" ","").Contains("hydrogentankcount"))
                        {
                            inventoryDisplay = inventoryDisplay + displayRequests[n].Substring(displayRequests[n].IndexOf("/") + 1, displayRequests[n].Length - displayRequests[n].IndexOf("/") - 1) + ": " + FormatNumber(hydrogenTankCount) + "\n";
                        }
                        else if (displayRequests[n].ToLower().Replace(" ", "").Contains("hydrogencapacity"))
                        {
                            inventoryDisplay = inventoryDisplay + displayRequests[n].Substring(displayRequests[n].IndexOf("/") + 1, displayRequests[n].Length - displayRequests[n].IndexOf("/") - 1) + ": " + FormatNumber(hydrogenCapacity) + "L" +  "\n";
                            //inventoryDisplay = inventoryDisplay + "it worked\n";
                        }
                        else if (displayRequests[n].ToLower().Replace(" ", "").Contains("oxygenstorage"))
                        {
                            inventoryDisplay = inventoryDisplay + displayRequests[n].Substring(displayRequests[n].IndexOf("/") + 1, displayRequests[n].Length - displayRequests[n].IndexOf("/") - 1) + ": " + FormatNumber(oxygenStorage) + "L" + "\n";
                        }
                        else if (displayRequests[n].ToLower().Replace(" ", "").Contains("oxygentankcount"))
                        {
                            inventoryDisplay = inventoryDisplay + displayRequests[n].Substring(displayRequests[n].IndexOf("/") + 1, displayRequests[n].Length - displayRequests[n].IndexOf("/") - 1) + ": " + FormatNumber(oxygenTankCount) + "\n";
                        }
                        else if (displayRequests[n].ToLower().Replace(" ", "").Contains("oxygencapacity"))
                        {
                            inventoryDisplay = inventoryDisplay + displayRequests[n].Substring(displayRequests[n].IndexOf("/") + 1, displayRequests[n].Length - displayRequests[n].IndexOf("/") - 1) + ": " + FormatNumber(oxygenCapacity) + "L" + "\n";
                        }
                    }
                }
            }


            //printing all the textlines onto the lcd
            allTextPanels[k].WriteText(inventoryDisplay);




        }
        //listing information onto the dumby text panels
        if (allTextPanels[k].CustomData.Contains("dumbylcd"))
        {
            FormatTextPanel(allTextPanels[k]);
            allTextPanels[k].WriteText("The custom data: " + allTextPanels[k].CustomData);
        }

    }


}

public void GetGasInventory()
{
    //clearing capacities for new run of script

    hydrogenCapacity = 0;
    hydrogenStorage = 0;
    hydrogenTankCount = 0;
    oxygenCapacity = 0;
    oxygenStorage = 0;
    oxygenTankCount = 0;

    //get the total possible volume of hydrogen and oxygen on the ship
    for (int i= 0; i<allGasTanks.Count(); i++)
    {

        //checking if the gas tank is hydrogen
        if(allGasTanks[i].BlockDefinition.SubtypeId.Contains("Hydro")&&allGasTanks[i].IsFunctional)
        {
            hydrogenCapacity += allGasTanks[i].Capacity;
            hydrogenStorage += allGasTanks[i].FilledRatio * allGasTanks[i].Capacity;
            hydrogenTankCount++;

        }
        //not hydrogen so it is oxygen
        else if(allGasTanks[i].IsFunctional)
        {
            oxygenCapacity += allGasTanks[i].Capacity;
            oxygenStorage += allGasTanks[i].FilledRatio * allGasTanks[i].Capacity;
            oxygenTankCount++;
        }




    }


}

public double GetNumberOfItems(List<IMyCargoContainer> cargoContainers, string item, string itemCategory)
{
    //initializing the number of steel plates found
    MyFixedPoint temp = 0;




    //checking all the cargo containers sent for the number of items found within
    for (int j = 0; j < cargoContainers.Count; j++)
    {
        switch (itemCategory)
        {
            case "ore":
                temp += cargoContainers[j].GetInventory(0).GetItemAmount(MyItemType.MakeOre(item));
                break;
            case "ingot":
                temp += cargoContainers[j].GetInventory(0).GetItemAmount(MyItemType.MakeIngot(item));
                break;
            case "component tool or ammo":
                temp += cargoContainers[j].GetInventory(0).GetItemAmount(MyItemType.MakeComponent(item));
                temp += cargoContainers[j].GetInventory(0).GetItemAmount(MyItemType.MakeTool(item));
                temp += cargoContainers[j].GetInventory(0).GetItemAmount(MyItemType.MakeAmmo(item));
                break;
        }







    }
    int forReturn = temp.ToIntSafe();
    return forReturn;
}

public void FormatTextPanel(IMyTextPanel textPanel)
{
    textPanel.FontSize = 1f;
    textPanel.TextPadding = 2f;
    textPanel.Alignment = TextAlignment.CENTER;
    textPanel.ContentType = ContentType.TEXT_AND_IMAGE;
    textPanel.Font = "Debug";
}
public string FormatNumber(double number)
{
    string originalNumber = null;
    string formattedNumber=null;
    int numberOfCommas = 0;

    originalNumber = number.ToString();

    //number is less than 10,000 so display full number
    if (originalNumber.Length < 5)
    {
        for (int i = 0; i < originalNumber.Length + numberOfCommas; i++)
        {
            //4th iteration so a comma should be used
            if (i == 3)
            {
                formattedNumber = "," + formattedNumber;
                numberOfCommas++;
            }
            else
            {
                formattedNumber = originalNumber.Substring(originalNumber.Length - i - 1 + numberOfCommas, 1) + formattedNumber;
            }
        }
    }
    //number is less than 100,000 but >= 10,000 display as ##.#k
    else if (originalNumber.Length<6)
    {
        formattedNumber = originalNumber.Substring(0, 2) + "." + originalNumber.Substring(2, 2) + "k";
    }
    //number is less than 1,000,000 but >= 100,000 display as ###.#k
    else if (originalNumber.Length < 7)
    {
        formattedNumber = originalNumber.Substring(0, 3) + "." + originalNumber.Substring(3, 1) + "k";
    }
    //number is less than 10,000,000 but >= 1,000,000 display as #.###M
    else if (originalNumber.Length < 8)
    {
        formattedNumber = originalNumber.Substring(0, 1) + "." + originalNumber.Substring(1, 3) + "M";
    }
    //number is less than 100,000,000 but >= 10,000,000 display as ##.##M
    else if (originalNumber.Length<9)
    {
        formattedNumber = originalNumber.Substring(0, 2) + "." + originalNumber.Substring(2, 2) + "M";
    }
    //number is less than 1,000,000,000 but >= 1,000,000 display as ###m
    else if (originalNumber.Length < 10)
    {
        formattedNumber = originalNumber.Substring(0, 3) + "." + originalNumber.Substring(3, 1) + "M";
    }
    //number is at least a billion
    else
    {
        formattedNumber = "> 1B";
    }



    return formattedNumber;
}
