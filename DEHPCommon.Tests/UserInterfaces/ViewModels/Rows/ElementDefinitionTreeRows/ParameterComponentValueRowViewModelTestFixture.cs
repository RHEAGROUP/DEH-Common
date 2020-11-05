﻿// -------------------------------------------------------------------------------------------------
// <copyright file="ParameterComponentValueRowViewModelTestFixture.cs" company="RHEA System S.A.">
//    Copyright (c) 2020-2020 RHEA System S.A.
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

namespace DEHPCommon.Tests.UserInterfaces.ViewModels.Rows.ElementDefinitionTreeRows
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using CDP4Common.CommonData;
    using CDP4Common.EngineeringModelData;
    using CDP4Common.SiteDirectoryData;
    using CDP4Common.Types;

    using CDP4Dal;
    using CDP4Dal.Events;
    using CDP4Dal.Permission;

    using DEHPCommon.UserInterfaces.ViewModels.Rows.ElementDefinitionTreeRows;

    using Moq;

    using NUnit.Framework;

    using ReactiveUI;

    /// <summary>
    /// Suite of tests for the <see cref="ParameterComponentValueRowViewModel"/>
    /// </summary>
    [TestFixture]
    public class ParameterComponentValueRowViewModelTestFixture
    {
        private readonly Uri uri = new Uri("http://www.rheagroup.com");
        private ConcurrentDictionary<CacheKey, Lazy<Thing>> cache;
        private Mock<IPermissionService> permissionService;
        private Mock<ISession> session;

        private Participant participant;
        private Person person;
        private DomainOfExpertise activeDomain;
        private DomainOfExpertise otherDomain;

        private SiteDirectory siteDirectory;
        private EngineeringModelSetup engineeringModelSetup;
        private EngineeringModel engineeringModel;
        private Iteration iteration;
        private ElementDefinition elementDefinition;
        private ElementDefinition otherElementDefinition;
        private ElementUsage elementUsage;

        [SetUp]
        public void SetUp()
        {
            this.cache = new ConcurrentDictionary<CacheKey, Lazy<Thing>>();
            this.permissionService = new Mock<IPermissionService>();
            this.permissionService.Setup(x => x.CanRead(It.IsAny<Thing>())).Returns(true);
            this.permissionService.Setup(x => x.CanWrite(It.IsAny<Thing>())).Returns(true);

            this.session = new Mock<ISession>();
            this.session.Setup(x => x.PermissionService).Returns(this.permissionService.Object);

            this.activeDomain = new DomainOfExpertise(Guid.NewGuid(), null, this.uri) { Name = "active", ShortName = "active" };
            this.otherDomain = new DomainOfExpertise(Guid.NewGuid(), null, this.uri) { Name = "other", ShortName = "other" };

            this.person = new Person(Guid.NewGuid(), null, this.uri) { GivenName = "test", Surname = "test" };
            this.participant = new Participant(Guid.NewGuid(), null, this.uri) { Person = this.person, SelectedDomain = this.activeDomain };
            this.session.Setup(x => x.ActivePerson).Returns(this.person);
            this.session.Setup(x => x.OpenIterations).Returns(new Dictionary<Iteration, Tuple<DomainOfExpertise, Participant>>());

            this.engineeringModelSetup = new EngineeringModelSetup(Guid.NewGuid(), null, this.uri);
            this.engineeringModelSetup.Participant.Add(this.participant);
            
            this.siteDirectory = new SiteDirectory(Guid.NewGuid(), this.cache, this.uri);

            this.engineeringModel = new EngineeringModel(Guid.NewGuid(), this.cache, this.uri)
            {
                EngineeringModelSetup = this.engineeringModelSetup
            };

            this.iteration = new Iteration(Guid.NewGuid(), this.cache, this.uri);
            this.elementDefinition = new ElementDefinition(Guid.NewGuid(), this.cache, this.uri);
            this.otherElementDefinition = new ElementDefinition(Guid.NewGuid(), this.cache, this.uri);

            this.elementUsage = new ElementUsage(Guid.NewGuid(), this.cache, this.uri)
            {
                ElementDefinition = this.otherElementDefinition
            };

            this.elementDefinition.ContainedElement.Add(this.elementUsage);

            this.engineeringModel.Iteration.Add(this.iteration);
            this.iteration.Element.Add(this.elementDefinition);
            this.iteration.Element.Add(this.otherElementDefinition);
        }

        [TearDown]
        public void TearDown()
        {
            CDPMessageBus.Current.ClearSubscriptions();
        }

        [Test]
        public void VerifyThatIfParameterTypeOfParameterBaseIsNotCompoundArgumentExecptionIsThrown()
        {
            var parameter = new Parameter(Guid.NewGuid(), this.cache, this.uri);
            var textParameterType = new TextParameterType(Guid.NewGuid(), this.cache, this.uri);

            parameter.ParameterType = textParameterType;

            Assert.Throws<InvalidOperationException>(() => new ParameterComponentValueRowViewModel(parameter, 0, this.session.Object, null, null, null));
        }

        [Test]
        public void VerifyThatIfComponentIndexIsLargerThatCompoundComponentCountExceptionIsThrown()
        {
            var parameter = new Parameter(Guid.NewGuid(), this.cache, this.uri) {Container = new ElementDefinition(Guid.NewGuid(), this.cache, this.uri)};
            var compoundParameterType = new CompoundParameterType(Guid.NewGuid(), this.cache, this.uri);
            var component1 = new ParameterTypeComponent(Guid.NewGuid(), this.cache, this.uri);
            var component2 = new ParameterTypeComponent(Guid.NewGuid(), this.cache, this.uri);

            compoundParameterType.Component.Add(component1);
            compoundParameterType.Component.Add(component2);

            parameter.ParameterType = compoundParameterType;

            Assert.Throws<ArgumentOutOfRangeException>(() => new ParameterComponentValueRowViewModel(parameter, 2, this.session.Object, null, null, null));
        }

        [Test]
        public void VerifyThatIfContainerRowIsNullArgumentNullExceptionIsThrown()
        {
            var parameter = new Parameter(Guid.NewGuid(), this.cache, this.uri);
            var compoundParameterType = new CompoundParameterType(Guid.NewGuid(), this.cache, this.uri);
            var component1 = new ParameterTypeComponent(Guid.NewGuid(), this.cache, this.uri);
            compoundParameterType.Component.Add(component1);
            parameter.ParameterType = compoundParameterType;

            Assert.Throws<ArgumentNullException>(() => new ParameterComponentValueRowViewModel(parameter, 0, this.session.Object, null, null, null));
        }

        [Test]
        public void VerifyThatTheSwitchIsUpdatedWhenContainerRowIsAParameterValueBaseRowViewModel()
        {
            var parameter = new Parameter(Guid.NewGuid(), this.cache, this.uri);
            var boolPt = new BooleanParameterType(Guid.NewGuid(), this.cache, this.uri);
            var compoundParameterType = new CompoundParameterType(Guid.NewGuid(), this.cache, this.uri);
            var component1 = new ParameterTypeComponent(Guid.NewGuid(), this.cache, this.uri) { ParameterType = boolPt };
            var component2 = new ParameterTypeComponent(Guid.NewGuid(), this.cache, this.uri) { ParameterType = boolPt };
            compoundParameterType.Component.Add(component1);
            compoundParameterType.Component.Add(component2);
            parameter.ParameterType = compoundParameterType;

            this.elementDefinition.Parameter.Add(parameter);

            var parameterRowViewModel = new ParameterRowViewModel(parameter, this.session.Object, null);
            var component1row = (ParameterComponentValueRowViewModel)parameterRowViewModel.ContainedRows.First();
            var component2row = (ParameterComponentValueRowViewModel)parameterRowViewModel.ContainedRows.Last();
            component1row.Switch = ParameterSwitchKind.COMPUTED;

            Assert.AreEqual(ParameterSwitchKind.COMPUTED, component2row.Switch);
        }

        [Test]
        public void VerifyThatTheSwitchIsUpdatedWhenContainerRowIsAParameterOrOverrideBaseRowViewModel()
        {
            var parameter = new Parameter(Guid.NewGuid(), this.cache, this.uri);
            var boolPt = new BooleanParameterType(Guid.NewGuid(), this.cache, this.uri);
            var compoundParameterType = new CompoundParameterType(Guid.NewGuid(), this.cache, this.uri);
            var component1 = new ParameterTypeComponent(Guid.NewGuid(), this.cache, this.uri) { ParameterType = boolPt };
            var component2 = new ParameterTypeComponent(Guid.NewGuid(), this.cache, this.uri) { ParameterType = boolPt };
            compoundParameterType.Component.Add(component1);
            compoundParameterType.Component.Add(component2);
            parameter.ParameterType = compoundParameterType;
            
            var parameterOverride = new ParameterOverride(Guid.NewGuid(), this.cache, this.uri);
            parameterOverride.Parameter = parameter;

            this.elementUsage.ParameterOverride.Add(parameterOverride);

            var parameterOverrideRowViewModel = new ParameterOverrideRowViewModel(parameterOverride, this.session.Object, null);

            var component1row = (ParameterComponentValueRowViewModel)parameterOverrideRowViewModel.ContainedRows.First();
            var component2row = (ParameterComponentValueRowViewModel)parameterOverrideRowViewModel.ContainedRows.Last();
            component1row.Switch = ParameterSwitchKind.COMPUTED;

            Assert.AreEqual(ParameterSwitchKind.COMPUTED, component2row.Switch);
        }

        [Test]
        public void VerifyThatTheSwitchIsUpdatedWhenContainerRowIsAParameterSubscriptionRowViewModel()
        {
            var parameter = new Parameter(Guid.NewGuid(), this.cache, this.uri);
            parameter.Owner = this.activeDomain;
            var boolPt = new BooleanParameterType(Guid.NewGuid(), this.cache, this.uri);
            var compoundParameterType = new CompoundParameterType(Guid.NewGuid(), this.cache, this.uri);
            var component1 = new ParameterTypeComponent(Guid.NewGuid(), this.cache, this.uri) { ParameterType = boolPt };
            var component2 = new ParameterTypeComponent(Guid.NewGuid(), this.cache, this.uri) { ParameterType = boolPt };
            compoundParameterType.Component.Add(component1);
            compoundParameterType.Component.Add(component2);
            parameter.ParameterType = compoundParameterType;
            this.elementDefinition.Parameter.Add(parameter);

            var parameterSubscription = new ParameterSubscription(Guid.NewGuid(), this.cache, this.uri);
            parameterSubscription.Owner = this.otherDomain;

            parameter.ParameterSubscription.Add(parameterSubscription);

            var parameterSubscriptionRowViewModel = new ParameterSubscriptionRowViewModel(parameterSubscription, this.session.Object, null);

            var component1row = (ParameterComponentValueRowViewModel)parameterSubscriptionRowViewModel.ContainedRows.First();
            var component2row = (ParameterComponentValueRowViewModel)parameterSubscriptionRowViewModel.ContainedRows.Last();
            component1row.Switch = ParameterSwitchKind.COMPUTED;

            Assert.AreEqual(ParameterSwitchKind.COMPUTED, component2row.Switch);
        }
        
        [Test]
        public void VerifyThatOwnerIsUpdatedForSubscription()
        {
            var parameter = new Parameter(Guid.NewGuid(), this.cache, this.uri);
            parameter.Owner = this.activeDomain;
            var boolPt = new BooleanParameterType(Guid.NewGuid(), this.cache, this.uri);
            var compoundParameterType = new CompoundParameterType(Guid.NewGuid(), this.cache, this.uri);
            var component1 = new ParameterTypeComponent(Guid.NewGuid(), this.cache, this.uri) { ParameterType = boolPt };
            var component2 = new ParameterTypeComponent(Guid.NewGuid(), this.cache, this.uri) { ParameterType = boolPt };
            compoundParameterType.Component.Add(component1);
            compoundParameterType.Component.Add(component2);
            parameter.ParameterType = compoundParameterType;
            this.elementDefinition.Parameter.Add(parameter);

            var parameterSubscription = new ParameterSubscription(Guid.NewGuid(), this.cache, this.uri);
            parameterSubscription.Owner = this.otherDomain;

            parameter.ParameterSubscription.Add(parameterSubscription);

            var parameterSubscriptionRowViewModel = new ParameterSubscriptionRowViewModel(parameterSubscription, this.session.Object, null);

            var component1row = (ParameterComponentValueRowViewModel)parameterSubscriptionRowViewModel.ContainedRows.First();
            Assert.IsTrue(component1row.OwnerName.Contains(this.activeDomain.Name));

            this.activeDomain.Name = "updated";
            var propertyInfo = typeof(DomainOfExpertise).GetProperty("RevisionNumber");
            propertyInfo?.SetValue(this.activeDomain, 100);

            CDPMessageBus.Current.SendObjectChangeEvent(this.activeDomain, EventKind.Updated);

            Assert.IsTrue(component1row.OwnerName.Contains("updated"));
        }

        [Test]
        public void VerifyThatOwnerIsUpdatedForParameterOrOverride()
        {
            var parameter = new Parameter(Guid.NewGuid(), this.cache, this.uri);
            parameter.Owner = this.activeDomain;
            var boolPt = new BooleanParameterType(Guid.NewGuid(), this.cache, this.uri);
            var compoundParameterType = new CompoundParameterType(Guid.NewGuid(), this.cache, this.uri);
            var component1 = new ParameterTypeComponent(Guid.NewGuid(), this.cache, this.uri) { ParameterType = boolPt };
            var component2 = new ParameterTypeComponent(Guid.NewGuid(), this.cache, this.uri) { ParameterType = boolPt };
            compoundParameterType.Component.Add(component1);
            compoundParameterType.Component.Add(component2);
            parameter.ParameterType = compoundParameterType;
            this.elementDefinition.Parameter.Add(parameter);

            var parameterSubscriptionRowViewModel = new ParameterRowViewModel(parameter, this.session.Object, null);

            var component1row = (ParameterComponentValueRowViewModel)parameterSubscriptionRowViewModel.ContainedRows.First();
            Assert.IsTrue(component1row.OwnerName.Contains(this.activeDomain.Name));

            this.activeDomain.Name = "updated";
            var propertyInfo = typeof(DomainOfExpertise).GetProperty("RevisionNumber");
            propertyInfo?.SetValue(this.activeDomain, 100);
            CDPMessageBus.Current.SendObjectChangeEvent(this.activeDomain, EventKind.Updated);

            Assert.IsTrue(component1row.OwnerName.Contains("updated"));
        }
    }
}