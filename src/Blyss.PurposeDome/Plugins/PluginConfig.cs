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

namespace Blyss.PurposeDome.Plugins
{
    public class PluginConfig
    {
        public PluginConfig(PluginTomlConfig toml)
        {
            Name = ReadRequired(toml.Name, nameof(toml.Name));
            Id = ReadRequired(toml.Id, nameof(toml.Id));
            Dependencies = (toml.Dependencies ?? new List<Guid>()).AsReadOnly();
        }

        public string Name { get; }

        public Guid Id { get; }

        public IReadOnlyList<Guid> Dependencies { get; }

        private static T ReadRequired<T>(T? property, string name)
            where T : class
        {
            if (property == null)
            {
                throw new IncompleteTomlConfigException(
                    name, $"{name} is required!");
            }

            return property;
        }

        private static T ReadRequired<T>(T? property, string name)
            where T : struct
        {
            if (!property.HasValue)
            {
                throw new IncompleteTomlConfigException(
                    name, $"{name} is required!");
            }

            return property.Value;
        }
    }
}
