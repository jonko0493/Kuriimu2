﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using Kontract.Models;
using Kontract.Models.Image;

namespace Kontract.Interfaces.Image
{
    /// <summary>
    /// This is the indexed image interface to edit a palette of a given <see cref="IndexedBitmapInfo"/>.
    /// </summary>
    public interface IIndexedImageAdapter : IImageAdapter
    {
        /// <summary>
        /// The list of formats provided by the image adapter to change encoding of the image data.
        /// </summary>
        IList<EncodingInfo> PaletteEncodingInfos { get; }

        /// <summary>
        /// Instructs the plugin to transcode a given image into a new encoding.
        /// </summary>
        /// <param name="image">The image to be transcoded.</param>
        /// <param name="imageEncoding">The <see cref="EncodingInfo"/> to transcode the image into.</param>
        /// <param name="paletteEncoding">The <see cref="EncodingInfo"/> to transcode the palette into.</param>
        /// <param name="updatePalette">If the palette should be updated.</param>
        /// <param name="progress">The <see cref="IProgress{ProgressReport}"/> to report progress through.</param>
        /// <returns>Transcoded image and if the operation was successful.</returns>
        Task<TranscodeResult> TranscodeImage(BitmapInfo image, EncodingInfo imageEncoding, EncodingInfo paletteEncoding, bool updatePalette, IProgress<ProgressReport> progress);

        /// <summary>
        /// Instructs the plugin to update the <see cref="BitmapInfo"/> accordingly with the new information.
        /// </summary>
        /// <param name="info">The <see cref="BitmapInfo"/> to be updated.</param>
        /// <param name="image">Image to commit.</param>
        /// <param name="imageEncoding"><see cref="EncodingInfo"/> the image is encoded in.</param>
        /// <param name="palette">The palette to commit.</param>
        /// <param name="paletteEncoding"><see cref="EncodingInfo"/> the palette is encoded in.</param>
        /// <returns>Is commitment successful.</returns>
        bool Commit(BitmapInfo info, Bitmap image, EncodingInfo imageEncoding, IList<Color> palette, EncodingInfo paletteEncoding);

        /// <summary>
        /// Sets the whole palette.
        /// </summary>
        /// <param name="info">The image info to set the palette in.</param>
        /// <param name="palette">The palette to set.</param>
        /// <param name="progress">The progress object to report progress through.</param>
        /// <returns>True if the palette was set successfully, False otherwise.</returns>
        Task<TranscodeResult> SetPalette(IndexedBitmapInfo info, IList<Color> palette, IProgress<ProgressReport> progress);

        /// <summary>
        /// Sets a single color in the palette.
        /// </summary>
        /// <param name="info">The image info to set the color in.</param>
        /// <param name="index">The index to set the color to in the palette.</param>
        /// <param name="color">The color to set in the palette.</param>
        /// <param name="progress">The progress object to report progress through.</param>
        /// <returns>True if the palette was set successfully, False otherwise.</returns>
        Task<TranscodeResult> SetColorInPalette(IndexedBitmapInfo info, int index, Color color, IProgress<ProgressReport> progress);
    }
}