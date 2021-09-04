//Assemblers will automatically empty their inventory into an available cargo container
        //The script will sort the items from there
        //ALL CARGO CONTAINERS
        List<IMyCargoContainer> allCargoContainers = new List<IMyCargoContainer>();
        //ALL ASSEMBLERS
        List<IMyAssembler> allAssemblers = new List<IMyAssembler>();
        //ALL REFINERIES
        List<IMyRefinery> allRefineries = new List<IMyRefinery>();

        //temporary inventory
        IMyInventory tempInventory;

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
        public void SortContainers()
        {
            //Defining a dumby variable to carry the information to check each inventory slot
            MyInventoryItem tempItem;

            //sorting all containers
            //all containers should exist in the allCargoContainers list and their individual lists
            for (int i = 0; i < allCargoContainers.Count(); i++)
            {
                //checking each slot in the container inventory
                //checking in decending order since SpaceEngineers will otherwise move items to always fill slot 1
                for (int x = allCargoContainers[i].GetInventory(0).ItemCount - 1; x >= 0; x--)
                {
                    //putting the item in container "i" at slot "x" into the dumby variable tempItem to check where it should go, value function stores everything about the item into tempItem

                    tempItem = allCargoContainers[i].GetInventory(0).GetItemAt(x).Value;




                    //clearing contents in tempCargoContainer so items are not moved accidentally if following assignment fails
                    tempInventory = null;
                    //putting the current container into the temp variable so it can be accessed by other functions
                    tempInventory = allCargoContainers[i].GetInventory(0);

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
                        || tempItem.Type.GetItemInfo().IsTool & !allCargoContainers[i].CustomData.ToLower().Contains("tool")))

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
            for(int k = 0; k < allRefineries.Count(); k++)
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


        }

        //methods supporting other methods or those that have specific purposes, more general use
        public bool MoveToCorrectCargoContainer(MyInventoryItem tempItem)
        {

            for (int j = 0; j < allCargoContainers.Count; j++)
            {
                //check if the container has been chosen to accept item type and if the container has space to accept the items
                if (tempItem.Type.GetItemInfo().IsOre & allCargoContainers[j].CustomData.ToLower().Contains("ore") & allCargoContainers[j].GetInventory(0).CanItemsBeAdded(tempItem.Amount, tempItem.Type))
                {
                    tempInventory.TransferItemTo(allCargoContainers[j].GetInventory(0), tempItem);
                    return true;
                }
                else if (tempItem.Type.GetItemInfo().IsIngot & allCargoContainers[j].CustomData.ToLower().Contains("ingot") & allCargoContainers[j].GetInventory(0).CanItemsBeAdded(tempItem.Amount, tempItem.Type))
                {
                    tempInventory.TransferItemTo(allCargoContainers[j].GetInventory(0), tempItem);
                    return true;
                }
                else if (tempItem.Type.GetItemInfo().IsComponent & allCargoContainers[j].CustomData.ToLower().Contains("component") & allCargoContainers[j].GetInventory(0).CanItemsBeAdded(tempItem.Amount, tempItem.Type))
                {
                    tempInventory.TransferItemTo(allCargoContainers[j].GetInventory(0), tempItem);
                    return true;
                }
                else if (tempItem.Type.GetItemInfo().IsAmmo & allCargoContainers[j].CustomData.ToLower().Contains("ammo") & allCargoContainers[j].GetInventory(0).CanItemsBeAdded(tempItem.Amount, tempItem.Type))
                {
                    tempInventory.TransferItemTo(allCargoContainers[j].GetInventory(0), tempItem);
                    return true;
                }
                else if (tempItem.Type.GetItemInfo().IsTool & allCargoContainers[j].CustomData.ToLower().Contains("tool") & allCargoContainers[j].GetInventory(0).CanItemsBeAdded(tempItem.Amount, tempItem.Type))
                {
                    tempInventory.TransferItemTo(allCargoContainers[j].GetInventory(0), tempItem);
                    return true;
                }


            }
            //failed to find a proper container with enough space, move item to misc container if possible
            for (int j = 0; j < allCargoContainers.Count; j++)
            {
                if (allCargoContainers[j].CustomData.ToLower().Contains("misc") & allCargoContainers[j].GetInventory(0).CanItemsBeAdded(tempItem.Amount, tempItem.Type))
                {
                    tempInventory.TransferItemTo(allCargoContainers[j].GetInventory(0), tempItem);
                    return true;
                }
            }
            return false;
        }

        
