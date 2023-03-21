//https://github.com/DavidF-Dev/Unity-DeveloperConsole

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DavidFDev.DevConsole;

public class ConsoleManager : MonoBehaviour {
    private void Awake() {
        DevConsole.EnableConsole();
        DevConsole.SetToggleKey(KeyCode.Escape);

        DevConsole.AddCommand(Command.Create(
            name: "Reload_Active_Scene",
            aliases: "r",
            helpText: "Reloads active scene",
            callback: () => {
                DevConsole.CloseConsole();
                GameManager.Instance.ReloadScene();
            }));

        DevConsole.AddCommand(Command.Create(
            name: "Skip_Current_State",
            aliases: "ss",
            helpText: "Skips currently active state",
            callback: () => {
                DevConsole.CloseConsole();
                BoardManager.Instance.battleSystem.SkipState();
            }));
    }
}
