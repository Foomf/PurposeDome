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
    public static class PluginLoaderExtensions
    {
        private static readonly ILogger Log = Serilog.Log.ForContext<PluginLoader>();

        public static IEnumerable<UnloadedPlugin> FindPlugins(this PluginLoader self)
        {
            return
                from pluginDir in Directory.EnumerateDirectories(self.PluginDir)
                let config = ReadPluginToml(pluginDir)
                where config != null
                select new UnloadedPlugin(pluginDir, config);
        }

        public static void Load(this PluginLoader self)
        {
            var unloadedPlugins = self.FindPlugins().ToList();
            foreach (var unloadedPlugin in unloadedPlugins)
            {
                self.Load(unloadedPlugin);
            }
        }

        internal static void Load(this PluginLoader self, UnloadedPlugin uPlugin)
        {
            try
            {
                var p = new Plugin(uPlugin);
            }
            catch (Exception e)
            {
                Log.Error("Failed to load plugin {p}: {message}", uPlugin.Config.Name, e.Message);
            }
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
