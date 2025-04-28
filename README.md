Console App Code Explanation:                                   

        The app uses Dependency Injection with IHost to manage services like logging and HttpClient.
        In Program.cs, the app: Loads configuration from appsettings.json 
        Registers ExternalUserService to make HTTP requests of GetAllUsersAsync() / GetUserByIdAsync() 
        Builds the host to set up everything              

Step To Run Console App : 

        In a start use to run ReqresApi.ConsoleApp to run the Console Application
        After that, it shows options to the user in the console:
        Press 1 to get all users (paginated)
        Press 2 to get a single user (hardcoded ID = 2)
        Based on the input
                If 1 is selected, it calls GetAllUsersAsync() and shows all user names and emails.
                If 2 is selected, it calls GetUserByIdAsync(2) and shows full details for that user.
                After the response, it asks the user if they want to continue. If yes(y), it repeats the process
                If No(n) then it end the process.

              
Unit Test Code Explanation:
                            
        Uses Moq to mock HttpClient, avoiding real API calls.
        Mocks IConfiguration to inject the base URL (https://reqres.in/api).
        Mocks ILogger to support logging without writing logs.
        Uses Newtonsoft.Json to simulate Reqres API JSON responses.   

        Responses are faked using HttpResponseMessage to test how the service handles API data.
        Verifications ensure the HTTP call was made exactly once.
        Assertions check if the returned user data is accurate.

        The below two fact to test the two API's:
        GetUserById_ReturnsCorrectUser() -  GetUserById: returns a single user when ID = 2
        GetAllUsers_ReturnsPagedList() -       GetAllUsers: returns a paged list of users.

Step to Run the Unit Test:
                       
        Open File -> ExternalUserServiceTests.cs -> Mock<IConfiguration> _configMock -> Mention the base Url.
        GetUserById_ReturnsCorrectUser() pass the single payload to test.
        GetAllUsers_ReturnsPagedList() pass the list payload to test the list with pagination.

        Go to -> Test -> Configuration Run Setting -> Select Solution Wide run setting file 
        -> Select "ExternalUserServiceTests.cs".

        Again go  to -> Test -> Run All Test based on that It shows the success result.  