//ALL CARGO CONTAINERS
List<IMyCargoContainer> allCargoContainers = new List<IMyCargoContainer>();
//ALL ASSEMBLERS
List<IMyAssembler> allAssemblers = new List<IMyAssembler>();
//ALL REFINERIES
List<IMyRefinery> allRefineries = new List<IMyRefinery>();
//ALL O2/H2 GENGERATORS
List<IMyGasGenerator> allO2H2Generators = new List<IMyGasGenerator>();
//ALL GAS TANKS (HYDROGEN AND OXYGEN)
List<IMyGasTank> allGasTanks = new List<IMyGasTank>();

//temporary inventory
IMyInventory tempInventory;

MyFixedPoint bottleVolume = (MyFixedPoint)0.120;


public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update100;
}

//the main program
public void Main(string argument, UpdateType updateSource)
{
    CreateCargoContainerLists();
    CreateAssemblerLists();
    CreateRefineryLists();
    CreateO2H2GeneratorList();
    CreateGasTankList();
    SortContainers();
}

//methods called directly by the main program
public void CreateCargoContainerLists()
{
    //clearing the lists
    allCargoContainers.Clear();
    //creating the initial container list of all the cargo containers on the same grid as the programmable block
    GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(allCargoContainers, b => b.CubeGrid == Me.CubeGrid);
}
public void CreateAssemblerLists()
{
    //clearing the lists
    allAssemblers.Clear();
    //creating the initial container list of all the cargo containers on the same grid as the programmable block
    GridTerminalSystem.GetBlocksOfType<IMyAssembler>(allAssemblers, b => b.CubeGrid == Me.CubeGrid);
}
public void CreateRefineryLists()
{
    //clearing the lists
    allRefineries.Clear();
    //creating the initial container list of all the cargo containers on the same grid as the programmable block
    GridTerminalSystem.GetBlocksOfType<IMyRefinery>(allRefineries, b => b.CubeGrid == Me.CubeGrid);
}
public void CreateO2H2GeneratorList()
{
    //clearing the lists
    allO2H2Generators.Clear();
    //creating the initial O2H2 list of all on the same grid as the programmable block
    GridTerminalSystem.GetBlocksOfType<IMyGasGenerator>(allO2H2Generators, b => b.CubeGrid == Me.CubeGrid);
}
public void CreateGasTankList()
{
    //clearing the lists
    allGasTanks.Clear();
    //creating the initial O2H2 list of all on the same grid as the programmable block
    GridTerminalSystem.GetBlocksOfType<IMyGasTank>(allGasTanks, b => b.CubeGrid == Me.CubeGrid);
}
public void SortContainers()
{
    //Defining a dumby variable to carry the information to check each inventory slot, reduces required text
    MyInventoryItem tempItem;

    /*
     Space Engineers will automatically try to sort bottles that need to be refilled into places that they can be refilled, this script will then take them out of there to tool storage
    */

    //sorting all containers
    //all containers should exist in the allCargoContainers list and their individual lists
    for (int i = 0; i < allCargoContainers.Count(); i++)
    {
        //clearing contents in tempCargoContainer so items are not moved accidentally if following assignment fails
        tempInventory = null;
        //putting the current container into the temp variable so it can be accessed by other functions
        tempInventory = allCargoContainers[i].GetInventory(0);

        //checking each slot in the container inventory
        //checking in decending order since SpaceEngineers will otherwise move items to always fill slot 1
        for (int x = allCargoContainers[i].GetInventory(0).ItemCount - 1; x >= 0; x--)
        {
            //putting the item in container "i" at slot "x" into the dumby variable tempItem to check where it should go, value function stores everything about the item into tempItem
            tempItem = allCargoContainers[i].GetInventory(0).GetItemAt(x).Value;



            //checking where to place the item
            bool itemInCorrectContainer = false;
            //item type was identified as an ore, move to the ore containers

            if (tempItem.Type.GetItemInfo().IsOre & !allCargoContainers[i].CustomData.ToLower().Contains("ore"))
            {
                itemInCorrectContainer = MoveToCorrectCargoContainer(tempItem);
            }
            //item type was identified as an ingot, move to the ingot containers
            else if (tempItem.Type.GetItemInfo().IsIngot & !allCargoContainers[i].CustomData.ToLower().Contains("ingot"))
            {
                itemInCorrectContainer = MoveToCorrectCargoContainer(tempItem);
            }
            //item type was identified as a component, move to the component containers
            else if (tempItem.Type.GetItemInfo().IsComponent & !allCargoContainers[i].CustomData.ToLower().Contains("component"))
            {
                itemInCorrectContainer = MoveToCorrectCargoContainer(tempItem);
            }
            //item type was identified as ammo, move to the ammo containers
            else if (tempItem.Type.GetItemInfo().IsAmmo & !allCargoContainers[i].CustomData.ToLower().Contains("ammo"))
            {
                itemInCorrectContainer = MoveToCorrectCargoContainer(tempItem);
            }
            //item type was identified as a tool, move to the tool containers
            else if (tempItem.Type.GetItemInfo().IsTool & !allCargoContainers[i].CustomData.ToLower().Contains("tool"))
            {
                itemInCorrectContainer = MoveToCorrectCargoContainer(tempItem);
            }
            //item was not identified, container doesn't exist, or all valid containers are full. Move to misc containers if not already in proper container
            else if (
                !allCargoContainers[i].CustomData.ToLower().Contains("misc")
                & !itemInCorrectContainer
                & (tempItem.Type.GetItemInfo().IsOre & !allCargoContainers[i].CustomData.ToLower().Contains("ore")
                || tempItem.Type.GetItemInfo().IsIngot & !allCargoContainers[i].CustomData.ToLower().Contains("ingot")
                || tempItem.Type.GetItemInfo().IsComponent & !allCargoContainers[i].CustomData.ToLower().Contains("component")
                || tempItem.Type.GetItemInfo().IsAmmo & !allCargoContainers[i].CustomData.ToLower().Contains("ammo")
                || tempItem.Type.GetItemInfo().IsTool & !allCargoContainers[i].CustomData.ToLower().Contains("tool")
                || tempItem.Type.SubtypeId.Contains("Bottle")))
            {
                MoveToCorrectCargoContainer(tempItem);
            }
        }
    }
    //for all the assemblers
    for (int j = 0; j < allAssemblers.Count(); j++)
    {
        //clearing contents in tempCargoContainer so items are not moved accidentally if following assignment fails
        tempInventory = null;
        //putting the current container into the temp variable so it can be accessed by other functions
        tempInventory = allAssemblers[j].OutputInventory;
        //checking each slot in the container inventory
        //checking in decending order since SpaceEngineers will otherwise move items to always fill slot 1
        for (int y = allAssemblers[j].OutputInventory.ItemCount - 1; y >= 0; y--)
        {
            //getting the item in the output inventory
            tempItem = allAssemblers[j].OutputInventory.GetItemAt(y).Value;
            //we always want to empty out the items from the output inventories if possible so by default we can already assume the condition that we found it in the incorrect container
            MoveToCorrectCargoContainer(tempItem);
        }
    }
    //for all refineries
    for (int k = 0; k < allRefineries.Count(); k++)
    {
        //clearing contents in tempCargoContainer so items are not moved accidentally if following assignment fails
        tempInventory = null;
        //putting the current container into the temp variable so it can be accessed by other functions
        tempInventory = allRefineries[k].OutputInventory;
        //checking each slot in the container inventory
        //checking in decending order since SpaceEngineers will otherwise move items to always fill slot 1
        for (int z = allRefineries[k].OutputInventory.ItemCount - 1; z >= 0; z--)
        {
            //getting the item in the output inventory
            tempItem = allRefineries[k].OutputInventory.GetItemAt(z).Value;
            //we always want to empty out the items from the output inventories if possible so by default we can already assume the condition that we found it in the incorrect container
            MoveToCorrectCargoContainer(tempItem);
        }
    }
    //for all O2/H2 generators
    for (int j = 0; j < allO2H2Generators.Count(); j++)
    {
        //clearing contents in tempCargoContainer so items are not moved accidentally if following assignment fails
        tempInventory = null;
        //putting the current container into the temp variable so it can be accessed by other functions
        tempInventory = allO2H2Generators[j].GetInventory(0);

        //checking each slot in the container inventory
        //checking in decending order since SpaceEngineers will otherwise move items to always fill slot 1
        for (int x = allO2H2Generators[j].GetInventory(0).ItemCount - 1; x >= 0; x--)
        {
            //getting the item in the slot
            tempItem = allO2H2Generators[j].GetInventory(0).GetItemAt(x).Value;
            //we always want to empty out the bottles from the inventories and keep at least one slot open
            //checking to make sure that there is volume available in the generator for a bottle and if not trying to take an item out and put it in the correct container
            //this would be relevant if filled with ice but no need to produce hydrogen or oxygen so the bottles never get refilled and player doesn't have tanks

            if(allO2H2Generators[j].GetInventory(0).CurrentVolume>allO2H2Generators[j].GetInventory(0).MaxVolume-bottleVolume)
            {
                MoveToCorrectCargoContainer(tempItem);
            }
            else if (allO2H2Generators[j].GetInventory(0).GetItemAt(x).Value.Type.SubtypeId.Contains("Bottle"))
            {
                MoveToCorrectCargoContainer(tempItem);
            }
        }

    }
    //for all Oxygen and Hydrogen tanks
    for (int i = 0; i < allGasTanks.Count(); i++)
    {
        //clearing contents in tempCargoContainer so items are not moved accidentally if following assignment fails
        tempInventory = null;
        //putting the current container into the temp variable so it can be accessed by other functions
        tempInventory = allGasTanks[i].GetInventory(0);
        for (int x= allGasTanks[i].GetInventory(0).ItemCount - 1; x >= 0; x--)
        {
            //getting the item in the slot
            tempItem = allGasTanks[i].GetInventory(0).GetItemAt(x).Value;
            //we always want to try to empty these tanks to store them with tools if possible
            MoveToCorrectCargoContainer(tempItem);
        }
    }

}

