using System;
using System.IO;
using System.Text;

namespace MultiDocument.Common.Helpers
{
    public class SerializationHelper
    {
        #region Methods

        public static bool IsPrimitiveType(Type type)
        {
            if (type == typeof(System.Boolean) ||
                type == typeof(System.Byte) ||
                type == typeof(System.Byte[]) ||
                type == typeof(System.Char) ||
                type == typeof(System.Char[]) ||
                type == typeof(System.Decimal) ||
                type == typeof(System.Double) ||
                type == typeof(System.Int16) ||
                type == typeof(System.Int32) ||
                type == typeof(System.Int64) ||
                type == typeof(System.SByte) ||
                type == typeof(System.Single) ||
                type == typeof(System.String) ||
                type == typeof(System.UInt16) ||
                type == typeof(System.UInt32) ||
                type == typeof(System.UInt64))
            {
                return true;
            }

            return false;
        }

        public static byte[] BinarySerializePrimitiveTypeToByte(object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            Type type = value.GetType();

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream, Encoding.Unicode))
                {
                    if (type == typeof(System.Boolean))
                    {
                        binaryWriter.Write((System.Boolean)value);
                    }
                    else if (type == typeof(System.Byte))
                    {
                        binaryWriter.Write((System.Byte)value);
                    }
                    else if (type == typeof(System.Byte[]))
                    {
                        return (System.Byte[])value;
                    }
                    else if (type == typeof(System.Char))
                    {
                        binaryWriter.Write((System.Char)value);
                    }
                    else if (type == typeof(System.Char[]))
                    {
                        binaryWriter.Write((System.Char[])value);
                    }
                    else if (type == typeof(System.Decimal))
                    {
                        binaryWriter.Write((System.Decimal)value);
                    }
                    else if (type == typeof(System.Double))
                    {
                        binaryWriter.Write((System.Double)value);
                    }
                    else if (type == typeof(System.Int16))
                    {
                        binaryWriter.Write((System.Int16)value);
                    }
                    else if (type == typeof(System.Int32))
                    {
                        binaryWriter.Write((System.Int32)value);
                    }
                    else if (type == typeof(System.Int64))
                    {
                        binaryWriter.Write((System.Int64)value);
                    }
                    else if (type == typeof(System.SByte))
                    {
                        binaryWriter.Write((System.SByte)value);
                    }
                    else if (type == typeof(System.Single))
                    {
                        binaryWriter.Write((System.Single)value);
                    }
                    else if (type == typeof(System.String))
                    {
                        binaryWriter.Write((System.String)value);
                    }
                    else if (type == typeof(System.UInt16))
                    {
                        binaryWriter.Write((System.UInt16)value);
                    }
                    else if (type == typeof(System.UInt32))
                    {
                        binaryWriter.Write((System.UInt32)value);
                    }
                    else if (type == typeof(System.UInt64))
                    {
                        binaryWriter.Write((System.UInt64)value);
                    }
                    else
                    {
                        throw new MultiDocumentException(string.Format("The type {0} cannot be serialized", type));
                    }

                    return memoryStream.ToArray();
                }
            }
        }


        public static object BinaryDeserializePrimitiveType(byte[] buffer, Type type)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            object value = null;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream, Encoding.Unicode))
                {
                    using (BinaryReader binaryReader = new BinaryReader(binaryWriter.BaseStream))
                    {
                        binaryWriter.Write(buffer);
                        binaryReader.BaseStream.Position = 0;

                        if (type == typeof(System.Boolean))
                        {
                            value = binaryReader.ReadBoolean();
                        }

                        else if (type == typeof(System.Byte))
                        {
                            value = binaryReader.ReadByte();
                        }
                        else if (type == typeof(System.Byte[]))
                        {
                            byte[] tmpBuffer = new byte[buffer.Length];
                            buffer.CopyTo(tmpBuffer, 0);
                            value = tmpBuffer;
                        }
                        else if (type == typeof(System.Char))
                        {
                            value = binaryReader.ReadChar();
                        }
                        else if (type == typeof(System.Char[]))
                        {
                            value = binaryReader.ReadChars(buffer.Length / sizeof(System.Char));
                        }
                        else if (type == typeof(System.Decimal))
                        {
                            value = binaryReader.ReadDecimal();
                        }
                        else if (type == typeof(System.Double))
                        {
                            value = binaryReader.ReadDouble();
                        }
                        else if (type == typeof(System.Int16))
                        {
                            value = binaryReader.ReadInt16();
                        }
                        else if (type == typeof(System.Int32))
                        {
                            value = binaryReader.ReadInt32();
                        }
                        else if (type == typeof(System.Int64))
                        {
                            value = binaryReader.ReadInt64();
                        }
                        else if (type == typeof(System.SByte))
                        {
                            value = binaryReader.ReadSByte();
                        }
                        else if (type == typeof(System.Single))
                        {
                            value = binaryReader.ReadSingle();
                        }
                        else if (type == typeof(System.String))
                        {
                            value = binaryReader.ReadString();
                        }
                        else if (type == typeof(System.UInt16))
                        {
                            value = binaryReader.ReadUInt16();
                        }
                        else if (type == typeof(System.UInt32))
                        {
                            value = binaryReader.ReadUInt32();
                        }
                        else if (type == typeof(System.UInt64))
                        {
                            value = binaryReader.ReadUInt64();
                        }
                        else
                        {
                            throw new MultiDocumentException(string.Format("The type {0} cannot be deserialized", type));
                        }
                    }
                }
            }

            return value;
        }

        public static int GetMinRequiredTypeSizeInFile(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            const int minArraySize = 0;
            const int minStringSize = 2;
            const int minDateSize = 8;

            if (type == typeof(System.Boolean))
            {
                return sizeof(System.Boolean);
            }
            else if (type == typeof(System.Byte))
            {
                return sizeof(System.Byte);
            }
            else if (type == typeof(System.Byte[]))
            {
                return minArraySize;
            }
            else if (type == typeof(System.Char))
            {
                return sizeof(System.Char);
            }
            else if (type == typeof(System.Char[]))
            {
                return minArraySize;
            }
            else if (type == typeof(System.Decimal))
            {
                return sizeof(System.Decimal);
            }
            else if (type == typeof(System.Double))
            {
                return sizeof(System.Double);
            }
            else if (type == typeof(System.Int16))
            {
                return sizeof(System.Int16);
            }
            else if (type == typeof(System.Int32))
            {
                return sizeof(System.Int32);
            }
            else if (type == typeof(System.Int64))
            {
                return sizeof(System.Int64);
            }
            else if (type == typeof(System.SByte))
            {
                return sizeof(System.SByte);
            }
            else if (type == typeof(System.Single))
            {
                return sizeof(System.Single);
            }
            else if (type == typeof(System.String))
            {
                return minStringSize;
            }
            else if (type == typeof(System.UInt16))
            {
                return sizeof(System.UInt16);
            }
            else if (type == typeof(System.UInt32))
            {
                return sizeof(System.UInt32);
            }
            else if (type == typeof(System.UInt64))
            {
                return sizeof(System.UInt64);
            }
            else if (type == typeof(System.DateTime))
            {
                return minDateSize;
            }
            else
            {
                throw new MultiDocumentException(string.Format("Cannot get size for type {0}", type));
            }
        }

        #endregion Methods
    }
}
