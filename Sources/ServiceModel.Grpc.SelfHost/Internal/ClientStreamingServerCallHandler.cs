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
using System.Threading.Tasks;
using Grpc.Core;

namespace ServiceModel.Grpc.SelfHost.Internal
{
    internal sealed class ClientStreamingServerCallHandler<TService, TRequest, TResponse>
        where TRequest : class
        where TResponse : class
    {
        private readonly Func<TService> _serviceFactory;
        private readonly Func<TService, IAsyncStreamReader<TRequest>, ServerCallContext, Task<TResponse>> _invoker;

        public ClientStreamingServerCallHandler(
            Func<TService> serviceFactory,
            Func<TService, IAsyncStreamReader<TRequest>, ServerCallContext, Task<TResponse>> invoker)
        {
            _serviceFactory = serviceFactory;
            _invoker = invoker;
        }

        public Task<TResponse> Handle(IAsyncStreamReader<TRequest> requestStream, ServerCallContext context)
        {
            var service = _serviceFactory();
            return _invoker(service, requestStream, context);
        }
    }
}