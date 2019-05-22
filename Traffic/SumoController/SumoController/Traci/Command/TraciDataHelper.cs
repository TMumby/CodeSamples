using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SumoController.Traci.Command
{
    /// <summary>
    /// helper class for converting to and from Traci data
    /// </summary>
    public static class TraciDataHelper
    {
        /// <summary>
        /// Retrieves an Integer from a Traci message (byte array)
        /// </summary>
        /// <param name="byteArray">Traci message</param>
        /// <param name="startPosition">data start position</param>
        /// <returns>a standard integer</returns>
        public static int GetTraciInteger(byte[] byteArray, int startPosition)
        {
            try
            {
                byte[] integerBytes = byteArray.Skip(startPosition).Take(4).Reverse().ToArray();
                int traciInteger = BitConverter.ToInt32(integerBytes, 0);
                return traciInteger;
            }
            catch (Exception ex)
            {
                throw new SumoControllerException("Error whilst getting Integer from Traci message", ex);
            }
        }

        /// <summary>
        /// Retrieves a string from a Traci message (byte array)
        /// </summary>
        /// <param name="byteArray">Traci message</param>
        /// <param name="startPosition">data start position</param>
        /// <returns>a standard string</returns>
        public static string GetTraciString(byte[] byteArray, int startPosition)
        {
            try
            {
                int stringLength = GetTraciInteger(byteArray, startPosition);
                byte[] stringBytes = byteArray.Skip(startPosition + 4).Take(stringLength).ToArray();
                return Encoding.Default.GetString(stringBytes);
            }
            catch (Exception ex)
            {
                throw new SumoControllerException("Error whilst getting string from Traci message", ex);
            }
        }

        /// <summary>
        /// Retrieves a double from a Traci message (byte array)
        /// </summary>
        /// <param name="byteArray">Traci message</param>
        /// <param name="startPosition">data start position</param>
        /// <returns>a standard double</returns>
        public static double GetTraciDouble(byte[] byteArray, int startPosition)
        {
            try
            {
                byte[] doubleBytes = byteArray.Skip(startPosition).Take(8).Reverse().ToArray();
                return BitConverter.ToDouble(doubleBytes, 0);
            }
            catch (Exception ex)
            {
                throw new SumoControllerException("Error whilst getting double from Traci message", ex);
            }
        }

        /// <summary>
        /// Retrieves a list of strings from a Traci message (byte array)
        /// </summary>
        /// <param name="byteArray">Traci message</param>
        /// <param name="startPosition">data start position</param>
        /// <returns>a list of strings</returns>
        public static List<string> GetTracStringList(byte[] byteArray, int startPosition)
        {
            try
            {
                List<string> stringList = new List<string>();
                int stringCount = GetTraciInteger(byteArray, startPosition);
                int stringPosition = startPosition + 4;


                for (int i = 1; i <= stringCount; i++)
                {
                    stringList.Add(GetTraciString(byteArray, stringPosition));
                    int currentStringLength = GetTraciInteger(byteArray, stringPosition);
                    stringPosition += currentStringLength + 4;
                }

                return stringList;
            }
            catch (Exception ex)
            {
                throw new SumoControllerException("Error whilst getting string list from Traci message", ex);
            }
        }

        /// <summary>
        /// Creates a string in the Traci data format, [size, string] in bytes
        /// </summary>
        /// <param name="stringToConvert"></param>
        /// <returns>a string in Traci data format in bytes</returns>
        public static List<byte> CreateTraciString(string stringToConvert)
        {
            try
            {
                List<byte> stringBytes = Encoding.ASCII.GetBytes(stringToConvert).ToList();
                List<byte> traciStringBytes = CreateTraciStringLength(stringBytes);
                traciStringBytes = AddToByteList(traciStringBytes, stringBytes);
                return traciStringBytes;
            }
            catch (Exception ex)
            {
                throw new SumoControllerException("Error whilst Creating Traci string", ex);
            }            
        }

        /// <summary>
        /// Finds the length of the string and turns the length into bytes
        /// </summary>
        /// <param name="stringBytes">string in bytes</param>
        /// <returns></returns>
        private static List<byte> CreateTraciStringLength(List<byte> stringBytes)
        {
            try
            {
                List<byte> stringLength = BitConverter.GetBytes(stringBytes.Count()).Reverse().ToList();
                return stringLength;
            }
            catch (Exception ex)
            {
                throw new SumoControllerException("Error Creating Traci Stringlength (bytes)", ex);
            }
        }

        /// <summary>
        /// Creates data with data type pre-ended to byte list. Only implemented for string at this time
        /// </summary>
        /// <param name="dataType">Traci data type</param>
        /// <param name="data">data to be turned into bytes</param>
        /// <returns>data(in bytes) with data type pre-ended</returns>
        public static List<byte> CreateTraciData(TraciConstants.DataType dataType, object data)
        {
            try
            {
                List<byte> dataBytes = new List<byte>();
                dataBytes.Add((byte)dataType);

                if (dataType == TraciConstants.DataType.TYPE_STRING)
                {
                    dataBytes = AddToByteList(dataBytes, CreateTraciString((string)data));
                }

                return dataBytes;
            }
            catch (Exception ex)
            {
                throw new SumoControllerException("Error Creating Traci Data", ex);
            }
        }

        /// <summary>
        /// Creates an integer in traci data format (bytes)
        /// </summary>
        /// <param name="integer"></param>
        /// <returns>byte list</returns>
        public static List<byte> CreateTraciIntBytes(int integer)
        {
            try
            {
                return BitConverter.GetBytes(integer).Reverse().ToList();
            }
            catch (Exception ex)
            {
                throw new SumoControllerException("Error Creating Traci integer", ex);
            }
        }

        /// <summary>
        /// Concatanates two byte lists
        /// </summary>
        /// <param name="originalByteList">first bytelist</param>
        /// <param name="toAddByteList">second bytelist</param>
        /// <returns>combined byte list</returns>
        public static List<byte> AddToByteList(List<byte> originalByteList, List<byte> toAddByteList)
        {
            try
            {
                List<byte> byteList = originalByteList.Concat(toAddByteList).ToList();
                return byteList;
            }
            catch (Exception ex)
            {
                throw new SumoControllerException("Error Concatanating byte lists", ex);
            }
        }

        /// <summary>
        /// Retrieves Traci command from message
        /// </summary>
        /// <param name="traciMessage">the traci message</param>
        /// <param name="position">position of the command in the message</param>
        /// <returns></returns>
        public static TraciConstants.Command GetTraciCommand(byte[] traciMessage, int position)
        {
            try
            {
                string commandString = Enum.GetName(typeof(TraciConstants.Command), traciMessage[position]);
                object command = Enum.Parse(typeof(TraciConstants.Command), commandString);
                return (TraciConstants.Command)command;
            }
            catch (Exception ex)
            {
                throw new SumoControllerException("Error Extracting traci command from message", ex);
            }
        }

        /// <summary>
        /// Retrieves Traci data type from message
        /// </summary>
        /// <param name="traciMessage">the traci message</param>
        /// <param name="position">position of the command in the message</param>
        /// <returns>Traci data type</returns>
        public static TraciConstants.DataType GetTraciDataType(byte[] traciMessage, int position)
        {
            try
            {
                string dataTypeString = Enum.GetName(typeof(TraciConstants.DataType), traciMessage[position]);
                object dataType = Enum.Parse(typeof(TraciConstants.DataType), dataTypeString);
                return (TraciConstants.DataType)dataType;
            }
            catch (Exception ex)
            {
                throw new SumoControllerException("Error Extracting traci datatype from message", ex);
            }        
        }
    }
}
