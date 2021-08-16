# CrudTesting
Sample testing approach for an API that does CRUD operations

# Author
Gary Butaud

# Branching
The main branch was created on inital test success to provide something as a baseline.  Afterwards, a develop branch was created 
for further testing.

# Service Under Test
The API 
The development team for a retail organization has built an API intended to be used for the maintenance of Stock Keeping Unit identifiers 
(SKUs) which are used to identify and track the items the company has for sale. 
 
This API implements the basic CRUD operations: 
 
Create and Update operations are through HTTP POSTs 
 POST https://1ryu4whyek.execute-api.us-west-2.amazonaws.com/dev/skus 

Posts expect a body with SKU, Description and Price 
{ 
    "sku":"berliner",  
    "description": "Jelly donut",  
    "price":"2.99" 
} 
 
Read operations are through HTTP GETs 
 GET https://1ryu4whyek.execute-api.us-west-2.amazonaws.com/dev/skus  
 GET https://1ryu4whyek.execute-api.us-west-2.amazonaws.com/dev/skus/{id} 

Delete operations are through HTTP DELETEs 
 DELETE https://1ryu4whyek.execute-api.us-west-2.amazonaws.com/dev/skus/{id}  

# Intentions For Testing
The project is first testing the basic creation and or updating of a sku, followed by deletion.  A few tests are added for 
min or max of a numeric value, null or empty for strings, and then a few languages were also included to test a range of 
characters for descriptions of a sku.

# Execute Tests
dotnet test .\CrudTesting.csproj

Example:
PS C:\Users\gbute\Documents\GitHub\CrudTesting> dotnet test .\CrudTesting.csproj