//methods supporting other methods or those that have specific purposes, more general use
public bool MoveToCorrectCargoContainer(MyInventoryItem itemToMove)
{
    for (int j = 0; j < allCargoContainers.Count; j++)
    {
        //check if the container has been chosen to accept item type and if the container has space to accept the items
        if (itemToMove.Type.GetItemInfo().IsOre & allCargoContainers[j].CustomData.ToLower().Contains("ore") & allCargoContainers[j].GetInventory(0).CanItemsBeAdded(itemToMove.Amount, itemToMove.Type))
        {
            tempInventory.TransferItemTo(allCargoContainers[j].GetInventory(0), itemToMove);
            return true;
        }
        else if (itemToMove.Type.GetItemInfo().IsIngot & allCargoContainers[j].CustomData.ToLower().Contains("ingot") & allCargoContainers[j].GetInventory(0).CanItemsBeAdded(itemToMove.Amount, itemToMove.Type))
        {
            tempInventory.TransferItemTo(allCargoContainers[j].GetInventory(0), itemToMove);
            return true;
        }
        else if (itemToMove.Type.GetItemInfo().IsComponent & allCargoContainers[j].CustomData.ToLower().Contains("component") & allCargoContainers[j].GetInventory(0).CanItemsBeAdded(itemToMove.Amount, itemToMove.Type))
        {
            tempInventory.TransferItemTo(allCargoContainers[j].GetInventory(0), itemToMove);
            return true;
        }
        else if (itemToMove.Type.GetItemInfo().IsAmmo & allCargoContainers[j].CustomData.ToLower().Contains("ammo") & allCargoContainers[j].GetInventory(0).CanItemsBeAdded(itemToMove.Amount, itemToMove.Type))
        {
            tempInventory.TransferItemTo(allCargoContainers[j].GetInventory(0), itemToMove);
            return true;
        }
        //hydrogen and oxygen bottles are stored with tools
        else if ((itemToMove.Type.GetItemInfo().IsTool||itemToMove.Type.SubtypeId.Contains("Bottle")) & allCargoContainers[j].CustomData.ToLower().Contains("tool") & allCargoContainers[j].GetInventory(0).CanItemsBeAdded(itemToMove.Amount, itemToMove.Type))
        {
            tempInventory.TransferItemTo(allCargoContainers[j].GetInventory(0), itemToMove);
            return true;
        }



    }

    //failed to find a proper container with enough space, move item to misc container if possible
    for (int j = 0; j < allCargoContainers.Count; j++)
    {
        if (allCargoContainers[j].CustomData.ToLower().Contains("misc") & allCargoContainers[j].GetInventory(0).CanItemsBeAdded(itemToMove.Amount, itemToMove.Type))
        {
            tempInventory.TransferItemTo(allCargoContainers[j].GetInventory(0), itemToMove);
            return true;
        }
    }
    return false;
}

