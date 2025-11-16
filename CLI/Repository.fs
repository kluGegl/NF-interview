module CLI.Repository

let dbFileName = "EF.db"

//Wiring dependencies
let execute<'a>  = RepositorySqlLite.Impl.execute<'a> dbFileName