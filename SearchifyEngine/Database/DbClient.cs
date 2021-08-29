using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime.CredentialManagement;
using SearchifyEngine.Store;

namespace SearchifyEngine.Database
{
    public static class DbClient
    {
        private static readonly string Host = Config.DatabaseHost;
        private static readonly int Port = Config.DatabasePort;
        private static readonly string EndpointUrl = "http://" + Host + ":" + Port;

        private static AmazonDynamoDBClient _client;
        public static InvertedIndexDynamoDbStore Store;

        private static bool IsPortInUse()
        {
            IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] tcpConnInfoArray = ipGlobalProperties.GetActiveTcpListeners();
            foreach (IPEndPoint endpoint in tcpConnInfoArray)
            {
                if (endpoint.Port == Port)
                {
                    return false;
                }
            }

            return true;
        }
        
        private static void WriteProfile(string profileName)
        {
            var options = new CredentialProfileOptions
            {
                AccessKey = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY") ?? "null",
                SecretKey = Environment.GetEnvironmentVariable("AWS_SECRET_KEY") ?? "null"
            };
            var profile = new CredentialProfile(profileName, options);
            var sharedFile = new SharedCredentialsFile();
            sharedFile.RegisterProfile(profile);
        }

        public static bool CreateClient(bool useLocal)
        {
            WriteProfile("default");
            if (useLocal)
            {
                var portUsed = IsPortInUse();
                if (portUsed)
                {
                    Console.WriteLine("The local version of DynamoDB is NOT running.");
                    return false;
                }
                
                Console.WriteLine("  -- Setting up a DynamoDB-Local client (DynamoDB Local seems to be running)");
                AmazonDynamoDBConfig ddbConfig = new AmazonDynamoDBConfig
                {
                    ServiceURL = EndpointUrl
                };
                try
                {
                    _client = new AmazonDynamoDBClient(ddbConfig);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("FAILED to create a DynamoDBLocal client; " + ex.Message);
                    return false;
                }
            }
            else
            {
                _client = new AmazonDynamoDBClient();
            }

            Store = new InvertedIndexDynamoDbStore(_client);
            return true;
        }

        public static async Task<bool> CheckTableExists(string tableName)
        {
            var response = await _client.ListTablesAsync();
            return response.TableNames.Contains(tableName);
        }

        public static async Task<bool> CreateTable(string tableName, List<AttributeDefinition> tableAttributes,
            List<KeySchemaElement> tableKeySchema, ProvisionedThroughput provisionedThroughput)
        {
            bool response = true;

            if (!await CheckTableExists(tableName))
            {
                var request = new CreateTableRequest
                {
                    TableName = tableName,
                    AttributeDefinitions = tableAttributes,
                    KeySchema = tableKeySchema,
                    ProvisionedThroughput = provisionedThroughput
                };

                try
                {
                    await _client.CreateTableAsync(request);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    response = false;
                }
            }
            return response;
        }
        
        public static async Task CreateTables()
        {
            bool status = await CreateTable("inverted_index", new List<AttributeDefinition>
            {
                new AttributeDefinition
                {
                    AttributeName = "Term",
                    AttributeType = "S"
                }
            }, new List<KeySchemaElement>
            {
                new KeySchemaElement
                {
                    AttributeName = "Term", 
                    KeyType = "HASH"
                }
            }, new ProvisionedThroughput
            {
                ReadCapacityUnits = 20,
                WriteCapacityUnits = 50
            });
        }

        public static async Task<TableDescription> GetTableDescription(string tableName)
        {
            TableDescription result = null;

            try
            {
                var response = await _client.DescribeTableAsync(tableName);
                result = response.Table;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return result;
        }
    }
}