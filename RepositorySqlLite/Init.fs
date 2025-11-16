module RepositorySqlLite.Init

open RepositorySqlLite.Impl

let ifTableNotExistsF (connection: Connection) tableName initializeTable =
    match executeScalar<System.Int64> connection Query.countTables [("table_name", tableName)] with
    | 0L -> initializeTable()
    | 1L -> () //Table exists do nothing
    | _ -> failwith $"Unable to check if table {tableName} exists"

let initUsers (connection: Connection) =
    ifTableNotExistsF connection "User" (fun () ->
        let executeScalar = (executeIgnore connection)
        let _ = executeScalar "CREATE TABLE User(\
            id text NOT NULL PRIMARY KEY,\
            email text NOT NULL)" []
        let _ = executeScalar "INSERT INTO User (id, email) VALUES\
            ('PAS:1', 'luca@mail.com'),\
            ('DL:1', 'alex@mail.com'),\
            ('PAS:2', 'klugegl@mail.com')" []
        ())

let initPayment (connection: Connection) =
    ifTableNotExistsF connection "Payment" (fun () ->
        let _ = executeIgnore connection "CREATE TABLE Payment(\
            payeeId text NOT NULL,\
            amountCents integer NOT NULL,\
            timeStamp integer NOT NULL,\
            PRIMARY KEY(payeeId, timeStamp),\
            FOREIGN KEY(payeeId) REFERENCES User(id))" []
        ())

let initReversal (connection: Connection) =
    ifTableNotExistsF connection "Reversal" (fun () ->
        printfn "Creating reversal"
        //No multi reversal for now
        let _ = executeIgnore connection "CREATE TABLE Reversal(\
            paymentPayeeId text NOT NULL,\
            paymentTimeStamp integer NOT NULL,\
            timeStamp integer NOT NULL,\
            PRIMARY KEY (paymentPayeeId, paymentTimeStamp),\
            FOREIGN KEY(paymentPayeeId, paymentTimeStamp) REFERENCES Payment(payeeId, timeStamp))" []
        ())

let initTables (connection: Connection) =
    initUsers connection
    initPayment connection
    initReversal connection
    
    