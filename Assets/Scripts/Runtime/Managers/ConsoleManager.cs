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
            name: "clear_playerprefs",
            aliases: "clearprefs",
            helpText: "Clears all PlayerPrefs and reloads scene",
            callback: () => {
                DevConsole.CloseConsole();
                PlayerPrefs.DeleteAll();
                GameManager.Instance.ReloadScene();
            }));

        DevConsole.AddCommand(Command.Create(
            name: "reload_active_scene",
            aliases: "r",
            helpText: "Reloads active scene",
            callback: () => {
                DevConsole.CloseConsole();
                GameManager.Instance.ReloadScene();
            }));

        DevConsole.AddCommand(Command.Create(
            name: "skip_current_state",
            aliases: "ss",
            helpText: "Skips currently active state",
            callback: () => {
                DevConsole.CloseConsole();
                BoardManager.Instance.battleSystem.SkipState();
            }));

        DevConsole.AddCommand(Command.Create<string>(
            name: "set_layout",
            aliases: "setLayout",
            helpText: "Changes layout",
            p1: Parameter.Create(
                name: "layoutKey",
                helpText: "Key of the layout to change to"
                ),
            callback: (string layoutKey) => {
                if (BoardManager.Instance.SetBoardLayout(layoutKey)) {
                    DevConsole.CloseConsole();
                    GameManager.Instance.ReloadScene();
                }
            }));

        DevConsole.AddCommand(Command.Create(
            name: "get_layout_keys",
            aliases: "getLayoutKeys",
            helpText: "Prints the keys of available layouts",
            callback: () => {
                BoardManager.Instance.PrintLayoutKeys();
            }));
    }
}
