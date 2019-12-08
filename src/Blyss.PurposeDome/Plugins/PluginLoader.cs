// Copyright (c) 2019 Foomf
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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Nett;
using Serilog;

namespace Blyss.PurposeDome.Plugins
{
    public class PluginLoader
    {
        private static readonly ILogger Log = Serilog.Log.ForContext<PluginLoader>();

        public PluginLoader(string pluginDir)
        {
            PluginDir = pluginDir;

            if (!Directory.Exists(pluginDir))
            {
                Log.Information("Creating directory {dir}", pluginDir);
                Directory.CreateDirectory(pluginDir);
            }
        }

        public string PluginDir { get; }

        public void Load()
        {
            var unloadedPlugins = FindPlugins().ToList();
            foreach (var unloadedPlugin in unloadedPlugins)
            {
                Log.Information("Found plugin {name} with id {id}", unloadedPlugin.Config.Name, unloadedPlugin.Config.Id);
            }
        }

        internal IEnumerable<UnloadedPlugin> FindPlugins()
        {
            return
                from pluginDir in Directory.EnumerateDirectories(PluginDir)
                let config = ReadPluginToml(pluginDir)
                where config != null
                select new UnloadedPlugin(pluginDir, config);
        }

        internal static PluginConfig? ReadPluginToml(string dir)
        {
            var pluginFilePath = Path.Join(dir, "Plugin.toml");
            if (!File.Exists(pluginFilePath))
            {
                return null;
            }

            try
            {
                var pluginToml = Toml.ReadFile<PluginTomlConfig>(pluginFilePath);
                return new PluginConfig(pluginToml);
            }
            catch (IncompleteTomlConfigException e)
            {
                Log.Error(
                    "{toml} is incomplete! There was a problem while reading {property}: " +
                    "{details}",
                    pluginFilePath,
                    e.Property,
                    e.Details);
                return null;
            }
            catch (Exception e)
            {
                Log.Error(
                    "Directory {dir} looks like a plugin dir, but failed to read/parse {file}: " +
                    "{message}",
                    dir,
                    pluginFilePath,
                    e.Message);
                return null;
            }
        }
    }
}
