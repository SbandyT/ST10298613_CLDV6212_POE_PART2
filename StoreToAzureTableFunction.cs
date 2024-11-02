using Azure.Data.Tables;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ST10298613_CLDV6212_POE_PART_
{
    public static class StoreToAzureTableFunction
    {
        [Function("StoreTableInfo")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Processing StoreToAzureTableFunction request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            string tableName = data?.tableName;
            string partitionKey = data?.partitionKey;
            string rowKey = data?.rowKey;
            string dataValue = data?.data;

            if (string.IsNullOrEmpty(tableName) || string.IsNullOrEmpty(partitionKey) ||
                string.IsNullOrEmpty(rowKey) || string.IsNullOrEmpty(dataValue))
            {
                return new BadRequestObjectResult("Table name, partition key, row key, and data must be provided.");
            }


            var connectionString = Environment.GetEnvironmentVariable("AzureStorage:ConnectionString");
            var serviceClient = new TableServiceClient(connectionString);
            var tableClient = serviceClient.GetTableClient(tableName);
            await tableClient.CreateIfNotExistsAsync();

            var entity = new TableEntity(partitionKey, rowKey) { ["Data"] = data };
            await tableClient.AddEntityAsync(entity);

            return new OkObjectResult("Data added to table");
        }
    }
}
