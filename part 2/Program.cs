using System;
using System.Collections.Generic;

public class Recipe
{
    // Properties of a recipe
    public string Name { get; set; }
    public List<Ingredient> Ingredients { get; set; }
    public List<string> Steps { get; set; }

    // Original quantities of ingredients for resetting
    private List<double> originalQuantities;

    // Delegate declaration for notifying when calories exceed a certain limit
    public delegate void CaloriesExceededHandler(string recipeName);

    // Event to be triggered when calories exceed a certain limit
    public event CaloriesExceededHandler CaloriesExceeded;

    // Constructor
    public Recipe()
    {
        Ingredients = new List<Ingredient>();
        Steps = new List<string>();
        originalQuantities = new List<double>();
    }

    // Method to add an ingredient to the recipe
    public void AddIngredient(string name, double quantity, string unit, int calories, string foodGroup)
    {
        Ingredients.Add(new Ingredient(name, quantity, unit, calories, foodGroup));
        originalQuantities.Add(quantity);
    }

    // Method to add a step to the recipe
    public void AddStep(string description)
    {
        Steps.Add(description);
    }

    // Method to print the recipe
    public void PrintRecipe(double scale)
    {
        Console.WriteLine($"Recipe: {Name}\n");
        Console.WriteLine("Ingredients:");
        for (int i = 0; i < Ingredients.Count; i++)
        {
            var originalQuantity = originalQuantities[i];
            var scaledQuantity = Ingredients[i].Quantity * scale;
            Console.WriteLine($"- {scaledQuantity} {Ingredients[i].Unit} {Ingredients[i].Name} ({Ingredients[i].Calories * scale} calories)");
        }
        Console.WriteLine("\nSteps:");
        int stepNumber = 1;
        foreach (var step in Steps)
        {
            Console.WriteLine($"{stepNumber++}. {step}");
        }
    }

    // Method to calculate total calories
    public int CalculateTotalCalories()
    {
        int totalCalories = 0;
        foreach (var ingredient in Ingredients)
        {
            totalCalories += ingredient.Calories;
        }
        return totalCalories;
    }

    // Method to reset quantities to original values
    public void ResetQuantities()
    {
        for (int i = 0; i < Ingredients.Count; i++)
        {
            Ingredients[i].Quantity = originalQuantities[i];
        }
    }

    // Method to clear the recipe
    public void ClearRecipe()
    {
        Name = null;
        Ingredients.Clear();
        Steps.Clear();
        originalQuantities.Clear();
    }

    // Method to check if calories exceed a limit and notify
    public void CheckCaloriesLimit(int limit)
    {
        if (CalculateTotalCalories() > limit)
        {
            CaloriesExceeded?.Invoke(Name);
        }
    }
}

public class Ingredient
{
    // Properties of an ingredient
    public string Name { get; set; }
    public double Quantity { get; set; }
    public string Unit { get; set; }
    public int Calories { get; set; }
    public string FoodGroup { get; set; }

    // Constructor
    public Ingredient(string name, double quantity, string unit, int calories, string foodGroup)
    {
        Name = name;
        Quantity = quantity;
        Unit = unit;
        Calories = calories;
        FoodGroup = foodGroup;
    }
}

class Program
{
    static void Main(string[] args)
    {
        List<Recipe> recipes = new List<Recipe>();

        // Main program loop
        while (true)
        {
            Console.WriteLine("Recipe Creator\n");
            Console.WriteLine("1. Create New Recipe");
            Console.WriteLine("2. List Recipes");
            Console.WriteLine("3. Select Recipe");
            Console.WriteLine("4. Scale Recipe");
            Console.WriteLine("5. Reset Quantities");
            Console.WriteLine("6. Clear Recipe");
            Console.WriteLine("7. Exit\n");

            string choice = Console.ReadLine();

            // Handling user choices
            switch (choice)
            {
                case "1":
                    CreateNewRecipe(recipes); // Create a new recipe
                    break;
                case "2":
                    ListRecipes(recipes); // List all available recipes
                    break;
                case "3":
                    SelectRecipe(recipes); // Select a recipe to view
                    break;
                case "4":
                    ScaleRecipe(recipes); // Scale a recipe
                    break;
                case "5":
                    ResetQuantities(recipes); // Reset quantities of a recipe
                    break;
                case "6":
                    ClearRecipe(recipes); // Clear a recipe
                    break;
                case "7":
                    Environment.Exit(0); // Exit the program
                    break;
                default:
                    Console.WriteLine("Invalid choice! Please enter a number from 1 to 7.\n");
                    break;
            }
        }
    }

    // Method to create a new recipe
    static void CreateNewRecipe(List<Recipe> recipes)
    {
        Recipe recipe = new Recipe();

        Console.Write("Enter recipe name: ");
        recipe.Name = Console.ReadLine();

        // Gather information about ingredients
        int ingredientCount = GetIntInput("Enter the number of ingredients: ");
        for (int i = 0; i < ingredientCount; i++)
        {
            Console.WriteLine($"\nIngredient {i + 1}:");
            string name = GetStringInput("Name: ");
            double quantity = GetDoubleInput("Quantity: ");
            string unit = GetStringInput("Unit: ");
            int calories = GetIntInput("Calories: ");
            string foodGroup = GetStringInput("Food Group: ");
            recipe.AddIngredient(name, quantity, unit, calories, foodGroup);
        }

        // Gather information about steps
        int stepCount = GetIntInput("Enter the number of steps: ");
        for (int i = 0; i < stepCount; i++)
        {
            Console.WriteLine($"\nStep {i + 1}:");
            string description = GetStringInput("Description: ");
            recipe.AddStep(description);
        }

        // Subscribe to the calories exceeded event
        recipe.CaloriesExceeded += OnCaloriesExceeded;

        // Add the created recipe to the list
        recipes.Add(recipe);
        Console.WriteLine("Recipe created successfully!\n");
    }

