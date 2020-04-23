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
using Grpc.Core;
using ServiceModel.Grpc.Configuration;

namespace ServiceModel.Grpc.Client
{
    /// <summary>
    /// Provides configuration used by <see cref="IClientFactory"/>.
    /// </summary>
    public sealed class ServiceModelGrpcClientOptions
    {
        /// <summary>
        /// Gets or sets a factory for serializing and deserializing messages.
        /// </summary>
        public IMarshallerFactory MarshallerFactory { get; set; }

        /// <summary>
        /// Gets or sets a methods which provides <see cref="CallOptions"/> for all calls made by all clients created by <see cref="IClientFactory"/>.
        /// </summary>
        public Func<CallOptions> DefaultCallOptionsFactory { get; set; }

        /// <summary>
        /// Gets or sets logger to handle possible output from <see cref="IClientFactory"/>.
        /// </summary>
        public ILogger Logger { get; set; }

        internal Func<IServiceClientBuilder> ClientBuilder { get; set; }
    }
}
