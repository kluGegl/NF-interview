namespace Model

open System

type DocumentType =
    | InternationalPassport
    | DrivingLicense
with
    static member Create shortString =
        match shortString with
        | Literals.InternationalPassportShortString -> InternationalPassport
        | Literals.DrivingLicenseShortString -> DrivingLicense
        | _ -> failwithf $"Unknown document type: {shortString}"
    
    member x.shortString =
        match x with
        | InternationalPassport -> "PAS"
        | DrivingLicense -> "DL"

//Identifies user
type Document = {DocumentType: DocumentType; DocumentId: String}
with
    static member FromString (idString: String) =
        match idString.Split ':' with
        | [| Literals.InternationalPassportShortString; id |] when not (String.IsNullOrWhiteSpace id) ->
            {DocumentType = DocumentType.InternationalPassport; DocumentId = id}
        | [| Literals.DrivingLicenseShortString; id |] when not (String.IsNullOrWhiteSpace id) ->
            {DocumentType = DocumentType.DrivingLicense; DocumentId = id}
        | _ -> failwithf $"{idString} is not valid document string"

    member x.Id =  $"%s{x.DocumentType.shortString}:%s{x.DocumentId}"