﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using System.Runtime.Serialization;

namespace ServiceModel.Grpc.Channel
{
    [Serializable]
    internal sealed class Message : ISerializable
    {
        public Message()
        {
        }

        public Message(SerializationInfo info, StreamingContext context)
        {
        }


        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
        }
    }

    [Serializable]
    internal sealed class Message<T1> : ISerializable
    {
        public Message(T1 value1)
        {
            Value1 = value1;
        }

        public Message(SerializationInfo info, StreamingContext context)
        {
            Value1 = (T1)info.GetValue(nameof(Value1), typeof(T1));
        }

        public T1 Value1 { get; }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(Value1), Value1);
        }
    }

    [Serializable]
    internal sealed class Message<T1, T2> : ISerializable
    {
        public Message(T1 value1, T2 value2)
        {
            Value1 = value1;
            Value2 = value2;
        }

        public Message(SerializationInfo info, StreamingContext context)
        {
            Value1 = (T1)info.GetValue(nameof(Value1), typeof(T1));
            Value2 = (T2)info.GetValue(nameof(Value2), typeof(T2));
        }

        public T1 Value1 { get; }
        public T2 Value2 { get; }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(Value1), Value1);
            info.AddValue(nameof(Value2), Value2);
        }
    }

    [Serializable]
    internal sealed class Message<T1, T2, T3> : ISerializable
    {
        public Message(T1 value1, T2 value2, T3 value3)
        {
            Value1 = value1;
            Value2 = value2;
            Value3 = value3;
        }

        public Message(SerializationInfo info, StreamingContext context)
        {
            Value1 = (T1)info.GetValue(nameof(Value1), typeof(T1));
            Value2 = (T2)info.GetValue(nameof(Value2), typeof(T2));
            Value3 = (T3)info.GetValue(nameof(Value3), typeof(T3));
        }

        public T1 Value1 { get; }
        public T2 Value2 { get; }
        public T3 Value3 { get; }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(Value1), Value1);
            info.AddValue(nameof(Value2), Value2);
            info.AddValue(nameof(Value3), Value3);
        }
    }

    [Serializable]
    internal sealed class Message<T1, T2, T3, T4> : ISerializable
    {
        public Message(T1 value1, T2 value2, T3 value3, T4 value4)
        {
            Value1 = value1;
            Value2 = value2;
            Value3 = value3;
            Value4 = value4;
        }

        public Message(SerializationInfo info, StreamingContext context)
        {
            Value1 = (T1)info.GetValue(nameof(Value1), typeof(T1));
            Value2 = (T2)info.GetValue(nameof(Value2), typeof(T2));
            Value3 = (T3)info.GetValue(nameof(Value3), typeof(T3));
            Value4 = (T4)info.GetValue(nameof(Value4), typeof(T4));
        }

        public T1 Value1 { get; }
        public T2 Value2 { get; }
        public T3 Value3 { get; }
        public T4 Value4 { get; }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(Value1), Value1);
            info.AddValue(nameof(Value2), Value2);
            info.AddValue(nameof(Value3), Value3);
            info.AddValue(nameof(Value4), Value4);
        }
    }

}