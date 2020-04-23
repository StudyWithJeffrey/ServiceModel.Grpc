﻿// <copyright>
// Copyright 2020 Max Ieremenko
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>

using System;
using NUnit.Framework;
using Shouldly;

namespace ServiceModel.Grpc.Internal
{
    [TestFixture]
    public partial class ContractDescriptionTest
    {
        [Test]
        [TestCase(typeof(IDuplicateOperationName))]
        [TestCase(typeof(IService))]
        public void DuplicateOperationName(Type serviceType)
        {
            var sut = new ContractDescription(serviceType);

            sut.Interfaces.Count.ShouldBe(0);
            sut.Services.Count.ShouldNotBe(0);

            foreach (var service in sut.Services)
            {
                service.Methods.Count.ShouldBe(0);
                service.Operations.Count.ShouldBe(0);
                service.NotSupportedOperations.Count.ShouldNotBe(0);

                Console.WriteLine(service.NotSupportedOperations[0].Error);
                service.NotSupportedOperations[0].Error.ShouldNotBeNullOrEmpty();
            }
        }
    }
}
