﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefinedThingRowViewModel.cs"company="RHEA System S.A.">
//    Copyright(c) 2020 RHEA System S.A.
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Kamil Wojnowski, Nathanael Smiechowski.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace DEHPCommon.CommonUserInterface.ViewModels.Rows
{
    using CDP4Common.CommonData;

    using ReactiveUI;

    using System;

    /// <summary>
    /// Row class representing a <see cref="DefinedThing"/> as a plain object
    /// </summary>
    public abstract class DefinedThingRowViewModel<T> : ReactiveObject where T : DefinedThing
    {
        /// <summary>
        /// Backing field for <see cref="Iid"/>
        /// </summary>
        private Guid iid;

        /// <summary>
        /// Backing field for <see cref="Name"/>
        /// </summary>
        private string name;

        /// <summary>
        /// Backing field for <see cref="ShortName"/>
        /// </summary>
        private string shortName;

        /// <summary>
        /// Backing field for <see cref="RevisionNumber"/>
        /// </summary>
        private int revisionNumber;

        /// <summary>
        /// Get or sets the <see cref="Thing"/>
        /// </summary>
        public T Thing { get; private set; }

        /// <summary>
        /// Gets or sets the iid
        /// </summary>
        public Guid Iid
        {
            get => this.iid;
            private set => this.RaiseAndSetIfChanged(ref this.iid, value);
        }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name
        {
            get => this.name;
            private set => this.RaiseAndSetIfChanged(ref this.name, value);
        }

        /// <summary>
        /// Gets or sets the shortName
        /// </summary>
        public string ShortName
        {
            get => this.shortName;
            private set => this.RaiseAndSetIfChanged(ref this.shortName, value);
        }

        /// <summary>
        /// Gets or sets the revisionNumber
        /// </summary>
        public int RevisionNumber
        {
            get => this.revisionNumber;
            private set => this.RaiseAndSetIfChanged(ref this.revisionNumber, value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefinedThingRowViewModel"/> class
        /// </summary>
        /// <param name="T">The <see cref="DefinedThing"/> associated with this row</param>
        protected DefinedThingRowViewModel(T thing)
        {
            this.Thing = thing;
            this.Iid = thing.Iid;
            this.Name = thing.Name;
            this.ShortName = thing.ShortName;
            this.RevisionNumber = thing.RevisionNumber;
        }
    }
}
