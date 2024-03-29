// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NetChangePreviewViewModel.cs" company="RHEA System S.A.">
//    Copyright (c) 2020-2021 RHEA System S.A.
// 
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski.
// 
//    This file is part of DEHP Common Library
// 
//    The DEHPCommon is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Lesser General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
// 
//    The DEHPCommon is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
// 
//    You should have received a copy of the GNU Lesser General Public License
//    along with this program; if not, write to the Free Software Foundation,
//    Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace DEHPCommon.UserInterfaces.ViewModels.NetChangePreview
{
    using CDP4Common.CommonData;

    using DEHPCommon.HubController.Interfaces;
    using DEHPCommon.Services.ObjectBrowserTreeSelectorService;
    using DEHPCommon.UserInterfaces.ViewModels.Interfaces;
    using DEHPCommon.UserInterfaces.ViewModels.NetChangePreview.Interfaces;

    using ReactiveUI;

    /// <summary>
    /// View model for the preview net change pane allowing the user to preview the change that will be transfered to either the Dst of the Hub
    /// </summary>
    public abstract class NetChangePreviewViewModel : ObjectBrowserBaseViewModel, INetChangePreviewViewModel
    {
        /// <summary>
        /// Gets the collection of <see cref="Thing"/>s at their previous state, this property is used for updating the Net change preview based on a selection
        /// </summary>
        protected ReactiveList<Thing> ThingsAtPreviousState { get; } = new ReactiveList<Thing>();

        /// <summary>
        /// Gets the collection of <see cref="Thing"/>s to be actually transfered
        /// </summary>
        protected ReactiveList<object> SelectedThingsToTransfer { get; } = new ReactiveList<object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="NetChangePreviewViewModel"/> class.
        /// </summary>
        /// <param name="hubController">The <see cref="IHubController"/></param>
        /// <param name="objectBrowserTreeSelectorService">The <see cref="IObjectBrowserTreeSelectorService"/></param>
        protected NetChangePreviewViewModel(IHubController hubController, IObjectBrowserTreeSelectorService objectBrowserTreeSelectorService) : base(hubController, objectBrowserTreeSelectorService)
        {
        }

        /// <summary>
        /// Computes the old values for each <see cref="ObjectBrowserBaseViewModel.Things"/>
        /// </summary>
        public abstract void ComputeValues();
    }
}
