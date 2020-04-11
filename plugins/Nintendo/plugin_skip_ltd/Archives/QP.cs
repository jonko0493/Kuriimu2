﻿using System.Collections.Generic;
using System.IO;
using Komponent.IO;
using Komponent.IO.Streams;
using Kontract.Models.Archive;
using Kontract.Models.IO;
using System.Linq;
using System.Text;

namespace plugin_skip_ltd.Archives
{
    public class QP
    {
        private static int _headerSize = 0x20;
        private static int _entrySize = Tools.MeasureType(typeof(QpEntry));

        public IReadOnlyList<ArchiveFileInfo> Load(Stream input)
        {
            using var br = new BinaryReaderX(input, true, ByteOrder.BigEndian);

            // Read header
            var header = br.ReadType<QpHeader>();

            // Read entries
            br.BaseStream.Position = header.entryDataOffset;
            var rootEntry = br.ReadType<QpEntry>();

            br.BaseStream.Position = header.entryDataOffset;
            var entries = br.ReadMultiple<QpEntry>(rootEntry.size);

            // Read names
            var nameStream = new SubStream(input, br.BaseStream.Position, header.entryDataSize + header.entryDataOffset - br.BaseStream.Position);

            // Add files
            using var nameBr = new BinaryReaderX(nameStream);

            var result = new List<ArchiveFileInfo>();
            var lastDirectoryEntry = entries[0];
            foreach (var entry in entries.Skip(1))
            {
                // A file does not know of its parent directory
                // The tree is structured so that the last directory entry read must hold the current file

                // Remember the last directory entry
                if (entry.IsDirectory)
                {
                    lastDirectoryEntry = entry;
                    continue;
                }

                // Find whole path recursively from lastDirectoryEntry
                var currentDirectoryEntry = lastDirectoryEntry;
                var currentPath = UPath.Empty;
                while (currentDirectoryEntry != entries[0])
                {
                    nameBr.BaseStream.Position = currentDirectoryEntry.NameOffset;
                    currentPath = nameBr.ReadCStringASCII() / currentPath;

                    currentDirectoryEntry = entries[currentDirectoryEntry.offset];
                }

                // Get file name
                nameBr.BaseStream.Position = entry.NameOffset;
                var fileName = currentPath / nameBr.ReadCStringASCII();

                var fileStream = new SubStream(input, entry.offset, entry.size);
                result.Add(new ArchiveFileInfo(fileStream, fileName.FullName));
            }

            return result;
        }

        public void Save(Stream output, IReadOnlyList<ArchiveFileInfo> files)
        {
            var darcTreeBuilder = new QpTreeBuilder(Encoding.ASCII);
            darcTreeBuilder.Build(files.Select(x => ("/." + x.FilePath.FullName, x)).ToArray());

            var entries = darcTreeBuilder.Entries;
            var nameStream = darcTreeBuilder.NameStream;

            var namePosition = _headerSize + entries.Count * _entrySize;
            var dataOffset = (namePosition + (int)nameStream.Length + 0x1F) & ~0x1F;

            using var bw = new BinaryWriterX(output, ByteOrder.BigEndian);

            // Write names
            bw.BaseStream.Position = namePosition;
            nameStream.Position = 0;
            nameStream.CopyTo(bw.BaseStream);
            bw.WriteAlignment(0x20);

            // Write files
            foreach (var (qpEntry, afi) in entries.Where(x => x.Item2 != null))
            {
                bw.WriteAlignment(0x20);
                var fileOffset = (int)bw.BaseStream.Position;

                var writtenSize = afi.SaveFileData(bw.BaseStream, null);

                qpEntry.offset = fileOffset;
                qpEntry.size = (int)writtenSize;
            }

            // Write entries
            bw.BaseStream.Position = _headerSize;
            bw.WriteMultiple(entries.Select(x=>x.Item1));

            // Write header
            bw.BaseStream.Position = 0;
            bw.WriteType(new QpHeader
            {
                entryDataOffset = _headerSize,
                entryDataSize = entries.Count * _entrySize + (int)nameStream.Length,
                dataOffset = dataOffset
            });
            bw.WritePadding(0x10, 0xCC);
        }
    }
}
