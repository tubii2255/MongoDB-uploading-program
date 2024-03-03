using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Xml;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;

namespace Jsonfilemoving
{
    class SendData
    {
        private string collectionName;
        private string collectionName1;
        private string collectionName2;
        private string coll;
        private string databaseName;

        public void Sends()
        {
            string message = "";
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("Config.xml");

            XmlNode mongoSettingsNode = xmlDoc.SelectSingleNode("/Configuration/mongoSettings");

            string connectionString = mongoSettingsNode.SelectSingleNode("connectionString").InnerText;


            collectionName = mongoSettingsNode.SelectSingleNode("collectionName").InnerText;
            collectionName1 = mongoSettingsNode.SelectSingleNode("collectionName1").InnerText;
            collectionName2 = mongoSettingsNode.SelectSingleNode("collectionName2").InnerText;
            

            XmlNode fileSettingsNode = xmlDoc.SelectSingleNode("/Configuration/fileSettings");
            string folderPath = fileSettingsNode.SelectSingleNode("folderPath").InnerText;
            string outputFolder = fileSettingsNode.SelectSingleNode("outputFolder").InnerText;

            var client = new MongoClient(connectionString);

            string[] jsonFiles = Directory.GetFiles(folderPath, "*.json");

            foreach (var jsonFile in jsonFiles)
            {
                string fileName = Path.GetFileName(jsonFile);
                string jsonString = File.ReadAllText(jsonFile);
                JObject jsonObject = JObject.Parse(jsonString);
                databaseName = (string)jsonObject.SelectToken("site.SiteCode");
                var database = client.GetDatabase(databaseName);

                coll = collectiona(fileName);

                var collection = database.GetCollection<BsonDocument>(coll);

                try
                {
                    try
                    {
                        var bsonArray = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonArray>(jsonString);

                        foreach (BsonDocument bsonDocument in bsonArray)
                        {
                            collection.InsertOne(bsonDocument);
                        }
                    }
                    catch
                    {
                        {
                            var bsonDocument = BsonDocument.Parse(jsonString);
                            collection.InsertOne(bsonDocument);
                        }
                    }

                    message = ($" {jsonFile} file inserted into MongoDB successfully.");

                    LogWriting Log = new LogWriting();
                    Log.logWriting(message);

                    string sourcepath = jsonFile;
                    string destination = Path.Combine(outputFolder + Path.GetFileName(jsonFile));

                    MoveFiles newmovefile = new MoveFiles();
                    newmovefile.MoveFile(sourcepath, destination);

                }
                catch (Exception ex)
                {
                    message = $" {jsonFile} error sending to MongoDB : {ex.Message}";
                    LogWriting Log = new LogWriting();
                    Log.logWriting(message);
                }

            }
        }

        // Corrected method signature to match the parameter name
        public string collectiona(string fileName)
        {
            string coll;
            if (fileName.Contains("ordertrace"))
            {
                coll = collectionName;
            }
            else if (fileName.Contains("trace"))
            {
                coll = collectionName1;
            }
            else if (fileName.Contains("order"))
            {
                coll = collectionName2;
            }
            else
            {
                coll = collectionName;
            }
            return coll;
        }
    }
}
