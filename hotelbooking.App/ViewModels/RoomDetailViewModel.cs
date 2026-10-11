using System;
using System.Collections.Generic;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using hotelbooking.App.Models;

namespace hotelbooking.App.ViewModels;

[QueryProperty(nameof(Room), "Room")]
public partial class RoomDetailViewModel : ObservableObject
{
    [ObservableProperty]
    private Room? room;

[RelayCommand]
private async Task GoBackAsync()
{
   await Shell.Current.GoToAsync("..");
}


}

