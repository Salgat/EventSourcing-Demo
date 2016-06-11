## EventSourcing-Demo
A basic example of using Event Sourcing to create and query different models purely from events. This example shows how two different models in two different services can be constructed from the same set of events using the [EventStore](https://geteventstore.com/) library.

## Installation and testing
1. If you don't have Visual Studio, go ahead and grab it from [here](https://www.visualstudio.com/en-us/products/vs-2015-product-editions.aspx). The Community Edition is free.
2. Before running the solutions, you will need to run EventStore. Download it from [here](https://geteventstore.com/downloads/) and extract it to a folder. Next, in command line run the following command: **EventStore.ClusterNode.exe --db ./db --log ./logs --run-projections=all --start-standard-projections=true**  It's important that you include the projections options, since the projections allow you to query by Category.
3. Run the DataServer solution first and POST and PATCH your object. When you create your object, the response will be the GUID string that is used to reference that object. For example, **POST http://localhost:61849/api/fullName/?firstName=Austin&lastName=Salgat** then **PATCH http://localhost:61849/api/fullName/36b41631-5499-401b-85cb-cfe41c71b0b5?firstName=Fangfang&lastName=Fu** creates an object with a name, then the PATCH updates this name to something else for that specific object.
4. Now, run the DataReader solution and do a query by Id **http://localhost:64589/api/fullName/36b41631-5499-401b-85cb-cfe41c71b0b5** This will return a different object than before, that is a combination of the first and last name.

## Explanation
The DataServer service handles the Commands to create and update name objects. These Commands map directly to events that are sent to EventStore to be saved. Each GUID represents one object and a single Event Stream that it belongs to. Any actions that occur for that particular object are appended to its event stream.

The DataReader service handles Querying the event stream data. Upon the application starting, it will read the Category projection from EventStore (**$ce-FullName**) which returns all events for FullName. It then iterates all events for FullName and creating new objects and mapping specific actions to these models based on their event type; a Created Event maps to creating a new model with a name, while the Updated Events map to a handler that updates the model's name. The DataReader service also subscribes to the Category stream for all future events, updating its models to represent these new events.

## Notes
This project is meant only to show one of the simplest examples of using the event sourcing pattern. Also, I understand that this example does not follow the CQRS pattern exactly, since the Command services supports both GetById in the same controller and the POST/PATCH endpoints return data.
