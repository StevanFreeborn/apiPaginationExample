using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RestSharp;

namespace apiPaginationExample
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            // get apiKey and baseUrl values from configuration file.
            var apiKey = config.GetSection("x-apikey").Value;
            var baseUrl = config.GetSection("baseUrl").Value;

            // create client using base api url.
            var client = new RestClient(baseUrl);

            // report id for report returning max record id value.
            var maxReportId = 823;

            // report id for report returning min record id value.
            var minReportId = 824;

            // resource end point to get report by id in api v1.
            var reportById = "/v1/reports/{reportId}";

            // query parameter values for requests
            var dataType = "ChartData";
            var dataFormat = "Formatted";


            // Get max record id using helper report that is returning max record id value.
            var maxIdRequest = new RestRequest(reportById)
                .AddUrlSegment("reportId", maxReportId)
                .AddParameter("dataType", dataType)
                .AddParameter("dataFormat", dataFormat)
                .AddHeader("x-apikey", apiKey);

            var maxIdReportResponse = client.GetAsync(maxIdRequest);

            if (maxIdReportResponse.Result.StatusCode != HttpStatusCode.OK)
            {
                Console.WriteLine("Could not get max record id. Exited.");
                return;
            }
            
            // deserialize the json response to a ReportResult object.
            var maxIdReport = JsonConvert.DeserializeObject<ReportResult>(maxIdReportResponse.Result.Content);

            // access the maximum record id value in the ReportResult and parse it to an integer.
            var maxRecordId = int.Parse(maxIdReport.Rows[0][1]);

            // Get min record id using helper report that is returning min record id value.
            var minIdrequest = new RestRequest(reportById)
                .AddUrlSegment("reportId", minReportId)
                .AddParameter("dataType", dataType)
                .AddParameter("dataFormat", dataFormat)
                .AddHeader("x-apikey", apiKey);

            var minIdReportResponse = client.GetAsync(minIdrequest);

            // check to see if request was successful. if it wasn't we write message to console
            // then return early since rest of program depends on this value.
            if (minIdReportResponse.Result.StatusCode != HttpStatusCode.OK)
            {
                Console.WriteLine("Could not get min record id. Exited.");
                return;
            }

            // deserialize the json response to a ReportResult object.
            var minIdReport = JsonConvert.DeserializeObject<ReportResult>(minIdReportResponse.Result.Content);

            // access the maximum record id value in the ReportResult and parse it to an integer.
            var minRecordId = int.Parse(minIdReport.Rows[0][1]);

            // Begin paginated requests to get all records
            // resource endpoint to get records by app id
            var recordsByAppId = "/v1/records/{appId}";

            // app id for the app I want to get records from.
            var appId = 246;

            // field id for the field I want to use in my requests filter. I used the Record Id field.
            var recordIdFieldId = 8967;

            // the number of records I want to bring back with each request.
            var pageSize = 2;

            // set the bottom of my filter range to the minRecordId value.
            var index = minRecordId;

            // create a new collection of record objects that I will use to store all the records I request.
            var allRecords = new List<Record>();
            
            // execute a loop to make recurring requests for records from the app
            // as long as the index value is less than the maxRecordId value.
            while (index < maxRecordId)
            {
                // calculate the top of the request filter range by adding the selected page size value
                // to the bottom value of the request filters range.
                var next = index + pageSize;

                // build filter string using the field id value for the Record Id field, the index value (bottom of filter range),
                // and the next value (top of filter range).
                var filter = $"(({recordIdFieldId} gt {index} or {recordIdFieldId} eq {index}) and {recordIdFieldId} lt {next})";
                
                // create request for records using the resource endpoint, appId, the filter, and the apiKey
                var recordsRequest = new RestRequest(recordsByAppId)
                    .AddUrlSegment("appId", appId)
                    .AddParameter("$filter", filter)
                    .AddHeader("x-apikey", apiKey);

                var recordsResponse = client.GetAsync(recordsRequest);

                // deserialize the json response to a list of Record objects.
                var records = JsonConvert.DeserializeObject<List<Record>>(recordsResponse.Result.Content);

                // add the collection of records I just requested to my allRecords collection.
                allRecords.AddRange(records);

                // sit the value of index to the value of next.
                index = next;
            }

            // serialize my allRecords collection to a json string.
            string json = JsonConvert.SerializeObject(allRecords.ToArray(), Formatting.Indented);
            
            // write the json string to a file.
            string filePath = AppDomain.CurrentDomain.BaseDirectory + @"\results\" +"records" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".json";
            File.WriteAllText(filePath, json);

            Console.WriteLine("Completed execution successfully.");
        }
    }
}