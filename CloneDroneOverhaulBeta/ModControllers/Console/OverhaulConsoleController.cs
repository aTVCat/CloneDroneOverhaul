using System.Collections.Generic;
using UnityEngine;

namespace CDOverhaul
{
    public static class OverhaulConsoleController
    {
        private static readonly List<IConsoleCommandReceiver> _commandReceivers = new List<IConsoleCommandReceiver>();
        private static readonly List<string> _commands = new List<string>();

        public const string DefaultString = "Command ran";

        internal static void Initialize()
        {
            _commandReceivers.Clear();
        }

        public static bool AddListener(IConsoleCommandReceiver receiver)
        {
            if (_commandReceivers.Contains(receiver))
            {
                return false;
            }

            string[] newCommands = receiver.Commands();
            if (newCommands != null && newCommands.Length != 0)
            {
                int i = 0;
                do
                {
                    if (!_commands.Contains(newCommands[i]))
                    {
                        _commands.Add(newCommands[i]);
                    }
                    i++;
                } while (i < newCommands.Length);
            }

            _commandReceivers.Add(receiver);
            return true;
        }

        public static bool RemoveListener(IConsoleCommandReceiver receiver)
        {
            if (!_commandReceivers.Contains(receiver))
            {
                return false;
            }
            _ = _commandReceivers.Remove(receiver);
            return true;
        }

        public static void RunCommand(string input, out string output, bool outPutAnything = false)
        {
            output = DefaultString;
            if (string.IsNullOrEmpty(input))
            {
                if (outPutAnything)
                {
                    output = "Command is empty";
                }

                Debug.Log(output);
                return;
            }

            string[] array = input.Split(' ');
            array[0] = array[0].Replace("/", string.Empty);
            if (!_commands.Contains(array[0]))
            {
                if (outPutAnything)
                {
                    output += "Unknown command: " + array[0];
                }

                Debug.Log(output);
                return;
            }

            if (outPutAnything)
            {
                foreach (IConsoleCommandReceiver r in _commandReceivers)
                {
                    output = output + "\n" + r.OnCommandRan(array);
                }
                Debug.Log(output);
            }
            else
            {
                foreach (IConsoleCommandReceiver r in _commandReceivers)
                {
                    _ = r.OnCommandRan(array);
                }
            }
        }
    }
}
