using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class NextCategory
{
    //register the category, if it already exists then don't

    //DO NOT TOUCH THIS BIT OF CODE
    public static void RegistCategory()
    {
        //DO NOT TOUCH THIS BIT OF CODE
        string name = "Nextbots";
        string description = "Nextbot-style Creations.";
        Sprite icon = (ModAPI.LoadSprite("caticon.png"));
        //DO NOT TOUCH THIS BIT OF CODE
        //DO NOT TOUCH THIS BIT OF CODE
        //DO NOT TOUCH THIS BIT OF CODE
        //DO NOT TOUCH THIS BIT OF CODE
        //DO NOT TOUCH THIS BIT OF CODE
        //DO NOT TOUCH THIS BIT OF CODE
        //DO NOT TOUCH THIS BIT OF CODE
        //DO NOT TOUCH THIS BIT OF CODE
        CatalogBehaviour manager = UnityEngine.Object.FindObjectOfType<CatalogBehaviour>();
        if (manager.Catalog.Categories.FirstOrDefault((Category c) => c.name == name) == null)
        {
            Category category = ScriptableObject.CreateInstance<Category>();
            category.name = name;
            category.Description = description;
            category.Icon = icon;
            Category[] NewCategories = new Category[manager.Catalog.Categories.Length + 1];
            Category[] categories = manager.Catalog.Categories;
            for (int i = 0; i < categories.Length; i++)
            {
                NewCategories[i] = categories[i];
            }
            NewCategories[NewCategories.Length - 1] = category;
            manager.Catalog.Categories = NewCategories;
        }
    }
}