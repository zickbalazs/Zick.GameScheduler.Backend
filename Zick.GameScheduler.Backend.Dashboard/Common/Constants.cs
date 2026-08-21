using System.Xml;
using Zick.GameScheduler.Backend.Dashboard.Models;

namespace Zick.GameScheduler.Backend.Dashboard.Common;

public static class Constants
{
    public const string AppName = "Assetto Corsa Game Scheduler";
    public static NavMenuGroup[] NavMenuGroups = [
        new()
        {
            Icon = MudBlazor.Icons.Material.Filled.DirectionsCar,
            Name = "Vehicles",
            Links = [
                new()
                {
                    Name = "Add Vehicle",
                    Icon = MudBlazor.Icons.Material.Filled.Add,
                    Href = "/vehicles/new"
                },
                new()
                {
                    Name = "Edit Vehicles",
                    Icon = MudBlazor.Icons.Material.Filled.Edit,
                    Href = "/vehicles/edit"
                }
            ]
        },
        new()
        {
            Icon = MudBlazor.Icons.Material.Filled.RoundaboutRight,
            Name = "Tracks",
            Links = [
                new()
                {
                    Name = "Add Track",
                    Icon = MudBlazor.Icons.Material.Filled.Add,
                    Href = "/tracks/add"
                },
                new()
                {
                    Name = "Edit Tracks",
                    Icon = MudBlazor.Icons.Material.Filled.Edit,
                    Href = "/tracks/edit"
                }
            ]
        },
        new()
        {
            Icon = MudBlazor.Icons.Material.Filled.SportsMotorsports,
            Name = "Leagues",
            Links = [
                new()
                {
                    Name = "Add League",
                    Icon = MudBlazor.Icons.Material.Filled.Add,
                    Href = "/leagues/add"
                },
                new()
                {
                    Name = "Edit Leagues",
                    Icon = MudBlazor.Icons.Material.Filled.Edit,
                    Href = "/leagues/edit"
                }
            ]
        },
        new()
        {
            Icon = MudBlazor.Icons.Material.Filled.Tag,
            Name = "Racing Classes",
            Links = [
                new()
                {
                    Name = "Add Class",
                    Icon = MudBlazor.Icons.Material.Filled.Add,
                    Href = "/classes/add"
                },
                new()
                {
                    Name = "Manage Classes",
                    Icon = MudBlazor.Icons.Material.Filled.Edit,
                    Href = "/classes/edit"
                }
            ]
        },
        new()
        {
            Icon = MudBlazor.Icons.Material.Filled.Timelapse,
            Name = "Sessions",
            Links = [
                new()
                {
                    Name = "Add Session",
                    Icon = MudBlazor.Icons.Material.Filled.Add,
                    Href = "/sessions/add"
                },
                new()
                {
                    Name = "Manage Sessions",
                    Icon = MudBlazor.Icons.Material.Filled.ManageAccounts,
                    Href = "/sessions/edit"
                }            
            ]
        }
    ];
}