using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

namespace WorkerRole1
{
    public class WorkerRole : RoleEntryPoint
    {
        static CloudQueue cloudQueue;

        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);
        
        public override void Run()
        {
            Trace.TraceInformation("WorkerRole1 is running");

            try
            {
                this.RunAsync(this.cancellationTokenSource.Token).Wait();
            }
            finally
            {
                this.runCompleteEvent.Set();
            }
        }

        public override bool OnStart()
        {            
            ServicePointManager.DefaultConnectionLimit = 12;                    

            bool result = base.OnStart();

            Trace.TraceInformation("WorkerRole1 has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("WorkerRole1 is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("WorkerRole1 has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                Trace.TraceInformation("Working");


                //Conecta na fila 1
                ConnectToStorage("f1");
                //recupera a mensagem
                string message = GetMessageFromQueue();

                //conecta na fila 2
                ConnectToStorage("f2");
                //adiciona a mensagem
                SendMessageToQueue(message);

                await Task.Delay(2000);
            }
        }

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

        public void SendMessageToQueue(string messageText)
        {
            var message = new CloudQueueMessage(messageText);

            cloudQueue.AddMessage(message);
        }

        public string GetMessageFromQueue()
        {
            var cloudQueueMessage = cloudQueue.GetMessage();

            if (cloudQueueMessage == null)
            {
                return string.Empty;
            }
            
            cloudQueue.DeleteMessage(cloudQueueMessage);

            return cloudQueueMessage.AsString;
        }

        public void ClearMessages()
        {
            cloudQueue.Clear();
        }

    }
}
