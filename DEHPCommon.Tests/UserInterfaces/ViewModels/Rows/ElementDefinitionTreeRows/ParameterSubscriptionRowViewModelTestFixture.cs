﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParameterSubscriptionRowViewModelTestFixture.cs" company="RHEA System S.A.">
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
    using System.Collections.Generic;
    using System.Linq;

    using CDP4Common.CommonData;
    using CDP4Common.EngineeringModelData;
    using CDP4Common.Helpers;
    using CDP4Common.SiteDirectoryData;
    using CDP4Common.Types;

    using CDP4Dal;
    using CDP4Dal.Events;
    using CDP4Dal.Operations;
    using CDP4Dal.Permission;

    using DEHPCommon.UserInterfaces.ViewModels.Rows.ElementDefinitionTreeRows;

    using Moq;

    using NUnit.Framework;

    using ReactiveUI;

    [TestFixture]
    internal class ParameterSubscriptionRowViewModelTestFixture
    {
        private Mock<IPermissionService> permissionService;
        
        private Mock<ISession> session;
        private readonly Uri uri = new Uri("http://test.com");
        private Participant participant;
        private Person person;
        private DomainOfExpertise activeDomain;
        private EngineeringModel model;
        private EngineeringModelSetup modelsetup;
        private QuantityKind qqParamType;
        private EnumerationParameterType enumPt;
        private EnumerationValueDefinition enum1;
        private EnumerationValueDefinition enum2;
        private CompoundParameterType cptType;
        private Iteration iteration;
        private ElementDefinition elementDefinition;
        private ElementDefinition elementDefinitionForUsage1;
        private ElementUsage elementUsage1;

        private Parameter parameter;
        private Parameter cptParameter;
        private ParameterSubscription subscription;

        private Option option1;
        private Option option2;

        private ActualFiniteStateList stateList;
        private ActualFiniteState actualState1;
        private ActualFiniteState actualState2;
        private PossibleFiniteState state1;
        private PossibleFiniteState state2;
        private PossibleFiniteStateList posStateList;

        private ParameterValueSet valueset1;
        private ParameterValueSet valueset2;
        private ParameterValueSet valueset3;
        private ParameterValueSet valueset4;

        private Assembler assembler;
        
        [SetUp]
        public void Setup()
        {
            this.assembler = new Assembler(this.uri);
            this.permissionService = new Mock<IPermissionService>();
            this.permissionService.Setup(x => x.CanWrite(It.IsAny<ClassKind>(), It.IsAny<Thing>())).Returns(true);
            this.session = new Mock<ISession>();
            this.session.Setup(x => x.PermissionService).Returns(this.permissionService.Object);
            this.stateList = new ActualFiniteStateList(Guid.NewGuid(), this.assembler.Cache, this.uri);
            this.state1 = new PossibleFiniteState(Guid.NewGuid(), this.assembler.Cache, this.uri) { Name = "state1" };
            this.state2 = new PossibleFiniteState(Guid.NewGuid(), this.assembler.Cache, this.uri) { Name = "state2" };

            this.posStateList = new PossibleFiniteStateList(Guid.NewGuid(), this.assembler.Cache, this.uri);
            this.posStateList.PossibleState.Add(this.state1);
            this.posStateList.PossibleState.Add(this.state2);
            this.posStateList.DefaultState = this.state1;

            this.actualState1 = new ActualFiniteState(Guid.NewGuid(), this.assembler.Cache, this.uri)
            {
                PossibleState = new List<PossibleFiniteState> { this.state1 },
                Kind = ActualFiniteStateKind.MANDATORY
            };

            this.actualState2 = new ActualFiniteState(Guid.NewGuid(), this.assembler.Cache, this.uri)
            {
                PossibleState = new List<PossibleFiniteState> { this.state2 },
                Kind = ActualFiniteStateKind.MANDATORY
            };

            this.stateList.ActualState.Add(this.actualState1);
            this.stateList.ActualState.Add(this.actualState2);

            this.stateList.PossibleFiniteStateList.Add(this.posStateList);

            this.option1 = new Option(Guid.NewGuid(), this.assembler.Cache, this.uri) { Name = "option1" };
            this.option2 = new Option(Guid.NewGuid(), this.assembler.Cache, this.uri) { Name = "option2" };

            this.qqParamType = new SimpleQuantityKind(Guid.NewGuid(), this.assembler.Cache, this.uri)
            {
                Name = "PTName",
                ShortName = "PTShortName"
            };

            this.enum1 = new EnumerationValueDefinition(Guid.NewGuid(), this.assembler.Cache, this.uri) { Name = "enum1" };
            this.enum2 = new EnumerationValueDefinition(Guid.NewGuid(), this.assembler.Cache, this.uri) { Name = "enum2" };
            this.enumPt = new EnumerationParameterType(Guid.NewGuid(), this.assembler.Cache, this.uri);
            this.enumPt.ValueDefinition.Add(this.enum1);
            this.enumPt.ValueDefinition.Add(this.enum2);

            this.cptType = new CompoundParameterType(Guid.NewGuid(), this.assembler.Cache, this.uri)
            {
                Name = "APTName",
                ShortName = "APTShortName"
            };

            this.cptType.Component.Add(new ParameterTypeComponent(Guid.NewGuid(), this.assembler.Cache, this.uri)
            {
                Iid = Guid.NewGuid(),
                ParameterType = this.qqParamType,
                ShortName = "c1"
            });

            this.cptType.Component.Add(new ParameterTypeComponent(Guid.NewGuid(), this.assembler.Cache, this.uri)
            {
                Iid = Guid.NewGuid(),
                ParameterType = this.enumPt,
                ShortName = "c2"
            });

            this.activeDomain = new DomainOfExpertise(Guid.NewGuid(), this.assembler.Cache, this.uri) { Name = "active", ShortName = "active" };

            this.parameter = new Parameter(Guid.NewGuid(), this.assembler.Cache, this.uri)
            {
                Owner = this.activeDomain,
                ParameterType = this.qqParamType
            };

            this.cptParameter = new Parameter(Guid.NewGuid(), this.assembler.Cache, this.uri)
            {
                Owner = this.activeDomain,
                ParameterType = this.cptType,
                IsOptionDependent = true,
                StateDependence = this.stateList
            };

            this.valueset1 = new ParameterValueSet(Guid.NewGuid(), this.assembler.Cache, this.uri)
            {
                ActualOption = this.option1,
                ActualState = this.stateList.ActualState.First()
            };

            this.valueset2 = new ParameterValueSet(Guid.NewGuid(), this.assembler.Cache, this.uri)
            {
                ActualOption = this.option1,
                ActualState = this.stateList.ActualState.Last()
            };

            this.valueset3 = new ParameterValueSet(Guid.NewGuid(), this.assembler.Cache, this.uri)
            {
                ActualOption = this.option2,
                ActualState = this.stateList.ActualState.First()
            };

            this.valueset4 = new ParameterValueSet(Guid.NewGuid(), this.assembler.Cache, this.uri)
            {
                ActualOption = this.option2,
                ActualState = this.stateList.ActualState.Last()
            };

            this.cptParameter.ValueSet.Add(this.valueset1);
            this.cptParameter.ValueSet.Add(this.valueset2);
            this.cptParameter.ValueSet.Add(this.valueset3);
            this.cptParameter.ValueSet.Add(this.valueset4);

            this.elementDefinition = new ElementDefinition(Guid.NewGuid(), this.assembler.Cache, this.uri)
            {
                Owner = this.activeDomain
            };
            this.elementDefinitionForUsage1 = new ElementDefinition(Guid.NewGuid(), this.assembler.Cache, this.uri);
            this.elementUsage1 = new ElementUsage(Guid.NewGuid(), this.assembler.Cache, this.uri){ElementDefinition = this.elementDefinitionForUsage1};

            this.elementDefinition.ContainedElement.Add(this.elementUsage1);

            this.elementDefinitionForUsage1.Parameter.Add(this.parameter);
            this.elementDefinitionForUsage1.Parameter.Add(this.cptParameter);

            this.iteration = new Iteration(Guid.NewGuid(), this.assembler.Cache, this.uri);
            this.iteration.Element.Add(this.elementDefinition);
            this.iteration.Element.Add(this.elementDefinitionForUsage1);

            this.iteration.Option.Add(this.option1);
            this.iteration.Option.Add(this.option2);

            this.model = new EngineeringModel(Guid.NewGuid(), this.assembler.Cache, this.uri);
            this.model.Iteration.Add(this.iteration);

            this.person = new Person(Guid.NewGuid(), this.assembler.Cache, this.uri) { GivenName = "test", Surname = "test" };
            this.participant = new Participant(Guid.NewGuid(), this.assembler.Cache, this.uri) { Person = this.person, SelectedDomain = this.activeDomain };
            this.session.Setup(x => x.ActivePerson).Returns(this.person);
            this.modelsetup = new EngineeringModelSetup(Guid.NewGuid(), this.assembler.Cache, this.uri);
            this.modelsetup.Participant.Add(this.participant);
            this.model.EngineeringModelSetup = this.modelsetup;

            this.permissionService.Setup(x => x.CanWrite(It.IsAny<Thing>())).Returns(true);
        }

        [TearDown]
        public void TearDown()
        {
            CDPMessageBus.Current.ClearSubscriptions();
        }

        [Test]
        public void VerifyThatUpdateValueSetWorksCompoundOptionState()
        {
            this.subscription = new ParameterSubscription(Guid.NewGuid(), this.assembler.Cache, this.uri);
            this.cptParameter.ParameterSubscription.Add(this.subscription);

            this.subscription.ValueSet.Add(new ParameterSubscriptionValueSet(Guid.NewGuid(), this.assembler.Cache, this.uri)
            {
                SubscribedValueSet = this.valueset1
            });
            this.subscription.ValueSet.Add(new ParameterSubscriptionValueSet(Guid.NewGuid(), this.assembler.Cache, this.uri)
            {
                SubscribedValueSet = this.valueset2
            });
            this.subscription.ValueSet.Add(new ParameterSubscriptionValueSet(Guid.NewGuid(), this.assembler.Cache, this.uri)
            {
                SubscribedValueSet = this.valueset3
            });
            this.subscription.ValueSet.Add(new ParameterSubscriptionValueSet(Guid.NewGuid(), this.assembler.Cache, this.uri)
            {
                SubscribedValueSet = this.valueset4
            });

            var row = new ParameterSubscriptionRowViewModel(this.subscription, this.session.Object, null);
            var option1Row =
                row.ContainedRows.OfType<ParameterOptionRowViewModel>().Single(x => x.ActualOption == this.option1);

            var option2Row =
                row.ContainedRows.OfType<ParameterOptionRowViewModel>().Single(x => x.ActualOption == this.option2);

            var o1s1Row = option1Row.ContainedRows.OfType<ParameterStateRowViewModel>()
                    .Single(x => x.ActualState == this.actualState1);
            var o1s2Row = option1Row.ContainedRows.OfType<ParameterStateRowViewModel>()
                    .Single(x => x.ActualState == this.actualState2);
            var o2s1Row = option2Row.ContainedRows.OfType<ParameterStateRowViewModel>()
                    .Single(x => x.ActualState == this.actualState1);
            var o2s2Row = option2Row.ContainedRows.OfType<ParameterStateRowViewModel>()
                    .Single(x => x.ActualState == this.actualState2);

            var o1s1c1Row = o1s1Row.ContainedRows.OfType<ParameterComponentValueRowViewModel>().First();
            var o1s1c2Row = o1s1Row.ContainedRows.OfType<ParameterComponentValueRowViewModel>().Last();
            var o1s2c1Row = o1s2Row.ContainedRows.OfType<ParameterComponentValueRowViewModel>().First();
            var o1s2c2Row = o1s2Row.ContainedRows.OfType<ParameterComponentValueRowViewModel>().Last();
            var o2s1c1Row = o2s1Row.ContainedRows.OfType<ParameterComponentValueRowViewModel>().First();
            var o2s1c2Row = o2s1Row.ContainedRows.OfType<ParameterComponentValueRowViewModel>().Last();
            var o2s2c1Row = o2s2Row.ContainedRows.OfType<ParameterComponentValueRowViewModel>().First();
            var o2s2c2Row = o2s2Row.ContainedRows.OfType<ParameterComponentValueRowViewModel>().Last();
            
            o1s1c1Row.Switch = ParameterSwitchKind.REFERENCE;
            o1s1c2Row.Switch = ParameterSwitchKind.REFERENCE;
            o1s2c1Row.Switch = ParameterSwitchKind.REFERENCE;
            o1s2c2Row.Switch = ParameterSwitchKind.REFERENCE;
            o2s1c1Row.Switch = ParameterSwitchKind.REFERENCE;
            o2s1c2Row.Switch = ParameterSwitchKind.REFERENCE;
            o2s2c1Row.Switch = ParameterSwitchKind.REFERENCE;
            o2s2c2Row.Switch = ParameterSwitchKind.REFERENCE;

            o1s1c2Row.Manual = new ReactiveList<EnumerationValueDefinition> { this.enum1 };
            o1s2c2Row.Manual = new ReactiveList<EnumerationValueDefinition> { this.enum1 };
            o2s1c2Row.Manual = new ReactiveList<EnumerationValueDefinition> { this.enum2 };
            o2s2c2Row.Manual = new ReactiveList<EnumerationValueDefinition> { this.enum2 };

            var o1s1Set = this.subscription.ValueSet.Single(x => x.ActualOption == this.option1 && x.ActualState == this.actualState1);
            var o1s2Set = this.subscription.ValueSet.Single(x => x.ActualOption == this.option1 && x.ActualState == this.actualState2);
            var o2s1Set = this.subscription.ValueSet.Single(x => x.ActualOption == this.option2 && x.ActualState == this.actualState1);
            var o2s2Set = this.subscription.ValueSet.Single(x => x.ActualOption == this.option2 && x.ActualState == this.actualState2);

            row.UpdateValueSets(o1s1Set);
            row.UpdateValueSets(o1s2Set);
            row.UpdateValueSets(o2s1Set);
            row.UpdateValueSets(o2s2Set);

            Assert.AreEqual(o1s1Set.ValueSwitch, o1s1c1Row.Switch);
            Assert.AreEqual(o1s2Set.ValueSwitch, o1s2c1Row.Switch);
            Assert.AreEqual(o2s1Set.ValueSwitch, o2s1c1Row.Switch);
            Assert.AreEqual(o2s2Set.ValueSwitch, o2s2c1Row.Switch);

            Assert.AreEqual(o1s1Set.Manual[0], o1s1c1Row.Manual);
            Assert.AreEqual(o1s2Set.Manual[0], o1s2c1Row.Manual);
            Assert.AreEqual(o2s1Set.Manual[0], o2s1c1Row.Manual);
            Assert.AreEqual(o2s2Set.Manual[0], o2s2c1Row.Manual);

            Assert.AreEqual(o1s1Set.Manual[1], o1s1c2Row.Manual.ToValueSetString(o1s1c2Row.ParameterType));
            Assert.AreEqual(o1s2Set.Manual[1], o1s2c2Row.Manual.ToValueSetString(o1s2c2Row.ParameterType));
            Assert.AreEqual(o2s1Set.Manual[1], o2s1c2Row.Manual.ToValueSetString(o2s1c2Row.ParameterType));
            Assert.AreEqual(o2s2Set.Manual[1], o2s2c2Row.Manual.ToValueSetString(o2s2c2Row.ParameterType));
        }

        [Test]
        public void VerifyThatUpdateValueSetUpdatesRow()
        {
            var valueset = new ParameterValueSet(Guid.NewGuid(), this.assembler.Cache, this.uri);
            this.parameter.ValueSet.Add(valueset);

            this.subscription = new ParameterSubscription(Guid.NewGuid(), this.assembler.Cache, this.uri);
            this.parameter.ParameterSubscription.Add(this.subscription);

            var subscriptionValueSet = new ParameterSubscriptionValueSet(Guid.NewGuid(), this.assembler.Cache, this.uri)
            {
                SubscribedValueSet = valueset
            };
            this.subscription.ValueSet.Add(subscriptionValueSet);

            var row = new ParameterSubscriptionRowViewModel(this.subscription, this.session.Object, null);

            var revInfo = typeof (Thing).GetProperty("RevisionNumber");
            revInfo.SetValue(subscriptionValueSet, 10);

            Assert.AreEqual("-", row.Manual);
            subscriptionValueSet.Manual = new ValueArray<string>(new List<string> { "test" });
            CDPMessageBus.Current.SendObjectChangeEvent(subscriptionValueSet, EventKind.Updated);

            Assert.AreEqual("test", row.Manual);
            row.Dispose();
        }

        [Test]
        public void VerifyThatComputedAndReferenceValueAreUpdated()
        {
            this.subscription = new ParameterSubscription(Guid.NewGuid(), this.assembler.Cache, this.uri);
            this.parameter.IsOptionDependent = false;
            this.parameter.ParameterSubscription.Add(this.subscription);
            this.parameter.StateDependence = null;

            var set1 = new ParameterValueSet(Guid.NewGuid(), this.assembler.Cache, this.uri);
            var liststring = new List<string> {"abc"};
            set1.Reference = new ValueArray<string>(liststring);
            set1.Published = new ValueArray<string>(liststring);

            this.parameter.ValueSet.Add(set1);

            var subset1 = new ParameterSubscriptionValueSet(Guid.NewGuid(), this.assembler.Cache, this.uri)
            {
                SubscribedValueSet = set1
            };

            this.subscription.ValueSet.Add(subset1);

            var row = new ParameterSubscriptionRowViewModel(this.subscription, this.session.Object, null);
            Assert.AreEqual("abc", row.Computed);
            Assert.AreEqual("abc", row.Reference);

            var updated = new List<string> { "123" };

            set1.Published = new ValueArray<string>(updated);
            set1.Reference = new ValueArray<string>(updated);

            var rev = typeof(Thing).GetProperty("RevisionNumber");
            rev.SetValue(set1, 50);

            CDPMessageBus.Current.SendObjectChangeEvent(set1, EventKind.Updated);
            Assert.AreEqual("123", row.Computed);
            Assert.AreEqual("123", row.Reference);
        }
    }
}