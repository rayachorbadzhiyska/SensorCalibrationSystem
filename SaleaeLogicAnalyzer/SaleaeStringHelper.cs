using System;
using System.ComponentModel;
using System.Net.Sockets;
using System.Reflection;

namespace SensorCalibrationSystem.SaleaeLogicAnalyzer
{
    public static class SaleaeStringHelper
    {
        public static byte[] toByteArray(this string str)
        {
            int count = str.Length;
            char[] char_array = str.ToCharArray();
            byte[] array = new byte[count];
            for (int i = 0; i < count; ++i)
            {
                array[i] = (byte)char_array[i];

            }

            return array;
        }

        public static string ReadString(this NetworkStream stream)
        {
            int max_length = 128;
            byte[] buffer = new byte[max_length];
            string str = "";
            int bytes_read = 0;
            while (true)
            {
                bytes_read = stream.Read(buffer, 0, max_length);

                for (int i = 0; i < bytes_read; ++i)
                {
                    str += (char)buffer[i];
                }

                // if there is no more data available, then we can stop reading.
                if (!stream.DataAvailable)
                    break;
            }

            return str;
        }

        public static void WriteLine(string str)
        {
            if (SaleaeClient.PrintCommandsToConsole)
                Console.WriteLine(str);
        }

        public static void Write(string str)
        {
            if (SaleaeClient.PrintCommandsToConsole)
                Console.Write(str);
        }

        public static string GetDescription<T>(this T? enumerationValue)
            where T : Enum
        {
            if (enumerationValue == null)
            {
                return string.Empty;
            }

            Type type = enumerationValue.GetType();
            if (!type.IsEnum)
            {
                throw new ArgumentException("EnumerationValue must be of Enum type", "enumerationValue");
            }

            //Tries to find a DescriptionAttribute for a potential friendly name
            //for the enum
            MemberInfo[] memberInfo = type.GetMember(enumerationValue.ToString());
            if (memberInfo != null && memberInfo.Length > 0)
            {
                object[] attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attrs != null && attrs.Length > 0)
                {
                    //Pull out the description value
                    return ((DescriptionAttribute)attrs[0]).Description;
                }
            }

            //If we have no description attribute, just return the ToString of the enum
            return enumerationValue.ToString();
        }
    }
}
