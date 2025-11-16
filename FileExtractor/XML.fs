module FileExtractor.XML

open System
open System.IO
open System.Xml.Linq
open Model

let loadXML filename =
    let xmlString =
        use reader = new StreamReader (File.Open (filename, FileMode.Open))
        reader.ReadToEnd()
    let root = (XDocument.Parse xmlString).Root
    root.Elements(XName.Get("Payment"))
    |> Seq.fold (fun (goodRows, badRows) row ->
        try
            let userId = Document.FromString(row.Attribute("id").Value)
            let amount =
                let cents = Decimal.Parse(row.Attribute("amount").Value)
                Decimal.Divide(cents, 100M)
            ((userId, amount) :: goodRows, badRows)
        with ex ->
            (goodRows, (row.ToString(), ex.Message) :: badRows)
        ) ([], [])
