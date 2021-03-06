//ALL CARGO CONTAINERS
List<IMyCargoContainer> allCargoContainers = new List<IMyCargoContainer>();
//ALL GAS TANKS(OXYGEN AND HYDROGEN)
List<IMyGasTank> allGasTanks = new List<IMyGasTank>();
//ALL TEXTPANELS
List<IMyTextPanel> allTextPanels = new List<IMyTextPanel>();
//ALL ASSEMBLERS
List<IMyAssembler> allAssemblers = new List<IMyAssembler>();
//All REFINERIES
List<IMyRefinery> allRefineries = new List<IMyRefinery>();



double hydrogenCapacity;
double hydrogenStorage;
int hydrogenTankCount;
double oxygenCapacity;
double oxygenStorage;
int oxygenTankCount;

double numberOfItems;

public Program()
{
    //updating only once every 100 ticks since timing is not critical
    Runtime.UpdateFrequency = UpdateFrequency.Update100;
}

public void Main(string argument, UpdateType updateSource)
{

    CreateCargoContainerList();
    CreateGasTankList();
    CreateTextPanelList();

    GetGasInventory();

    CreateAssemblerList();
    CreateRefineryList();

    CargoDisplay();



}
public void CreateCargoContainerList()
{
    //clearing the list
    allCargoContainers.Clear();
    //creating the list
    GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(allCargoContainers, b => b.CubeGrid == Me.CubeGrid);
}
public void CreateGasTankList()
{
    //clearing the list
    allGasTanks.Clear();
    //creating the list
    GridTerminalSystem.GetBlocksOfType<IMyGasTank>(allGasTanks, b => b.CubeGrid == Me.CubeGrid);
}
public void CreateTextPanelList()
{
    //clearing the list
    allTextPanels.Clear();
    //creating the list
    GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(allTextPanels, b => b.CubeGrid == Me.CubeGrid);
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
    for (int i = 0; i < allGasTanks.Count(); i++)
    {

        //checking if the gas tank is hydrogen
        if (allGasTanks[i].BlockDefinition.SubtypeId.Contains("Hydro") && allGasTanks[i].IsFunctional)
        {
            hydrogenCapacity += allGasTanks[i].Capacity;
            hydrogenStorage += allGasTanks[i].FilledRatio * allGasTanks[i].Capacity;
            hydrogenTankCount++;

        }
        //not hydrogen so it is oxygen
        else if (allGasTanks[i].IsFunctional)
        {
            oxygenCapacity += allGasTanks[i].Capacity;
            oxygenStorage += allGasTanks[i].FilledRatio * allGasTanks[i].Capacity;
            oxygenTankCount++;
        }




    }


}
public void CargoDisplay()
{
    //method to display all the ores, ingots, components, ammo, hydrogen, oxygen, and other things in the cargo containers


    //string of the text panel's custom data
    string[] displayRequests;
    //null assignment on separate line to make sure it is explicit over multiple runs
    displayRequests = null;

    string itemCategory = null;

    //"*******************************************"
    string inventoryDisplay;


    //side idea is if there is a way for the game to go through the list of string values of items types for me
    for (int k = 0; k < allTextPanels.Count(); k++)
    {
        //following if statements check the custom data field of the textpanel and display the categories that are called for
        displayRequests = allTextPanels[k].CustomData.Split(new string[] { "\n" }, StringSplitOptions.None);
        if (displayRequests[0].ToLower().Replace(" ", "") == "cargodisplay")
        {

            inventoryDisplay = "GRID CARGO\n";
            FormatTextPanel(allTextPanels[k]);
            //add changing of background if necessary




            //for every line except the first, since it is the text panel identifier, check to see if they can be counted and displayed
            for (int n = 1; n < displayRequests.Count(); n++)
            {



                if (displayRequests[n].ToLower().Replace(" ", "").Contains("linebreak"))
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


                    //adding to the display if it is an item and not a gas type
                    if (itemCategory != "hydrogen" && itemCategory != "oxygen")
                    {
                        numberOfItems = GetNumberOfItems(allCargoContainers, allAssemblers, allRefineries, displayRequests[n].Substring(0, displayRequests[n].IndexOf("/")).Replace(" ", "").Replace("Ore", "").Replace("ore", "").Replace("Ingot", "").Replace("ingot", ""), itemCategory);
                        //The line is requesting a production goal so use the = sign to find the display name and display the production goal
                        if (displayRequests[n].IndexOf("=") > 0)
                        {
                            inventoryDisplay = inventoryDisplay + displayRequests[n].Substring(displayRequests[n].IndexOf("/") + 1, displayRequests[n].IndexOf("=") - displayRequests[n].IndexOf("/") - 1) + ": " + FormatNumber(numberOfItems);
                            inventoryDisplay = inventoryDisplay + "/" + FormatNumber(Convert.ToDouble(displayRequests[n].Substring(displayRequests[n].IndexOf("=") + 1, displayRequests[n].Length - displayRequests[n].IndexOf("=") - 1))) + "\n";

                        }
                        //No production goal so use the line length to find the display name and don't display any production goal
                        else
                        {
                            inventoryDisplay = inventoryDisplay + displayRequests[n].Substring(displayRequests[n].IndexOf("/") + 1, displayRequests[n].Length - displayRequests[n].IndexOf("/") - 1) + ": " + FormatNumber(numberOfItems) + "\n";
                        }




                    }
                    else //it is requesting a gas type
                    {
                        if (displayRequests[n].ToLower().Replace(" ", "").Contains("hydrogenstorage"))
                        {
                            inventoryDisplay = inventoryDisplay + displayRequests[n].Substring(displayRequests[n].IndexOf("/") + 1, displayRequests[n].Length - displayRequests[n].IndexOf("/") - 1) + ": " + FormatNumber(hydrogenStorage) + "L" + "\n";
                        }
                        else if (displayRequests[n].ToLower().Replace(" ", "").Contains("hydrogentankcount"))
                        {
                            inventoryDisplay = inventoryDisplay + displayRequests[n].Substring(displayRequests[n].IndexOf("/") + 1, displayRequests[n].Length - displayRequests[n].IndexOf("/") - 1) + ": " + FormatNumber(hydrogenTankCount) + "\n";
                        }
                        else if (displayRequests[n].ToLower().Replace(" ", "").Contains("hydrogencapacity"))
                        {
                            inventoryDisplay = inventoryDisplay + displayRequests[n].Substring(displayRequests[n].IndexOf("/") + 1, displayRequests[n].Length - displayRequests[n].IndexOf("/") - 1) + ": " + FormatNumber(hydrogenCapacity) + "L" + "\n";
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
public void CreateAssemblerList()
{
    //clearing the list
    allAssemblers.Clear();
    //creating the assembler list
    GridTerminalSystem.GetBlocksOfType<IMyAssembler>(allAssemblers, b => b.CubeGrid == Me.CubeGrid);
}
public void CreateRefineryList()
{
    //clearing the list
    allRefineries.Clear();
    //creating the refinery list
    GridTerminalSystem.GetBlocksOfType<IMyRefinery>(allRefineries, b => b.CubeGrid == Me.CubeGrid);
}
public double GetNumberOfItems(List<IMyCargoContainer> cargoContainers, List<IMyAssembler> assemblers, List<IMyRefinery> refineries, string subtypeId, string itemCategory)
{
    //initializing the number of items found
    MyFixedPoint temp = 0;

    //checking all the cargo containers sent for the number of items found within
    for (int j = 0; j < cargoContainers.Count; j++)
    {
        //finds the number of the type of item and stores the type of item into the global variable
        switch (itemCategory)
        {
            case "ore":
                temp += cargoContainers[j].GetInventory(0).GetItemAmount(MyItemType.MakeOre(subtypeId));
                break;
            case "ingot":
                temp += cargoContainers[j].GetInventory(0).GetItemAmount(MyItemType.MakeIngot(subtypeId));

                break;
            //names do not overlap so they result in 0 if the item name does not match   
            case "component tool or ammo":
                temp += cargoContainers[j].GetInventory(0).GetItemAmount(MyItemType.MakeComponent(subtypeId));
                temp += cargoContainers[j].GetInventory(0).GetItemAmount(MyItemType.MakeTool(subtypeId));
                temp += cargoContainers[j].GetInventory(0).GetItemAmount(MyItemType.MakeAmmo(subtypeId));

                break;
        }
    }



    //checking all the assemblers sent for the number of items found within
    for (int k = 0; k < assemblers.Count; k++)
    {


        //finds the number of the type of item and stores the type of item into the global variable
        switch (itemCategory)
        {
            case "ore":
                temp += assemblers[k].InputInventory.GetItemAmount(MyItemType.MakeOre(subtypeId));
                temp += assemblers[k].OutputInventory.GetItemAmount(MyItemType.MakeOre(subtypeId));
                break;
            case "ingot":
                temp += assemblers[k].InputInventory.GetItemAmount(MyItemType.MakeIngot(subtypeId));
                temp += assemblers[k].OutputInventory.GetItemAmount(MyItemType.MakeIngot(subtypeId));
                break;
            //names do not overlap so they result in 0 if the item name does not match   
            case "component tool or ammo":
                temp += assemblers[k].InputInventory.GetItemAmount(MyItemType.MakeComponent(subtypeId));
                temp += assemblers[k].OutputInventory.GetItemAmount(MyItemType.MakeComponent(subtypeId));

                temp += assemblers[k].InputInventory.GetItemAmount(MyItemType.MakeTool(subtypeId));
                temp += assemblers[k].OutputInventory.GetItemAmount(MyItemType.MakeTool(subtypeId));

                temp += assemblers[k].InputInventory.GetItemAmount(MyItemType.MakeAmmo(subtypeId));
                temp += assemblers[k].OutputInventory.GetItemAmount(MyItemType.MakeAmmo(subtypeId));
                break;
        }
    }
    //checking all the refineries sent for the number of items found within
    for (int l = 0; l < refineries.Count; l++)
    {
        //finds the number of the type of item and stores the type of item into the global variable
        switch (itemCategory)
        {
            case "ore":
                temp += refineries[l].InputInventory.GetItemAmount(MyItemType.MakeOre(subtypeId));
                temp += refineries[l].OutputInventory.GetItemAmount(MyItemType.MakeOre(subtypeId));
                break;
            case "ingot":
                temp += refineries[l].InputInventory.GetItemAmount(MyItemType.MakeIngot(subtypeId));
                temp += refineries[l].OutputInventory.GetItemAmount(MyItemType.MakeIngot(subtypeId));
                break;
            //names do not overlap so they result in 0 if the item name does not match   
            case "component tool or ammo":
                temp += refineries[l].InputInventory.GetItemAmount(MyItemType.MakeComponent(subtypeId));
                temp += refineries[l].OutputInventory.GetItemAmount(MyItemType.MakeComponent(subtypeId));

                temp += refineries[l].InputInventory.GetItemAmount(MyItemType.MakeTool(subtypeId));
                temp += refineries[l].OutputInventory.GetItemAmount(MyItemType.MakeTool(subtypeId));

                temp += refineries[l].InputInventory.GetItemAmount(MyItemType.MakeAmmo(subtypeId));
                temp += refineries[l].OutputInventory.GetItemAmount(MyItemType.MakeAmmo(subtypeId));
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
    string truncatedNumber = null;
    string formattedNumber = null;
    int numberOfCommas = 0;

    originalNumber = number.ToString();

    //number is less than 10,000 so display full number

    if (originalNumber.Contains("."))
    {
        truncatedNumber = originalNumber.Substring(0, originalNumber.IndexOf(".") - 1);
    }
    else
    {
        truncatedNumber = originalNumber;
    }

    if (truncatedNumber.Length < 5)
    {
        for (int i = 0; i < truncatedNumber.Length + numberOfCommas; i++)
        {
            //4th iteration so a comma should be used
            if (i == 3)
            {
                formattedNumber = "," + formattedNumber;
                numberOfCommas++;
            }
            else
            {
                formattedNumber = truncatedNumber.Substring(truncatedNumber.Length - i - 1 + numberOfCommas, 1) + formattedNumber;
            }
        }
    }
    //number is less than 100,000 but >= 10,000 display as ##.#k
    else if (truncatedNumber.Length < 6)
    {
        formattedNumber = truncatedNumber.Substring(0, 2) + "." + truncatedNumber.Substring(2, 2) + "k";
    }
    //number is less than 1,000,000 but >= 100,000 display as ###.#k
    else if (truncatedNumber.Length < 7)
    {
        formattedNumber = truncatedNumber.Substring(0, 3) + "." + truncatedNumber.Substring(3, 1) + "k";
    }
    //number is less than 10,000,000 but >= 1,000,000 display as #.###M
    else if (truncatedNumber.Length < 8)
    {
        formattedNumber = truncatedNumber.Substring(0, 1) + "." + truncatedNumber.Substring(1, 3) + "M";
    }
    //number is less than 100,000,000 but >= 10,000,000 display as ##.##M
    else if (truncatedNumber.Length < 9)
    {
        formattedNumber = truncatedNumber.Substring(0, 2) + "." + truncatedNumber.Substring(2, 2) + "M";
    }
    //number is less than 1,000,000,000 but >= 1,000,000 display as ###m
    else if (truncatedNumber.Length < 10)
    {
        formattedNumber = truncatedNumber.Substring(0, 3) + "." + truncatedNumber.Substring(3, 1) + "M";
    }
    //number is at least a billion
    else
    {
        formattedNumber = "> 1B";
    }



    return formattedNumber;
}      
