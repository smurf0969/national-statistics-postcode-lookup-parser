# national-statistics-postcode-lookup-parser ![.NET Core](https://github.com/smurf0969/national-statistics-postcode-lookup-parser/workflows/.NET%20Core/badge.svg)  
This project is a Visual Studio C# .net core cross-platform console application for parsing csv formatted files into json and also reducing the input fields.  
This came about due to a friends request to provide a quick solution to replace an existing application by a company that has unfortunately ceased trading.  
The upcoming changes to the layout of the NSPL data would break the existing application that can no longer be obtained.  
Data obtained from ![National Statistics Postcode Lookup](https://geoportal.statistics.gov.uk/search?collection=Dataset&sort=-modified&tags=all(PRD_NSPL))

## Functionality 
As this project was going to be used as a direct replacement, existing functionality and compatibility has to be maintained.  
* [ ] Automatically download and extract the zipped data.
* [x] Remove unwanted files and folders to regain space and speed up processing.
* [x] Parse specific csv files
* [x] Reduce outputted fields and future proof for source data column renaming. 
* [x] Output data structures to Json to allow importing into different systems..
* [x] Zip outputted files
* [x] Remove all other files. 

Whilst most of the functionality is complete, you will notice there is still much to be done i.e. code documentation, error handling & logging, command args.  

## Improvements
There is a lot of scope to improve & adapt this application to suit multiple needs.
Help and suggestions are more than welcome.  

## Postcode, Geo, Latitude, Longitude, County, Country
All data is available freely via the gov.uk website and  Office for National Statistics sites, but care must be taken to adhere to the license agreements as the data is compiled from multiple sources. 
>  Contains OS data © Crown copyright and database rights [year]  
>  Contains Royal Mail data © Royal Mail copyright and database rights [year]  
>  Contains National Statistics data © Crown copyright and database rights [year]. 
