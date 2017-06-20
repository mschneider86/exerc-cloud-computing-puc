using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

namespace StorageHelper
{
    public static class StorageHelper
    {
        static CloudQueue cloudQueue;

        public static void ConnectToStorage(string nomeFila)
        {
            var connectionString = "DefaultEndpointsProtocol=https;AccountName=mssm;AccountKey=wUy4hJ4D0Ae0pVlBDY+dL5KK/BRmAW9Mt4QxcYpWo2ax6Zujv7nde9swyKjKlXieEhTrDESVBzz34KSO8wmoGw==;EndpointSuffix=core.windows.net";

            CloudStorageAccount cloudStorageAccount;

            if (!CloudStorageAccount.TryParse(connectionString, out cloudStorageAccount))
            {
                Console.WriteLine("Expected connection string 'Azure Storage Account Demo Primary' to be a valid Azure Storage Connection String.");
            }

            var cloudQueueClient = cloudStorageAccount.CreateCloudQueueClient();
            cloudQueue = cloudQueueClient.GetQueueReference(nomeFila);

            cloudQueue.CreateIfNotExists();
        }

        public static void SendMessageToQueue(string messageText)
        {
            var message = new CloudQueueMessage(messageText);

            cloudQueue.AddMessage(message);
        }

        public static string GetMessageFromQueue()
        {
            var cloudQueueMessage = cloudQueue.GetMessage();

            if (cloudQueueMessage == null)
            {
                return string.Empty;
            }

            cloudQueue.DeleteMessage(cloudQueueMessage);

            return cloudQueueMessage.AsString;
        }

        public static void ClearMessages()
        {
            cloudQueue.Clear();
        }
    }
}
