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
using System.Globalization;
using System.Linq;
using System.Threading;
using Grpc.Core;
using ServiceModel.Grpc.Hosting;

namespace ServiceModel.Grpc.DesignTime.Generator.Internal.CSharp
{
    internal sealed class CSharpServiceEndpointBuilder : CodeGeneratorBase
    {
        private readonly ContractDescription _contract;

        public CSharpServiceEndpointBuilder(ContractDescription contract)
        {
            _contract = contract;
        }

        public override string GetGeneratedMemberName() => _contract.EndpointClassName;

        protected override void Generate()
        {
            Output
                .Append($"internal sealed class {_contract.EndpointClassName}")
                .AppendLine();
            Output.AppendLine("{");

            using (Output.Indent())
            {
                BuildFields();
                BuildCtor();

                foreach (var interfaceDescription in _contract.Services)
                {
                    foreach (var method in interfaceDescription.Operations)
                    {
                        ImplementMethod(interfaceDescription.InterfaceTypeName, method);
                    }
                }
            }

            Output.AppendLine("}");
        }

        private static string GetMethodHeaderMarshallerField(string grpcMethodHeaderName)
        {
            return "_" + char.ToLowerInvariant(grpcMethodHeaderName[0]) + grpcMethodHeaderName.Substring(1);
        }

        private void BuildFields()
        {
            foreach (var interfaceDescription in _contract.Services)
            {
                foreach (var method in interfaceDescription.Operations)
                {
                    if (method.HeaderRequestType != null)
                    {
                        Output
                            .Append("private readonly Marshaller<")
                            .Append(method.HeaderRequestType.ClassName)
                            .Append("> ")
                            .Append(GetMethodHeaderMarshallerField(method.GrpcMethodHeaderName))
                            .AppendLine(";");
                    }
                }
            }
        }

        private void BuildCtor()
        {
            Output
                .Append("public ")
                .Append(_contract.EndpointClassName)
                .Append("(")
                .Append(_contract.ContractClassName)
                .AppendLine(" contract)");

            Output.Append("{");
            using (Output.Indent())
            {
                Output.AppendLine("if (contract == null) throw new ArgumentNullException(\"contract\");");

                foreach (var interfaceDescription in _contract.Services)
                {
                    foreach (var method in interfaceDescription.Operations)
                    {
                        if (method.HeaderRequestType != null)
                        {
                            Output
                                .Append(GetMethodHeaderMarshallerField(method.GrpcMethodHeaderName))
                                .Append(" = ")
                                .Append("contract.")
                                .Append(method.GrpcMethodHeaderName)
                                .Append(";");
                        }
                    }
                }
            }

            Output.Append("}");
        }

        private void ImplementMethod(string interfaceType, OperationDescription operation)
        {
            switch (operation.OperationType)
            {
                case MethodType.Unary:
                    BuildUnary(interfaceType, operation);
                    break;

                case MethodType.ClientStreaming:
                    BuildClientStreaming(interfaceType, operation);
                    break;

                case MethodType.ServerStreaming:
                    BuildServerStreaming(interfaceType, operation);
                    break;

                case MethodType.DuplexStreaming:
                    BuildDuplexStreaming(interfaceType, operation);
                    break;

                default:
                    throw new NotImplementedException("{0} operation is not implemented.".FormatWith(operation.OperationType));
            }
        }

        private void BuildUnary(string interfaceType, OperationDescription operation)
        {
            // Task<TResponse> Invoke(TService service, TRequest request, ServerCallContext context)
            Output.Append("public ");
            if (operation.IsAsync)
            {
                Output.Append("async ");
            }

            Output
                .Append("Task<").Append(operation.ResponseType.ClassName).Append("> ")
                .Append(operation.OperationName)
                .Append("(")
                .Append(interfaceType).Append(" service, ")
                .Append(operation.RequestType.ClassName).Append(" request, ")
                .Append(nameof(ServerCallContext)).AppendLine(" context)")
                .AppendLine("{");

            using (Output.Indent())
            {
                if (operation.ResponseType.Properties.Length > 0)
                {
                    Output.Append("var result = ");
                }

                if (operation.IsAsync)
                {
                    Output.Append("await ");
                }

                Output
                    .Append("service.")
                    .Append(operation.Method.Name)
                    .Append("(");

                for (var i = 0; i < operation.Method.Parameters.Length; i++)
                {
                    if (i > 0)
                    {
                        Output.Append(", ");
                    }

                    var parameter = operation.Method.Parameters[i];
                    if (operation.ContextInput.Contains(i))
                    {
                        PushContext(parameter);
                    }
                    else
                    {
                        Output
                            .Append("request.Value")
                            .Append((Array.IndexOf(operation.RequestTypeInput, i) + 1).ToString(CultureInfo.InvariantCulture));
                    }
                }

                Output.Append(")");
                if (operation.IsAsync)
                {
                    Output.Append(".ConfigureAwait(false)");
                }

                Output
                    .AppendLine(";")
                    .Append("return ");

                if (!operation.IsAsync)
                {
                    Output.Append("Task.FromResult(");
                }

                Output
                    .Append("new ")
                    .Append(operation.ResponseType.ClassName)
                    .Append("(");

                if (operation.ResponseType.Properties.Length > 0)
                {
                    Output.Append("result");
                }

                Output.Append(")");

                if (!operation.IsAsync)
                {
                    Output.Append(")");
                }

                Output.AppendLine(";");
            }

            Output.AppendLine("}");
        }

