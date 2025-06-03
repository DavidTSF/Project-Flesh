
using System;
using System.Collections.Generic;

public class MenuItemOld
{
    public string Title;
    public string Description;
    public List<MenuItemOld> Children = new();
    public Action OnSelect;
}