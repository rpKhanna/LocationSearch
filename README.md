# Location Search

## Steps to run the project
- Downlod the source code from the github url: [Location Search](https://github.com/rpKhanna/LocationSearch).
- Open it in Visual Studio
- Rebuild it and press Ctrl+F5

## Performance matrix
- For 10 Lakh records
    - Autocomplete location search takes 2 to 3 seconds
    - Search takes 2-3 seconds
    - Pagination takes even lesser as we are fetching it from cache unless the search criteria changes.

## Technical information on the implementation
- Main focus in implementation was on performance.
- Data source used is the csv file given.
- Hence for testing with large data just paste the csv file in the LocationSearch.Data/Data folder
- Below are the concepts that are used in the project:
    - Three layer architecture
    - Repository pattern
    - Dependency Injection
    - Delegate
    - Automapper
    - SeriLog logger
    - Global exception handling using Middleware
    - Caching 
    - Ajax call to populate data in grid
    - Jquery Datatable for grid
    - Client side validation
    - Pagination on grid with lazy loading i.e. one page at a time.
    - Bootstrap
    - Last but not the least, a cute earth spinner to relate the search of locations.

## Functional explaination
- As soon as we run the project Search page is displayed.
- Location box is an auto complete with minimum 3 char length. i.e. as soon as 3rd character is pressed it will fetch all the locations starting with search chars and display top 50 of them.
- Location and MaxDistance is required field whereas MaxResult is optional.
- If while searching 
    - user inputs MaxDistance as 0 then all the locations that have distance 0 respective to the searched source location will be displayed.
    - user inputs MaxResult as 0 or nothing then all the locations matching the search criteria (Location and MaxDistance) will be displayed.
    - Data will be sorted by Distance.
    - As soon as user clicks on Search button, first page of the list of locations that matches the search criteria will be displayed.
    - Grid has paging on it which is way faster even with lakhs of records.
    - Reset button to reset the search.

- Due to crunch of time and office workload couldn't implement unit testing. Apologies for that.
- We can also use Sql Server as the data source and use entityframework to communicate with database to implement the same thing.
- <b>Creative thought:</b> For searching of location we can also integrate google maps.
