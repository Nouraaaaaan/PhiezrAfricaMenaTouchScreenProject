using System;
using System.Collections.Generic;
using UnityEngine;


public class Category : MonoBehaviour
{
    //each category has a list of projects.
    //each project has :-   1. a button.
    //                      2. a highlighted map. 
    //                      3. an info panel.   
    public List<Pages> Pages;
}

[Serializable]
public struct Pages
{
    public List<Project> Projects;
    public Sprite PageStaticMapSprite;
}
