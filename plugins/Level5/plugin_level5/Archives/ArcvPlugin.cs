﻿using System;
using System.Threading.Tasks;
using Komponent.IO;
using Kontract.Interfaces.FileSystem;
using Kontract.Interfaces.Managers;
using Kontract.Interfaces.Plugins.Identifier;
using Kontract.Interfaces.Plugins.State;
using Kontract.Interfaces.Providers;
using Kontract.Models;
using Kontract.Models.IO;

namespace plugin_level5.Archives
{
    public class ArcvPlugin : IFilePlugin, IIdentifyFiles
    {
        public Guid PluginId => Guid.Parse("db8c2deb-f11d-43c8-bb9e-e271408fd896");
        public string[] FileExtensions => new[] { "*.arc" };
        public PluginMetadata Metadata { get; }

        public ArcvPlugin()
        {
            Metadata = new PluginMetadata("ARCV", "onepiecefreak", "Generic archive for 3DS Level-5 games");
        }

        public async Task<bool> IdentifyAsync(IFileSystem fileSystem, UPath filePath, ITemporaryStreamProvider temporaryStreamProvider)
        {
            var fileStream = await fileSystem.OpenFileAsync(filePath);
            using var br = new BinaryReaderX(fileStream);

            return br.ReadString(4) == "ARCV";
        }

        public IPluginState CreatePluginState(IPluginManager pluginManager)
        {
            return new ArcvState();
        }
    }
}