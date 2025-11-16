module FileExtractor.CSV

open System
open System.IO
open FSharp.Data
open Model

let loadCSV filename =
    let csv = CsvFile.Load(File.Open(filename, FileMode.Open), hasHeaders = false)
    
    csv.Rows
    |> Seq.fold (fun (goodRows, badRows) row ->
        try
            let document = Document.FromString(row.GetColumn 0)
            let amount = Decimal.Parse(row.GetColumn 1)
            ((document, amount) :: goodRows, badRows)
        with ex ->
            (goodRows, (row.ToString(), ex.Message) :: badRows)
        
        ) ([], [])