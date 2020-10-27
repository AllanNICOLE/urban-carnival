using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UC.Core.Services
{
    public class CosmosTableService
    {
        private const string TableReference = "Foo";
        private const string DefaultPartitionKey = "DOCUMENTS";
        private CloudTable _table;

        public CosmosTableService(string connectionString)
        {
            CloudStorageAccount account = CloudStorageAccount.Parse(connectionString);
            CloudTableClient tableClient = account.CreateCloudTableClient();
            _table = tableClient.GetTableReference(TableReference);
        }

        public async Task AddData(string guid, IEnumerable<KeyValuePair<string, object>> properties, string partitionKey = DefaultPartitionKey)
        {
            DynamicTableEntity dyn = new DynamicTableEntity(partitionKey, guid);
            foreach (var item in properties)
            {
                dyn.Properties.Add(item.Key, new EntityProperty(item.Value.ToString()));
            }
            await _table.ExecuteAsync(TableOperation.InsertOrMerge(dyn));
        }

        public async Task<IEnumerable<DynamicTableEntity>> GetAllData()
        {
            var continuationToken = default(TableContinuationToken);
            return await _table.ExecuteQuerySegmentedAsync(new TableQuery<DynamicTableEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, DefaultPartitionKey)), continuationToken);
            
        }

        public async Task<DynamicTableEntity> GetDataByGuid(string guid)
        {
            var continuationToken = default(TableContinuationToken);
            var a = await _table.ExecuteQuerySegmentedAsync(new TableQuery<DynamicTableEntity>().Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, guid)), continuationToken);
            return a.Results.FirstOrDefault();

        }

        // TODO : To be removed ... 
        //
        //    public void Foo()
        //    {
        //        DynamicTableEntity dyn = new DynamicTableEntity("enfait", "caexiste");
        //        dyn.Properties.Add("Foo", new EntityProperty("Bar"));
        //        _fooTable.Execute(TableOperation.InsertOrMerge(dyn));
        //    }


    }
}
