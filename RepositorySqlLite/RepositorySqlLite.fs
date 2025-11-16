module RepositorySqlLite.Impl

open System
open Microsoft.Data.Sqlite

type Connection = SqliteConnection

let execute<'a> (dbFileName: String) (f: Connection -> 'a) =
    use connection = new SqliteConnection($"Data Source = {dbFileName}")
    connection.Open() 
    f connection

let private prepareCommand (connection: Connection) (query: String) (sqlParams: (string * obj) list) =
    let command = connection.CreateCommand (CommandText = query)
    sqlParams
    |> List.map (fun (name, value) -> SqliteParameter (name, value))
    |> command.Parameters.AddRange
    command

let executeScalar<'a> connection query sqlParams=
    let command = prepareCommand connection query sqlParams 
    
    match command.ExecuteScalar() with
    | :? 'a as result -> result
    | result -> failwithf $"{result} is not of type {typeof<'a>}"

let executeNonQuery connection query sqlParams=
    (prepareCommand connection query sqlParams).ExecuteNonQuery()

let executeIgnore connection query sqlParams=
    (prepareCommand connection query sqlParams).ExecuteScalar() |> ignore

let checkTables (connection: Connection) =
    use command = connection.CreateCommand (CommandText = "SELECT * FROM sqlite_master WHERE type='table' ORDER BY name")
    
    let reader = command.ExecuteReader ()
    [
     yield [List.map (reader.GetName >> box) [1..reader.FieldCount - 1]]
     while reader.Read() do
        yield [ List.map reader.GetValue [1..reader.FieldCount - 1]
            ]
    ]