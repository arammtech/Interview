using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Google.Apis;

namespace Muqabalati.Service.Genaric
{
    public class Global
    {
        public string SeriliazeObject<T>(T obj)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
                serializer.WriteObject(stream, obj);
                string jsonString = System.Text.Encoding.UTF8.GetString(stream.ToArray());
                return jsonString;
            }
        }

        public T? DeSeriliazeObject<T>(T obj)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
                T? deserialized = (T)serializer.ReadObject(stream);
                return deserialized;
            }
        }


    }
}
