# grpcAPI

Basic Store items CRUD. Using Cosmos DB assuming the partitionKey is basically store sections (i.e: Tech Store sections-> Phones, Consoles, Monitors, etc).
>The items only assume Name and ID, probably will add Price, description, quantity, for the items later.

I used facade for meddling cache and used changefeed to monitor changes on cosmosDB and keep the redis cache updated, so basically on a get we will mostly always 
get a cached response in order to get quicker responses.

I tried to mimic a statusCode and message response on the GRPC for clarity, this was achieved using interceptors.

LOGICS:
>Assumes the container is locked in a way that we will only remove things on it using GRPC, this is in order to maintain the cache updated, this happens because we 
can't monitor deletes since cosmosDB soft deletes therefore they are not captured by the changefeed monitor.
