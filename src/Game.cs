class Game
{
	private Parser parser;
	private Room currentRoom;
	private Player player;
	private Item knife;

	public Game()
	{
		parser = new Parser();
		player = new Player();
		CreateItems();
		CreateRooms();
	}

	private void CreateItems()
	{
		knife = new Item(5, "knife", true, 10);
	}

	private void CreateRooms()
	{
		Room outside = new Room("outside the main entrance of the university");
		Room theatre = new Room("in a lecture theatre");
		Room pub = new Room("in the campus pub");
		Room lab = new Room("in a computing lab");
		Room office = new Room("in the computing admin office");

		outside.AddExit("east", theatre);
		outside.AddExit("south", lab);
		outside.AddExit("west", pub);

		theatre.AddExit("west", outside);

		pub.AddExit("east", outside);

		lab.AddExit("north", outside);
		lab.AddExit("east", office);

		office.AddExit("west", lab);
		office.AddItem(knife);
		// office.AddEnemy("Anonymous Person", 100, 5);

		currentRoom = outside;
		player.CurrentRoom = outside;
	}

	public void Play()
	{
		PrintWelcome();

		bool finished = false;
		while (!finished)
		{
			Command command = parser.GetCommand();
			finished = ProcessCommand(command);
			if (!player.IsAlive())
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("\nYou have died. Game over.");
				Console.ResetColor();
				finished = true;
			}
		}
		Console.WriteLine("");
		Console.WriteLine("Thank you for playing.");
		Console.WriteLine("Press [Enter] to close the game...");
		Console.ReadLine();
	}

	private void PrintWelcome()
	{
		Console.WriteLine();
		Console.WriteLine("Zuul is a new, incredibly fun tekst adventure game.");
		Console.ForegroundColor = ConsoleColor.Green;
		Console.WriteLine("Type 'help' for more options.");
		Console.ResetColor();
		Console.WriteLine();
		Console.ForegroundColor = ConsoleColor.Blue;
		Console.WriteLine("You are hurt and looking for help. You decided to go to the university to search for help.");
		Console.ResetColor();
		Console.WriteLine();
		Console.WriteLine(currentRoom.GetLongDescription());
	}

	private bool ProcessCommand(Command command)
	{
		bool wantToQuit = false;

		if (command.IsUnknown())
		{
			Console.WriteLine();
			Console.WriteLine("I don't know what you mean. Try 'help'.");
			return wantToQuit;
		}

		switch (command.CommandWord)
		{
			case "help":
				PrintHelp();
				break;
			case "go":
				GoRoom(command);
				break;
			case "quit":
				wantToQuit = true;
				break;
			case "look":
				Look(command);
				break;
			case "status":
				Status(command);
				break;
			case "take":
				Take(command);
				break;
			case "drop":
				Drop(command);
				break;
			case "use":
				Use(command);
				break;
			case "attack":
				Attack(command);
				break;
		}

		return wantToQuit;
	}

	private void PrintHelp()
	{
		// Console.WriteLine("You are lost and alone.");
		// Console.WriteLine("You wander around at the university.");
		Console.WriteLine();
		Console.ForegroundColor = ConsoleColor.Yellow;
		parser.PrintValidCommands();
		Console.ResetColor();

	}

	private void GoRoom(Command command)
	{
		if (!command.HasSecondWord())
		{
			Console.WriteLine();
			Console.WriteLine("Where do you want to go? Use 'go' followed by a direction.");
			Console.WriteLine("Go where?");
			return;
		}

		string direction = command.SecondWord;

		Room nextRoom = currentRoom.GetExit(direction);

		if (nextRoom == null)
		{
			Console.WriteLine();
			Console.WriteLine("There is no door!");
		}
		else
		{
			currentRoom = nextRoom;
			player.CurrentRoom = nextRoom;
			player.Damage(10);
			Console.WriteLine();
			Console.WriteLine(currentRoom.GetLongDescription());
		}
	}

	private void Look(Command command)
	{
		Console.WriteLine(currentRoom.GetLongDescription());
		if (currentRoom.Items.Count > 0)
		{
			Console.Write("You see ");
			bool firstItem = true;
			foreach (var item in currentRoom.Items.Values)
			{
				if (!firstItem)
				{
					Console.Write(", ");
				}
				Console.Write("a " + item.Description);
				firstItem = false;
			}
			Console.WriteLine();
		}
	}

	private void Status(Command command)
	{
		Console.WriteLine("Health: " + player.GetHealth());
		Console.WriteLine();
		Console.WriteLine("Player Inventory:");

		if (player.Inventory.Items.Count == 0)
		{
			Console.WriteLine("Inventory is empty");
		}
		else
		{
			foreach (var item in player.Inventory.Items)
			{
				Console.WriteLine(item.Value.Description);
			}
		}
	}

	private void Take(Command command)
	{
		if (!command.HasSecondWord())
		{
			Console.WriteLine("What do you want to take?");
			return;
		}

		string item = command.SecondWord;

		if (currentRoom.Items.ContainsKey(item))
		{
			Item itemToTake = currentRoom.RemoveItem(item);
			player.Inventory.AddItem(itemToTake);
			Console.WriteLine("You took the " + item);
		}
		else
		{
			Console.WriteLine("There is no " + item + " in this room");
		}
	}
	private void Drop(Command command)
	{
		if (!command.HasSecondWord())
		{
			Console.WriteLine("What do you want to dropped?");
			return;
		}

		string item = command.SecondWord;

		if (player.Inventory.Contains(item))
		{
			Item removedItem = player.Inventory.RemoveItem(item);
			currentRoom.Items.Add(item, removedItem);
			Console.WriteLine("You dropped a " + item);
		}
		else
		{
			Console.WriteLine("You don't have a " + item);
		}
	}

	private void Use(Command command)
	{
		if (!command.HasSecondWord())
		{
			Console.WriteLine("What do you want to use?");
			return;
		}

		string item = command.SecondWord;

		if (!command.HasThirdWord())
		{
			if (player.Inventory.Contains(item))
			{
				Console.WriteLine("You used " + item);
			}
			else
			{
				Console.WriteLine("You don't have " + item);
			}
		}
		else
		{
			if (!player.Inventory.Contains(item))
			{
				Console.WriteLine("You don't have " + item);
			}
			else
			{
				string target = command.ThirdWord;
				Console.WriteLine("There is nothing at " + target + " to use " + item + " on");
			}
		}
	}

	private void Attack(Command command)
	{
		if (!command.HasSecondWord())
		{
			Console.WriteLine("Who do you want to attack?");
			return;
		}

		string target = command.SecondWord;

		if (!command.HasThirdWord())
		{
			Console.WriteLine("What do you want to attack " + target + " with?");
			return;
		}

		string weapon = command.ThirdWord;

		if (player.Inventory.Contains(weapon))
		{
			Console.WriteLine("You attacked " + target + " with " + weapon);
		}
		else
		{
			Console.WriteLine("You don't have " + weapon);
		}
	}

}