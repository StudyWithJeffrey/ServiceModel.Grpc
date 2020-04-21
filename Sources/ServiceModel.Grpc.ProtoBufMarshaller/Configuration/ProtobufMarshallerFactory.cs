﻿using System;
using System.IO;
using Grpc.Core;
using ProtoBuf;
using ProtoBuf.Meta;

namespace ServiceModel.Grpc.Configuration
{
    /// <summary>
    /// Provides the method to create the <see cref="Marshaller{T}"/> for serializing and deserializing messages by <see cref="Serializer"/>.
    /// </summary>
    public sealed class ProtobufMarshallerFactory : IMarshallerFactory
    {
        /// <summary>
        /// Default instance of <see cref="ProtobufMarshallerFactory"/> with <see cref="RuntimeTypeModel"/>.Default.
        /// </summary>
        public static readonly IMarshallerFactory Default = new ProtobufMarshallerFactory();

        private readonly RuntimeTypeModel _runtimeTypeModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProtobufMarshallerFactory"/> class.
        /// </summary>
        public ProtobufMarshallerFactory()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProtobufMarshallerFactory"/> class with specific <see cref="RuntimeTypeModel"/>.
        /// </summary>
        /// <param name="runtimeTypeModel">The <see cref="RuntimeTypeModel"/>.</param>
        public ProtobufMarshallerFactory(RuntimeTypeModel runtimeTypeModel)
        {
            if (runtimeTypeModel == null)
            {
                throw new ArgumentNullException(nameof(runtimeTypeModel));
            }

            _runtimeTypeModel = runtimeTypeModel;
        }

        /// <summary>
        /// Creates the <see cref="Marshaller{T}"/>.
        /// </summary>
        /// <typeparam name="T">The message type.</typeparam>
        /// <returns>The instance of <see cref="Marshaller{T}"/> for serializing and deserializing messages.</returns>
        public Marshaller<T> CreateMarshaller<T>()
        {
            if (_runtimeTypeModel == null)
            {
                return ProtobufMarshaller<T>.Default;
            }

            return new Marshaller<T>(Serialize, Deserialize<T>);
        }

        internal static byte[] Serialize<T>(T value, RuntimeTypeModel runtimeTypeModel)
        {
            using (var buffer = new MemoryStream())
            {
                runtimeTypeModel.Serialize(buffer, value);
                return buffer.ToArray();
            }
        }

        internal static T Deserialize<T>(byte[] value, RuntimeTypeModel runtimeTypeModel)
        {
            if (value == null)
            {
                return default;
            }

            using (var buffer = new MemoryStream(value))
            {
                return (T)runtimeTypeModel.Deserialize(buffer, null, typeof(T));
            }
        }

        private byte[] Serialize<T>(T value) => Serialize(value, _runtimeTypeModel);

        private T Deserialize<T>(byte[] value) => Deserialize<T>(value, _runtimeTypeModel);
    }
}
