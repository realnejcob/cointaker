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
            name: "ReloadActiveScene",
            aliases: "r",
            helpText: "Reloads active scene",
            callback: () => {
                DevConsole.CloseConsole();
                GameManager.Instance.ReloadScene();
            }));
    }
}
