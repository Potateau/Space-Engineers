public Program()
        {
            //updating only once every 100 ticks since timing is not critical
            Runtime.UpdateFrequency = UpdateFrequency.Update100;
        }
        public void Main(string argument, UpdateType updateSource)
        {
            //ALL CARGO CONTAINERS
            List<IMyCargoContainer> allCargoContainers = new List<IMyCargoContainer>();
            allCargoContainers.Clear();
            allCargoContainers = CreateCargoContainerList();

            //ALL GAS TANKS(OXYGEN AND HYDROGEN)
            List<IMyGasTank> allGasTanks = new List<IMyGasTank>();
            allGasTanks.Clear();
            allGasTanks = CreateGasTankList();
            double hydrogenCapacity = 0;
            double hydrogenStorage = 0;
            int hydrogenTankCount = 0;
            double oxygenCapacity = 0;
            double oxygenStorage = 0;
            int oxygenTankCount = 0;
            GetGasInventory(allGasTanks, out hydrogenCapacity, out hydrogenStorage, out hydrogenTankCount, out oxygenCapacity, out oxygenStorage, out oxygenTankCount);

            //ALL TEXTPANELS
            List<IMyTextPanel> allTextPanels = new List<IMyTextPanel>();
            allTextPanels.Clear();
            allTextPanels = CreateTextPanelList();

            //ALL ASSEMBLERS
            List<IMyAssembler> allAssemblers = new List<IMyAssembler>();
            allAssemblers.Clear();
            allAssemblers = CreateAssemblerList();

            //All REFINERIES
            List<IMyRefinery> allRefineries = new List<IMyRefinery>();
            allRefineries.Clear();
            allRefineries = CreateRefineryList();

            //Assembly is contained within
            CargoDisplayAndAssembly(allCargoContainers, hydrogenCapacity, hydrogenStorage, hydrogenTankCount, oxygenCapacity, oxygenStorage, oxygenTankCount, allTextPanels, allAssemblers, allRefineries);
        }
        public List<IMyCargoContainer> CreateCargoContainerList()
        {
            List<IMyCargoContainer> allCargoContainers_InFunction = new List<IMyCargoContainer>();
            //creating the list
            GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(allCargoContainers_InFunction, b => b.CubeGrid == Me.CubeGrid);
            return allCargoContainers_InFunction;
        }
        public List<IMyGasTank> CreateGasTankList()
        {
            List<IMyGasTank> allGasTanks_InFunction = new List<IMyGasTank>();
            //creating the list
            GridTerminalSystem.GetBlocksOfType<IMyGasTank>(allGasTanks_InFunction, b => b.CubeGrid == Me.CubeGrid);
            return allGasTanks_InFunction;
        }
        public void GetGasInventory(List<IMyGasTank> allGasTanks, out double hydrogenCapacity_InFunction, out double hydrogenStorage_InFunction, out int hydrogenTankCount_InFunction, out double oxygenCapacity_InFunction, out double oxygenStorage_InFunction, out int oxygenTankCount_InFunction)
        {
            //need to give the varialbes an initial value else the += functionality of the following for loop fails on first iteration
            hydrogenCapacity_InFunction = 0;
            hydrogenStorage_InFunction = 0;
            hydrogenTankCount_InFunction = 0;
            oxygenCapacity_InFunction = 0;
            oxygenStorage_InFunction = 0;
            oxygenTankCount_InFunction = 0;

            //get the total possible volume of hydrogen and oxygen on the ship
            for (int i = 0; i < allGasTanks.Count(); i++)
            {
                //checking if the gas tank is hydrogen
                if (allGasTanks[i].BlockDefinition.SubtypeId.Contains("Hydro") && allGasTanks[i].IsFunctional)
                {
                    hydrogenCapacity_InFunction += allGasTanks[i].Capacity;
                    hydrogenStorage_InFunction += allGasTanks[i].FilledRatio * allGasTanks[i].Capacity;
                    hydrogenTankCount_InFunction++;
                }
                //not hydrogen so it is oxygen
                else if (allGasTanks[i].IsFunctional)
                {
                    oxygenCapacity_InFunction += allGasTanks[i].Capacity;
                    oxygenStorage_InFunction += allGasTanks[i].FilledRatio * allGasTanks[i].Capacity;
                    oxygenTankCount_InFunction++;
                }
            }
        }
        public List<IMyTextPanel> CreateTextPanelList()
        {
            List<IMyTextPanel> allTextPanels_InFunction = new List<IMyTextPanel>();
            //creating the list
            GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(allTextPanels_InFunction, b => b.CubeGrid == Me.CubeGrid);
            return allTextPanels_InFunction;
        }
        public List<IMyAssembler> CreateAssemblerList()
        {
            List<IMyAssembler> allAssemblers_InFunction = new List<IMyAssembler>();
            //creating the assembler list
            GridTerminalSystem.GetBlocksOfType<IMyAssembler>(allAssemblers_InFunction, b => b.CubeGrid == Me.CubeGrid);
            return allAssemblers_InFunction;
        }
        public List<IMyRefinery> CreateRefineryList()
        {
            List<IMyRefinery> allRefineries_InFunction = new List<IMyRefinery>();
            //creating the refinery list
            GridTerminalSystem.GetBlocksOfType<IMyRefinery>(allRefineries_InFunction, b => b.CubeGrid == Me.CubeGrid);
            return allRefineries_InFunction;
        }
        public void CargoDisplayAndAssembly(List<IMyCargoContainer> allCargoContainers_InFunction, double hydrogenCapacity_InFunction, double hydrogenStorage_InFunction, int hydrogenTankCount_InFunction, double oxygenCapacity_InFunction, double oxygenStorage_InFunction, int oxygenTankCount_InFunction, List<IMyTextPanel> allTextPanels_InFunction, List<IMyAssembler> allAssemblers_InFunction, List<IMyRefinery> allRefineries_InFunction)
        {
            //method to display all the ores, ingots, components, ammo, hydrogen, oxygen, and other things in the cargo containers

            //string of the text panel's custom data
            string[] displayRequests;
            //null assignment on separate line to make sure it is explicit over multiple runs
            displayRequests = null;
            string itemCategory = null;
            string inventoryDisplay;
            //"*******************************************" - general max length of text for display

            //variable to keep track of how many of each item type there are in containers
            double numberOfItems;

            //side idea is if there is a way for the game to go through the list of string values of items types for me
            for (int k = 0; k < allTextPanels_InFunction.Count(); k++)
            {
                //following if statements check the custom data field of the textpanel and display the categories that are called for
                displayRequests = allTextPanels_InFunction[k].CustomData.Split(new string[] { "\n" }, StringSplitOptions.None);
                if (displayRequests[0].ToLower().Replace(" ", "") == "cargodisplayandassembly")
                {
                    inventoryDisplay = "GRID CARGO AND ASSEMBLY\n";
                    FormatTextPanel(allTextPanels_InFunction[k]);
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
                                numberOfItems = GetNumberOfItems(allCargoContainers_InFunction, allAssemblers_InFunction, allRefineries_InFunction, displayRequests[n].Substring(0, displayRequests[n].IndexOf("/")).Replace(" ", "").Replace("Ore", "").Replace("ore", "").Replace("Ingot", "").Replace("ingot", ""), itemCategory);
                                //The line is requesting a production goal so use the = sign to find the display name and display the production goal
                                if (displayRequests[n].IndexOf("=") > 0)
                                {
                                    inventoryDisplay = inventoryDisplay + displayRequests[n].Substring(displayRequests[n].IndexOf("/") + 1, displayRequests[n].IndexOf("=") - displayRequests[n].IndexOf("/") - 1) + ": " + FormatNumber(numberOfItems);
                                    inventoryDisplay = inventoryDisplay + "/" + FormatNumber(Convert.ToDouble(displayRequests[n].Substring(displayRequests[n].IndexOf("=") + 1, displayRequests[n].Length - displayRequests[n].IndexOf("=") - 1))) + "\n";
                                    //check if the number of items requested is more than the number of items stored
                                    if (Convert.ToDouble(displayRequests[n].Substring(displayRequests[n].IndexOf("=") + 1, displayRequests[n].Length - displayRequests[n].IndexOf("=") - 1)) > numberOfItems)
                                    {
                                        //send the difference to the assemblers to check if the required items are in queue, otherwise create item queue
                                        Assembly(displayRequests[n].Substring(0, displayRequests[n].IndexOf("/")).Replace(" ", "").Replace("Ore", "").Replace("ore", "").Replace("Ingot", "").Replace("ingot", ""), allAssemblers_InFunction);
                                    }
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
                                    inventoryDisplay = inventoryDisplay + displayRequests[n].Substring(displayRequests[n].IndexOf("/") + 1, displayRequests[n].Length - displayRequests[n].IndexOf("/") - 1) + ": " + FormatNumber(hydrogenStorage_InFunction) + "L" + "\n";
                                }
                                else if (displayRequests[n].ToLower().Replace(" ", "").Contains("hydrogentankcount"))
                                {
                                    inventoryDisplay = inventoryDisplay + displayRequests[n].Substring(displayRequests[n].IndexOf("/") + 1, displayRequests[n].Length - displayRequests[n].IndexOf("/") - 1) + ": " + FormatNumber(hydrogenTankCount_InFunction) + "\n";
                                }
                                else if (displayRequests[n].ToLower().Replace(" ", "").Contains("hydrogencapacity"))
                                {
                                    inventoryDisplay = inventoryDisplay + displayRequests[n].Substring(displayRequests[n].IndexOf("/") + 1, displayRequests[n].Length - displayRequests[n].IndexOf("/") - 1) + ": " + FormatNumber(hydrogenCapacity_InFunction) + "L" + "\n";
                                    //inventoryDisplay = inventoryDisplay + "it worked\n";
                                }
                                else if (displayRequests[n].ToLower().Replace(" ", "").Contains("oxygenstorage"))
                                {
                                    inventoryDisplay = inventoryDisplay + displayRequests[n].Substring(displayRequests[n].IndexOf("/") + 1, displayRequests[n].Length - displayRequests[n].IndexOf("/") - 1) + ": " + FormatNumber(oxygenStorage_InFunction) + "L" + "\n";
                                }
                                else if (displayRequests[n].ToLower().Replace(" ", "").Contains("oxygentankcount"))
                                {
                                    inventoryDisplay = inventoryDisplay + displayRequests[n].Substring(displayRequests[n].IndexOf("/") + 1, displayRequests[n].Length - displayRequests[n].IndexOf("/") - 1) + ": " + FormatNumber(oxygenTankCount_InFunction) + "\n";
                                }
                                else if (displayRequests[n].ToLower().Replace(" ", "").Contains("oxygencapacity"))
                                {
                                    inventoryDisplay = inventoryDisplay + displayRequests[n].Substring(displayRequests[n].IndexOf("/") + 1, displayRequests[n].Length - displayRequests[n].IndexOf("/") - 1) + ": " + FormatNumber(oxygenCapacity_InFunction) + "L" + "\n";
                                }
                            }
                        }
                    }

                    //printing all the textlines onto the lcd
                    allTextPanels_InFunction[k].WriteText(inventoryDisplay);
                }
                //listing information onto the dumby text panels
                if (allTextPanels_InFunction[k].CustomData.Contains("dumbylcd"))
                {
                    FormatTextPanel(allTextPanels_InFunction[k]);
                    allTextPanels_InFunction[k].WriteText("The custom data: " + allTextPanels_InFunction[k].CustomData);
                }
            }
        }
        public void Assembly(string filename, List<IMyAssembler> allAssemblers_InFunction)
        {
            MyDefinitionId blueprint;
            List<MyProductionItem> allProductionItems = new List<MyProductionItem>();
            MyFixedPoint count = 10;

            for (int i = 0; i < allAssemblers_InFunction.Count(); i++)
            {
                //puts the production queue of the assembler into allProductionItems
                allAssemblers_InFunction[i].GetQueue(allProductionItems);

                //checks if the assembler has a queue and only assigns new items into production if the queue is empty
                if (allProductionItems.Count == 0)
                {
                    //gets the blueprint of the filename, hardcoded functionality so updates may break this
                    blueprint = StringToMyDefinitionId(filename);
                    //only assigns work to assemblers that are not set to cooperative mode, recommended only one assembler not set to cooperative mode else the missing items will be entered into production multiple times
                    if (!allAssemblers_InFunction[i].CooperativeMode)
                    {
                        //ensures the assembler can accept the requested blueprint
                        if (allAssemblers_InFunction[i].CanUseBlueprint(blueprint))
                        {
                            //number of missing items will be added to the production queue
                            allAssemblers_InFunction[i].AddQueueItem(blueprint, count);
                            //the item has been sent to an assembler so break the loop to make sure it doesn't get assigned to every idle assembler
                            break;
                        }
                    }
                }
            }
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
        public MyDefinitionId StringToMyDefinitionId(String subtypeId)
        {
            //game requires things to be hard coded so this is a series of if statements of supported blueprints to send to assemblers
            MyDefinitionId blueprint;
            MyItemType itemType;
            itemType = MyItemType.MakeComponent(subtypeId);
            if (subtypeId == "Missile200mm")
            {
                blueprint = MyDefinitionId.Parse("MyObjectBuilder_BlueprintDefinition/Missile200mm");
            }
            else if (subtypeId == "NATO_25x184mm")
            {
                blueprint = MyDefinitionId.Parse("MyObjectBuilder_BlueprintDefinition/NATO_25x184mmMagazine");
            }
            else if (subtypeId == "NATO_5p56x45mm")
            {
                blueprint = MyDefinitionId.Parse("MyObjectBuilder_BlueprintDefinition/NATO_5p56x45mmMagazine");
            }
            else if (subtypeId == "BulletproofGlass")
            {
                blueprint = MyDefinitionId.Parse("MyObjectBuilder_BlueprintDefinition/BulletproofGlass");
            }
            else if (subtypeId == "Canvas")
            {
                blueprint = MyDefinitionId.Parse("MyObjectBuilder_BlueprintDefinition/Canvas");
            }
            else if (subtypeId == "Computer")
            {
                blueprint = MyDefinitionId.Parse("MyObjectBuilder_BlueprintDefinition/ComputerComponent");
            }
            else if (subtypeId == "Construction")
            {
                blueprint = MyDefinitionId.Parse("MyObjectBuilder_BlueprintDefinition/ConstructionComponent");
            }
            else if (subtypeId == "Detector")
            {
                blueprint = MyDefinitionId.Parse("MyObjectBuilder_BlueprintDefinition/DetectorComponent");
            }
            else if (subtypeId == "Display")
            {
                blueprint = MyDefinitionId.Parse("MyObjectBuilder_BlueprintDefinition/Display");
            }
            else if (subtypeId == "Explosives")
            {
                blueprint = MyDefinitionId.Parse("MyObjectBuilder_BlueprintDefinition/ExplosivesComponent");
            }
            else if (subtypeId == "Girder")
            {
                blueprint = MyDefinitionId.Parse("MyObjectBuilder_BlueprintDefinition/GirderComponent");
            }
            else if (subtypeId == "GravityGenerator")
            {
                blueprint = MyDefinitionId.Parse("MyObjectBuilder_BlueprintDefinition/GravityGeneratorComponent");
            }
            else if (subtypeId == "InteriorPlate")
            {
                blueprint = MyDefinitionId.Parse("MyObjectBuilder_BlueprintDefinition/InteriorPlate");
            }
            else if (subtypeId == "LargeTube")
            {
                blueprint = MyDefinitionId.Parse("MyObjectBuilder_BlueprintDefinition/LargeTube");
            }
            else if (subtypeId == "Medical")
            {
                blueprint = MyDefinitionId.Parse("MyObjectBuilder_BlueprintDefinition/MedicalComponent");
            }
            else if (subtypeId == "MetalGrid")
            {
                blueprint = MyDefinitionId.Parse("MyObjectBuilder_BlueprintDefinition/MetalGrid");
            }
            else if (subtypeId == "Motor")
            {
                blueprint = MyDefinitionId.Parse("MyObjectBuilder_BlueprintDefinition/MotorComponent");
            }
            else if (subtypeId == "AutomaticRifleGun_Mag_20rd")
            {
                blueprint = MyDefinitionId.Parse("MyObjectBuilder_BlueprintDefinition/AutomaticRifleGun_Mag_20rd");
            }
            else if (subtypeId == "PowerCell")
            {
                blueprint = MyDefinitionId.Parse("MyObjectBuilder_BlueprintDefinition/PowerCell");
            }
            else if (subtypeId == "RadioCommunication")
            {
                blueprint = MyDefinitionId.Parse("MyObjectBuilder_BlueprintDefinition/RadioCommunicationComponent");
            }
            else if (subtypeId == "Reactor")
            {
                blueprint = MyDefinitionId.Parse("MyObjectBuilder_BlueprintDefinition/ReactorComponent");
            }
            else if (subtypeId == "SmallTube")
            {
                blueprint = MyDefinitionId.Parse("MyObjectBuilder_BlueprintDefinition/SmallTube");
            }
            else if (subtypeId == "SolarCell")
            {
                blueprint = MyDefinitionId.Parse("MyObjectBuilder_BlueprintDefinition/SolarCell");
            }
            else if (subtypeId == "SteelPlate")
            {
                blueprint = MyDefinitionId.Parse("MyObjectBuilder_BlueprintDefinition/SteelPlate");
            }
            else if (subtypeId == "Superconductor")
            {
                blueprint = MyDefinitionId.Parse("MyObjectBuilder_BlueprintDefinition/Superconductor");
            }
            else if (subtypeId == "Thrust")
            {
                blueprint = MyDefinitionId.Parse("MyObjectBuilder_BlueprintDefinition/ThrustComponent");
            }
            else if (subtypeId == "ZoneChip")
            {
                blueprint = MyDefinitionId.Parse("MyObjectBuilder_BlueprintDefinition/ZoneChip");
            }
            else
            {
                blueprint = MyDefinitionId.Parse("MyObjectBuilder_BlueprintDefinition/none");
            }
            return blueprint;
        }
        