        private void BuildClientStreaming(string interfaceType, OperationDescription operation)
        {
            // Task<TResponse> Invoke(TService service, IAsyncStreamReader<TRequest> request, ServerCallContext context)
            Output
                .Append("public async Task<").Append(operation.ResponseType.ClassName).Append("> ")
                .Append(operation.OperationName)
                .Append("(")
                .Append(interfaceType).Append(" service, ")
                .Append("IAsyncStreamReader<").Append(operation.RequestType.ClassName).Append(">").Append(" request, ")
                .Append(nameof(ServerCallContext)).AppendLine(" context)")
                .AppendLine("{");

            using (Output.Indent())
            {
                DeclareHeaderValues(operation);
                if (operation.ResponseType.Properties.Length > 0)
                {
                    Output.Append("var result = ");
                }

                Output
                    .Append("await service.")
                    .Append(operation.Method.Name)
                    .Append("(");

                for (var i = 0; i < operation.Method.Parameters.Length; i++)
                {
                    if (i > 0)
                    {
                        Output.Append(", ");
                    }

                    var parameter = operation.Method.Parameters[i];
                    if (operation.ContextInput.Contains(i))
                    {
                        PushContext(parameter);
                    }
                    else if (operation.HeaderRequestTypeInput.Contains(i))
                    {
                        Output
                            .Append("headers.Value")
                            .Append((Array.IndexOf(operation.HeaderRequestTypeInput, i) + 1).ToString(CultureInfo.InvariantCulture));
                    }
                    else
                    {
                        Output
                            .Append(nameof(ServerChannelAdapter))
                            .Append(".")
                            .Append(nameof(ServerChannelAdapter.ReadClientStream))
                            .Append("<")
                            .Append(operation.RequestType.Properties[0])
                            .Append(">(request, context)");
                    }
                }

                Output
                    .AppendLine(").ConfigureAwait(false);")
                    .Append("return ");

                Output
                    .Append("new ")
                    .Append(operation.ResponseType.ClassName)
                    .Append("(");

                if (operation.ResponseType.Properties.Length > 0)
                {
                    Output.Append("result");
                }

                Output.AppendLine(");");
            }

            Output.AppendLine("}");
        }

        private void BuildServerStreaming(string interfaceType, OperationDescription operation)
        {
            // Task Invoke(TService service, TRequest request, IServerStreamWriter<TResponse> response, ServerCallContext context)
            Output
                .Append("public async Task ")
                .Append(operation.OperationName)
                .Append("(")
                .Append(interfaceType).Append(" service, ")
                .Append(operation.RequestType.ClassName).Append(" request, ")
                .Append("IServerStreamWriter<").Append(operation.ResponseType.ClassName).Append("> response, ")
                .Append(nameof(ServerCallContext)).AppendLine(" context)")
                .AppendLine("{");

            using (Output.Indent())
            {
                Output.Append("var result = ");
                if (operation.IsAsync)
                {
                    Output.Append("await ");
                }

                Output
                    .Append("service.")
                    .Append(operation.Method.Name)
                    .Append("(");

                for (var i = 0; i < operation.Method.Parameters.Length; i++)
                {
                    if (i > 0)
                    {
                        Output.Append(", ");
                    }

                    var parameter = operation.Method.Parameters[i];
                    if (operation.ContextInput.Contains(i))
                    {
                        PushContext(parameter);
                    }
                    else
                    {
                        Output
                            .Append("request.Value")
                            .Append((Array.IndexOf(operation.RequestTypeInput, i) + 1).ToString(CultureInfo.InvariantCulture));
                    }
                }

                Output.Append(")");
                if (operation.IsAsync)
                {
                    Output.Append(".ConfigureAwait(false)");
                }

                Output.AppendLine(";");

                Output
                    .Append("await ")
                    .Append(nameof(ServerChannelAdapter))
                    .Append(".")
                    .Append(nameof(ServerChannelAdapter.WriteServerStreamingResult))
                    .AppendLine("(result, response, context).ConfigureAwait(false);");
            }

            Output.AppendLine("}");
        }

