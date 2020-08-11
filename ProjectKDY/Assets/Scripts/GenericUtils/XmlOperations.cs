using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public static class XmlOperations {
    public static void Serialize(object obj, string outputPath) {
        XmlSerializer serializer = new XmlSerializer(obj.GetType());
        StreamWriter writer = new StreamWriter(outputPath);
        serializer.Serialize(writer.BaseStream, obj);
        writer.Close();
    }

    public static T Deserialize<T>(string path) {
        XmlSerializer serializer = new XmlSerializer(typeof(T));
        StreamReader reader = new StreamReader(path);
        T obj = (T)serializer.Deserialize(reader.BaseStream);
        reader.Close();
        return obj;
    }
}