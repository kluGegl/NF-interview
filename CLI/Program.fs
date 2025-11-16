open System
open System.IO

open CLI
open Model

[<EntryPoint>]
let main args =
    Repository.execute RepositorySqlLite.Init.initTables    
    
    match args with

    | [| "reverse";  userId; ticks |] ->
        let document = Document.FromString userId
        let paymentTimeStamp = DateTime(Int64.Parse ticks)
        let timeStamp = DateTime.Now
        
        let _ = Repository.execute (RepositorySqlLite.Payment.insertReversal document paymentTimeStamp timeStamp)
        do printfn $"Successfully reversed payment {document.Id} made at {paymentTimeStamp}"

    | [| "balance"; userId |] ->
        let document = Document.FromString userId
        let balance =
            let cents = Repository.execute (RepositorySqlLite.Payment.getBalanceFor document)
            Decimal.Divide(Decimal(cents), 100M)
        do printfn $"Balance for userId:{document.Id} is $%0.3f{balance}"
        
    | [| filename |] ->
        let extractor = 
            match (Path.GetExtension filename).ToLower() with
            | ".csv" -> FileExtractor.CSV.loadCSV
            | ".xml" -> FileExtractor.XML.loadXML
            | extension -> failwith $"Extension {extension} is not supported"
        
        match extractor filename with
        | goodRows, [] ->
            goodRows
            |> List.iter (fun (document, amount) ->
                let timeStamp = DateTime.Now
                let _ = Repository.execute (RepositorySqlLite.Payment.insertPayment document amount timeStamp)
                
                do printfn $"Successfully inserted payment for {document} for ${amount} at {timeStamp.Ticks}"
                )
            //Sql errors not taken into account
            printfn $"{goodRows.Length} payment(s) added from file:{filename}"
            
        | (_, badRows) ->
            printfn "Some rows malformed. Nothing processed"
            badRows
            |> List.iter (fun (row, errorMessage) ->
                printfn $"Row:{row} error:{errorMessage}"
                ) 

    | [| userId; amount |] ->
        let document = Document.FromString userId
        let amount = Decimal.Parse amount
        let timeStamp = DateTime.Now

        let _ = Repository.execute (RepositorySqlLite.Payment.insertPayment document amount timeStamp)
        
        do printfn $"Successfully inserted payment for {document} for ${amount} at {timeStamp.Ticks}"
        
    
    | _ ->
        printfn "No usage help available"
    0