    // Method to list all available recipes
    static void ListRecipes(List<Recipe> recipes)
    {
        if (recipes.Count == 0)
        {
            Console.WriteLine("No recipes available.\n");
            return;
        }

        Console.WriteLine("Recipes:\n");
        for (int i = 0; i < recipes.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {recipes[i].Name}");
        }
        Console.WriteLine();
    }

    // Method to select a recipe to view
    static void SelectRecipe(List<Recipe> recipes)
    {
        if (recipes.Count == 0)
        {
            Console.WriteLine("No recipes available.\n");
            return;
        }

        ListRecipes(recipes);

        Console.WriteLine("Select a recipe by entering its number:");
        int recipeNumber = GetIntInput("Recipe Number: ");

        if (recipeNumber >= 1 && recipeNumber <= recipes.Count)
        {
            Recipe selectedRecipe = recipes[recipeNumber - 1];
            Console.WriteLine();
            selectedRecipe.PrintRecipe(1); // Display the recipe without scaling
            Console.WriteLine($"\nTotal Calories: {selectedRecipe.CalculateTotalCalories()}");
            // Check if calories exceed 300
            selectedRecipe.CheckCaloriesLimit(300);
        }
        else
        {
            Console.WriteLine("Invalid recipe number.\n");
        }
    }

    // Method to scale a recipe
    static void ScaleRecipe(List<Recipe> recipes)
    {
        if (recipes.Count == 0)
        {
            Console.WriteLine("No recipes available.\n");
            return;
        }

        ListRecipes(recipes);

        Console.WriteLine("Select a recipe to scale by entering its number:");
        int recipeNumber = GetIntInput("Recipe Number: ");

        if (recipeNumber >= 1 && recipeNumber <= recipes.Count)
        {
            Recipe selectedRecipe = recipes[recipeNumber - 1];
            Console.WriteLine("Enter scaling factor (0.5, 2, or 3):");
            double factor = GetDoubleInput("Scaling Factor: ");
            selectedRecipe.PrintRecipe(factor); // Display the recipe scaled by the factor
        }
        else
        {
            Console.WriteLine("Invalid recipe number.\n");
        }
    }

    // Method to reset quantities of a recipe
    static void ResetQuantities(List<Recipe> recipes)
    {
        if (recipes.Count == 0)
        {
            Console.WriteLine("No recipes available.\n");
            return;
        }

        ListRecipes(recipes);

        Console.WriteLine("Select a recipe to reset quantities by entering its number:");
        int recipeNumber = GetIntInput("Recipe Number: ");

        if (recipeNumber >= 1 && recipeNumber <= recipes.Count)
        {
            Recipe selectedRecipe = recipes[recipeNumber - 1];
            selectedRecipe.ResetQuantities();
            Console.WriteLine("Quantities reset successfully!\n");
        }
        else
        {
            Console.WriteLine("Invalid recipe number.\n");
        }
    }

    // Method to clear a recipe
    static void ClearRecipe(List<Recipe> recipes)
    {
        if (recipes.Count == 0)
        {
            Console.WriteLine("No recipes available.\n");
            return;
        }

        ListRecipes(recipes);

        Console.WriteLine("Select a recipe to clear by entering its number:");
        int recipeNumber = GetIntInput("Recipe Number: ");

        if (recipeNumber >= 1 && recipeNumber <= recipes.Count)
        {
            Recipe selectedRecipe = recipes[recipeNumber - 1];
            selectedRecipe.ClearRecipe();
            Console.WriteLine("Recipe cleared!\n");
        }
        else
        {
            Console.WriteLine("Invalid recipe number.\n");
        }
    }

    // Method to handle calories exceeded event
    static void OnCaloriesExceeded(string recipeName)
    {
        Console.WriteLine($"WARNING: Calories in '{recipeName}' exceed 300!");
    }

    // Helper method to get integer input from the user
    static int GetIntInput(string message)
    {
        Console.Write(message);
        int input;
        while (!int.TryParse(Console.ReadLine(), out input))
        {
            Console.WriteLine("Invalid input! Please enter an integer.\n");
            Console.Write(message);
        }
        return input;
    }

    // Helper method to get double input from the user
    static double GetDoubleInput(string message)
    {
        Console.Write(message);
        double input;
        while (!double.TryParse(Console.ReadLine(), out input))
        {
            Console.WriteLine("Invalid input! Please enter a number.\n");
            Console.Write(message);
        }
        return input;
    }

    // Helper method to get string input from the user
    static string GetStringInput(string message)
    {
        Console.Write(message);
        return Console.ReadLine();
    }
}
