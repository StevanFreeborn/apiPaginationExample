# apiPaginationExample
A brief example of how you can execute pagination with v1 of the Onspring API.

## Notes
- v2 of the Onspring API does support documentation. It is highly recommend to make use of v2 of the API.
- This is not necessarily a step by step tutorial of how to execute this workaround.
- Rather it is a for instance to hopefully offer an example of how this can be done.
- My example is done using a .NET Core console application in conjuction with [RestSharp](https://restsharp.dev/) and [Newtonsoft.Json](https://www.newtonsoft.com/json).
- For many of the hard-coded values such as appId, apikey, base url, etc. I read them in from a configuration file.
- This console application has room for improvement as in such an integration there would be...
  - Better logging
  - Better error handling
  - Accounting for failed requests and re-attemting failed requests
  - etc.

### My Steps:
- Onspring setup
  - Within the target app in Onspring I built a formula field with a number output that just returns the value from the Record Id field.

    ![image](https://user-images.githubusercontent.com/65925598/157813801-00b7cc90-e3a8-4ff8-8482-444dd4230e1a.png)
  - Within the target app in Onspring I built a single select list field with a single value and set the fields default value to that lone value.
 
    ![image (1)](https://user-images.githubusercontent.com/65925598/157813952-1e2aa12b-c0d6-4c23-86c9-1ebfc3918780.png)
    - Note: For an existing records created prior to this field's creation you will need to update them so that they possess this fields lone value.
  - Next I created a report to aggregate the records and give me a maximum record id value. Below are those configurations.
 
    ![image (2)](https://user-images.githubusercontent.com/65925598/157814161-0a034871-c890-4081-a818-b1ec83195c4d.png)
    ![image (3)](https://user-images.githubusercontent.com/65925598/157814189-7af800b6-2671-45d8-8f74-0d1dc391d37c.png)
  - Next I created another report to aggregate the records and give me a minimum record id value. Below are those configurations.
    
    ![image (4)](https://user-images.githubusercontent.com/65925598/157814460-2d11b1cf-dff9-4adc-9035-26bd52f68f5c.png)
    ![image (5)](https://user-images.githubusercontent.com/65925598/157814483-a02640e1-c445-4bf1-8b0a-ce40a8343d9e.png)
- Non-Onspring setup
  - Next I wrote my program using the following logic:
    - use the api to get the max and min record id values from the reports I created.
    - use those max and min values and an arbitrary page size value to calculate a range of records.
    - use that range of records to build a filter for the Get Records endpoint
    - use a loop to make recurring Get Records request and increment the record range with each request until I've exceeded the max record id value.
  - Note at the end the program I write the collected records to a file, but once collected what to do next is dependent upon the use case.
