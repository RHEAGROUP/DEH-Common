﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IOpenSaveFileDialogService.cs"company="RHEA System S.A.">
//    Copyright(c) 2020 RHEA System S.A.
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Kamil Wojnowski, Nathanael Smiechowski.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace DEHPCommon.Services.FileDialogService
{
    /// <summary>
    /// The Interface for the service responsible for opening and saving files
    /// </summary>
    public interface IOpenSaveFileDialogService
    {
        /// <summary>
        /// Gets the list of file paths to open.
        /// </summary>
        /// <param name="checkFileExists">if true, Check whether the file exists</param>
        /// <param name="checkPathExists">if true check whether the path exists</param>
        /// <param name="multiSelect">if true, multiple files may be selected</param>
        /// <param name="filter">the file filter</param>
        /// <param name="extension">the default extension of the file(s) to open</param>
        /// <param name="initialPath">the initial path</param>
        /// <param name="filterIndex">the index of the filter currently selected</param>
        /// <returns>
        /// An array of file paths to open or null if the operation of the dialog was cancelled.
        /// </returns>
        string[] GetOpenFileDialog(bool checkFileExists, bool checkPathExists, bool multiSelect, string filter, string extension, string initialPath, int filterIndex);

        /// <summary>
        /// Gets the location of the file to be saved
        /// </summary>
        /// <param name="defaultFilename">the default filename</param>
        /// <param name="extension">the extension of the file to create</param>
        /// <param name="filter">the filter for the dialog</param>
        /// <param name="initialPath">the initial path for the dialog</param>
        /// <param name="filterIndex">the index of the filter currently selected</param>
        /// <returns>the path of the file to create or null if the operation was cancelled.</returns>
        string GetSaveFileDialog(string defaultFilename, string extension, string filter, string initialPath, int filterIndex);
    }
}
