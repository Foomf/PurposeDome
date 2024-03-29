﻿// Copyright (c) 2019 Foomf
//
// This software is provided 'as-is', without any express or implied
// warranty. In no event will the authors be held liable for any damages
// arising from the use of this software.
//
// Permission is granted to anyone to use this software for any purpose,
// including commercial applications, and to alter it and redistribute it
// freely, subject to the following restrictions:
//
// 1. The origin of this software must not be misrepresented; you must not
//    claim that you wrote the original software. If you use this software
//    in a product, an acknowledgment in the product documentation would be
//    appreciated but is not required.
// 2. Altered source versions must be plainly marked as such, and must not be
//    misrepresented as being the original software.
// 3. This notice may not be removed or altered from any source distribution.

using System;
using System.IO;
using MoonSharp.Interpreter;
using Serilog;

namespace Blyss.PurposeDome.Plugins
{
    public class Plugin
    {
        private static readonly ILogger Log = Serilog.Log.ForContext<Plugin>();

        private readonly Script _script = new Script();

        public Plugin(UnloadedPlugin p)
        {
            _script.Options.DebugPrint = Log.Information;

            var entryPoint = Path.Combine(p.Dir, p.Config.EntryFile);
            if (!File.Exists(entryPoint))
            {
                throw new PluginLoadFailException($"File {p.Config.EntryFile} not found!");
            }

            _script.DoFile(entryPoint);
            var mainFunc = _script.Globals[p.Config.EntryFunction];
            _script.Call(mainFunc);
        }
    }
}
