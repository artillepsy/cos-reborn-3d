using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Shared.AppStartup.CommandLine
{
    public class ArgsS
    {
        public int PlayersCount => _playerCountArg.Value;
        public string Secret => _secretArg.Value;
        
        private readonly Arg<int> _playerCountArg = new Arg<int>{Name = "player-count"};
        private readonly Arg<string> _secretArg = new Arg<string>{Name = "secret"};

        public ArgsS(IEnumerable<string> args)
        {
            foreach (var arg in args)
            {
                var nameVal = arg.Split(CommandLineArgs.Separator);
                if (nameVal.Length != 2)
                {
                    Debug.LogWarning($"{arg} name-val pair length != 2. it equals to {nameVal.Length}");
                    continue;
                }
                var name = nameVal[0];
                var val = nameVal[1];
                if (_playerCountArg.Name == name)
                {
                    _playerCountArg.Value = int.Parse(val);
                }
                else if (_secretArg.Name == name)
                {
                    _secretArg.Value = val;
                }
            }
        }
    }
}