        private void BuildDuplexStreaming(string interfaceType, OperationDescription operation)
        {
            // Task Invoke(TService service, IAsyncStreamReader<TRequest> request, IServerStreamWriter<TResponse> response, ServerCallContext context)
            Output
                .Append("public async Task ")
                .Append(operation.OperationName)
                .Append("(")
                .Append(interfaceType).Append(" service, ")
                .Append("IAsyncStreamReader<").Append(operation.RequestType.ClassName).Append("> request, ")
                .Append("IServerStreamWriter<").Append(operation.ResponseType.ClassName).Append("> response, ")
                .Append(nameof(ServerCallContext)).AppendLine(" context)")
                .AppendLine("{");

            using (Output.Indent())
            {
                DeclareHeaderValues(operation);

                Output.Append("var result = ");
                if (operation.IsAsync)
                {
                    Output.Append("await  ");
                }

                Output
                    .Append("service.")
                    .Append(operation.Method.Name)
                    .Append("(");

                for (var i = 0; i < operation.Method.Parameters.Length; i++)
                {
                    if (i > 0)
                    {
                        Output.Append(", ");
                    }

                    var parameter = operation.Method.Parameters[i];
                    if (operation.ContextInput.Contains(i))
                    {
                        PushContext(parameter);
                    }
                    else if (operation.HeaderRequestTypeInput.Contains(i))
                    {
                        Output
                            .Append("headers.Value")
                            .Append((Array.IndexOf(operation.HeaderRequestTypeInput, i) + 1).ToString(CultureInfo.InvariantCulture));
                    }
                    else
                    {
                        Output
                            .Append(nameof(ServerChannelAdapter))
                            .Append(".")
                            .Append(nameof(ServerChannelAdapter.ReadClientStream))
                            .AppendLine("(request, context)");
                    }
                }

                Output.Append(")");

                if (operation.IsAsync)
                {
                    Output.Append(".ConfigureAwait(false)");
                }

                Output.AppendLine(";");

                Output
                    .Append("await ")
                    .Append(nameof(ServerChannelAdapter))
                    .Append(".")
                    .Append(nameof(ServerChannelAdapter.WriteServerStreamingResult))
                    .AppendLine("(result, response, context).ConfigureAwait(false);");
            }

            Output.AppendLine("}");
        }

        private void PushContext(ParameterDescription parameter)
        {
            if (parameter.TypeSymbol.IsAssignableFrom(typeof(ServerCallContext))
                || parameter.TypeSymbol.IsAssignableFrom(typeof(CallContext)))
            {
                Output.Append("context");
                return;
            }

            if (parameter.TypeSymbol.IsAssignableFrom(typeof(CancellationToken)))
            {
                Output
                    .Append("context.")
                    .Append(nameof(ServerCallContext.CancellationToken));
                return;
            }

            if (parameter.TypeSymbol.IsAssignableFrom(typeof(CallOptions)))
            {
                // new CallOptions(context.RequestHeaders, context.Deadline, context.CancellationToken, context.WriteOptions)
                Output
                    .Append("new ")
                    .Append(nameof(CallOptions))
                    .Append("(")
                    .Append("context.").Append(nameof(ServerCallContext.RequestHeaders))
                    .Append(", context.").Append(nameof(ServerCallContext.Deadline))
                    .Append(", context.").Append(nameof(ServerCallContext.CancellationToken))
                    .Append(", context.").Append(nameof(ServerCallContext.WriteOptions))
                    .Append(")");
                return;
            }

            throw new NotImplementedException();
        }

        private void DeclareHeaderValues(OperationDescription operation)
        {
            if (operation.HeaderRequestType == null)
            {
                return;
            }

            Output
                .Append("var headers = ")
                .Append(nameof(ServerChannelAdapter))
                .Append(".")
                .Append(nameof(ServerChannelAdapter.GetMethodInputHeader))
                .Append("(")
                .Append(GetMethodHeaderMarshallerField(operation.GrpcMethodHeaderName))
                .AppendLine(", context);");
        }
    }
}
